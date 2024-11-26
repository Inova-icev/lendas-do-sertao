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
            if (CalcularAcerto())
            {
                DefesaSistema defesaInimigo = inimigo.GetComponent<DefesaSistema>();
                if (defesaInimigo != null)
                {
                    defesaInimigo.ReceberDano(statusBase.dano, 0);
                    Debug.Log($"Inimigo atingido com {statusBase.dano} de dano físico!");
                }

                AplicarRouboDeVida(statusBase.dano);
            }
            else
            {
                Debug.Log("O ataque errou!");
            }
        }
    }

    void AplicarRouboDeVida(float danoCausado)
    {
        float vidaRoubada = danoCausado * (statusBase.rouboDeVida / 100);
        statusBase.vidaAtual = Mathf.Min(statusBase.vidaAtual + vidaRoubada, statusBase.vidaMaxima);
        Debug.Log("Cabeça de Cuia recuperou " + vidaRoubada + " de vida com roubo de vida.");
    }

    bool CalcularAcerto()
    {
        float chance = Random.Range(0f, 100f);
        Debug.Log($"Chance sorteada: {chance}, Precisão: {statusBase.precisao}");
        return chance < statusBase.precisao;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(ataquePoint.position, ataqueRange);
    }
}
