using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public float vidaAtual = 100f;
    public float Velocidade = 5f;
    private bool estaAtordoado = false;

    // Método para receber dano
    public void ReceberDano(float dano)
    {
        if (!estaAtordoado)
        {
            vidaAtual -= dano;
            if (vidaAtual <= 0)
            {
                Morrer();
            }
        }
    }

    // Método para lidar com a morte do inimigo
    private void Morrer()
    {
        Destroy(gameObject);
    }

    // Método para aplicar atordoamento
    public void Atordoar(float duracao)
    {
        if (!estaAtordoado)
        {
            StartCoroutine(AtordoamentoCoroutine(duracao));
        }
    }   
    private IEnumerator AtordoamentoCoroutine(float duracao)
    {   
    estaAtordoado = true;
    float velocidadeOriginal = Velocidade;
    Velocidade = 0f; // Para o movimento enquanto atordoado
    yield return new WaitForSeconds(duracao);
    Velocidade = velocidadeOriginal;
    estaAtordoado = false;
    }
}
