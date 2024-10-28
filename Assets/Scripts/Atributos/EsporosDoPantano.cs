using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsporosDoPantano : MonoBehaviour
{
    public float baseDamageMin = 10f; 
    public float baseDamageMax = 60f; 
    public float damageOverTimeDuration = 3f; 
    public float magicResistReduction = 0.04f; 
    public int maxStacks = 3; 
    public float cooldown = 5f; 

    private Dictionary<Collider, Coroutine> infectedEnemies = new Dictionary<Collider, Coroutine>(); 

   
    public void ApplySpores(Collider enemy, int level, float abilityPower)
    {
        
        if (enemy.CompareTag("Enemy"))
        {
            if (!infectedEnemies.ContainsKey(enemy))
            {
                
                Coroutine sporesCoroutine = StartCoroutine(ApplySporesEffect(enemy, level, abilityPower));
                infectedEnemies[enemy] = sporesCoroutine; 
            }
        }
    }

    private IEnumerator ApplySporesEffect(Collider enemy, int level, float abilityPower)
    {
        
        float damagePerTick = Mathf.Lerp(baseDamageMin, baseDamageMax, (level - 1) / 9f) + (0.1f * abilityPower);
        int currentStacks = 0; 

       
        while (currentStacks < maxStacks)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                
                enemyScript.TakeDamage(damagePerTick); 

                
                enemyScript.ReduceMagicResist(magicResistReduction);

                currentStacks++; 

                
                yield return new WaitForSeconds(1f);
            }
        }

        
        yield return new WaitForSeconds(cooldown);

        
        infectedEnemies.Remove(enemy);
    }

    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Inimigos Infectados: " + infectedEnemies.Count);
    }
}
