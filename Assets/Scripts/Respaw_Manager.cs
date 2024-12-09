using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform[] respawnPointsTeamLeft;  
    public Transform[] respawnPointsTeamRight; 

    public void RespawnPlayer(GameObject player)
    {
        Transform[] selectedSpawnPoints;

        // Verifica a tag do jogador e seleciona os pontos de respawn adequados
        if (player.CompareTag("Left"))
        {
            selectedSpawnPoints = respawnPointsTeamLeft;
        }
        else if (player.CompareTag("Right"))
        {
            selectedSpawnPoints = respawnPointsTeamRight;
        }
        else
        {
            Debug.LogError("Player tag not recognized!");
            return;
        }

        // Escolhe um ponto aleatório entre os pontos de respawn selecionados
        int randomIndex = Random.Range(0, selectedSpawnPoints.Length);

        // Reposiciona o jogador no ponto de respawn escolhido
        player.transform.position = selectedSpawnPoints[randomIndex].position;
        player.transform.rotation = selectedSpawnPoints[randomIndex].rotation;
    }
}
