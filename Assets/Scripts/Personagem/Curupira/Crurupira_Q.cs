using System.Collections;
using UnityEngine;

public class CurupiraQ : MonoBehaviour
{
    public KeyCode teclaAtivacao = KeyCode.Q;
    public float alcanceHabilidade = 3f; 
    public float duracaoAtordoamento = 2f;
    public float tempoDeRecarga = 60f; 
    public string inimigoTag = "Inimigo"; // Tag que o inimigo deve ter

    private float tempoRecargaAtual;
    private bool podeUsarHabilidade = true;

    void Update()
    {
        if (Input.GetKeyDown(teclaAtivacao) && podeUsarHabilidade)
        {
            UsarHabilidadeQ();
        }

        // Atualiza o tempo de recarga
        if (!podeUsarHabilidade)
        {
            tempoRecargaAtual -= Time.deltaTime;
            if (tempoRecargaAtual <= 0)
            {
                podeUsarHabilidade = true;
            }
        }
    }

    void UsarHabilidadeQ()
    {
        // Alvo da habilidade será a posição do personagem (centro do personagem)
        // Agora verificamos os inimigos pela tag e não pela layer
        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(transform.position, alcanceHabilidade);

        if (inimigosAfetados.Length > 0)
        {
            podeUsarHabilidade = false;
            tempoRecargaAtual = tempoDeRecarga;

            foreach (Collider2D inimigo in inimigosAfetados)
            {
                // Verifica se o inimigo tem a tag especificada
                if (inimigo.CompareTag(inimigoTag))
                {
                    // Puxar o inimigo para o Curupira
                    inimigo.transform.position = transform.position;

                    // Aplicar atordoamento no inimigo
                    StartCoroutine(AtordoarInimigo(inimigo));
                }
            }
        }
        else
        {
            Debug.Log("Nenhum inimigo dentro do alcance da habilidade.");
        }
    }

    IEnumerator AtordoarInimigo(Collider2D inimigo)
    {
        PlayerMovement movimentoInimigo = inimigo.GetComponent<PlayerMovement>();
        AtaquePlayer ataqueInimigo = inimigo.GetComponent<AtaquePlayer>();

        if (movimentoInimigo != null)
        {
            movimentoInimigo.estaAtordoado = true;
        }

        if (ataqueInimigo != null)
        {
            ataqueInimigo.estaAtordoado = true;
        }

        yield return new WaitForSeconds(duracaoAtordoamento);

        if (movimentoInimigo != null)
        {
            movimentoInimigo.estaAtordoado = false;
        }

        if (ataqueInimigo != null)
        {
            ataqueInimigo.estaAtordoado = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualização do alcance da habilidade na cena (baseado na posição do personagem)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceHabilidade);
    }
}
