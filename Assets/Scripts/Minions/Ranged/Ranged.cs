using UnityEngine;
using UnityEngine.AI;

public class MinionRanged : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do minion
    public float attackRange = 5f; // Alcance de ataque para lançar projéteis
    public int attackDamage = 5; // Dano causado pelos projéteis
    public float attackCooldown = 2f; // Tempo de recarga entre ataques
    public GameObject projectilePrefab; // Prefab do projétil a ser lançado
    public string enemyTag; // Tag dos inimigos que este minion deve atacar
    public Transform waypoint; // Ponto de caminho que o minion deve alcançar
    public Transform projectileSpawnPoint; // Ponto onde o projétil é instanciado

    private Transform target; // Alvo atual do minion
    private float attackTimer = 0f; // Temporizador de ataque
    private NavMeshAgent agent; // Componente NavMeshAgent para movimentação
    private bool isMovingToWaypoint = true; // Indica se o minion está se movendo para o waypoint

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        // Inicializa o movimento para o waypoint
        if (waypoint != null)
        {
            MoveTowardsWaypoint();
        }

        // Verifica se o minion está no NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("MinionRanged não está sobre o NavMesh! Verifique a posição inicial.");
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent não está no NavMesh!");
            return;
        }

        // Atualiza o temporizador de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Se não houver alvo, procure um novo alvo
        if (target == null)
        {
            if (isMovingToWaypoint && agent.remainingDistance < 0.1f)
            {
                isMovingToWaypoint = false;
            }

            FindTarget(); // Continuamente procura por um alvo se não tiver um
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                MoveTowardsTarget(); // Move em direção ao alvo apenas se estiver fora do alcance de ataque
            }
            else
            {
                StopAndAttack(); // Para o movimento e ataca se o alvo estiver dentro do alcance
            }
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null && agent.isOnNavMesh)
        {
            agent.SetDestination(waypoint.position);
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }

    void StopAndAttack(){
    // Para o movimento do agente
    agent.ResetPath();

    // Só ataca se o cooldown estiver pronto e houver um alvo
    if (attackTimer <= 0f && target != null)
    {
        // Instancia e dispara o projétil em direção ao alvo
        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + transform.forward;
            
            // Ajusta a rotação para mirar no alvo
            Quaternion rotationToTarget = Quaternion.LookRotation((target.position - spawnPosition).normalized);
            
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, rotationToTarget);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(target);
                projectileScript.attackDamage = attackDamage; // Define o dano do projétil
            }
            Debug.Log(gameObject.name + " lançou um projétil.");
        }
        else
        {
            Debug.LogWarning("Prefab de projétil não atribuído.");
        }
        attackTimer = attackCooldown; // Reseta o temporizador de ataque
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform; // Define o alvo
            isMovingToWaypoint = false; // Para de mover para o waypoint se encontrar um alvo
        }
        else
        {
            target = null; // Se não houver alvos dentro do alcance, reseta o alvo
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
