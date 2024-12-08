using System.Collections;
using UnityEngine;

public class CurupiraF : MonoBehaviour
{
    private StatusBase statusBase;

    public KeyCode teclaAtivacao = KeyCode.F;  
    public float danoInicialPorcentagem = 20f;   
    public float danoQueimaduraPorcentagem = 1f; 
    public float duracaoQueimadura = 10f;       
    public float cooldown = 90f;              
    public Transform pontoAtaque;             
    public float alcanceAtaque = 5f;            
    private float tempoRecarga;                 
    private bool emCooldown = false;            

    public LayerMask inimigoLayers;            

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaAtivacao) && !emCooldown)
        {
            StartCoroutine(AtivarCurupiraF());
        }

        if (emCooldown)
        {
            tempoRecarga -= Time.deltaTime;
        }
    }

    IEnumerator AtivarCurupiraF()
    {
        emCooldown = true;

        Collider2D[] inimigosNaArea = Physics2D.OverlapCircleAll(pontoAtaque.position, alcanceAtaque, inimigoLayers);

        foreach (Collider2D inimigo in inimigosNaArea)
        {
            float danoInicial = inimigo.GetComponent<StatusBase>().vidaAtual * (danoInicialPorcentagem / 100);
            DefesaSistema defesaInimigo = inimigo.GetComponent<DefesaSistema>();
            if (defesaInimigo != null)
            {
                defesaInimigo.ReceberDano(danoInicial, 0);
                Debug.Log($"Inimigo atingido com {danoInicial} de dano inicial!");
            }

            StartCoroutine(AplicarQueimadura(inimigo));
        }

        yield return new WaitForSeconds(cooldown);

        emCooldown = false;
    }

    IEnumerator AplicarQueimadura(Collider2D inimigo)
    {
        float vidaInimigo = inimigo.GetComponent<StatusBase>().vidaAtual;

        for (float tempo = 0; tempo < duracaoQueimadura; tempo += 1f)
        {
            float danoQueimadura = vidaInimigo * (danoQueimaduraPorcentagem / 100);
            DefesaSistema defesaInimigo = inimigo.GetComponent<DefesaSistema>();
            if (defesaInimigo != null)
            {
                defesaInimigo.ReceberDano(danoQueimadura, 0);
                Debug.Log($"Inimigo sofreu {danoQueimadura} de queimadura.");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar o alcance da habilidade no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pontoAtaque.position, alcanceAtaque);
    }
}
