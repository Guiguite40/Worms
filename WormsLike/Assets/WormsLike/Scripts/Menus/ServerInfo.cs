using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image background;
    [SerializeField] private Image cadenas;
    [SerializeField] private Text textRoomName;
    [SerializeField] private Text textNbPlayer;
    [SerializeField] private Text textGamemode;
    [SerializeField] private Text textMapName;

    string roomName;
    int nbPlayer;
    byte nbPlayerMax;
    float currentAlphaBg;
    int id = 0;
    string password;
    string mapName;

    int gamemode;

    bool hasPassword = false;
    void Start()
    {
        //cadenas.gameObject.SetActive(false);
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

    public void SetServerInfo(int _id, string _roomName, int _nbPlayer, byte _nbPlayerMax, bool _hasPassword, string _password, int _indexGamemode)
	{
        id = _id;
        nbPlayer = _nbPlayer;
        nbPlayerMax = _nbPlayerMax;
        roomName = _roomName;
        hasPassword = _hasPassword;

        textRoomName.text = roomName;
        textNbPlayer.text = nbPlayer.ToString() + "/" + nbPlayerMax.ToString();

        if (hasPassword)
            cadenas.gameObject.SetActive(true);
        else
            cadenas.gameObject.SetActive(false);

        password = _password;
        gamemode = _indexGamemode;
        if (gamemode == 0)
            textGamemode.text = "Team deathmatch";
        else if (gamemode == 1)
            textGamemode.text = "Forts";

        //mapName = _mapName;
        //textMapName.text = mapName;
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

    public void SetCadenas()
	{
        cadenas.gameObject.SetActive(true);
	}

    public bool IsCadenasActive()
	{
        if (cadenas.gameObject.activeSelf)
            return true;
        else
            return false;
	}

    public byte GetNbPlayerMax()
	{
        return nbPlayerMax;
	}

    public string GetPassword()
	{
        return password;
	}

    public string GetMapName()
	{
        return mapName;
	}
}
