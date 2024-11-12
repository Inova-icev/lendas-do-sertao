using System.Collections;
using UnityEngine;

public class MapinguariPassiva : MonoBehaviour
{
    public float areaDeEfeito = 5f;
    public float danoBase = 15f; 
    public float danoMaximo = 45f; 
    public float intervaloDano = 1f; 
    public LayerMask inimigoLayer; 
    private StatusBase statusBase;

    private float tempoAcumulado = 0f; 

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no Mapinguari!");
        }
    }

    void Update()
    {
        AplicarDanoAosInimigos();
    }

    void AplicarDanoAosInimigos()
    {
        if (statusBase == null) return;

        int nivel = statusBase.level;
        float danoPorSegundoCalculado = Mathf.Lerp(15f, 45f, Mathf.InverseLerp(1f, 12f, statusBase.level));

        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(transform.position, areaDeEfeito, inimigoLayer);

        tempoAcumulado += Time.deltaTime;

        if (tempoAcumulado >= intervaloDano)
        {
            tempoAcumulado = 0f;

            foreach (Collider2D col in inimigosAfetados)
            {
                MorteDoInimigo morteInimigo = col.GetComponent<MorteDoInimigo>();
                if (morteInimigo != null && !morteInimigo.IsDead())
                {
                    morteInimigo.DanoNoinimigo(danoPorSegundoCalculado);
                    Debug.Log("O inimigo recebeu " + danoPorSegundoCalculado + " de dano");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, areaDeEfeito);
    }
}
