using UnityEngine;

public class Movimento : MonoBehaviour
{
    public Rigidbody rb;
    public Transform cam;
    public LayerMask ground;
    public float velocidade, velocidadeMax, drag, jumpForce;
    bool esq, dir, frente, tras;
    bool grounded;

    void Update()
    {
        HandleInput();
        LimiterVelocity();
        HandleDrag();
        CheckGrounded();
        HandleJump(); // Colocando o pulo no Update para maior precisão
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void CheckGrounded()
    {
        // Verifica se o personagem está tocando o chão com um Raycast
        grounded = Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, .2f, ground);
    }

    void HandleDrag()
    {
        // Reduz a velocidade horizontal aplicando o drag
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) / (1 + drag / 100) + new Vector3(0, rb.velocity.y, 0);
    }

    void LimiterVelocity()
    {
        // Calcula a velocidade horizontal (sem o eixo Y, para ignorar gravidade)
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // Limita a velocidade horizontal se ela for maior que o valor máximo
        if (horizontalVelocity.magnitude > velocidadeMax)
        {
            // Normaliza a velocidade horizontal e multiplica pelo limite máximo
            Vector3 limitedVelocity = horizontalVelocity.normalized * velocidadeMax;

            // Define a nova velocidade, preservando o eixo Y para manter a influência da gravidade
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    void HandleMovement()
    {
        // Define a direção com base na rotação da câmera
        Quaternion direcao = Quaternion.Euler(0f, cam.rotation.eulerAngles.y, 0f);

        // Movimentação para a esquerda
        if (esq)
        {
            rb.AddForce(direcao * Vector3.left * velocidade);
            esq = false;
        }

        // Movimentação para a direita
        if (dir)
        {
            rb.AddForce(direcao * Vector3.right * velocidade);
            dir = false;
        }

        // Movimentação para trás
        if (tras)
        {
            rb.AddForce(direcao * Vector3.back * velocidade);
            tras = false;
        }

        // Movimentação para frente
        if (frente)
        {
            rb.AddForce(direcao * Vector3.forward * velocidade);
            frente = false;
        }
    }

    void HandleJump()
    {
        // Verifica se a tecla de pulo foi pressionada e o personagem está no chão
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Zera a velocidade vertical antes do pulo
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Adiciona a força do pulo
        }
    }

    void HandleInput()
    {
        // Verifica se o jogador está pressionando as teclas de movimento
        if (Input.GetKey(KeyCode.A))
            esq = true;
        if (Input.GetKey(KeyCode.W))
            frente = true;
        if (Input.GetKey(KeyCode.D))
            dir = true;
        if (Input.GetKey(KeyCode.S))
            tras = true;
    }
}
