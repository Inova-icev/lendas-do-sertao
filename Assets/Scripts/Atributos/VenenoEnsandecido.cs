using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenenoEnsandecido : MonoBehaviour
{
    public float minDamage = 15f;     public float maxDamage = 90f; 
    public float duration = 2.5f; 
    public float healingReduction = 0.25f; 
    public int maxStacks = 2; 

    private float damagePerSecond; 
    private Dictionary<Collider, int> poisonStacks = new Dictionary<Collider, int>(); 

    private void Start()
    {
        
        damagePerSecond = Mathf.Lerp(minDamage, maxDamage, GetComponent<CharacterStats>().level / 100f); 
    }

    
    public void OnAttack(Collider enemy)
    {
        
        if (enemy.CompareTag("Enemy"))
        {
            if (!poisonStacks.ContainsKey(enemy))
            {
                poisonStacks[enemy] = 0; 
            }

            if (poisonStacks[enemy] < maxStacks)
            {
                
                StartCoroutine(ApplyPoison(enemy));
                poisonStacks[enemy]++;
            }
        }
    }

    private IEnumerator ApplyPoison(Collider enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            
            enemyScript.ApplyHealingReduction(healingReduction, duration);

            float totalDamage = 0f;
            float damagePerTick = damagePerSecond * (duration / 5f); 
            for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
            {
                
                enemyScript.TakeDamage(damagePerTick * Time.deltaTime);
                totalDamage += damagePerTick * Time.deltaTime;
                yield return null; 
            }

            
            if (poisonStacks[enemy] >= maxStacks)
            {
                poisonStacks[enemy] = 0; 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 3f); 
    }
}
