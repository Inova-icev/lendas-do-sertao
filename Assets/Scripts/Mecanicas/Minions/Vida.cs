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

    private Vector3 healthBarScale; // Escala original da barra de preenchimento
    private float healthPercent;

    private GameManager gameManager;
    public string teamName;

    public float regeneracaoVida;
    public float armadura;
    public float defesaMagica;

    void Start()
    {
        if (healthBar == null)
        {
            // Procura pela barra de preenchimento automaticamente
            healthBar = transform.Find("BarraVida/Verde");
        }

        if (healthBarObject == null)
        {
            // Procura pelo objeto completo da barra de vida
            healthBarObject = transform.Find("BarraVida").gameObject;
        }

        if (healthBar == null || healthBarObject == null)
        {
            Debug.LogError("A configuração da barra de vida está incompleta!", this);
        }

        // Inicializa a barra de vida
        healthBarScale = healthBar.localScale;
        healthPercent = 1f; // Começa com 100% de vida
        currentHealth = maxHealth; // Garante que a vida atual seja máxima no início
        UpdateHealthBar(); // Atualiza a barra de vida visualmente
        gameManager = GameManager.Instance;
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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Garante que a vida esteja entre 0 e o máximo

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
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
            // Calcula o percentual de vida
            healthPercent = currentHealth / maxHealth;
            healthPercent = Mathf.Clamp01(healthPercent); // Garante que esteja entre 0 e 1

            // Atualiza a escala da barra de preenchimento
            healthBarScale.x = healthPercent * healthBarScale.x;
            healthBar.localScale = new Vector3(healthBarScale.x, healthBarScale.y, healthBarScale.z);
        }
    }

    void Die()
    {
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
}
