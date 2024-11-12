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

    // Método para calcular o dano causado com o buff
    private void Attack()
    {
        int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
        Debug.Log($"Dano causado: {damageToDeal}");
        // Aqui você pode implementar lógica para encontrar inimigos próximos e aplicar dano a eles
    }

    public void ApplyBuff(float multiplier)
    {
        currentSpeed *= multiplier;
        Debug.Log($"Buff de velocidade aplicado! Nova velocidade: {currentSpeed}");
    }

    public void RemoveBuff(float multiplier)
    {
        currentSpeed /= multiplier;
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
