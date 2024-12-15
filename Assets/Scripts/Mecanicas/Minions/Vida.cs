using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Vida : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public Transform healthBar; // Referência à barra verde (preenchimento)
    public GameObject healthBarObject; // Objeto completo da barra de vida
    public GameObject healthBarPrefab; // Prefab para a barra de vida

    private Vector3 healthBarScale; // Escala original da barra de preenchimento
    private float healthPercent;

    private GameManager gameManager;
    private RespawnManager respawnManager; // Gerenciador de respawn
    

    public float regeneracaoVida;
    public float armadura;
    public float defesaMagica;
    

    void Start()
    {
        // Detecta ou instancia a barra de vida
        if (healthBar == null || healthBarObject == null)
        {
            SetupHealthBar();
        }

        healthBarScale = healthBar.localScale;
        healthPercent = 1f;
        currentHealth = maxHealth;
        UpdateHealthBar();

        gameManager = GameManager.Instance;
        respawnManager = FindObjectOfType<RespawnManager>();

        if (respawnManager == null)
        {
            Debug.LogError("RespawnManager não foi encontrado na cena!");
        }
    }

    private void Update()
    {
        RegenerarAtributos();
    }

    private float Mitigacao(float dano, float mitigacao)
    {
        return dano / (1 + mitigacao / 100f);
    }

    [PunRPC]
    public void TakeDamageRPC(int damage, int damageType)
    {
        TakeDamage(damage, damageType); // Aplica o dano no cliente dono do alvo
    }
    public void TakeDamage(float dano, int tipoDano = 0)
    {
        float danoFinal = tipoDano == 0 ? Mitigacao(dano, armadura) : Mitigacao(dano, defesaMagica);

        currentHealth -= danoFinal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Sincroniza a vida com todos os clientes
        if (PhotonNetwork.IsConnected)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);
            }
        }
        else
        {
            UpdateHealthBar(); // Atualiza localmente no modo offline
        }

        // Verifica se o personagem morreu
        if (currentHealth <= 0)
        {
            Player playerComponent = GetComponent<Player>();
            if (playerComponent != null)
            {
                StartCoroutine(HandleRespawn());
            }
            else
            {
                Die();
            }
        }

        Debug.Log($"Dano recebido: {danoFinal} (Tipo: {(tipoDano == 0 ? "Físico" : "Mágico")})");
    }
    private void RegenerarAtributos()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += regeneracaoVida * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Sincroniza a vida regenerada
            if (PhotonNetwork.IsConnected)
            {
                PhotonView photonView = GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);
                }
            }
            else
            {
                UpdateHealthBar(); // Atualiza localmente no modo offline
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null && maxHealth > 0)
        {
            healthPercent = currentHealth / maxHealth;
            healthPercent = Mathf.Clamp01(healthPercent);

            // Define a escala somente no eixo X
            healthBar.localScale = new Vector3(healthPercent, 1, 1);
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} morreu e foi destruído.");

        Minions minionComponent = GetComponent<Minions>();
        Torre torreComponent = GetComponent<Torre>();

        if (minionComponent != null)
        {
            minionComponent.OnDeath();
        }

        if (torreComponent != null)
        {
            torreComponent.GrantGoldToNearbyEnemies();
        }

        Destroy(healthBarObject);
        Destroy(gameObject);
    }
    private void DisablePlayer()
    {
        // Sincroniza com Photon (RPC)
        if (PhotonNetwork.IsConnected)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("SyncDisablePlayer", RpcTarget.AllBuffered);
            }
        }
        else
        {
            SyncDisablePlayer();
        }
    }

    private void EnablePlayer()
    {
        // Sincroniza com Photon (RPC)
        if (PhotonNetwork.IsConnected)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("SyncEnablePlayer", RpcTarget.AllBuffered);
            }
        }
        else
        {
            SyncEnablePlayer();
        }
    }
    [PunRPC]
    private void SyncDisablePlayer()
    {
        // Desativa o SpriteRenderer (personagem desaparece visualmente)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Desativa a colisão (impede interações físicas)
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Trava a movimentação do personagem
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Zera velocidade
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Congela posição e rotação
        }

        // Desativa scripts de controle do jogador
        Player playerController = GetComponent<Player>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Esconde a barra de vida
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(false);
        }
    }

    [PunRPC]
    private void SyncEnablePlayer()
    {
        // Reativa o SpriteRenderer (personagem reaparece visualmente)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Reativa a colisão
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Libera movimentação do personagem
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.None; // Libera movimentação
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Apenas trava rotação
        }

        // Reativa scripts de controle do jogador
        Player playerController = GetComponent<Player>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Reexibe a barra de vida
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(true);
        }
    }

    private System.Collections.IEnumerator HandleRespawn()
    {
        Debug.Log($"{gameObject.name} morreu e será respawnado em 5 segundos.");

        // Desativa o jogador
        DisablePlayer();

        // Espera 5 segundos
        yield return new WaitForSeconds(5);

        // Chama o RespawnManager para escolher o ponto de respawn
        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(gameObject);
        }
        else
        {
            Debug.LogError("RespawnManager não configurado.");
        }

        // Restaura a vida máxima e sincroniza
        if (PhotonNetwork.IsConnected)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("SyncMaxHealth", RpcTarget.All, maxHealth); // Sincroniza a vida máxima
            }
        }
        else
        {
            currentHealth = maxHealth; // Atualiza localmente no modo offline
            UpdateHealthBar();
        }

        // Reativa o jogador
        EnablePlayer();
    }


    private void SetupHealthBar()
    {
        // Procura pela barra de vida existente
        if (healthBar == null)
        {
            healthBar = transform.Find("BarraVida/Verde");
        }

        if (healthBarObject == null)
        {
            healthBarObject = transform.Find("BarraVida")?.gameObject;
        }

        // Se não encontrar, instancia a partir de um prefab
        if (healthBar == null || healthBarObject == null)
        {
            if (healthBarPrefab != null)
            {
                // Instancia o prefab
                healthBarObject = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);

                // Define o pai e ajusta a escala
                healthBarObject.transform.SetParent(transform);
                healthBarObject.transform.localScale = new Vector3(0.4f, 0.4f, 1f); // Ajusta o tamanho do pai

                // Ajusta a posição da barra em relação ao personagem
                healthBarObject.transform.localPosition = new Vector3(-0.25f, 0.3f, 0); // 1.5 unidades acima

                // Localiza as barras de vida (verde e vermelho)
                healthBar = healthBarObject.transform.Find("Verde");
                Transform redBar = healthBarObject.transform.Find("Vermelho");

                // Configura escala padrão dos filhos, caso necessário (deixe em 1 para herdar do pai)
                if (healthBar != null)
                {
                    healthBar.localScale = Vector3.one;
                }

                if (redBar != null)
                {
                    redBar.localScale = Vector3.one;
                }
            }
            else
            {
                Debug.LogError("Prefab de barra de vida não configurado!", this);
            }
        }
    }
    [PunRPC]
    private void SyncHealth(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthBar();
    }

    [PunRPC]
    private void SyncMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth; // Restaura a vida ao máximo
        UpdateHealthBar();
    }
}
