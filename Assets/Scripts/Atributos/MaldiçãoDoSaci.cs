using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaldiçãoDoSaci : MonoBehaviour
{
    public float[] precisionReduction = { 0.05f, 0.10f, 0.15f, 0.20f }; 
    public float[] damageIncrease = { 0.05f, 0.10f, 0.15f, 0.20f };
    public float duration = 3f; 

    private Dictionary<Collider, Coroutine> markedEnemies = new Dictionary<Collider, Coroutine>();

    
    public void ApplyConfusion(Collider enemy, int abilityLevel)
    {
        
        if (enemy.CompareTag("Enemy"))
        {
            if (!markedEnemies.ContainsKey(enemy))
            {
                
                Coroutine confusionCoroutine = StartCoroutine(ApplyConfusionEffect(enemy, abilityLevel));
                markedEnemies[enemy] = confusionCoroutine; 
            }
        }
    }

    private IEnumerator ApplyConfusionEffect(Collider enemy, int abilityLevel)
    {
        
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ReduceAccuracy(precisionReduction[abilityLevel - 1]); 
            enemyScript.ApplyDamageIncrease(damageIncrease[abilityLevel - 1]); 

            yield return new WaitForSeconds(duration); 

            
            enemyScript.RestoreAccuracy(precisionReduction[abilityLevel - 1]); 
            enemyScript.RemoveDamageIncrease(damageIncrease[abilityLevel - 1]); 
        }

        
        markedEnemies.Remove(enemy);
    }

    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Inimigos Marcados: " + markedEnemies.Count);
    }
}
