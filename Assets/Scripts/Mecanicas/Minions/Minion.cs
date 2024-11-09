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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Inicializa o Rigidbody2D
        vidaComponent = GetComponent<Vida>(); // Inicializa o componente Vida

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
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null)
        {
            Vector2 direction = (waypoint.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            Debug.Log($"{gameObject.name} movendo-se para o waypoint em {waypoint.position}");
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
                }
                attackTimer = attackCooldown; // Reinicia o cooldown do ataque
            }
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
            isMovingToWaypoint = false;
            Debug.Log($"{gameObject.name} encontrou o inimigo {nearestEnemy.name} a uma distância de {shortestDistance}");
        }
    }

    // Verifica se há chão abaixo do minion
    private bool IsGroundBelow()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
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
}
