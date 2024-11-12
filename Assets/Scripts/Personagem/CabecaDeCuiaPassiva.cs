using UnityEngine;

public class CabecaDeCuiaPassiva : MonoBehaviour
{
    private StatusBase statusBase;
    private int acumulacoes = 0;
    private bool bonusRouboDeVidaAtivado = false;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }

        MorteDoInimigo.OnInimigoEliminado += AdicionarAcumulo;
    }

    private void OnDestroy()
    {
        MorteDoInimigo.OnInimigoEliminado -= AdicionarAcumulo;
    }

    private void AdicionarAcumulo()
    {
        if (acumulacoes < 7)
        {
            acumulacoes++;
            float danoExtra = statusBase.dano * 0.05f;
            statusBase.dano += danoExtra;
            Debug.Log("Cabeça de Cuia ganhou um acúmulo de dano! Acumulações: " + acumulacoes + ". Dano adicional: " + danoExtra);
        }

        if (acumulacoes == 7 && !bonusRouboDeVidaAtivado)
        {
            statusBase.rouboDeVida += 10f;
            bonusRouboDeVidaAtivado = true;
            Debug.Log("Cabeça de Cuia atingiu o máximo de acúmulos e ganhou 10% de roubo de vida!");
        }
    }
}
