using UnityEngine;

public class TriggerMandacaru : MonoBehaviour
{
    public float buffDuration = 5f;  // Duração do buff em segundos
    public Color buffColor = Color.yellow; // Cor do jogador com o buff ativo

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger.");
            ApplyBuff(other.gameObject);
        }
    }

    // Método para aplicar o buff ao jogador
    private void ApplyBuff(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>(); // Obtém o componente PlayerMovement do jogador

        // Verifica se o jogador possui o componente PlayerMovement
        if (playerMovement != null)
        {
            Debug.Log("Aplicando buff ao jogador."); // Log para confirmar que o buff está sendo aplicado
            playerMovement.speed *= 2;  // Dobra a velocidade do jogador
            playerMovement.isBuffActive = true; // Marca o buff como ativo
            playerMovement.ChangeColor(buffColor);

            // Inicia a coroutine para remover o buff após a duração especificada
            StartCoroutine(RemoveBuffAfterDuration(playerMovement));
        }
        else
        {
            Debug.LogWarning("PlayerMovement componente não encontrado no jogador."); // Aviso caso o componente PlayerMovement não seja encontrado
        }
    }

    // Coroutine para remover o buff após a duração especificada
    private System.Collections.IEnumerator RemoveBuffAfterDuration(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(buffDuration); // Espera pelo tempo de duração do buff
        
        // Verifica se playerMovement ainda é válido antes de restaurar a velocidade
        if (playerMovement != null)
        {
            playerMovement.speed /= 2;  // Restaura a velocidade original do jogador
            playerMovement.isBuffActive = false; // Marca o buff como inativo
            playerMovement.RestoreOriginalColor(); // Restaura a cor original do jogador
        }

        Debug.Log("Buff removido do jogador."); // Log para confirmar que o buff foi removido
    }
}
