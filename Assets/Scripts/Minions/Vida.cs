using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100; // Saúde máxima
    private int currentHealth;  // Saúde atual

    void Start()
    {
        currentHealth = maxHealth; // Define a saúde inicial
    }

    // Função para aplicar dano ao minion
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduz a saúde pelo valor de dano
        Debug.Log(gameObject.name + " tomou " + damage + " de dano.");

        if (currentHealth <= 0)
        {
            Die(); // Chama a função de morte se a saúde for zero ou menos
        }
    }

    // Função para lidar com a morte do minion
    void Die()
    {
        Debug.Log(gameObject.name + " morreu.");
        Destroy(gameObject); // Destroi o minion
    }
}
