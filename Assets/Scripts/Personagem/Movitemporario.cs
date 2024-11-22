using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //movimneto do player
    public Animator animator;
    Rigidbody2D rb;
    float speed = 6f;
    float inputX;

     
     //flip
     bool faceRight;

     [Header("Sistema de Pulo")]
     public float jumpForce;

     bool isGrounded;
     public Transform groundedCheck;

     public LayerMask whatIsLayer;


     void Start(){
        rb= GetComponent<Rigidbody2D>();
        
     }
     void Update(){
        CheckGrounded();
            inputX=Input.GetAxis("Horizontal");
            if(Input.GetButtonDown("Jump")){
                Jumpinf();
            }
            if(inputX >0 && faceRight ==true){
                Flip();
            }
            if(inputX < 0 && faceRight ==false){
                Flip();
            }
     }

     private void FixedUpdate(){
        rb.velocity = new Vector2(inputX*speed,rb.velocity.y);
     }
     

    void Flip(){
         animator.SetTrigger("corridaDiretita");
         
        faceRight =!faceRight;
        float x = transform.localScale.x;
        x*=-1;
        transform.localScale = new Vector3(x,transform.localScale.y,transform.localScale.z);
    }
    void CheckGrounded(){
        isGrounded=Physics2D.OverlapCircle(groundedCheck.position,0.2f,whatIsLayer);
    }

    void Jumpinf(){
        if(isGrounded==true){
             animator.SetTrigger("pulo");
            rb.velocity= new Vector2(rb.velocity.x,jumpForce);
        }
    }
}
