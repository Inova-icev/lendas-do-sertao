using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player; // Jogador que a câmera seguirá
    public Vector3 offset; // Deslocamento da câmera
    public float smoothness = 0.1f; // Suavidade do movimento da câmera

    private void Update()
    {
        // Se o jogador ainda não foi encontrado, tenta encontrá-lo
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("Jogador encontrado pela câmera!");
            }
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
}
