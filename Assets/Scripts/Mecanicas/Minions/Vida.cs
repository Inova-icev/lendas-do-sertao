using UnityEngine;

public class Vida : MonoBehaviour
{
    public int currentHealth;
    public Transform healthBar; // Referência à barra verde (preenchimento)
    public GameObject healthBarObject; // Objeto completo da barra de vida

    private Vector3 healthBarScale; // Escala original da barra de preenchimento
    private float healthPercent;

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


    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Atualiza a escala no eixo X proporcional à saúde
            healthBarScale.x = healthPercent * currentHealth;
            healthBar.localScale = healthBarScale;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
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
