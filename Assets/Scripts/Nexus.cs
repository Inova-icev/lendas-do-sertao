using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Nexus : MonoBehaviourPunCallbacks
{
    public int health = 100; 
    public string teamName; 

    private GameManager gameManager; // aqui é onde a referência para o GameManager será armazenada

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
