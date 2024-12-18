using Photon.Pun;
using UnityEngine;
using ManagmentScripts;

public class Nexus : MonoBehaviourPun
{
    public string nexusTag; // Tag do Nexus (ex: "Left" ou "Right")

    private PanelManager panelManager;

    void Start()
    {
        // Localiza o PanelManager na cena
        panelManager = FindObjectOfType<PanelManager>();
        if (panelManager == null)
        {
            Debug.LogError("PanelManager não foi encontrado na cena!");
        }

        // Verifica se o PhotonView está configurado corretamente
        if (photonView == null)
        {
            Debug.LogError("PhotonView não encontrado neste GameObject!");
        }
    }

    void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient && photonView != null) // Apenas o MasterClient notifica a destruição
        {
            photonView.RPC("HandleGameEndRPC", RpcTarget.AllBuffered, nexusTag);
        }
    }

    [PunRPC]
    private void HandleGameEndRPC(string destroyedNexusTag)
    {
        if (panelManager == null)
        {
            Debug.LogError("PanelManager não foi encontrado durante HandleGameEndRPC!");
            return;
        }

        GameObject[] allPlayers = FindAllPlayers();
        foreach (GameObject player in allPlayers)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                string teamTag = GameManager.Instance.GetTeamTag();
                panelManager.ShowEndGamePanel(destroyedNexusTag, teamTag);
            }
        }
    }

    private GameObject[] FindAllPlayers()
    {
        // Combina os jogadores das tags "Left" e "Right"
        GameObject[] leftPlayers = GameObject.FindGameObjectsWithTag("Left");
        GameObject[] rightPlayers = GameObject.FindGameObjectsWithTag("Right");

        // Junta os dois arrays manualmente
        GameObject[] allPlayers = new GameObject[leftPlayers.Length + rightPlayers.Length];
        leftPlayers.CopyTo(allPlayers, 0);
        rightPlayers.CopyTo(allPlayers, leftPlayers.Length);

        return allPlayers;
    }
}
