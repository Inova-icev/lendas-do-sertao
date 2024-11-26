using UnityEngine;

public class Nexus : MonoBehaviour
{
    public int health = 100; 
    public string teamName; 

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Nexus {teamName} recebeu dano! Vida restante: {health}");

        if (health <= 0)
        {
            health = 0;
            Debug.Log($"Nexus {teamName} destruído!");

            if (gameManager != null)
            {
                gameManager.photonView.RPC("NexusDestroyed", RpcTarget.All, teamName);
            }
        }
    }
}
