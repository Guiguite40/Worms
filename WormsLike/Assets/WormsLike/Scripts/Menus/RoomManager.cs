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

    [SerializeField] private Button buttonPlusPlayerMax;
    [SerializeField] private Button buttonMinusPlayerMax;


    byte playerMax;
    void Start()
    {
        playerMax = PhotonNetwork.CurrentRoom.MaxPlayers;
        SetRoomInfo();
    }

    void Update()
    {
        UpdateParameters();
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

    void UpdateParameters()
	{
        UpdateButtons();
        textPlayerMax.text = playerMax.ToString();
    }

    public void UpdateButtons()
	{
        if (PhotonNetwork.CurrentRoom.MaxPlayers >= 6)
            buttonPlusPlayerMax.interactable = false;
        else
            buttonPlusPlayerMax.interactable = true;

        if (PhotonNetwork.CurrentRoom.MaxPlayers <= 2)
            buttonMinusPlayerMax.interactable = false;
        else
            buttonMinusPlayerMax.interactable = true;

    }

    public void IncrementPlayerMax()
	{
        if (playerMax < 6)
            playerMax += 2;

        PhotonNetwork.CurrentRoom.MaxPlayers = playerMax;
        UpdateServerInfo();
    }

    public void DecrementPlayerMax()
    {
        if (playerMax > 2)
            playerMax -= 2;

        PhotonNetwork.CurrentRoom.MaxPlayers = playerMax;
        UpdateServerInfo();
    }

    public void UpdateServerInfo()
    {
        ServerInfo[] currentServersFind = MenuManager.instance.GetServersInfo();
        for (int y = 0; y < currentServersFind.Length; y++)
        {
            if(currentServersFind[y].GetRoomName() == PhotonNetwork.CurrentRoom.Name)
			{
                bool hasPassword;
                if (textRoomPassword.text == "")
                    hasPassword = false;
                else
                    hasPassword = true;

                currentServersFind[y].SetServerInfo(currentServersFind[y].GetServerId(), PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, playerMax, hasPassword);

            }
        }
    }
}
