using System.Collections;
using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    public bool atacando;
    public Animator animator;

    public Transform ataquePoint;
    public float ataqueRange = 0.5f;
    private StatusBase statusBase;

    public LayerMask inimigoLayers;

    // Evento para notificar a passiva de que um ataque foi realizado
    public delegate void AtaqueRealizadoHandler();
    public static event AtaqueRealizadoHandler OnAtaqueRealizado;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }
    }

    void Update()
    {
        atacando = Input.GetButtonDown("Fire1");
        if (atacando)
        {
            Ataque();
        }
    }

    void Ataque()
    {
        animator.SetTrigger("ataque");

        OnAtaqueRealizado?.Invoke();

        Collider2D[] hitInimigos = Physics2D.OverlapCircleAll(ataquePoint.position, ataqueRange, inimigoLayers);

        foreach (Collider2D inimigo in hitInimigos)
        {
            inimigo.GetComponent<MorteDoInimigo>().DanoNoinimigo(statusBase.dano);
            Debug.Log("O inimigo recebeu " + statusBase.dano + " de dano");
            AplicarRouboDeVida(statusBase.dano);
        }
    }

    void AplicarRouboDeVida(float danoCausado)
    {
        float vidaRoubada = danoCausado * (statusBase.rouboDeVida / 100);
        statusBase.vidaAtual = Mathf.Min(statusBase.vidaAtual + vidaRoubada, statusBase.vidaMaxima);
        Debug.Log("Cabeça de Cuia recuperou " + vidaRoubada + " de vida com roubo de vida.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(ataquePoint.position, ataqueRange);
    }
}
