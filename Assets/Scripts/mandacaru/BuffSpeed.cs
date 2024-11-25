using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMandacaru : MonoBehaviour
{
    public float buffMultiplier = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyBuff(buffMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.RemoveBuff(); // Atualizado para não passar o multiplicador
        }
    }
}
