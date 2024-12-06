using Unity.VisualScripting;
using UnityEngine;

public class CurupiraPassiva : MonoBehaviour
{
    private StatusBase statusBase;

    public float bonusVelocidade = 20f; 
    public float bonusDano = 20f;       

    private float danoOriginal;  
    private float velocidadeOriginal;
    private bool bonusAtivo = false;  

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }

        danoOriginal = statusBase.dano; 
        velocidadeOriginal = statusBase.velocidadeMovimento;
    }

    void Update()
    {
        if (statusBase.vidaAtual < statusBase.vidaMaxima * 0.5f)
        {
            if (!bonusAtivo)
            {
                AplicarBonusPassivo();
            }
        }
        else
        {
            if (bonusAtivo)
            {
                RemoverBonusPassivo();
            }
        }
    }

    void AplicarBonusPassivo()
    {
        statusBase.velocidadeMovimento = velocidadeOriginal + velocidadeOriginal * (bonusVelocidade / 100);

        statusBase.dano = danoOriginal + danoOriginal * (bonusDano / 100);

        bonusAtivo = true;
    }

    void RemoverBonusPassivo()
    {
        statusBase.velocidadeMovimento = velocidadeOriginal;

        statusBase.dano = danoOriginal;

        bonusAtivo = false; 
    }
}
