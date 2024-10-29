using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do minion
    public float attackRange = 1.5f; // Alcance de ataque
    public int attackDamage = 10; // Dano de ataque
    public float attackCooldown = 2f; // Tempo de recarga entre ataques
    public string enemyTag; // Tag dos inimigos que este minion deve atacar
    public Transform waypoint; // Ponto de caminho que o minion deve alcançar

    private Transform target; // Alvo atual do minion
    private float attackTimer = 0f; // Temporizador de ataque
    private NavMeshAgent agent; // Componente NavMeshAgent para movimentação
    private bool isMovingToWaypoint = true; // Indica se o minion está se movendo para o waypoint

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Inicializa o NavMeshAgent
        agent.speed = speed; // Define a velocidade do agente

        MoveTowardsWaypoint(); // Move para o waypoint no início
        FindTarget(); // Encontra o alvo inicial ao começar
    }

    void Update()
    {
        if (target == null)
        {
            if (isMovingToWaypoint && agent.remainingDistance < 0.1f)
            {
                // Verifica se o minion alcançou o waypoint
                isMovingToWaypoint = false;
            }
            FindTarget();
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                MoveTowardsTarget();
            }
            else
            {
                Attack();
            }
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null)
        {
            agent.SetDestination(waypoint.position); // Define o destino do NavMeshAgent para o waypoint
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            agent.SetDestination(target.position); // Define o destino do NavMeshAgent para o alvo
        }
    }

    void Attack()
    {
        if (attackTimer <= 0f)
        {
            if (target != null)
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage);
                    Debug.Log(gameObject.name + " atacou " + target.name + " e causou " + attackDamage + " de dano.");
                }
                attackTimer = attackCooldown;
            }
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Procura inimigos com a tag especificada
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
            isMovingToWaypoint = false; // Para de mover para o waypoint se encontrar um alvo
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualizar o alcance de ataque no Editor
    }
}
