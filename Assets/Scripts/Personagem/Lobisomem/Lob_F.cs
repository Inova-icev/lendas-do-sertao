using UnityEngine;
using System.Collections;

public class LobisomemF : MonoBehaviour
{
    public LayerMask inimigoLayer;
    public KeyCode teclaAtivacao = KeyCode.F;
    public float cooldown = 90f;
    public float duracao = 15f; // Duração da aura
    public float alcanceAura = 5f; // Alcance da aura
    public float porcentagemDrenada = 0.04f; // Porcentagem de vida drenada por segundo
    public float intervaloDeDano = 1f; // Intervalo em segundos entre os tiques de dano
    private bool habilidadeDisponivel = true;
    private StatusBase statusBase;

    private void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Lobisomem!");
        }
    }

    private void Update()
    {
        if (habilidadeDisponivel && Input.GetKeyDown(teclaAtivacao))
        {
            AtivarHabilidade();
        }
    }

    void AtivarHabilidade()
    {
        habilidadeDisponivel = false;
        StartCoroutine(AplicarAura());
        Invoke(nameof(ResetarCooldown), cooldown);
    }

    private IEnumerator AplicarAura()
    {
        float tempoRestante = duracao;
        while (tempoRestante > 0)
        {
            // Detecta inimigos dentro da área da aura
            Collider2D[] inimigosNaAura = Physics2D.OverlapCircleAll(transform.position, alcanceAura, inimigoLayer);

            foreach (Collider2D inimigo in inimigosNaAura)
            {
                DrenarVida(inimigo);
            }

            tempoRestante -= intervaloDeDano;
            yield return new WaitForSeconds(intervaloDeDano); 
        }
    }

    void DrenarVida(Collider2D inimigo)
    {
        StatusBase inimigoStatus = inimigo.GetComponent<StatusBase>();
        if (inimigoStatus != null && inimigoStatus.vidaAtual > 0)
        {
            float vidaDrenada = inimigoStatus.vidaMaxima * porcentagemDrenada;

            inimigoStatus.vidaAtual = Mathf.Max(inimigoStatus.vidaAtual - vidaDrenada, 0); 
            Debug.Log($"Dano ao {inimigo.name}: {vidaDrenada}");

            float cura = vidaDrenada / 2f; 
            statusBase.vidaAtual = Mathf.Min(statusBase.vidaAtual + cura, statusBase.vidaMaxima); 
            Debug.Log($"Lobisomem curado por {cura}");

            // Se o inimigo morrer, ele é destruído
            if (inimigoStatus.vidaAtual <= 0)
            {
                Destroy(inimigo.gameObject);
                Debug.Log($"{inimigo.name} foi destruído.");
            }
        }
    }

    void ResetarCooldown()
    {
        habilidadeDisponivel = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, alcanceAura);
    }
}
