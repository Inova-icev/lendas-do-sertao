using UnityEngine;
using ManagmentScripts; // Namespace do PanelManager

public class Nexus : MonoBehaviour
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
        HandleGameEnd();
    }

    private void HandleGameEnd()
    {
        // Obtém todos os objetos com as tags "Left" e "Right"
        GameObject[] leftTeam = GameObject.FindGameObjectsWithTag("Left");
        GameObject[] rightTeam = GameObject.FindGameObjectsWithTag("Right");

        // Processa apenas jogadores com o componente Player
        ProcessPlayers(leftTeam);
        ProcessPlayers(rightTeam);
    }

    private void ProcessPlayers(GameObject[] team)
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
                    panelManager.ShowEndGamePanel(nexusTag, playerTag);
                }
            }
        }
    }
}
