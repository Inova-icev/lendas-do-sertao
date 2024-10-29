using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspiritodoCangaco : MonoBehaviour
{
    public float damageBonusPercentage = 0.02f; 
    public int maxStacks = 5; 
    public float stackDuration = 4f; 
    public float bonusDamagePercentage = 0.20f; 

    private int currentStacks = 0; 
    private float stackTimer = 0f; 

    
    private void Update()
    {
        
        if (currentStacks > 0)
        {
            stackTimer -= Time.deltaTime;
            if (stackTimer <= 0)
            {
                currentStacks = 0;
            }
        }
    }

    
    public void OnAbilityUsed(GameObject target)
    {
        
        if (currentStacks < maxStacks)
        {
            currentStacks++;
            stackTimer = stackDuration; 
        }

        if (currentStacks >= maxStacks)
        {
            ApplyBonusToNextAbility();
        }
    }

    private void ApplyBonusToNextAbility()
    {
        Debug.Log("Próxima habilidade causará " + (bonusDamagePercentage * 100) + "% de dano adicional!");

        currentStacks = 0; 
    }
}
