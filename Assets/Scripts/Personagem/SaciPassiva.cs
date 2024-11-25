using UnityEngine;

public class SaciPassiva : MonoBehaviour
{
    public float[] reducaoPrecisao = { 5f, 10f, 15f, 20f };
    public float[] aumentoDanoHabilidades = { 5f, 10f, 15f, 20f };
    public float duracaoMarca = 3f;

    private StatusBase statusBase;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Saci!");
        }
    }

    public void AplicarMarca(GameObject inimigo)
    {
        DefesaSistema defesaInimigo = inimigo.GetComponent<DefesaSistema>();
        if (defesaInimigo != null)
        {
            int nivel = Mathf.Clamp(statusBase.level, 1, reducaoPrecisao.Length) - 1;

            defesaInimigo.AplicarReducaoPrecisao(reducaoPrecisao[nivel], duracaoMarca);
            defesaInimigo.AplicarAumentoDanoHabilidades(aumentoDanoHabilidades[nivel], duracaoMarca);
        }
    }
}
