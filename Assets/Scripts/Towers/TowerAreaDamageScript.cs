using UnityEngine;

public class TowerAreaDamageScript : MonoBehaviour
{
    public int damageAmount = 10; 

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Opa, entrou na área de dano");
    }
    
}