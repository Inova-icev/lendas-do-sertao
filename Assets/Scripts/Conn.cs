using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace ManagmentScripts{
    public class Conn : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private InputField nickName;
        [SerializeField]
        private Text txtNick, txtSala;



        public void loginConection()
        {
            PhotonNetwork.NickName = nickName.text;
            txtNick.text = PhotonNetwork.NickName;
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Dentro do lobby!");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Conexão perdida");
        }



        public override void OnJoinedRoom()
        {
            Debug.Log("Entrou na sala!" + PhotonNetwork.CurrentRoom.Name);
            txtSala.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
            print("Jogador atual:" + PhotonNetwork.NickName);
            print("Sala atual: " + PhotonNetwork.CurrentRoom.Name);
            print("Numero de jogadores na sala: " + PhotonNetwork.CurrentRoom.PlayerCount);

        }
    }
}