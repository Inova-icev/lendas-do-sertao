using System.Collections;
using UnityEngine;

public class CurupiraShift : MonoBehaviour
{
    private StatusBase statusBase;

    public KeyCode teclaAtivacao = KeyCode.LeftShift;  
    public float aumentoRegeneracaoVida = 30f;
    public float duracaoEfeito = 5f;            
    public float cooldown = 75f;                

    private float tempoRecarga;                 
    private bool emCooldown = false;           

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
            StartCoroutine(AtivarCurupiraShift());
        }

        if (emCooldown)
        {
            tempoRecarga -= Time.deltaTime;
        }
    }

    IEnumerator AtivarCurupiraShift()
    {
        emCooldown = true;
        float valorOriginal = statusBase.regeneracaoVida; 
        statusBase.regeneracaoVida += valorOriginal * (aumentoRegeneracaoVida / 100);

        yield return new WaitForSeconds(duracaoEfeito);

        statusBase.regeneracaoVida = valorOriginal;

        yield return new WaitForSeconds(cooldown - duracaoEfeito);  
        emCooldown = false; 
    }
}
