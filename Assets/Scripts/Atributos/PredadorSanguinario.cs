using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredadorSanguinario : MonoBehaviour
{
    public float attackSpeedBonusLowHealth = 0.1f; 
    public float attackSpeedBonusCriticalHealth = 0.15f; 
    public float lifeStealLowHealth = 0.01f; 
    public float lifeStealCriticalHealth = 0.02f; 
    public float lowHealthThreshold = 0.25f;
    
    private CharacterStats characterStats; 
    private float originalAttackSpeed; 

    private void Start()
    {
        characterStats = GetComponent<CharacterStats>(); 
        originalAttackSpeed = characterStats.attackSpeed; 
    }

    private void Update()
    {
        
        Collider[] enemies = Physics.OverlapSphere(transform.position, 5f); 

        bool isNearLowHealthEnemy = false;

        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    
                    if (enemyScript.currentHealth <= enemyScript.maxHealth * lowHealthThreshold)
                    {
                        isNearLowHealthEnemy = true;

                        
                        characterStats.attackSpeed = originalAttackSpeed * (1 + attackSpeedBonusCriticalHealth);
                        characterStats.lifeSteal = lifeStealCriticalHealth; 
                        break; 
                    }
                }
            }
        }

        
        if (!isNearLowHealthEnemy)
        {
            characterStats.attackSpeed = originalAttackSpeed * (1 + attackSpeedBonusLowHealth); 
            characterStats.lifeSteal = lifeStealLowHealth;
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f); 
    }
}
