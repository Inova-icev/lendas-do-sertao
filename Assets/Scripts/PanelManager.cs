using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace ManagmentScripts
{
    public class PanelManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject painelMenu, painelOptions, painelLogin, painelLobby, painelTeam, painelPersonagens, background, panelVitoria, panelDerrota;

        [SerializeField]
        private GameObject GameHUDCanvas; // HUD do jogo

        [SerializeField]
        private bool isOffline = false;

        [SerializeField]
        private string roomName = "ICEV-Match";

        [SerializeField]
        private bool overrideRoomDefaultRules = false; // Não sendo utilizado ainda

        [SerializeField]
        private Button teamBlue, teamRed;

        [SerializeField]
        private MinionSpawner[] minionSpawners; // Array para os spawners de minions (esquerda e direita)

        public string teamChoiceTag;

        private GameManager gameManager; // Referência ao GameManager

        // Start is called before the first frame update
        void Start()
        {
            // Inicializa os painéis
            painelMenu.SetActive(true);
            painelOptions.SetActive(false);
            painelLogin.SetActive(false);
            painelLobby.SetActive(false);
            painelTeam.SetActive(false);
            painelPersonagens.SetActive(false);
            panelVitoria.SetActive(false);
            panelDerrota.SetActive(false);

            // Certifica-se de que o GameHUDCanvas começa desativado
            if (GameHUDCanvas != null)
            {
                GameHUDCanvas.SetActive(false);
            }

            // Configura os botões de seleção de time
            teamBlue.onClick.AddListener(() =>teamChoiceTag="Right");
            teamBlue.onClick.AddListener(() =>Debug.LogError("equipe azul escolhida"));
            teamRed.onClick.AddListener(() => teamChoiceTag="Left");
            teamRed.onClick.AddListener(() =>Debug.LogError("equipe vermelha escolhida"));

            // Obtém a referência ao GameManager na cena
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager não foi encontrado na cena!");
            }
        }

        public void OpenOptions()
        {
            painelMenu.SetActive(false);
            painelOptions.SetActive(true);
        }
        public void CloseOptions()
        {
            painelMenu.SetActive(true);
            painelOptions.SetActive(false);
        }
        public void GoToLogin()
        {
            painelMenu.SetActive(false);
            painelLogin.SetActive(true);
        }

        public void GoToLobby()
        {
            PhotonNetwork.OfflineMode = isOffline;
            if (isOffline)
            {
                PhotonNetwork.CreateRoom("Offline-ICEV-Match", new RoomOptions());
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Conexão com o serviço bem-sucedida!");
            PhotonNetwork.JoinLobby();
            painelLogin.SetActive(false);
            painelLobby.SetActive(true);
        }

        public void GoToTeamChoice()
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
        }

        public void GoToPersonagens()
        {
            painelTeam.SetActive(false);
            painelPersonagens.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Falha ao entrar na sala!");
            PhotonNetwork.CreateRoom("ICEV-Match", new RoomOptions());
            painelLobby.SetActive(false);
            painelTeam.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            painelLobby.SetActive(false);
            painelTeam.SetActive(true);
        }  

        public void SelectedPersonagem()
        {
            gameManager.SpawnPlayer();
            painelPersonagens.SetActive(false);
            background.SetActive(false);

            if (GameHUDCanvas != null)
            {
                GameHUDCanvas.SetActive(true);
            }

            // Ativa o spawn de minions
            foreach (var spawner in minionSpawners)
            {
                if (spawner != null)
                {
                    StartCoroutine(spawner.SpawnWave());
                    Debug.Log("AAAAAAAAAAA");
                }
                else
                {
                    Debug.LogError("Spawner está faltando no array MinionSpawners!");
                }
            }
        }
        public void ShowEndGamePanel(string nexusTag, string playerTag)
        {
            if (playerTag == nexusTag)
            {
                // Jogador perdeu
                panelDerrota.SetActive(true);
                Debug.Log("Derrota para o jogador com tag: " + playerTag);
            }
            else
            {
                // Jogador venceu
                panelVitoria.SetActive(true);
                Debug.Log("Vitória para o jogador com tag: " + playerTag);
            }

            // Opcional: Desativa a HUD do jogo
            if (GameHUDCanvas != null)
            {
                GameHUDCanvas.SetActive(false);
            }
        }
         public void SetTag(string newTag)
        {
            gameObject.tag = newTag;
            Debug.Log($"Tag alterada para: {newTag}");
        }
    }
}

