using UnityEngine;

public class LobisomemPassiva : MonoBehaviour
{
    private StatusBase status;
    private float buffVelocidadeAtaque;
    private float buffRouboDeVida;
    private bool buffAtivo = false;

    public LayerMask inimigoLayers; 
    public float distanciaDeteccao = 5f; 

    void Start()
    {
        status = GetComponent<StatusBase>();
        AtualizarBuffs();
    }

    void Update()
    {
        if (DetectarInimigosComPocaVida())
        {
            AtivarBuff();
        }
        else if (buffAtivo)
        {
            DesativarBuff();
        }
    }

    private void AtualizarBuffs()
    {
        if (status.level >= 2)
        {
            buffVelocidadeAtaque = 0.15f; 
            buffRouboDeVida = 2f;      
        }
        else
        {
            buffVelocidadeAtaque = 0.10f; 
            buffRouboDeVida = 1f;     
        }
    }

    private bool DetectarInimigosComPocaVida()
    {
        Collider2D[] inimigosProximos = Physics2D.OverlapCircleAll(transform.position, distanciaDeteccao, inimigoLayers);
        foreach (Collider2D inimigo in inimigosProximos)
        {
            StatusBase statusInimigo = inimigo.GetComponent<StatusBase>();
            if (statusInimigo != null && statusInimigo.vidaAtual <= statusInimigo.vidaMaxima * 0.3f)
            {
                return true;
            }
        }
        return false;
    }

    private void AtivarBuff()
    {
        if (!buffAtivo)
        {
            status.velocidadeAtaque += status.velocidadeAtaque * buffVelocidadeAtaque;
            status.rouboDeVida += buffRouboDeVida; 
            buffAtivo = true;

            Debug.Log("Buff de velocidade de ataque e roubo de vida ativado!");
        }
    }

    private void DesativarBuff()
    {
        status.velocidadeAtaque /= (1 + buffVelocidadeAtaque);
        status.rouboDeVida -= buffRouboDeVida;
        buffAtivo = false;

        Debug.Log("Buff de velocidade de ataque e roubo de vida desativado!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }
}
