using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject player;
    private Transform respawnPointTeamLeft;
    private Transform respawnPointTeamRight;
    private float respawnTime = 2f;

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
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate("Saci", new Vector3(-128.3f, 43.5f, 0), Quaternion.identity);
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
}
