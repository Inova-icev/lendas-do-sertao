using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.UIElements;
using ManagmentScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }
    private PanelManager panelManager;

    [SerializeField]
    private GameObject player;
    private GameObject nexusRed, nexusBlue;
    private Transform respawnPointTeamLeft;
    private Transform respawnPointTeamRight;
    private float respawnTime = 2f;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        panelManager = FindAnyObjectByType<PanelManager>();
        if (panelManager.teamChoiceTag == "Right")
        {
            player.tag = "Right";
            player.layer = 9;
        }
        else
        {
            player.tag = "Left";
            player.layer = 8;
        }

    }
    private void Awake()
    {
        // Ensure only one instance of the GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps GameManager persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any additional instances
        }
        PhotonNetwork.SendRate = 20; // Pacotes por segundo
        PhotonNetwork.SerializationRate = 20; // Dados de sincronização por segundo
    }

    public void SpawnPlayer(string team, string seletedCharacter)
    {
        if(team=="Left"){
        PhotonNetwork.Instantiate(seletedCharacter, new Vector3(-155.3f, 46.75f, 0), Quaternion.identity);
        }
        else{
        PhotonNetwork.Instantiate(seletedCharacter, new Vector3(-8.97f, 41.38f, 0), Quaternion.identity);
        }
    }

    private void OnEnable()
    {
        Vida_Player.OnPlayerDeath += HandlePlayerDeath; // Subscri��o no evento
    }

    private void OnDisable()
    {
        Vida_Player.OnPlayerDeath -= HandlePlayerDeath; // Desinscri��o no evento
    }

    private void HandlePlayerDeath(GameObject player)
    {
        StartCoroutine(RespawnPlayer(player));
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        Debug.Log($"{player.name} vai respawnar em {respawnTime} segundos...");
        yield return new WaitForSeconds(respawnTime);

        // Determina o ponto de respawn com base na tag
        Transform respawnPoint = player.CompareTag("Left") ? respawnPointTeamLeft : respawnPointTeamRight;

        // Reposiciona e reativa o jogador
        player.transform.position = respawnPoint.position;
        player.GetComponent<Vida_Player>().ResetHealth();
        player.SetActive(true);

        Debug.Log($"{player.name} respawnou!");
    }

    [PunRPC]
    public void EndGame(string winningTeam)
    {
        Debug.Log($"Fim de jogo! Vencedor: {winningTeam}");

        //if (gameOverUI != null)
        //{
        //    gameOverUI.SetActive(true);
        //}

        //var winnerText = gameOverUI.GetComponentInChildren<UnityEngine.UI.Text>();
        //if (winnerText != null)
        //{
        //    winnerText.text = $"Vencedor: {winningTeam}";
        //}

        //Time.timeScale = 0f;
    }

    [PunRPC]
    public void NexusDestroyed(string teamName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("EndGame", RpcTarget.All, teamName);
        }
    }
    public string GetTeamTag()
    {
        return panelManager.teamChoiceTag;
    }
}
