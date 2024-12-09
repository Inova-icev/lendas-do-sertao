using UnityEditor;
using UnityEngine;

public class Vida : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public Transform healthBar; // Referência à barra verde (preenchimento)
    public GameObject healthBarObject; // Objeto completo da barra de vida

    private Vector3 healthBarScale; // Escala original da barra de preenchimento
    private float healthPercent;
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

        healthBarScale = healthBar.localScale;
        healthPercent = healthBarScale.x / currentHealth;
    }

    private void Update() {
        RegenerarAtributos();
    }

    // Função para calcular a mitigação de dano (aplicada ao dano físico ou mágico)
    private float Mitigacao(float dano, float mitigacao)
    {
        return dano / (1 + mitigacao / 100f);
    }

    // Função modificada para calcular o dano recebido com base na mitigação
    public void TakeDamage(float dano, int tipoDano = 0) // tipo de dano: 0 = físico, 1 = mágico
    {
        float danoFinal = dano;

        // Calcula o dano final com base no tipo de dano
        if (tipoDano == 0) // Dano Físico
        {
            danoFinal = Mitigacao(dano, armadura);
        }
        else if (tipoDano == 1) // Dano Mágico
        {
            danoFinal = Mitigacao(dano, defesaMagica);
        }

        // Aplica o dano na vida atual
        currentHealth -= danoFinal;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar(); // Atualiza a barra de vida

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
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }

        /*if (manaAtual < manaMaxima)
        {
            manaAtual += regeneracaoMana * Time.deltaTime;
            if (manaAtual > manaMaxima) manaAtual = manaMaxima;
        }*/

        UpdateHealthBar();
    }


    // Atualiza a barra de vida visualmente
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Atualiza a escala no eixo X proporcional à saúde
            healthBarScale.x = healthPercent * currentHealth;
            healthBar.localScale = healthBarScale;
        }
    }

    // Função para lidar com a morte do objeto
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
