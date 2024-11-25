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
    public float manaAtual;
    public float regeneracaoVida;
    public float regeneracaoMana;
    public float rouboDeVida;
    public float chanceDeCritico;
    public float precisao;

    public float danoMagico;
    public float vidaEscudoMaxima;
    public float vidaAtualEscudo;

    [Header("Defesas")]
    public float armadura;
    public float resistenciaMagica;

    [Header("Movimentação")]
    public float velocidadeMovimento;
    public float jumpForce;

    [Header("Level")]
    public int level = 1;



     [Header("Barra vida")]
    public Transform barraVidaverde;

    public GameObject barravidaPai;

    private Vector3 barravidaScala;


    [Header("Escudo")]
     public bool temEscudo;

     public GameObject EscudoPai;


    private float perceBarravida;



     void Start(){ 
          barravidaScala =barraVidaverde.localScale;
          perceBarravida=barravidaScala.x/vidaMaxima;
         
          
    }

     public void UpdateBarraVida(){
        barravidaScala.x =perceBarravida*vidaAtual;
        barraVidaverde.localScale = barravidaScala;
    }

    public void AtivarEscudo(){
        EscudoPai.SetActive(true);
        temEscudo =true;
    }

        private void Update()
    {
        if (vidaAtual < vidaMaxima)
        {
            vidaAtual += regeneracaoVida * Time.deltaTime;
            if (vidaAtual > vidaMaxima)
            {
                vidaAtual = vidaMaxima;
            }
        }

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

