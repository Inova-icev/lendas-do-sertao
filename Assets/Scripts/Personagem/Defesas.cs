using UnityEngine;
using System.Collections;

public class DefesaSistema : MonoBehaviour
{
    private StatusBase statusBase;

    private float reducaoPrecisao = 0f;
    private float aumentoDanoHabilidades = 0f;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no objeto!");
        }
    }

    public void ReceberDano(float dano, int tipoDano)
    {
        if (statusBase == null) return;

        float danoFinal = dano;

        if (tipoDano == 0) // Dano Físico
        {
            danoFinal = CalcularMitigacao(dano, statusBase.armadura);
        }
        else if (tipoDano == 1) // Dano Mágico
        {
            danoFinal = CalcularMitigacao(dano, statusBase.resistenciaMagica);
            danoFinal *= (1 + aumentoDanoHabilidades / 100f); // Aplica aumento de dano mágico
        }

        statusBase.vidaAtual -= danoFinal;

        if (statusBase.vidaAtual <= 0)
        {
            statusBase.vidaAtual = 0;
            Debug.Log("O personagem morreu!");
            Destroy(this.gameObject);
        }

        Debug.Log($"Dano recebido: {danoFinal} (Tipo: {(tipoDano == 0 ? "Físico" : "Mágico")})");
    }

    private float CalcularMitigacao(float dano, float mitigacao)
    {
        return dano / (1 + mitigacao / 100f);
    }

    public void AplicarReducaoPrecisao(float valor, float duracao)
    {
        StartCoroutine(AplicarModificadorPrecisao(valor, duracao));
    }

    private IEnumerator AplicarModificadorPrecisao(float valor, float duracao)
    {
        reducaoPrecisao += valor;
        statusBase.precisao = Mathf.Max(0, statusBase.precisao - valor); 
        yield return new WaitForSeconds(duracao);
        statusBase.precisao += valor;
        reducaoPrecisao -= valor;
    }

    public void AplicarAumentoDanoHabilidades(float porcentagem, float duracao)
    {
        StartCoroutine(AumentarDanoHabilidades(porcentagem, duracao));
    }

    private IEnumerator AumentarDanoHabilidades(float porcentagem, float duracao)
    {
        aumentoDanoHabilidades += porcentagem;
        yield return new WaitForSeconds(duracao);
        aumentoDanoHabilidades -= porcentagem;
    }
}
