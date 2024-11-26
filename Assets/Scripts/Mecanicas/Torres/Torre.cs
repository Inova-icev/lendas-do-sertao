using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : MonoBehaviour
{
    public float range = 5f; // Alcance da torre
    public int damage = 10; // Dano causado por ataque da torre
    public float attackCooldown = 1f; // Tempo entre os ataques
    private float lastAttackTime;
    public string enemyTag;

    private Transform target;
    private float attackTimer = 0f;

    public float destructionRadius = 10f; // Raio para distribuir ouro ao destruir
    public int goldReward = 100; // Quantidade de ouro dada aos inimigos

    private Vida vida; // Referência ao componente Vida

    private float findTargetCooldown = 0.5f; // Intervalo para procurar alvos
    private float findTargetTimer = 0f;

    void Start()
    {
        vida = GetComponent<Vida>(); // Inicializa a referência ao componente Vida
    }

    void Update()
    {
        // Atualiza o temporizador de busca
        findTargetTimer -= Time.deltaTime;

        // Reduz o temporizador de ataque
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Busca novos alvos periodicamente
        if (findTargetTimer <= 0f)
        {
            target = DetectEnemyInRange()?.transform;
            findTargetTimer = findTargetCooldown;
        }

        // Ataca se houver um alvo e o cooldown de ataque tiver terminado
        if (target != null && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    // Método para detectar o inimigo mais próximo dentro do alcance
    Collider2D DetectEnemyInRange()
    {
        // Detecta todos os objetos em um círculo de colisão ao redor da torre
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        float closestDistance = Mathf.Infinity;
        Collider2D closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            // Filtra os inimigos pela tag com base na tag da torre
            if ((CompareTag("Left") && hit.CompareTag("Right")) || (CompareTag("Right") && hit.CompareTag("Left")))
            {
                // Calcula a distância do inimigo até a torre
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit;
                }
            }
        }
        return closestEnemy; // Retorna o inimigo mais próximo no alcance
    }

    // Método de ataque que causa dano ao inimigo
    void Attack()
    {
        if (target != null)
        {
            Vida targetVida = target.GetComponent<Vida>();
            if (targetVida != null)
            {
                targetVida.TakeDamage(damage); // Aplica o dano
                Debug.Log($"{gameObject.name} atacou {target.name} e causou {damage} de dano.");
            }
            else
            {
                Debug.Log($"O alvo {target.name} não possui um componente Vida.");
            }
        }
    }


    // Método para conceder ouro aos inimigos próximos
    public void GrantGoldToNearbyEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, destructionRadius);
        foreach (Collider2D enemy in enemiesInRange)
        {
            Player player = enemy.GetComponent<Player>();
            if (player != null && enemy.CompareTag(enemyTag))
            {
                player.GainGold(goldReward);
                Debug.Log($"{enemy.name} recebeu {goldReward} de ouro pela destruição da torre.");
            }
        }
    }


    // Método para a torre receber dano
    public void TakeDamage(int amount)
    {
        if (vida != null)
        {
            vida.TakeDamage(amount); // Usa o componente Vida para gerenciar o dano
            Debug.Log($"Tomou {amount} de dano.");
        }
    }

    // Método para desenhar o alcance da torre no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range); // Desenha o alcance da torre no editor
    }
}
