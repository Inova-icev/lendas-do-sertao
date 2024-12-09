using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rb;
    float speed = 6f;
    float inputX;

    // Controle de direção
    bool faceRight;

    [Header("Sistema de Pulo")]
    public float jumpForce;

    bool isGrounded;
    public Transform groundedCheck;

    public LayerMask whatIsLayer;

    // Controle de atordoamento
    public bool estaAtordoado;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGrounded();

        if (!estaAtordoado) // Apenas permite movimento se não estiver atordoado
        {
            inputX = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                Jumpinf();
            }
            if (inputX > 0 && faceRight == true)
            {
                Flip();
            }
            if (inputX < 0 && faceRight == false)
            {
                Flip();
            }
        }
        else
        {
            inputX = 0; // Para garantir que não haja movimento
            Debug.Log("Personagem está atordoado e não pode se mover.");
        }
    }

    private void FixedUpdate()
    {
        if (!estaAtordoado) // Apenas move se não estiver atordoado
        {
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Impede movimento horizontal
        }
    }

    void Flip()
    {
        animator.SetTrigger("corridaDiretita");

        faceRight = !faceRight;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundedCheck.position, 0.2f, whatIsLayer);
    }

    void Jumpinf()
    {
        if (isGrounded == true)
        {
            animator.SetTrigger("pulo");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
