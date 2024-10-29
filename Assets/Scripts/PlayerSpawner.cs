using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Vector2 spawnPosition = new Vector2(0,0);
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady){
            SpawnPlayer();
        }
    }

    public void SpawnPlayer(){
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
