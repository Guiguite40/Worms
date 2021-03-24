using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuSample : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonNetwork.JoinLobby();
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void OnClickCreate()
	{
        PhotonNetwork.CreateRoom("roomSample");
	}

    public void OnClickJoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        LaunchGame();
    }

    void LaunchGame()
	{
        print("game launched");
        PhotonNetwork.LoadLevel(1);
    }
}
