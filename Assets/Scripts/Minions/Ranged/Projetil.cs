using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Velocidade do projétil
    public int attackDamage = 10; // Dano causado pelo projétil
    private Transform target; // Alvo atual do projétil

    // Função para definir o alvo do projétil
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroi o projétil se o alvo não estiver mais disponível
            return;
        }

        // Move o projétil em direção ao alvo
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Função para lidar com a colisão do projétil
    void OnTriggerEnter(Collider other)
    {
        // Verifica se o projétil colidiu com o inimigo alvo
        if (other.transform == target)
        {
            HitTarget(other);
        }
    }

    // Função para lidar com o impacto do projétil
    void HitTarget(Collider targetCollider)
    {
        Health targetHealth = targetCollider.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage); // Aplica dano ao alvo
            Debug.Log(gameObject.name + " causou " + attackDamage + " de dano a " + targetCollider.gameObject.name); // Log do dano causado
        }

        Destroy(gameObject); // Destroi o projétil
    }
}