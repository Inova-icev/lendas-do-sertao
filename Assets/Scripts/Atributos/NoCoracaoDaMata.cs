using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCoracaoDaMata : MonoBehaviour
{
    public float lifeRegenerationBonusLow = 0.30f; 
    public float lifeRegenerationBonusHigh = 0.35f;
    public float attackBonusLow = 0.35f; 
    public float attackBonusHigh = 0.40f; 
    public string vegetationTag = "Vegetacao"; 

    private bool isInVegetation = false; 
    private float originalLifeRegeneration; 
    private float originalAttack; 
    private CharacterStats stats; 

    private void Start()
    {
        stats = GetComponent<CharacterStats>(); 
        if (stats != null)
        {
            originalLifeRegeneration = stats.lifeRegeneration; 
            originalAttack = stats.attack; 
        }
    }

    private void Update()
    {
        
        if (isInVegetation)
        {
            ApplyBonuses(true); 
        }
        else
        {
            ApplyBonuses(false); 
        }
    }

    private void ApplyBonuses(bool isInHighVegetation)
    {
        if (stats != null)
        {
           
            if (isInHighVegetation)
            {
                stats.lifeRegeneration = originalLifeRegeneration * (1 + lifeRegenerationBonusHigh); 
                stats.attack = originalAttack * (1 + attackBonusHigh); 
            }
            else
            {
                stats.lifeRegeneration = originalLifeRegeneration * (1 + lifeRegenerationBonusLow); 
                stats.attack = originalAttack * (1 + attackBonusLow); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(vegetationTag)) 
        {
            isInVegetation = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(vegetationTag)) 
        {
            isInVegetation = false; 
        }
    }

    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Em Vegetação: " + isInVegetation);
    }
}
