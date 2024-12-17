using UnityEngine;
using Photon.Pun;

public class RespawnManager : MonoBehaviour
{
    public Transform[] respawnPointsTeamLeft;  // Pontos de respawn do time Left
    public Transform[] respawnPointsTeamRight; // Pontos de respawn do time Right

    public void RespawnPlayer(GameObject player)
    {
        Debug.Log("RespawnManager: Tentando reposicionar o jogador...");

        Transform[] selectedSpawnPoints;

        // Determina o ponto de respawn baseado na tag do jogador
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
            Debug.LogError("Tag do jogador não reconhecida!");
            return;
        }

        // Seleciona um ponto de respawn aleatório
        int randomIndex = Random.Range(0, selectedSpawnPoints.Length);
        Transform spawnPoint = selectedSpawnPoints[randomIndex];

        Debug.Log($"RespawnManager: Ponto de respawn selecionado: {spawnPoint.position}");

        // Sincroniza o reposicionamento usando RPC
        PhotonView photonView = player.GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine)
        {
            Debug.Log("RespawnManager: Chamando SyncRespawn via RPC...");
            photonView.RPC("SyncRespawn", RpcTarget.AllBuffered, spawnPoint.position);
        }
        else
        {
            Debug.LogWarning("PhotonView inválido ou o Player não é meu.");
            player.transform.position = spawnPoint.position; // Offline fallback
        }
    }
}
