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
    private bool isDead = false;
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

        if (currentHealth <= 0 && !isDead)
        {
            PhotonView photonView = GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient) // Somente o MasterClient gerencia a morte
            {
                if (GetComponent<Player>() != null) // Se o objeto for um Player
                {
                    isDead = true; // Evita chamadas repetidas
                    StartCoroutine(HandleRespawn());
                }
                else // Minions e Torres
                {
                    isDead = true;
                    photonView.RPC("DieRPC", RpcTarget.AllBuffered);
                }
            }
        }
    }



    private float Mitigacao(float dano, float mitigacao)
    {
        return dano / (1 + mitigacao / 100f);
    }

    [PunRPC]
    public void TakeDamageRPC(int damage, int damageType)
    {
        if (!PhotonNetwork.IsMasterClient) return; // Apenas o MasterClient processa o dano

        TakeDamage(damage, damageType);

        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;

            if (GetComponent<Player>() != null) // Para Players
            {
                StartCoroutine(HandleRespawn());
            }
            else // Para Minions e Torres
            {
                photonView.RPC("DieRPC", RpcTarget.AllBuffered);
            }
        }
    }


    [PunRPC]
    private void DieRPC()
    {
        Die(); 
    }

    public void TakeDamage(float dano, int tipoDano = 0, bool respawnPlayer = false)
    {
        float danoFinal = tipoDano == 0 ? Mitigacao(dano, armadura) : Mitigacao(dano, defesaMagica);

        currentHealth -= danoFinal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            if (respawnPlayer)
            {
                // Para Players
                StartCoroutine(HandleRespawn());
            }
            else
            {
                // Para Minions e Torres
                PhotonView photonView = GetComponent<PhotonView>();
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("DieRPC", RpcTarget.AllBuffered);
                }
            }
        }
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

    public void UpdateHealthBar()
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
        // Verifica se o objeto é um Nexus
        Nexus nexusComponent = GetComponent<Nexus>();
        PhotonView photonView = GetComponent<PhotonView>();
        if (nexusComponent != null)
        {
            Debug.Log($"{gameObject.name} (Nexus) foi destruído!");

            if (PhotonNetwork.IsMasterClient && photonView != null)
            {
                photonView.RPC("HandleGameEndRPC", RpcTarget.AllBuffered, nexusComponent.nexusTag);
            }

            return; // Evita continuar com a lógica comum de morte
        }

        // Lógica existente para Players, Minions ou Torres
        if (GetComponent<Player>() != null)
        {
            Debug.LogError("Die foi chamado para um Player, isso não deve acontecer!");
            StartCoroutine(HandleRespawn());
            return;
        }

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
        Debug.Log($"HandleRespawn chamado para {gameObject.name}. Desativando o jogador...");

        // Desativa o jogador
        DisablePlayer();

        Debug.Log("Jogador desativado. Aguardando 5 segundos...");
        yield return new WaitForSeconds(5);

        if (respawnManager != null)
        {
            Debug.Log("Chamando RespawnManager para reposicionar o jogador...");
            respawnManager.RespawnPlayer(gameObject);
        }
        else
        {
            Debug.LogError("RespawnManager não configurado!");
        }
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
    public void SyncHealth(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthBar();
    }

    [PunRPC]
    private void SyncMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth; 
        UpdateHealthBar();
    }
    [PunRPC]
    public void SyncRespawn(Vector3 position)
    {
        Debug.Log($"{gameObject.name} recebeu o SyncRespawn. Nova posição: {position}");

        transform.position = position; // Reposiciona o jogador
        currentHealth = maxHealth;     // Restaura a vida
        UpdateHealthBar();

        Debug.Log("Vida restaurada. Reativando o jogador...");
        SyncEnablePlayer(); // Reativa o jogador
    }
}