using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamasDaMaldição : MonoBehaviour
{
    public float damagePercent = 0.01f; 
    public float additionalDamagePerAbilityPower = 0.005f; 
    public float damageDuration = 3f;
    public float healingReduction = 0.4f; 
    public float movementSpeedBonus = 0.1f; 
    public float combatDurationThreshold = 10f; 

    private float combatStartTime;
    private bool inCombat = false;
    private float originalMovementSpeed;
    private CharacterController characterController; 
    private float abilityPower; 

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalMovementSpeed = characterController.speed; 
        abilityPower = 0; 
    }

    private void Update()
    {
        
        if (inCombat && Time.time - combatStartTime >= combatDurationThreshold)
        {
            
            characterController.speed = originalMovementSpeed * (1 + movementSpeedBonus);
        }
    }

    
    public void OnAttack(Collider enemy)
    {
        
        if (enemy.CompareTag("Enemy"))
        {
            
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                float damage = CalculateDamage(enemyScript.maxHealth);
                enemyScript.TakeDamage(damage);

                
                enemyScript.ApplyHealingReduction(healingReduction, damageDuration);

                
                if (!inCombat)
                {
                    inCombat = true;
                    combatStartTime = Time.time;
                    StartCoroutine(DamageOverTime(enemyScript));
                }
            }
        }
    }

    private float CalculateDamage(float maxHealth)
    {
        
        float additionalDamage = (abilityPower / 100) * additionalDamagePerAbilityPower;
        return maxHealth * (damagePercent + additionalDamage);
    }

    private IEnumerator DamageOverTime(Enemy enemy)
    {
        float totalDamage = CalculateDamage(enemy.maxHealth);
        float damagePerSecond = totalDamage / damageDuration;

        for (float elapsed = 0; elapsed < damageDuration; elapsed += Time.deltaTime)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            yield return null; 
        }

        
        ResetMovementSpeed();
    }

    private void ResetMovementSpeed()
    {
        characterController.speed = originalMovementSpeed;
        inCombat = false; 
    }

    
    public void UpdateAbilityPower(float power)
    {
        abilityPower = power;
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f); 
    }
}
