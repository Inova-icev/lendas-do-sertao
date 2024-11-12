// Classe base que contém os atributos gerais de status
using UnityEngine;

public class StatusBase : MonoBehaviour
{
    [Header("Atributos Básicos")]
    public float vidaMaxima;
    public float vidaAtual;
    public float dano;
    public float velocidadeAtaque;
    public float manaMaxima;
    public float regeneracaoVida;
    public float regeneracaoMana;
    public float rouboDeVida;
    public float chanceDeCritico;

    [Header("Movimentação")]
    public float velocidadeMovimento;
    public float jumpForce;

    [Header("Level")]
    public int level = 1;
}

