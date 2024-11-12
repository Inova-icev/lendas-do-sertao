using UnityEngine;

public class Player : MonoBehaviour
{
<<<<<<< HEAD
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
=======
    public Animator anim;
    public int gold = 0;
    public float speed = 5f; // Velocidade base do jogador
    private float currentSpeed;
    private Vida vidaComponent; // Referência ao componente Vida
    private float damageMultiplier = 1f; // Multiplicador de dano inicial

    public int baseDamage = 10; // Dano base do jogador

    void Start()
    {
        anim = GetComponent<Animator>();
        currentSpeed = speed; // Inicialize com a velocidade base
        vidaComponent = GetComponent<Vida>(); // Obtém o componente Vida
>>>>>>> 70ded898909b97da8024a840adf3b9af57b21863
    }

    void Update()
    {
<<<<<<< HEAD
        if (controlEnabled)
        {
            Move();
            CheckGround();

            // Pular com a tecla Espaço
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
=======
        // Movimento simples de exemplo
        float move = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Correndo", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetBool("Correndo", false);
>>>>>>> 70ded898909b97da8024a840adf3b9af57b21863
        }
    }

<<<<<<< HEAD
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
=======
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("Atacando", true);
            Attack(); // Exemplo de ataque
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("Atacando", false);
        }
>>>>>>> 70ded898909b97da8024a840adf3b9af57b21863
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
        if (vidaComponent != null)
        {
            vidaComponent.TakeDamage(damageAmount); // Passa o dano para o componente Vida
        }
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
