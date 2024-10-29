using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento do player
    public float jumpForce = 10f; // Força do pulo
    public int maxJumps = 2; // Limite de pulos consecutivos

    private Rigidbody2D rb;
    private int jumpCount; // Contador de pulos
    private bool isGrounded; // Verifica se o player está no chão

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimento horizontal com as teclas A e D
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        // Pulo ao pressionar "W" e verificar se há pulos restantes
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        // Rotaciona o player para olhar em direção ao mouse
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        // Calcula a direção entre a posição do mouse e o player
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Calcula o ângulo e aplica a rotação
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o player está no chão e reinicia o contador de pulos
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Define que o player não está mais no chão
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
