using UnityEngine;

public class DefesaSistema : MonoBehaviour
{
    private StatusBase statusBase;

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
        }

        statusBase.vidaAtual -= danoFinal;

        if (statusBase.vidaAtual <= 0)
        {
            statusBase.vidaAtual = 0;
            Debug.Log("O personagem morreu!");
            // Aqui você pode chamar um evento ou script de morte
        }

        Debug.Log($"Dano recebido: {danoFinal} (Tipo: {(tipoDano == 0 ? "Físico" : "Mágico")})");
    }
    private float CalcularMitigacao(float dano, float mitigacao)
    {
        return dano / (1 + mitigacao / 100f);
    }
}
