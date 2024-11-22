using System.Collections;
using UnityEngine;

public class EfeitoMarcaConfusao : MonoBehaviour
{
    private StatusBase statusBase;
    private float precisaoOriginal;
    private float danoRecebidoOriginal;

    private bool efeitosAtivos = false;

    public float precisaoReducaoPercentual;
    public float aumentoDanoPercentual;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no inimigo!");
            return;
        }

        // Armazenar os valores originais de precisão e dano
        precisaoOriginal = 100f; // Valor inicial de precisão (100%)
        danoRecebidoOriginal = 1f;  // Valor inicial de dano recebido (sem multiplicação)
    }

    // Método para aplicar os efeitos de confusão
    public void AplicarEfeitos()
    {
        if (!efeitosAtivos)
        {
            efeitosAtivos = true;
            ReduzirPrecisao(precisaoReducaoPercentual);
            AumentarDanoRecebido(aumentoDanoPercentual);
            StartCoroutine(RetirarEfeitos());
        }
    }

    // Método para remover os efeitos após 3 segundos
    private IEnumerator RetirarEfeitos()
    {
        yield return new WaitForSeconds(3f);
        RestaurarPrecisao();
        RestaurarDanoRecebido();
        Destroy(this);  // Remove o componente de efeitos após o tempo
    }

    private void ReduzirPrecisao(float percentual)
    {
        precisaoOriginal -= percentual;
        if (precisaoOriginal < 0) precisaoOriginal = 0;  // Garante que a precisão não seja negativa
    }

    private void AumentarDanoRecebido(float percentual)
    {
        danoRecebidoOriginal += percentual / 100f;
    }

    private void RestaurarPrecisao()
    {
        precisaoOriginal = 100f;  // Restaura a precisão original
    }

    private void RestaurarDanoRecebido()
    {
        danoRecebidoOriginal = 1f;  // Restaura o dano recebido original (sem aumento)
    }
}
