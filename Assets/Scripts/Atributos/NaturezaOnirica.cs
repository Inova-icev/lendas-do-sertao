using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturezaOnirica : MonoBehaviour
{
    public float lifeRegenerationRate = 0.015f; 
    public float manaRegenerationRate = 0.01f; 
    public int manaGainOnNightmare = 20; 
    public string vegetationTag = "Vegetacao"; 
    private bool isInVegetation = false;
    private CharacterStats stats; 

    private void Start()
    {
        stats = GetComponent<CharacterStats>(); 
    }

    private void Update()
    {
        
        if (isInVegetation && stats != null)
        {
            Regenerate();
        }
    }

    private void Regenerate()
    {
        
        stats.currentLife += stats.maxLife * lifeRegenerationRate * Time.deltaTime; 
        stats.currentMana += stats.maxMana * manaRegenerationRate * Time.deltaTime; 

        
        stats.currentLife = Mathf.Clamp(stats.currentLife, 0, stats.maxLife);
        stats.currentMana = Mathf.Clamp(stats.currentMana, 0, stats.maxMana);
    }

    public void ApplyNightmareMark()
    {
        if (stats != null)
        {
            stats.currentMana += manaGainOnNightmare; 
            stats.currentMana = Mathf.Clamp(stats.currentMana, 0, stats.maxMana); 
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
