using UnityEngine;
using Photon.Pun;
using ManagmentScripts; // Namespace do PanelManager

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
    }

    void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            photonView.RPC("HandleGameEnd", RpcTarget.All, nexusTag); 
        }
    }

    [PunRPC]
    private void HandleGameEnd(string destroyedNexusTag)
    {
        // Obtém todos os objetos com as tags "Left" e "Right"
        GameObject[] leftTeam = GameObject.FindGameObjectsWithTag("Left");
        GameObject[] rightTeam = GameObject.FindGameObjectsWithTag("Right");

        // Processa apenas jogadores com o componente Player
        ProcessPlayers(leftTeam, destroyedNexusTag);
        ProcessPlayers(rightTeam, destroyedNexusTag);
    }

    private void ProcessPlayers(GameObject[] team, string destroyedNexusTag)
    {
        foreach (GameObject obj in team)
        {
            // Verifica se o objeto tem o componente Player
            Player playerComponent = obj.GetComponent<Player>();
            if (playerComponent != null)
            {
                string playerTag = obj.tag;

                // Exibe vitória ou derrota baseado na tag
                if (panelManager != null)
                {
                    panelManager.ShowEndGamePanel(destroyedNexusTag, playerTag);
                }
            }
        }
    }
}
