using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaciPassiva : MonoBehaviour
{
    public float[] precisaoReduzida = { 5f, 10f, 15f, 20f };
    public float[] danoAdicional = { 5f, 10f, 15f, 20f };  

    public int nivelHabilidade = 1;

    private void OnEnable()
    {
        AtaquePlayer.OnAtaqueRealizado += AplicarMarcaConfusao;
    }

    private void OnDisable()
    {
        AtaquePlayer.OnAtaqueRealizado -= AplicarMarcaConfusao;
    }

    private void AplicarMarcaConfusao()
    {
        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(ataquePoint.position, ataqueRange, inimigoLayers);
        
        foreach (Collider2D inimigo in inimigosAfetados)
        {
            EfeitoMarcaConfusao efeito = inimigo.GetComponent<EfeitoMarcaConfusao>();
            
            if (efeito == null)  
            {
                efeito = inimigo.gameObject.AddComponent<EfeitoMarcaConfusao>();
            }

            efeito.precisaoReducaoPercentual = precisaoReduzida[nivelHabilidade - 1];
            efeito.aumentoDanoPercentual = danoAdicional[nivelHabilidade - 1];

            efeito.AplicarEfeitos();
        }
    }
}
