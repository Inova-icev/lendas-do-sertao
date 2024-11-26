using UnityEngine;

public class Vida : MonoBehaviour
{
    public int currentHealth; // Saúde atual (atribua individualmente no Inspector)

    void Start()
    {
        // currentHealth será definido individualmente para cada GameObject no Inspector,
        // então não é necessário inicializá-lo com um valor padrão aqui.
    }

    // Função para aplicar dano ao objeto
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduz a saúde pelo valor de dano

        if (currentHealth <= 0)
        {
            Die(); // Chama a função de morte se a saúde for zero ou menos
        }
    }

    // Função para lidar com a morte do objeto
    void Die()
    {
        Minions minionComponent = GetComponent<Minions>();

        Torre torreComponent = GetComponent<Torre>();

        if (minionComponent != null)
        {
            minionComponent.OnDeath(); // Chama a lógica de recompensa em ouro dos inimigos, se aplicável
        }

        if (torreComponent != null)
        {
            torreComponent.GrantGoldToNearbyEnemies();
        }
        
        Debug.Log(gameObject.name + " morreu.");
        Destroy(gameObject); // Destroi o objeto
    }
}
