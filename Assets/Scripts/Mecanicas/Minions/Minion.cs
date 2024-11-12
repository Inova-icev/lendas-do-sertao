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
    private float findTargetTimer = 0f; // Temporizador de busca de alvo
    private bool isMovingToWaypoint = true; // Indica se o minion está se movendo para o waypoint
    private Rigidbody2D rb;
    private Vida vidaComponent; // Referência ao componente Vida para o próprio minion

    private float goldRewardRadius = 5f;
    private int goldReward = 50;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Inicializa o Rigidbody2D
        vidaComponent = GetComponent<Vida>(); // Inicializa o componente Vida

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
            Debug.LogWarning($"{gameObject.name} tem uma tag desconhecida. enemyTag não pode ser configurado corretamente.");
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

        if (waypoint != null)
        {
            Debug.Log($"{gameObject.name} está se movendo em direção ao waypoint {waypoint.name}");
            MoveTowardsWaypoint(); // Move para o waypoint no início
        }

        FindTarget(); // Encontra o alvo inicial ao começar
    }

    void Update()
    {
        // Movimentação e ataque
        if (target == null)
        {
            if (findTargetTimer <= 0f)
            {
                FindTarget();
                findTargetTimer = 1f;
            }
            else
            {
                findTargetTimer -= Time.deltaTime;
            }

            if (isMovingToWaypoint)
            {
                MoveTowardsWaypoint();
                if (waypoint != null && Vector2.Distance(transform.position, waypoint.position) < 0.1f)
                {
                    isMovingToWaypoint = false;
                    rb.velocity = Vector2.zero; // Parar movimento ao chegar no waypoint
                }
            }
        }
        else
        {
            // Checa a distância até o alvo
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                MoveTowardsTarget(); // Aproxima-se do alvo
            }
            else
            {
                Attack(); // Ataca o alvo quando está no alcance
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Verifica se deve descer ao sair de um obstáculo
        if (!IsGroundBelow())
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed); // Aplicação da queda
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Fixa no chão quando detectado
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null)
        {
            Vector2 position = Vector2.MoveTowards(transform.position, waypoint.position, speed * Time.fixedDeltaTime);
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


    void Attack()
    {
        if (attackTimer <= 0f)
        {
            if (target != null)
            {
                Vida targetVida = target.GetComponent<Vida>();
                if (targetVida != null)
                {
                    targetVida.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name} atacou {target.name} e causou {attackDamage} de dano.");
                    attackTimer = attackCooldown; // Reinicia o cooldown do ataque
                }
                else
                {
                    Debug.LogWarning($"{target.name} não possui um componente Vida.");}

            }
        }
    }
    void FindTarget()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Collider2D closestTarget = null;

        foreach (Collider2D potentialTarget in potentialTargets)
        {
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
        if (closestTarget != null)
        {
            target = closestTarget.transform;
            Debug.Log($"{gameObject.name} encontrou o alvo {closestTarget.name} a uma distância de {closestDistance}");
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