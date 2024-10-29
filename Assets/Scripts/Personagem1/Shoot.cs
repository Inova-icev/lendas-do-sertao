using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject projectilePrefab;  // Prefab do projetil
    public Transform shootPoint;         // Ponto de disparo
    public float projectileSpeed = 10f;  // Velocidade do projetil

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        // Instancia o projetil no ponto de disparo
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        // Calcula a direção do disparo com base na posição do mouse
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - shootPoint.position).normalized;
        rb.velocity = shootDirection * projectileSpeed;
    }
}
