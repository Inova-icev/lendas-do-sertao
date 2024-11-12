using System.Collections;
using UnityEngine;

public class BoitataPassiva : MonoBehaviour
{
    private float danoAtual;
    public float duracaoVeneno = 3f; 
    public float intervaloDano = 1f; 
    public float reducaoCura = 0.25f; 
    public int maxAcumulos = 2; 
    public LayerMask inimigoLayer; 
    private StatusBase statusBase;

    private int acumulosVeneno = 0; 
    private float danoAcumulado = 0f; 
    private bool venenoAtivo = false; 

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Boitatá!");
        }

        AtaquePlayer.OnAtaqueRealizado += AtivarVeneno;
    }

    void OnDestroy()
    {
        AtaquePlayer.OnAtaqueRealizado -= AtivarVeneno;
    }

    void AtivarVeneno()
    {
        danoAtual = Mathf.Lerp(15f, 90f, Mathf.InverseLerp(1f, 12f, statusBase.level));

        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(transform.position, 0.5f, inimigoLayer);

        foreach (Collider2D inimigo in inimigosAfetados)
        {
            MorteDoInimigo morteInimigo = inimigo.GetComponent<MorteDoInimigo>();
            if (morteInimigo != null && !morteInimigo.IsDead())
            {
                AplicarVeneno(morteInimigo.transform, danoAtual);
            }
        }
    }

    void AplicarVeneno(Transform inimigo, float danoAtual)
    {
        MorteDoInimigo morteInimigo = inimigo.GetComponent<MorteDoInimigo>();
        if (morteInimigo != null)
        {
            if (!venenoAtivo)
            {
                venenoAtivo = true; 
                StartCoroutine(AplicarVenenoCoroutine(morteInimigo, danoAtual));
            }

            if (acumulosVeneno < maxAcumulos)
            {
                if (acumulosVeneno == 0)
                {
                    danoAcumulado = danoAtual; 
                }
                else if (acumulosVeneno == 1)
                {
                    danoAcumulado = danoAtual * 2f; 
                }
                else if (acumulosVeneno == maxAcumulos)
                {
                    danoAcumulado = danoAtual; 
                }
                acumulosVeneno++;  
            }
        }
    }

    private IEnumerator AplicarVenenoCoroutine(MorteDoInimigo inimigo, float danoAtual)
    {
        float tempoAcumuladoInterno = 0f;
        float tempoMaximoInterno = 0f;

        while (tempoMaximoInterno < duracaoVeneno)
        {
            tempoAcumuladoInterno += Time.deltaTime;
            tempoMaximoInterno += Time.deltaTime;

            if (tempoAcumuladoInterno >= intervaloDano)
            {
                inimigo.DanoNoinimigo(danoAcumulado);
                Debug.Log("O inimigo recebeu " + danoAcumulado + " de dano por veneno");

                tempoAcumuladoInterno = 0f;
            }

            yield return null;
        }

        danoAcumulado = danoAtual; 
        acumulosVeneno = 0; 
        venenoAtivo = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
