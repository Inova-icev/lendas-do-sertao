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
        healthPercent = 1f; // Começa com 100% de vida
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

    public void TakeDamage(float dano, int tipoDano = 0)
    {
        float danoFinal = tipoDano == 0 ? Mitigacao(dano, armadura) : Mitigacao(dano, defesaMagica);

        currentHealth -= danoFinal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Player playerComponent = GetComponent<Player>();
            if (playerComponent != null)
            {
                StartCoroutine(HandleRespawn()); // Respawn para jogadores
            }
            else
            {
                Die(); // Destruição para outros objetos
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
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null && maxHealth > 0)
        {
            healthPercent = currentHealth / maxHealth;
            healthPercent = Mathf.Clamp01(healthPercent);

            healthBar.localScale = new Vector3(healthPercent, healthBarScale.y, healthBarScale.z);
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

    private System.Collections.IEnumerator HandleRespawn()
    {
        Debug.Log($"{gameObject.name} morreu e será respawnado em 5 segundos.");

        // Desativa o jogador
        gameObject.SetActive(false);

        // Espera 5 segundos antes do respawn
        yield return new WaitForSeconds(5);

        // Restaura a vida e reposiciona o jogador
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(gameObject);
        }
        else
        {
            Debug.LogError("RespawnManager não configurado.");
        }

        // Reativa o jogador
        gameObject.SetActive(true);
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
                healthBarObject = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                healthBarObject.transform.SetParent(transform); // Faz a barra de vida seguir o objeto
                healthBar = healthBarObject.transform.Find("Verde"); // Acha a barra de preenchimento
            }
            else
            {
                Debug.LogError("Prefab de barra de vida não configurado!", this);
            }
        }
    }
}
