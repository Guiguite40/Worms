using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image background;
    [SerializeField] private Text textRoomName;

    float currentAlphaBg;
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
    }

    public void UndrawBackground()
	{
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
    }

    public void SetServerInfo(string _roomName)
	{
        textRoomName.text = _roomName;
	}
}
