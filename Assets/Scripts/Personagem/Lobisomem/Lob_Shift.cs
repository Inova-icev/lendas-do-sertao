using UnityEngine;

public class LobisomemShift : MonoBehaviour
{
    public LayerMask inimigoLayer;
    public KeyCode teclaAtivacao = KeyCode.LeftShift;
    public float aumentoDano = 20f; 
    public float aumentoDefesa = 20f;
    public float aumentoEstatisticas = 30f; 
    public float duracaoBuff = 60f; 
    public float duracaoMarca = 10f; 
    public float cooldown = 90f;
    public float alcanceArea = 10f; 
    private bool habilidadeDisponivel = true;
    private bool marcaAtiva = false;
    private Transform alvoMarcado;
    private MorteDoInimigo inimigoMarcado;
    private DefesaSistema defesaSistema;
    private StatusBase statusBase;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        defesaSistema = GetComponent<DefesaSistema>();
    }

    void Update()
    {
        if (habilidadeDisponivel && Input.GetKeyDown(teclaAtivacao) && !marcaAtiva)
        {
            AtivarHabilidade();
        }
    }

    void AtivarHabilidade()
    {
        // Detecta todos os inimigos na área
        Collider2D[] inimigosNaArea = Physics2D.OverlapCircleAll(transform.position, alcanceArea, inimigoLayer);

        if (inimigosNaArea.Length > 0)
        {
            // Escolhe um inimigo aleatório
            Collider2D inimigoEscolhido = inimigosNaArea[Random.Range(0, inimigosNaArea.Length)];
            alvoMarcado = inimigoEscolhido.transform;
            inimigoMarcado = alvoMarcado.GetComponent<MorteDoInimigo>();

            if (inimigoMarcado != null)
            {
                Debug.Log("Inimigo marcado: " + alvoMarcado.name);
                marcaAtiva = true; // Marca o alvo
                StartCoroutine(AplicarEfeitosMarcado());
                StartCoroutine(TempoDeDuracaoMarca()); // Inicia o temporizador da marca
            }
            else
            {
                Debug.LogError("O inimigo marcado não possui o script MorteDoInimigo.");
            }

            habilidadeDisponivel = false;
            Invoke(nameof(ResetarCooldown), cooldown);
        }
        else
        {
            Debug.Log("Nenhum inimigo encontrado na área.");
        }
    }

    private System.Collections.IEnumerator AplicarEfeitosMarcado()
    {
        // Aplica os aumentos de dano e defesa enquanto o alvo marcado não morre
        statusBase.dano *= 1 + aumentoDano / 100f;
        statusBase.armadura *= 1 + aumentoDefesa / 100f;

        // Espera até que o inimigo morra
        while (alvoMarcado != null && inimigoMarcado != null && !inimigoMarcado.IsDead())
        {
            yield return null; 
        }

        if (alvoMarcado != null && inimigoMarcado != null && inimigoMarcado.IsDead())
        {
            AlvoEliminado();
        }
    }

    void AlvoEliminado()
    {
        Debug.Log("Alvo marcado eliminado!");
        alvoMarcado = null;
        inimigoMarcado = null;

        // Aplica aumento temporário nos atributos
        StartCoroutine(AumentarEstatisticasTemporarias());

        habilidadeDisponivel = true; 
        marcaAtiva = false;
    }

    private System.Collections.IEnumerator AumentarEstatisticasTemporarias()
    {
        float danoOriginal = statusBase.dano;
        float defesaOriginal = statusBase.armadura;
        float velocidadeOriginal = statusBase.velocidadeMovimento;

        statusBase.dano *= 1 + aumentoEstatisticas / 100f;
        statusBase.armadura *= 1 + aumentoEstatisticas / 100f;
        statusBase.velocidadeMovimento *= 1 + aumentoEstatisticas / 100f;

        Debug.Log("Estatísticas aumentadas temporariamente!");

        yield return new WaitForSeconds(duracaoBuff);

        statusBase.dano = danoOriginal;
        statusBase.armadura = defesaOriginal;
        statusBase.velocidadeMovimento = velocidadeOriginal;

        Debug.Log("Estatísticas retornaram ao normal.");
    }

    private System.Collections.IEnumerator TempoDeDuracaoMarca()
    {
        yield return new WaitForSeconds(duracaoMarca);
        // Quando o tempo da marca acaba, remove o aumento de dano e defesa
        if (alvoMarcado != null)
        {
            statusBase.dano /= 1 + aumentoDano / 100f;
            statusBase.armadura /= 1 + aumentoDefesa / 100f;
            alvoMarcado = null;
            inimigoMarcado = null;
            Debug.Log("Marca expirada.");
        }
    }

    void ResetarCooldown()
    {
        habilidadeDisponivel = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceArea); // Alcance de marcação
    }
}
