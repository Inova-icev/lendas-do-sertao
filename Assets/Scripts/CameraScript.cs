using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player; // Jogador que a câmera seguirá
    public Vector3 offset; // Deslocamento da câmera
    public float smoothness = 0.1f; // Suavidade do movimento da câmera
    public string[] playerTags = { "Left", "Right" }; // Tags para procurar os jogadores

    private void Update()
    {
        // Se o jogador ainda não foi encontrado, tenta encontrá-lo
        if (player == null)
        {
            FindPlayer();
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calcula a posição desejada
            Vector3 desiredPosition = player.position + offset;

            // Suaviza a transição
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);
        }
    }

    private void FindPlayer()
    {
        foreach (string tag in playerTags)
        {
            // Procura todos os objetos com a tag específica
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objectsWithTag)
            {
                // Verifica se o objeto tem o componente Player
                Player playerComponent = obj.GetComponent<Player>();
                if (playerComponent != null)
                {
                    player = obj.transform; // Define o Transform do jogador
                    Debug.Log($"Jogador com a tag '{tag}' encontrado pela câmera!");
                    return; // Para de procurar após encontrar o jogador
                }
            }
        }

        Debug.LogWarning("Nenhum jogador com o componente 'Player' foi encontrado.");
    }
}
