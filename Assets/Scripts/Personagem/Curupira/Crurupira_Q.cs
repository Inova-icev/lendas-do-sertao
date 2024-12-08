using System.Collections;
using UnityEngine;

public class CurupiraQ : MonoBehaviour
{
    public KeyCode teclaAtivacao = KeyCode.Q;
    public Transform ataquePoint;
    public float alcanceHabilidade = 3f; 
    public float duracaoAtordoamento = 2f;
    public float tempoDeRecarga = 60f; 
    public LayerMask inimigoLayers; 

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
        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(ataquePoint.position, alcanceHabilidade, inimigoLayers);

        if (inimigosAfetados.Length > 0)
        {
            podeUsarHabilidade = false;
            tempoRecargaAtual = tempoDeRecarga;

            foreach (Collider2D inimigo in inimigosAfetados)
            {
                // Puxar o inimigo para o Curupira
                inimigo.transform.position = transform.position;

                // Aplicar atordoamento no inimigo
                StartCoroutine(AtordoarInimigo(inimigo));
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
        if (ataquePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ataquePoint.position, alcanceHabilidade);
    }
}
