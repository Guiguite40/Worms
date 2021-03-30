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
       StartCoroutine(LaunchGame());
    }

    IEnumerator LaunchGame()
    {
        yield return new WaitForSeconds(5);
        print("game launched by coroutine");
        PhotonNetwork.LoadLevel(2);
    }

    //   void LaunchGame()
    //{
    //       print("game launched");
    //       PhotonNetwork.LoadLevel(2);
    //   }
}
