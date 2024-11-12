using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // Velocidade de movimento
    public float jumpForce = 7f; // Força do pulo
    public LayerMask groundLayer; // Camada do chão
    private Rigidbody2D rb;
    private bool isGrounded;

    public int gold = 0; // Quantidade de ouro do jogador

    private Vida vida; // Referência ao componente Vida

    private bool controlEnabled = true; // Controle se o jogador pode se mover

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vida = GetComponent<Vida>(); // Inicializa a referência ao componente Vida
    }

    void Update()
    {
        if (controlEnabled)
        {
            Move();
            CheckGround();

            // Pular com a tecla Espaço
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void CheckGround()
    {
        // Lança um raio para baixo para verificar se há chão abaixo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }

    public void TakeDamage(int damageAmount)
    {
        if (vida != null)
        {
            vida.TakeDamage(damageAmount); // Usa o método do componente Vida
        }
    }
}
