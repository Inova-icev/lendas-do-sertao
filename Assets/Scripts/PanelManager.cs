using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using Unity.Mathematics;
using System;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using System.Linq.Expressions;

namespace ManagmentScripts
{
    public class PanelManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject painelMenu, painelOptions, painelLogin, painelLobby, painelTeam, painelPersonagens, background;

        [SerializeField]
        private Boolean isOffline = false;

        [SerializeField]
        private String roomName = "ICEV-Match";

        [SerializeField]
        private Boolean overrideRoomDefaultRules = false; // Não sendo utilizado ainda

        [SerializeField]
        private Button teamBlue, teamRed;

        public int teamChoice;

        GameManager gameManager = new GameManager();

        // Start is called before the first frame update
        void Start()
        {
            painelMenu.SetActive(true);
            painelOptions.SetActive(false);
            painelLogin.SetActive(false);
            painelLobby.SetActive(false);
            painelTeam.SetActive(false);
            painelPersonagens.SetActive(false);

            teamBlue.onClick.AddListener(() => ChooseTeam(1));
            teamRed.onClick.AddListener(() => ChooseTeam(2));
        }

        //fluxo menu(options)>login>lobby>nomesala
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
            Debug.Log("Conexão com o serviço bem sucessedida!");
            PhotonNetwork.JoinLobby();
            painelLogin.SetActive(false);
            painelLobby.SetActive(true);
        }

        public void GoToTeamChoice()
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
        }

        private void ChooseTeam(int team)
        {
            teamChoice = team;
            Debug.Log($"Time escolhido: {(team == 1 ? "Azul" : "Vermelho")}");
            GoToPersonagens(); // Chama a próxima etapa
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
            gameManager.SpawnPalyer();
            painelPersonagens.SetActive(false);
            background.SetActive(false);
        }

    }
}
