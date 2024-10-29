using UnityEngine;

public class Damage : MonoBehaviour
{
    public int attackDamage = 10; // Dano causado por ataque

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido tem o componente Health
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage); // Aplica dano
            Debug.Log(gameObject.name + " atacou " + collision.gameObject.name + " e causou " + attackDamage + " de dano.");
        }
    }
}
