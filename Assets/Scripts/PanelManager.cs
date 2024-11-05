using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;


public class PanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject painelMenu, painelOptions, painelLogin, painelLobby, painelPersonagens, background;


    // Start is called before the first frame update
    void Start()
    {
        painelOptions.SetActive(false);
        painelLogin.SetActive(false);
        painelLobby.SetActive(false);
        painelPersonagens.SetActive(false);
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
        PhotonNetwork.ConnectUsingSettings();
    }

      public override void OnConnectedToMaster()
    {
        Debug.Log("Conexão com o serviço bem sucessedida!");
        PhotonNetwork.JoinLobby();
        painelLogin.SetActive(false);
        painelLobby.SetActive(true);
    }

    public void GoToPersonagens()
    {
        PhotonNetwork.JoinOrCreateRoom("ICEV-Match", new RoomOptions(), TypedLobby.Default);
    }

       public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Falha ao entrar na sala!");
        PhotonNetwork.CreateRoom("ICEV-Match", new RoomOptions());
        painelLobby.SetActive(false);
        painelPersonagens.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        painelLobby.SetActive(false);
        painelPersonagens.SetActive(true);
    }
    public void SelectedPersonagem()
    {
        painelPersonagens.SetActive(false);
        background.SetActive(false);
    }
}