using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // Velocidade base do jogador
    public float jumpForce = 7f; // Força do pulo
    public float fallMultiplier = 2.5f; // Multiplicador para aumentar a velocidade de queda
    public float lowJumpMultiplier = 2f; // Multiplicador para uma queda mais suave ao soltar o botão de pulo
    public LayerMask groundLayer; // Camada do chão
    private Rigidbody2D rb;
    private bool isGrounded;

    public int gold = 0; // Quantidade de ouro do jogador

    private Vida vida; // Referência ao componente Vida

    private bool controlEnabled = true; // Controle se o jogador pode se mover

    private float currentSpeed;
    private float damageMultiplier = 1f; // Multiplicador de dano inicial

    public int baseDamage = 10; // Dano base do jogador

    void Start()
    {
        currentSpeed = speed; // Inicialize com a velocidade base
        vida = GetComponent<Vida>(); // Obtém o componente Vida
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (controlEnabled)
        {
            Move();
            CheckGround();

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Pulo detectado!"); // Verificação
                Jump();
            }

            // Aplica multiplicador de queda para tornar a descida mais rápida
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            // Aplica um multiplicador mais leve para pular se o jogador liberar a tecla de pulo
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    void Move()
    {
        float moveX = 0f;

        // Movimentação horizontal com A e D ou setas esquerda/direita
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1f;
        }

        // Aplica a velocidade ao Rigidbody2D para movimento horizontal
        rb.velocity = new Vector2(moveX * currentSpeed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
    }

    void CheckGround()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.5f); // Ajuste o valor para baixo conforme necessário
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, groundLayer);
        isGrounded = hit.collider != null;

        // Debug para visualizar o Raycast no Editor
        Debug.DrawRay(origin, Vector2.down * 1f, Color.red);
    }

    // Método para calcular o dano causado com o buff
    private void Attack()
    {
        int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
        Debug.Log($"Dano causado: {damageToDeal}");
        // Aqui você pode implementar lógica para encontrar inimigos próximos e aplicar dano a eles
    }

    public void ApplyBuff(float multiplier)
    {
        currentSpeed = speed * multiplier; // Aplica o multiplicador ao valor base
        Debug.Log($"Buff de velocidade aplicado! Nova velocidade: {currentSpeed}");
    }

    public void RemoveBuff(float multiplier)
    {
        currentSpeed = speed; // Reverte para o valor base
        Debug.Log($"Buff de velocidade removido. Velocidade de volta ao normal: {currentSpeed}");
    }

    // Aplicar um buff de dano
    public void ApplyDamageBuff(float multiplier)
    {
        damageMultiplier *= multiplier;
        Debug.Log($"Buff de dano aplicado! Multiplicador de dano atual: {damageMultiplier}");
    }

    // Remover o buff de dano
    public void RemoveDamageBuff(float multiplier)
    {
        damageMultiplier /= multiplier;
        Debug.Log($"Buff de dano removido. Multiplicador de dano de volta ao normal: {damageMultiplier}");
    }

    public void TakeDamage(int damageAmount)
    {
        if (vida != null)
        {
            vida.TakeDamage(damageAmount); // Passa o dano para o componente Vida
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }
}
