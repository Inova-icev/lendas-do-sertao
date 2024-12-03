using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobisomemE : MonoBehaviour
{
    public LayerMask inimigoLayer;
    public KeyCode teclaAtivacao = KeyCode.E;
    public float cooldown = 90f;
    public float danoAtaque = 50f; // Dano base dos primeiros ataques
    public float danoFinal = 200f; // Dano do golpe final
    public float alcance = 5f; // Alcance da habilidade
    public float anguloCone = 45f; // Ângulo do cone em graus
    public int numeroAtaques = 4; // Número de ataques
    private bool habilidadeDisponivel = true;
    private StatusBase statusBase;
    private float anguloMeio;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Lobisomem!");
        }

        anguloMeio = anguloCone / 2f; // Calcula o ângulo central apenas uma vez
    }

    void Update()
    {
        if (habilidadeDisponivel && Input.GetKeyDown(teclaAtivacao))
        {
            AtivarHabilidade();
        }
    }

    void AtivarHabilidade()
    {
        habilidadeDisponivel = false;
        StartCoroutine(ExecutarAtaques());
        Invoke(nameof(ResetarCooldown), cooldown);
    }

    private IEnumerator ExecutarAtaques()
    {
        for (int i = 0; i < numeroAtaques; i++)
        {
            // Calcula o dano para cada ataque
            float danoAtual = i == numeroAtaques - 1 ? danoFinal : danoAtaque;

            // Detecta inimigos no cone
            List<Collider2D> inimigos = DetectarInimigosNoCone();

            foreach (Collider2D inimigo in inimigos)
            {
                DefesaSistema defesaSistema = inimigo.GetComponent<DefesaSistema>();
                if (defesaSistema != null)
                {
                    defesaSistema.ReceberDano(danoAtual, 0); // Tipo de dano: físico
                }
            }

            yield return new WaitForSeconds(0.2f); // Pequeno intervalo entre os ataques
        }
    }

    private List<Collider2D> DetectarInimigosNoCone()
    {
        List<Collider2D> inimigosNoCone = new List<Collider2D>();

        // Calcula a direção do mouse
        Vector2 posicaoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcaoMouse = (posicaoMouse - (Vector2)transform.position).normalized;

        // Detecta todos os inimigos na área circular
        Collider2D[] inimigosNaArea = Physics2D.OverlapCircleAll(transform.position, alcance, inimigoLayer);
        Debug.Log("Inimigos detectados no alcance: " + inimigosNaArea.Length);

        foreach (Collider2D inimigo in inimigosNaArea)
        {
            Vector2 direcaoAlvo = ((Vector2)inimigo.transform.position - (Vector2)transform.position).normalized;

            // Calcula o ângulo entre a direção do mouse e a direção para o inimigo
            float anguloAlvo = Vector2.Angle(direcaoMouse, direcaoAlvo);
            Debug.Log($"Ângulo com o inimigo: {anguloAlvo}, Ângulo máximo permitido: {anguloMeio}");

            if (anguloAlvo <= anguloMeio)
            {
                inimigosNoCone.Add(inimigo);
                Debug.Log("Inimigo adicionado ao cone: " + inimigo.name);
            }
        }

        return inimigosNoCone;
    }

    void ResetarCooldown()
    {
        habilidadeDisponivel = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Visualiza o alcance e o cone da habilidade
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcance);

        // Desenha o cone em direção ao mouse
        Vector2 origem = transform.position;
        Vector2 posicaoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcaoMouse = (posicaoMouse - origem).normalized;

        Vector3 pontoDireita = Quaternion.Euler(0, 0, anguloCone / 2) * direcaoMouse * alcance;
        Vector3 pontoEsquerda = Quaternion.Euler(0, 0, -anguloCone / 2) * direcaoMouse * alcance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)pontoDireita);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)pontoEsquerda);
    }
}
