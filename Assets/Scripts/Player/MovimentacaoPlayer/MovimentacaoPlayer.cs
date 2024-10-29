using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;  // Default player speed
    public bool isBuffActive = false;  // Indicates if the buff is active
    private Color originalColor;  // Stores the original color of the player
    private Vector3 targetPosition;  // Posição de destino para o jogador
    private bool isMoving = false;   // Verifica se o jogador está se movendo
    private float initialYPosition;  // Para armazenar a posição Y inicial do jogador
    private Renderer playerRenderer; // Referência ao Renderer do jogador

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
        targetPosition = transform.position;
        initialYPosition = transform.position.y; // Armazena a posição Y inicial do jogador

        // Inicializa o Renderer e armazena a cor original do jogador
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color; // Armazena a cor original
        }
    }

    void Update()
    {
        // Detecta clique do botão direito do mouse
        if (Input.GetMouseButtonDown(1))
        {
            // Cria um raio da câmera até o ponto onde o mouse foi clicado
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se o raio colidiu com algo
            if (Physics.Raycast(ray, out hit))
            {
                // Define a posição de destino como o ponto onde o raio colidiu, mantendo a posição Y original
                targetPosition = new Vector3(hit.point.x, initialYPosition, hit.point.z);
                isMoving = true;
            }
        }

        // Movimenta o jogador em direção ao ponto de destino
        if (isMoving)
        {
            // Move o jogador suavemente em direção ao ponto de destino, mantendo a posição Y constante
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Para de se mover quando o jogador chega ao destino
            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    // Método para mudar a cor do jogador
    public void ChangeColor(Color newColor)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = newColor; // Aplica a nova cor
        }
    }

    // Método para restaurar a cor original do jogador
    public void RestoreOriginalColor()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor; // Restaura a cor original
        }
    }
}
