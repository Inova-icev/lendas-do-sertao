using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDano : MonoBehaviour
{
    public float damageMultiplier = 2f; // Multiplicador de dano do buff

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyDamageBuff(damageMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.RemoveDamageBuff(damageMultiplier);
        }
    }
}