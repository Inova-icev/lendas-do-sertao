using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset; // Deslocamento da câmera em relação ao jogador
    public float smoothness = 0.1f; // Suavidade do movimento da câmera
    private Transform player; // Jogador que a câmera segue

    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform; // Define o jogador para a câmera seguir
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calcula a posição desejada da câmera
            Vector3 desiredPosition = player.position + offset;

            // Suaviza a transição da câmera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);
        }
    }
}