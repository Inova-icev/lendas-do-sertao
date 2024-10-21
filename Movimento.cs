using UnityEngine;

public class Movimento : MonoBehaviour
{
    public Rigidbody rb;
    public Transform cam;
    public LayerMask ground;
    public float velocidade, velocidadeMax, drag, jumpForce;
    bool esq, dir, frente, tras;
    bool grounded, jump;

    void Update()
    {
        HandleInput();
        LimiterVelocity();
        HandleDrag();
        CheckGrounded();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void CheckGrounded()
    {
        grounded = Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, .2f, ground);
    }

    void HandleDrag()
    {
        rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.z) / (1+drag/100) + new Vector3(0, rb.velocity.y, 0);
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
        if (jump)
        {   
            transform.position += Vector3.up * .1f;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
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
         if (Input.GetKey(KeyCode.Space))
            jump = true;
    }
}
