using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text textRoomName;
    [SerializeField] private Text textRoomPassword;
    [SerializeField] private Text textPlayerMax;
    [SerializeField] private Text textGamemode;
    [SerializeField] private Text textMap;

    void Start()
    {
        SetRoomInfo();
    }

    void Update()
    {
        
    }

    void SetRoomInfo()
	{
        print("room info set");
        textRoomName.text = PhotonNetwork.CurrentRoom.Name;
        textPlayerMax.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            textRoomPassword.text = MenuManager.instance.GetRoomPassword();
        else
            textRoomPassword.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["password"];
    }
}
