using UnityEngine;

public class Minions : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do minion
    public float fallSpeed = 2f; // Velocidade para descer caso não haja chão
    public float groundCheckDistance = 1f; // Distância para checar o chão abaixo
    public float attackRange = 1.5f; // Alcance de ataque
    public int attackDamage = 10; // Dano de ataque
    public float attackCooldown = 2f; // Tempo de recarga entre ataques
    public string enemyTag; // Tag dos inimigos que este minion deve atacar
    public LayerMask groundLayer; // Camada do chão
    public Transform waypoint; // Ponto de caminho que o minion deve alcançar

    private Transform target; // Alvo atual do minion
    private float attackTimer = 0f; // Temporizador de ataque
    private Rigidbody2D rb;
    private Vida vidaComponent; // Referência ao componente Vida para o próprio minion

    private float goldRewardRadius = 5f;
    private int goldReward = 50;

    private float findTargetCooldown = 1f; // Tempo entre buscas
    private float findTargetTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaComponent = GetComponent<Vida>();

        // Configurar o enemyTag com base na tag do minion
        if (CompareTag("Left"))
        {
            enemyTag = "Right";
        }
        else if (CompareTag("Right"))
        {
            enemyTag = "Left";
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tem uma tag desconhecida. enemyTag não foi configurado.");
        }
        // Configura o waypoint inicial
        if (CompareTag("Left"))
        {
            GameObject waypointObject = GameObject.Find("WaypointRight");
            if (waypointObject != null)
            {
                waypoint = waypointObject.transform;
            }
        }
        else if (CompareTag("Right"))
        {
            GameObject waypointObject = GameObject.Find("WaypointLeft");
            if (waypointObject != null)
            {
                waypoint = waypointObject.transform;
            }
        }

        FindTarget(); // Encontra o alvo inicial ao começar
    }

    void Update()
    {
        // Atualiza temporizador de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Se não houver um alvo, procure um
        if (target == null)
        {
            FindTarget();
            if (waypoint != null)
            {
                MoveTowardsWaypoint();
            }
        }
        else
        {
            // Se o alvo estiver fora do alcance, mova-se em direção a ele
            if (Vector2.Distance(transform.position, target.position) > attackRange)
            {
                MoveTowardsTarget();
            }
            else
            {
                // Ataque o alvo se estiver no alcance
                Attack();
            }
        }

        // Verifica se o minion deve cair
        if (!IsGroundBelow())
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null)
        {
            Vector2 position = Vector2.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);
            rb.MovePosition(position); // Move o Rigidbody2D em direção ao waypoint
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        }
    }

    
    // Procurar o alvo mais próximo (minion ou torre inimiga)
    void FindTarget()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Collider2D closestTarget = null;

        foreach (Collider2D potentialTarget in potentialTargets)
        {
            // Verifica se o alvo é inimigo (minion ou torre)
            if ((CompareTag("Left") && potentialTarget.CompareTag("Right")) ||
                (CompareTag("Right") && potentialTarget.CompareTag("Left")))
            {
                Vida vidaComponent = potentialTarget.GetComponent<Vida>();
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

        // Define o alvo se encontrado
        if (closestTarget != null)
        {
            target = closestTarget.transform;
            Debug.Log($"{gameObject.name} encontrou o alvo {closestTarget.name}");
        }
    }

    // Método de ataque
    private void Attack()
    {
        // Verifica se o cooldown acabou
        if (attackTimer <= 0f && target != null)
        {
            Vida targetVida = target.GetComponent<Vida>();
            if (targetVida != null)
            {
                targetVida.TakeDamage(attackDamage); // Aplica o dano
                Debug.Log($"{gameObject.name} atacou {target.name} causando {attackDamage} de dano.");
                attackTimer = attackCooldown; // Reinicia o cooldown
            }
        }
    }

    // Verifica se há chão abaixo do minion
    private bool IsGroundBelow()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.red); // Visualize o Raycast para depuração
        return hit.collider != null;
    }

    // Função para receber dano
    public void TakeDamage(int damage)
    {
        if (vidaComponent != null)
        {
            vidaComponent.TakeDamage(damage);
        }
    }

    public void OnDeath()
    {
        // Encontre todos os jogadores inimigos em um raio
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, goldRewardRadius);
        foreach (Collider2D enemy in enemiesInRange)
        {
            // Verifica se o objeto é um jogador inimigo
            Player player = enemy.GetComponent<Player>();
            if (player != null && enemy.CompareTag(enemyTag))
            {
                player.GainGold(goldReward); // Distribui ouro ao jogador inimigo
                Debug.Log($"Jogador {player.name} ganhou {goldReward} de ouro pela morte do {gameObject.name}.");
            }
        }

        // Destroi o minion
        Destroy(gameObject);
    }
}
