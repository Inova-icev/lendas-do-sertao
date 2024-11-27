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
