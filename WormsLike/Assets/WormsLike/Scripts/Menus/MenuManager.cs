using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject canvasCreateGame;
    [SerializeField] private GameObject canvasListServer;
    [SerializeField] private GameObject canvasParameter;
    [SerializeField] private InputField impFieldRoomName;
    [SerializeField] private GameObject impFieldPassword;
    [Space(10)]
    [SerializeField] private Text textStateInfoCreate;
    [SerializeField] private Text textStateInfoJoin;

    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    bool isGamePrivate = false;

    private void Awake()
    {
        PhotonNetwork.JoinLobby();
    }

    void Start()
    {
        canvas.Add("create", canvasCreateGame);
        canvas.Add("join", canvasListServer);
        canvas.Add("parameter", canvasParameter);
        CloseCanvas("all");
    }

    void Update()
    {
        
    }

    void OpenCanvas(string _keyName)
    {
        canvas["create"].SetActive(false);
        canvas["join"].SetActive(false);
        canvas["parameter"].SetActive(false);

        canvas[_keyName].SetActive(true);
    }

    void CloseCanvas(string _keyName)
	{
        if(_keyName == "all")
		{
            canvas["create"].SetActive(false);
            canvas["join"].SetActive(false);
            canvas["parameter"].SetActive(false);
            impFieldPassword.SetActive(false);
            textStateInfoCreate.gameObject.SetActive(false);
            textStateInfoJoin.gameObject.SetActive(false);
        }
        else
            canvas["_keyName"].SetActive(false);
    }

    public void OnClickCloseCanvas()
    {
        CloseCanvas("all");
    }

    void SendText(string _textToModify, string _textToSend)
	{
        if (_textToModify == "create")
        {
            textStateInfoCreate.gameObject.SetActive(true);
            textStateInfoCreate.text = _textToSend;
        }
        else if (_textToModify == "join")
        {
            textStateInfoJoin.gameObject.SetActive(true);
            textStateInfoJoin.text = _textToSend;
        }
    }

    ////////////////// CREATE GAME //////////////////
    public void OnClickCreateGame()
	{
        CloseCanvas("all");
        OpenCanvas("create");
	}

    public void OnClickIsPrivate()
	{
        if (impFieldPassword.activeSelf)
        {
            impFieldPassword.SetActive(false);
            isGamePrivate = false;
        }
        else
        {
            impFieldPassword.SetActive(true);
            isGamePrivate = true;
        }
    }

    public void OnClickCreateLobby()
	{
        if(PhotonNetwork.InRoom)
		{
            SendText("create", "<color=#ff0000> Room already created </color>");
            return;
		}

        if(string.IsNullOrEmpty(impFieldRoomName.text))
		{
            SendText("create", "<color=#ff0000> Room name cannot be empty </color>");
            return;
        }

        PhotonNetwork.CreateRoom(impFieldRoomName.text);
	}

	public override void OnCreatedRoom()
    {
        SendText("create", "<color=#00ff00> Room created, joining room </color>");
    }

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
        SendText("create", "<color=#00ff00> Room creation failed, error code : " + returnCode + "</color>"); 
	}

	////////////////// /////////// //////////////////

	////////////////// JOIN GAME //////////////////
	public void OnClickJoinGame()
    {
        CloseCanvas("all");
        OpenCanvas("join");
    }

	public override void OnJoinedRoom()
	{
        SendText("join", "<color=#00ff00> Room joined, loading lobby </color>");
        if (canvasCreateGame.activeSelf)
            SendText("create", "<color=#00ff00> Room joined, loading lobby </color>");
        LaunchLobby();
    }

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
        SendText("join", "<color=#00ff00> join room failed, error code : " + returnCode + "</color>");
        if(canvasCreateGame.activeSelf)
            SendText("create", "<color=#00ff00> join room failed, error code : " + returnCode + "</color>");
    }

	public void LaunchLobby()
	{
        PhotonNetwork.LoadLevel("Lobby");
    }

    ////////////////// /////////// //////////////////

    public void OnClickParameter()
    {
        CloseCanvas("all");
        OpenCanvas("parameter");
    }

    public void OnClickQuit()
	{
        Application.Quit();
	}
}
