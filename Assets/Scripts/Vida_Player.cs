using System.Collections.Generic;
using UnityEngine;

public class Vida_Player : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealthP;
    public delegate void OnDeathHandler(GameObject player);
    public static event OnDeathHandler OnPlayerDeath; // Evento global para morte

    private void Start()
    {
        currentHealthP = maxHealth;
    }

    public void TakeDamageP(int amount)
    {
        currentHealthP -= amount;
        if (currentHealthP <= 0)
        {
            currentHealthP = 0;
            DieP();
        }
    }

    private void DieP()
    {
        Debug.Log($"{gameObject.name} morreu!");
        OnPlayerDeath?.Invoke(gameObject); // Dispara o evento de morte
        gameObject.SetActive(false); // Desativa o objeto do jogador
    }

    public void ResetHealth()
    {
        currentHealthP = maxHealth;
    }
}
