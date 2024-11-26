using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumSePodePassiva : MonoBehaviour
{
    private StatusBase statusBase;

    [Header("Configuração da Aura")]
    public float alcanceAura = 3f;
    public LayerMask inimigoLayer;
    private List<StatusBase> inimigosAfetados = new List<StatusBase>();

    [Header("Redução de Atributos dos Inimigos")]
    public float reducaoDano = 0.1f; 
    public float reducaoVelocidadeAtaque = 0.1f;

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Num-se-pode!");
        }
    }

    void Update()
    {
        AplicarAura();
    }

    private void AplicarAura()
    {
        foreach (var inimigo in inimigosAfetados)
        {
            if (inimigo != null)
            {
                inimigo.dano /= (1 - reducaoDano);
                inimigo.velocidadeAtaque /= (1 - reducaoVelocidadeAtaque);
            }
        }
        inimigosAfetados.Clear();

        Collider2D[] inimigosDentroDaAura = Physics2D.OverlapCircleAll(transform.position, alcanceAura, inimigoLayer);

        foreach (Collider2D collider in inimigosDentroDaAura)
        {
            StatusBase inimigoStatus = collider.GetComponent<StatusBase>();
            if (inimigoStatus != null)
            {
                inimigoStatus.dano *= (1 - reducaoDano);
                inimigoStatus.velocidadeAtaque *= (1 - reducaoVelocidadeAtaque);
                inimigosAfetados.Add(inimigoStatus);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, alcanceAura);
    }
}
