using System.Collections;
using UnityEngine;

public class LobisomemQ : MonoBehaviour
{
    public float alcanceHabilidade = 4f;
    public LayerMask inimigoLayer;
    public float reducaoVelocidadeAtaque = 0.1f;
    public float reducaoResistencia = 0.2f;
    public float duracaoEfeito = 5f;
    public float cooldown = 60f;
    public int level = 0;
    private bool emCooldown = false;
    public KeyCode teclaHabilidade = KeyCode.Q;


    void Update()
    {
        if (Input.GetKeyDown(teclaHabilidade))
        {
            UsarHabilidade();
        }
    }

    public void UsarHabilidade()
    {
        if (emCooldown)
        {
            Debug.Log("Habilidade ainda está em recarga!");
            return;
        }

        Debug.Log("Habilidade ativada!");
        StartCoroutine(AtivarHabilidade());
    }

    private IEnumerator AtivarHabilidade()
    {
        // Ativação da habilidade
        Debug.Log("Lobisomem soltou um uivo!");
        Collider2D[] inimigosAfetados = Physics2D.OverlapCircleAll(transform.position, alcanceHabilidade, inimigoLayer);

        foreach (Collider2D inimigo in inimigosAfetados)
        {
            StatusBase inimigoStatus = inimigo.GetComponent<StatusBase>();
            if (inimigoStatus != null)
            {
                // Aplica as reduções aos inimigos
                inimigoStatus.velocidadeAtaque *= (1 - reducaoVelocidadeAtaque);
                inimigoStatus.armadura *= (1 - reducaoResistencia);
                inimigoStatus.resistenciaMagica *= (1 - reducaoResistencia);
                Debug.Log($"Inimigo {inimigo.name} afetado: Velocidade e resistência reduzidas.");
            }
        }

        // Aguarda a duração do efeito
        yield return new WaitForSeconds(duracaoEfeito);

        // Remove os efeitos
        foreach (Collider2D inimigo in inimigosAfetados)
        {
            StatusBase inimigoStatus = inimigo.GetComponent<StatusBase>();
            if (inimigoStatus != null)
            {
                inimigoStatus.velocidadeAtaque /= (1 - reducaoVelocidadeAtaque);
                inimigoStatus.armadura /= (1 - reducaoResistencia);
                inimigoStatus.resistenciaMagica /= (1 - reducaoResistencia);
                Debug.Log($"Efeito do uivo removido do inimigo {inimigo.name}.");
            }
            else
            {
                emCooldown = true;
                yield return new WaitForSeconds(cooldown);
                emCooldown = false;
            }
        }

        // Inicia o cooldown
        emCooldown = true;
        yield return new WaitForSeconds(cooldown);
        emCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, alcanceHabilidade);
    }
}
