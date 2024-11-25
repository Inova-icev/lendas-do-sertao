using UnityEngine;

public class TipoPersonagem : StatusBase
{
    public enum ClassePersonagem
    {
        Mago,
        Tanque,
        Assassino,
        Suporte,
        Atirador
    }

    [Header("Classe do Personagem")]
    public ClassePersonagem classeSelecionada;

    private void OnValidate()
    {
        AplicarAtributosDaClasse();
    }

    private void AplicarAtributosDaClasse()
    {
        switch (classeSelecionada)
        {
            case ClassePersonagem.Mago:
                vidaMaxima = 70f;
                vidaAtual = 70f;
                dano = 15f;
                velocidadeAtaque = 1.2f;
                manaMaxima = 100f;
                manaAtual = 100f;
                regeneracaoVida = 1.5f;
                regeneracaoMana = 3f;
                precisao = 100f;
                velocidadeMovimento = 4f;
                jumpForce = 8f;
                break;

            case ClassePersonagem.Tanque:
                vidaMaxima = 150f;
                vidaAtual = 150f;
                dano = 10f;
                velocidadeAtaque = 0.8f;
                manaMaxima = 50f;
                manaAtual = 50f;
                regeneracaoVida = 3f;
                regeneracaoMana = 1f;
                precisao = 100f;
                velocidadeMovimento = 3f;
                jumpForce = 6f;
                break;

            case ClassePersonagem.Assassino:
                vidaMaxima = 90f;
                vidaAtual = 90f;
                dano = 20f;
                velocidadeAtaque = 1.5f;
                manaMaxima = 60f;
                manaAtual = 60f;
                regeneracaoVida = 1f;
                regeneracaoMana = 1.5f;
                precisao = 100f;
                velocidadeMovimento = 6f;
                jumpForce = 10f;
                break;

            case ClassePersonagem.Suporte:
                vidaMaxima = 80f;
                vidaAtual = 80f;
                dano = 8f;
                velocidadeAtaque = 1f;
                manaMaxima = 80f;
                manaAtual = 80f;
                regeneracaoVida = 2f;
                regeneracaoMana = 2.5f;
                precisao = 100f;
                velocidadeMovimento = 4.5f;
                jumpForce = 7f;
                break;

            case ClassePersonagem.Atirador:
                vidaMaxima = 100f;
                vidaAtual = 100f;
                dano = 12f;
                velocidadeAtaque = 1.3f;
                manaMaxima = 40f;
                manaAtual = 40f;
                regeneracaoVida = 1.2f;
                regeneracaoMana = 1f;
                precisao = 100f;
                velocidadeMovimento = 5f;
                jumpForce = 8f;
                break;
        }
    }
}