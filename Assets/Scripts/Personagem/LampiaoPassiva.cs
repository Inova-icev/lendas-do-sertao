using System.Collections;
using UnityEngine;

public class LampiaoPassiva : MonoBehaviour
{
    private StatusBase statusBase;
    public LayerMask inimigoLayer;
    public Transform ataquePoint;
    public float ataqueRange = 0.5f;
    private int ataquesConsecutivos = 0;
    private int acumulosCritico = 0;
    private float tempoBonusCritico = 3f;
    private float bonusCriticoPorAcumulo = 0.15f;
    private float criticoMaximo = 0.45f;
    private Collider2D ultimoInimigoAtacado;
    private Coroutine criticoCoroutine;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }

        AtaquePlayer.OnAtaqueRealizado += VerificarAtaque;
    }

    void OnDestroy()
    {
        AtaquePlayer.OnAtaqueRealizado -= VerificarAtaque;
    }

    private void VerificarAtaque()
    {
        Collider2D inimigoAtual = GetAlvoDoAtaque();

        if (inimigoAtual != null)
        {
            Debug.Log($"Inimigo atacado: {inimigoAtual.name}");

            if (inimigoAtual == ultimoInimigoAtacado)
            {
                ataquesConsecutivos++;

                if (ataquesConsecutivos == 3)
                {
                    AplicarBonusCritico();
                    ataquesConsecutivos = 0; 
                }
            }
            else
            {
                ataquesConsecutivos = 1;
            }

            ultimoInimigoAtacado = inimigoAtual;
        }
    }

    private Collider2D GetAlvoDoAtaque()
    {
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(ataquePoint.position, ataqueRange, inimigoLayer);
        if (inimigos.Length > 0)
        {
            return inimigos[0];
        }
        return null;
    }

    private void AplicarBonusCritico()
    {
        if (acumulosCritico < 3)
        {
            acumulosCritico++;
            statusBase.chanceDeCritico += bonusCriticoPorAcumulo;

            if (statusBase.chanceDeCritico > criticoMaximo)
            {
                statusBase.chanceDeCritico = criticoMaximo;
            }

            if (criticoCoroutine != null)
            {
                StopCoroutine(criticoCoroutine);
            }
            criticoCoroutine = StartCoroutine(TemporizadorCritico());
        }
    }

    private IEnumerator TemporizadorCritico()
    {
        yield return new WaitForSeconds(tempoBonusCritico);

        statusBase.chanceDeCritico -= bonusCriticoPorAcumulo * acumulosCritico;

        acumulosCritico = 0;

        if (statusBase.chanceDeCritico < 0f)
        {
            statusBase.chanceDeCritico = 0f;
        }

        criticoCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (ataquePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ataquePoint.position, ataqueRange);
        }
    }
}
