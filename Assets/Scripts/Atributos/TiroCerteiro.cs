using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiroCerteiro : MonoBehaviour
{
    public int attackThreshold = 3; 
    public float criticalChanceIncrease = 0.15f;
    public float effectDuration = 3f; 
    public int maxStacks = 3; 
    public float maxCriticalChance = 0.45f; 

    private Dictionary<Collider, int> targetAttackCount = new Dictionary<Collider, int>(); 

    
    public void OnBasicAttack(Collider target)
    {
        
        if (target.CompareTag("Enemy"))
        {
            if (!targetAttackCount.ContainsKey(target))
            {
                targetAttackCount[target] = 0; 
            }

            targetAttackCount[target]++;

            
            if (targetAttackCount[target] % attackThreshold == 0)
            {
                int currentStacks = targetAttackCount[target] / attackThreshold;

                
                if (currentStacks > maxStacks)
                {
                    currentStacks = maxStacks;
                }

               
                ApplyCriticalChance(target, currentStacks);

                
                StartCoroutine(ResetAttackCount(target, effectDuration));
            }
        }
    }

    private void ApplyCriticalChance(Collider target, int stacks)
    {
        
        float additionalCriticalChance = Mathf.Min(stacks * criticalChanceIncrease, maxCriticalChance);

        
        Enemy enemyScript = target.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.IncreaseCriticalChance(additionalCriticalChance);
        }
    }

    private IEnumerator ResetAttackCount(Collider target, float duration)
    {
        yield return new WaitForSeconds(duration);

        
        if (targetAttackCount.ContainsKey(target))
        {
            targetAttackCount.Remove(target);
        }
    }

    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Ataques por Alvo: " + targetAttackCount.Count);
    }
}
