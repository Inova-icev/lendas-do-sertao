using UnityEngine;
using System.Collections.Generic;
using TMPro;
using ManagmentScripts;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public LayerMask groundLayer;
    public LayerMask platformLayer; // Nova camada para as plataformas que podem ser atravessadas
    private Rigidbody2D rb;
    private bool isGrounded;

    public int gold = 0;

    private Vida_Player vida;

    private bool controlEnabled = true;

    private float currentSpeed;
    private float damageMultiplier = 1f;

    public int baseDamage = 10;

    public List<Item> items;

    private bool isSpeedBuffed = false;
    private bool isDamageBuffed = false;

    public float attackRange = 1f;
    public float attackCooldown = 0.5f; // Intervalo entre os ataques

    private float lastAttackTime = 0f;
    public float acceleration = 20f; // Aceleração ao mover
    public float deceleration = 10f; // Desaceleração ao parar

    public float jumpHoldTime = 0.5f; // Tempo máximo de sustentação do pulo
    private float jumpHoldTimer = 0f;

    private float buffStartTimeDamage = 0f;
    private float buffStartTimeSpeed = 0f;
    private float buffDuration = 10f;

    private PanelManager panelManager;

    void Start()
    {
        panelManager = FindAnyObjectByType<PanelManager>();
        gameObject.tag = panelManager.teamChoiceTag;
        if(panelManager.teamChoiceTag=="Right"){
            gameObject.layer = 9;
        }
        else{
            gameObject.layer = 8;

        }
        vida = GetComponent<Vida_Player>();
        rb = GetComponent<Rigidbody2D>();

        Vida_Player.OnPlayerDeath += HandleDeath;
    }

    void OnDestroy()
    {
        // Remove a inscrição no evento para evitar erros
        Vida_Player.OnPlayerDeath -= HandleDeath;
    }

    private void HandleDeath(GameObject deadPlayer)
    {
        // Verifica se o evento é para este jogador
        if (deadPlayer == gameObject)
        {
            Debug.Log($"{gameObject.name} foi derrotado!");
            controlEnabled = false; // Desativa os controles do jogador
        }
    }

    void Update()
    {
        if (controlEnabled)
        {
            // Substituí a chamada direta para Move pela lógica integrada no próprio Update
            float moveX = Input.GetAxisRaw("Horizontal"); // Obtém entrada de movimento horizontal
            float targetSpeed = moveX * speed;

            // Aceleração e desaceleração suaves
            if (moveX != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            }

            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

            CheckGround();

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Pulo detectado!");
                Jump();
                TryJumpThroughPlatform();
            }

            HandleFalling();

            if (Input.GetKey(KeyCode.S))
            {
                DropThroughPlatform();
            }

            if (Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse
            {
                Attack();
            }

            if (isDamageBuffed && Time.time - buffStartTimeDamage >= buffDuration)
            {
                RemoveDamageBuff(2f);
            }

            if (isSpeedBuffed && Time.time - buffStartTimeSpeed >= buffDuration)
            {
                RemoveBuff();
            }
        }
    }


    void Move()
    {
        // Determina a direção do movimento com base nas entradas do jogador
        float moveX = Input.GetAxisRaw("Horizontal"); // -1 para esquerda, 1 para direita, 0 para parado

        // Calcula a velocidade alvo com base na entrada do jogador e na velocidade atual
        float targetSpeed = moveX * speed;

        // Suaviza a transição entre parada e movimento
        if (moveX != 0)
        {
            // Aceleração para a direção alvo
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Desaceleração para 0 quando não há entrada de movimento
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Atualiza a velocidade do rigidbody mantendo a componente Y
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
    }


    void Jump()
    {
        if (jumpHoldTimer < jumpHoldTime)  // Verifica se o jogador está pressionando o botão de pulo
        {
            jumpHoldTimer += Time.deltaTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            jumpHoldTimer = jumpHoldTime;  // Limita o tempo máximo de pulo
        }
    }

    void CheckGround()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, groundLayer);

        // Raycasts extras podem ajudar a garantir a detecção correta
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.25f, transform.position.y - 0.5f), Vector2.down, 0.5f, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.25f, transform.position.y - 0.5f), Vector2.down, 0.5f, groundLayer);

        isGrounded = hit.collider != null || hitLeft.collider != null || hitRight.collider != null;
    }


    void HandleFalling()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }


    void TryJumpThroughPlatform()
    {
        // Ajustar a posição de origem para ser um pouco acima do topo do colisor do personagem
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y / 2);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 3.5f, platformLayer);
        if (hit.collider != null)
        {
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            Invoke("ReactivateCollider", 0.8f); // Reativa o colisor rapidamente
        }
        else
        {
            Debug.Log("Nenhuma plataforma detectada acima para subir");
        }
    }


    void DropThroughPlatform()
    {
        // Obtém a posição ligeiramente abaixo do centro do jogador para detectar plataformas
        Vector2 feetPosition = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(feetPosition, Vector2.down, 1f, platformLayer);

        // Verifica se o jogador está sobre uma plataforma que é da camada 6
        if (hit.collider != null && hit.collider.gameObject.layer == 6)
        {
            Collider2D collider = GetComponent<Collider2D>();
            // Desativa temporariamente o colisor do personagem
            collider.enabled = false;
            Invoke("ReactivateCollider", 0.2f); // Reativa o colisor após 0.5 segundos
        }
        else
        {
            Debug.Log("Não está em uma plataforma que permite descer.");
        }
    }

    void ReactivateCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }
    // Método para calcular o dano causado com o buff

    public void ApplyBuff(float multiplier)
    {
        Debug.Log($"Tentando aplicar buff: Atualmente Buffed = {isSpeedBuffed}");
        if (!isSpeedBuffed)
        {
            currentSpeed = speed * multiplier;
            isSpeedBuffed = true;
            buffStartTimeSpeed = Time.time;
            Debug.Log($"Buff aplicado com sucesso: Velocidade Atual = {currentSpeed}");
        }
        else
        {
            Debug.Log("Buff não aplicado porque já está buffed.");
        }
    }

    public void RemoveBuff()
    {
        Debug.Log($"Tentando remover buff: Atualmente Buffed = {isSpeedBuffed}");
        if (isSpeedBuffed)
        {
            currentSpeed = speed;
            isSpeedBuffed = false;
            Debug.Log("Buff removido com sucesso.");
        }
        else
        {
            Debug.Log("Buff não removido porque não estava buffed.");
        }
    }

    public void ApplyDamageBuff(float multiplier)
    {
        if (!isDamageBuffed)
        {
            damageMultiplier *= multiplier;
            isDamageBuffed = true;
            buffStartTimeDamage = Time.time;
            Debug.Log($"Buff de dano aplicado! Multiplicador de dano atual: {damageMultiplier}");
        }
    }

    public void RemoveDamageBuff(float multiplier)
    {
        if (isDamageBuffed)
        {
            damageMultiplier /= multiplier;
            isDamageBuffed = false;
            Debug.Log($"Buff de dano removido. Multiplicador de dano de volta ao normal: {damageMultiplier}");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (vida != null)
        {
            vida.TakeDamageP(damageAmount); // Passa o dano para o componente Vida
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }

    public void BuyItem(Item item)
    {
        if (gold >= item.price)
        {
            gold -= item.price;
            items.Add(item);
            ApplyItemEffect(item);
            Debug.Log($"Você comprou {item.name}! Ouro restante: {gold}");
        }
        else
        {
            Debug.Log("Você não tem ouro suficiente para comprar este item.");
        }
    }

    public void ApplyItemEffect(Item item)
    {
        switch (item.type)
        {
            case ItemType.SpeedBuff:
                ApplyBuff(item.effectValue);
                break;
            case ItemType.DamageBuff:
                ApplyDamageBuff(item.effectValue);
                break;
            default:
                Debug.Log("Item sem efeito aplicável.");
                break;
        }
    }

    private void Attack()
    {
        // Verifica se o tempo de recarga entre os ataques passou
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time; // Atualiza o tempo do último ataque

            int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
            Debug.Log($"Dano causado: {damageToDeal}");

            // Encontrar e atacar o inimigo mais próximo (ou qualquer lógica de ataque que você tenha)
            FindTargetAndAttack();
        }
        else
        {
            Debug.Log("Aguardando recarga do ataque...");
        }
    }

    private void FindTargetAndAttack()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Collider2D closestTarget = null;

        foreach (Collider2D potentialTarget in potentialTargets)
        {
            if ((CompareTag("Left") && potentialTarget.CompareTag("Right")) ||
                (CompareTag("Right") && potentialTarget.CompareTag("Left")))
            {
                Vida_Player vidaComponent = potentialTarget.GetComponent<Vida_Player>();
                if (vidaComponent != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, potentialTarget.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = potentialTarget;
                    }
                }
            }
        }

        if (closestTarget != null)
        {
            // Aplica o dano no alvo
            Vida_Player targetVida = closestTarget.GetComponent<Vida_Player>();
            if (targetVida != null)
            {
                int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
                targetVida.TakeDamageP(damageToDeal);
                Debug.Log($"{gameObject.name} atacou {closestTarget.name} causando {damageToDeal} de dano");
            }
        }
    }
}
