using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maldição : MonoBehaviour
{
    public float baseAttackDamageIncrease = 5f; 
    public int maxStacks = 7; 
    public float lifeStealBonus = 0.1f; 

    private int currentStacks = 0; 
    private CharacterStats characterStats; 

    private void Start()
    {
        characterStats = GetComponent<CharacterStats>(); 
    }

    
    public void OnKill()
    {
        if (currentStacks < maxStacks)
        {
            currentStacks++; 
            ApplyDamageIncrease();
        }

        
        if (currentStacks >= maxStacks)
        {
            ApplyLifeSteal();
        }
    }

    private void ApplyDamageIncrease()
    {
        
        characterStats.attackDamage += baseAttackDamageIncrease; 
    }

    private void ApplyLifeSteal()
    {
        
        characterStats.lifeSteal += lifeStealBonus; 
    }

    
    public void ResetStacks()
    {
        currentStacks = 0; 
        characterStats.attackDamage -= baseAttackDamageIncrease * maxStacks; 
        characterStats.lifeSteal -= lifeStealBonus; 
    }

    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Acúmulos de Maldição: " + currentStacks);
        GUI.Label(new Rect(10, 30, 200, 20), "Dano de Ataque Atual: " + characterStats.attackDamage);
        GUI.Label(new Rect(10, 50, 200, 20), "Roubo de Vida Atual: " + characterStats.lifeSteal);
    }
}
