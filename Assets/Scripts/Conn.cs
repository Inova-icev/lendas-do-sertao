using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class Conn : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject painelL, painelS;
    [SerializeField]
    private InputField nickName, nomeSala;
    [SerializeField]
    private Text txtNick;

    // Start is called before the first frame update
    void Start()
    {
        painelS.SetActive(false);
    }

    public void Login()
    {
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.ConnectUsingSettings();
        painelL.SetActive(false);
        painelS.SetActive(true);
    }

    public void CriaSala()
    {
        PhotonNetwork.JoinOrCreateRoom(nomeSala.text, new RoomOptions(), TypedLobby.Default);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexão com o serviço bem sucessedida!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Dentro do lobby!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Conexão perdida");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Falha ao entrar na sala!");
        //PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala!");
        print(PhotonNetwork.CurrentRoom.Name);
        print(PhotonNetwork.CurrentRoom.PlayerCount);
        print(PhotonNetwork.NickName);
        txtNick.text = PhotonNetwork.NickName;
    }
}