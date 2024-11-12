using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : MonoBehaviour
{
    public float range = 5f; // Alcance da torre
    public int health = 500; // Vida da torre
    public int damage = 10; // Dano causado por ataque da torre
    public float attackCooldown = 1f; // Tempo entre os ataques
    private float lastAttackTime;

    // Tag do inimigo para detectar apenas inimigos com a tag correspondente
    public string enemyTag; // Mudança feita aqui para usar uma tag ao invés de layer

    private Transform target;
    private float attackTimer = 0f;

    void Update()
    {
        // Verifica se há um inimigo no alcance
        if (target != null && attackTimer <= 0f)
        {
            Attack(); // Chama o método de ataque
            attackTimer = attackCooldown; // Reinicia o cooldown do ataque
        }

        // Reduz o tempo do temporizador de ataque
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Código para detectar o inimigo (mantido simples)
        Collider2D detectedEnemy = DetectEnemyInRange();
        if (detectedEnemy != null)
        {
            target = detectedEnemy.transform; // Atribui o alvo detectado
        }
        else
        {
            target = null; // Se nenhum inimigo for encontrado
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
            Debug.Log($"Objeto detectado: {hit.gameObject.name}");
            // Filtra os inimigos pela tag com base na tag da torre
            if ((CompareTag("Right") && hit.CompareTag("Left")) || (CompareTag("Left") && hit.CompareTag("Right")))
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

        if (closestEnemy != null)
        {
            Debug.Log($"Inimigo mais próximo encontrado: {closestEnemy.gameObject.name} a {closestDistance} unidades de distância.");
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

    // Método para a torre receber dano
    public void TakeDamage(int amount)
    {
        health -= amount; // Reduz a vida da torre
        Debug.Log($"Torre recebeu {amount} de dano. Vida restante: {health}");

        if (health <= 0)
        {
            DestroyTower(); // Destrói a torre se a vida chegar a zero
        }
    }

    // Método para destruir a torre
    void DestroyTower()
    {
        // Aqui você pode adicionar uma animação ou som de destruição
        Debug.Log("Torre destruída!");
        Destroy(gameObject); // Remove a torre da cena
    }

    // Método para desenhar o alcance da torre no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range); // Desenha o alcance da torre no editor
    }
}
