using UnityEngine;

public class StatusBase : MonoBehaviour
{
    [Header("Atributos Básicos")]
    public float vidaMaxima;
    public float vidaAtual;
    public float dano;
    public float velocidadeAtaque;
    public float manaMaxima;
    public float manaAtual;  
    public float regeneracaoVida;
    public float regeneracaoMana;
    public float rouboDeVida;
    public float chanceDeCritico;
    public float precisao = 100f;

    [Header("Defesas")]
    public float armadura;
    public float resistenciaMagica;

    [Header("Movimentação")]
    public float velocidadeMovimento;
    public float jumpForce;

    [Header("Level")]
    public int level = 1;

    private void Update()
    {
        // Regeneração de Vida
        if (vidaAtual < vidaMaxima)
        {
            vidaAtual += regeneracaoVida * Time.deltaTime;
            if (vidaAtual > vidaMaxima)
            {
                vidaAtual = vidaMaxima; 
            }
        }

        // Regeneração de Mana
        if (manaAtual < manaMaxima)
        {
            manaAtual += regeneracaoMana * Time.deltaTime;
            if (manaAtual > manaMaxima)
            {
                manaAtual = manaMaxima; 
            }
        }
    }
}
