using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject player;

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(PhotonNetwork.NickName,
        new Vector3(-22.37f, -1.43f,0),Quaternion.identity);
    }
}
