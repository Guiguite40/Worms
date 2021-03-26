using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image background;
    [SerializeField] private Text textRoomName;
    [SerializeField] private Text textNbPlayer;

    string roomName;
    int nbPlayer;
    byte nbPlayerMax;
    float currentAlphaBg;
    int id = 0;
    void Start()
    {
        currentAlphaBg = background.color.a;
        DrawBackground();
    }

    void Update()
    {
        
    }

    public void DrawBackground()
    {
        MenuManager.instance.UndrawAllServer();

        if (background.color.a == 0)
            background.color = new Color(background.color.r, background.color.g, background.color.b, currentAlphaBg);
        else
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0);

        MenuManager.instance.OnClickOnServer();
    }

    public void UndrawBackground()
	{
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
    }

    public void SetServerInfo(int _id, string _roomName, int _nbPlayer, byte _nbPlayerMax)
	{
        id = _id;
        nbPlayer = _nbPlayer;
        nbPlayerMax = _nbPlayerMax;
        roomName = _roomName;

        textRoomName.text = roomName;
        textNbPlayer.text = nbPlayer.ToString() + "/" + nbPlayerMax.ToString();
	}

    public int GetServerId()
	{
        return id;
	}

    public bool IsSelected()
	{
        if (background.color.a != 0)
            return true;
        else
            return false;
	}

    public string GetRoomName()
	{
        return roomName;
	}
}
