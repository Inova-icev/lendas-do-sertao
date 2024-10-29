using System.Collections;
using UnityEngine;

public class Imortal : MonoBehaviour
{
    public float resurrectionTime = 5f; 
    public float shieldAmountMin = 20f; 
    public float shieldAmountMax = 85f; 
    public float cooldownTime = 300f; 

    private CharacterStats characterStats; 
    private bool isDead = false; 
    private float nextAvailableTime; 
    private void Start()
    {
        characterStats = GetComponent<CharacterStats>(); 
    }

    public void TakeDamage(float damage)
    {
        if (isDead || Time.time < nextAvailableTime) return; 

        characterStats.currentHealth -= damage; 

        
        if (characterStats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        characterStats.currentHealth = 0; 
        StartCoroutine(Resurrection()); 
    }

    private IEnumerator Resurrection()
    {
        yield return new WaitForSeconds(resurrectionTime); 

        
        characterStats.currentHealth = characterStats.maxHealth * 0.5f; 

        
        float shieldAmount = Mathf.Lerp(shieldAmountMin, shieldAmountMax, characterStats.level / 100f); 
        characterStats.ApplyShield(shieldAmount); 

        isDead = false; 
        nextAvailableTime = Time.time + cooldownTime; 
    }
}
