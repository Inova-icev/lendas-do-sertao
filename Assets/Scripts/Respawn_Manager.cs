using UnityEngine;
using Photon.Pun;

public class RespawnManager : MonoBehaviour
{
    public Transform[] respawnPointsTeamLeft;  // Pontos de respawn do time Left
    public Transform[] respawnPointsTeamRight; // Pontos de respawn do time Right

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
        Transform selectedPoint = selectedSpawnPoints[randomIndex];

        Vector3 respawnPosition = selectedPoint.position;
        Quaternion respawnRotation = selectedPoint.rotation;

        // Sincroniza o respawn com Photon
        if (PhotonNetwork.IsConnected)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("SyncRespawn", RpcTarget.All, respawnPosition, respawnRotation);
            }
        }
        else
        {
            // Atualiza diretamente no modo offline
            player.transform.position = respawnPosition;
            player.transform.rotation = respawnRotation;
            Debug.Log($"Player {player.name} respawnado em {respawnPosition}");
        }
    }

    [PunRPC]
    private void SyncRespawn(Vector3 position, Quaternion rotation)
    {
        // Atualiza a posição e rotação do jogador
        transform.position = position;
        transform.rotation = rotation;
        Debug.Log($"SyncRespawn: Posição sincronizada para {position}");
    }

}
