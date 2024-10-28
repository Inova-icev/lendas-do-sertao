using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeuDaEscuridao : MonoBehaviour
{
    public float auraRadius = 5f; 
    public float damageReduction = 0.10f; 
    public float attackSpeedReduction = 0.10f; 
    public LayerMask enemyLayer; 

    private void Update()
    {
        
        Collider[] enemies = Physics.OverlapSphere(transform.position, auraRadius, enemyLayer);
        foreach (Collider enemy in enemies)
        {
            ApplyEffect(enemy.gameObject);
        }
    }

    private void ApplyEffect(GameObject enemy)
    {
        
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>(); 

        if (enemyStats != null)
        {
            
            enemyStats.ReduceDamage(damageReduction);
            enemyStats.ReduceAttackSpeed(attackSpeedReduction);
        }
    }

    private void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }
}
