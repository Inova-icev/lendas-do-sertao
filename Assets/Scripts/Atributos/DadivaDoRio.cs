using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavidaDoRio : MonoBehaviour
{
    public float baseMovementSpeed = 5f; 
    public float minSpeedBonus = 0.30f; 
    public float maxSpeedBonus = 0.60f; 
    public string riverTag = "Rio"; 

    private CharacterController characterController; 
    private float currentSpeed; 

    private void Start()
    {
        characterController = GetComponent<CharacterController>(); 
        currentSpeed = baseMovementSpeed;
    }

    private void Update()
    {
        
        characterController.Move(transform.forward * currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(riverTag)) 
        {
            
            float speedBonusPercentage = Mathf.Lerp(minSpeedBonus, maxSpeedBonus, CalculateLevel() / 10f);
            currentSpeed = baseMovementSpeed * (1 + speedBonusPercentage); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(riverTag)) 
        {
            currentSpeed = baseMovementSpeed; 
        }
    }

    private float CalculateLevel()
    {
        
        return 1; 
    }
}
