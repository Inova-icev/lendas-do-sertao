using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgoniaPersistente : MonoBehaviour
{
    public float[] damagePerSecond = { 15f, 25f, 35f }; 
    public float damageMultiplier = 0.1f; 
    public float duration = 3f; 
    public float armorReduction = 0.05f; 
    public float largeMonsterBonus = 0.5f; 
    public float reapplyCooldown = 5f; 

    private Dictionary<Collider, int> agonyStacks = new Dictionary<Collider, int>(); 
    private Dictionary<Collider, float> lastAppliedTime = new Dictionary<Collider, float>(); 

    private void Start()
    {
        
    }

        public void ApplyAgony(Collider enemy, float abilityPower)
    {
        
        if (enemy.CompareTag("Enemy"))
        {
            if (!agonyStacks.ContainsKey(enemy))
            {
                agonyStacks[enemy] = 0; 
                lastAppliedTime[enemy] = Time.time; 
            }

            
            if (Time.time - lastAppliedTime[enemy] >= reapplyCooldown)
            {
                StartCoroutine(ApplyAgonyEffect(enemy, abilityPower));
                lastAppliedTime[enemy] = Time.time; 
            }
        }
    }

    private IEnumerator ApplyAgonyEffect(Collider enemy, float abilityPower)
    {
        int currentStacks = agonyStacks[enemy];
        
        
        if (currentStacks < 3)
        {
            currentStacks++;
            agonyStacks[enemy] = currentStacks; 

            
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.ReduceArmor(armorReduction); 
            }

            
            float damage = damagePerSecond[currentStacks - 1] + (abilityPower * damageMultiplier);

            
            for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
            {
                
                float finalDamage = enemy.CompareTag("LargeMonster") || enemy.CompareTag("EpicMonster") 
                    ? damage * (1 + largeMonsterBonus) 
                    : damage;

                enemyScript.TakeDamage(finalDamage * Time.deltaTime);
                yield return null; 
            }
        }

        if (agonyStacks[enemy] >= 3)
        {
            agonyStacks[enemy] = 0; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f); 
    }
}
