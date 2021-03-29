using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager instance;

    [SerializeField] private GameObject canvasCreateGame;
    [SerializeField] private GameObject canvasListServer;
    [SerializeField] private GameObject canvasParameter;
    [SerializeField] private GameObject canvasEnterPassword;
    [SerializeField] private InputField impFieldRoomName;
    [SerializeField] private GameObject impFieldPassword;
    [SerializeField] private InputField impFieldPasswordText;
    [SerializeField] private InputField impFieldPasswordToEnter;
    [Space(10)]
    [SerializeField] private Text textStateInfoCreate;
    [SerializeField] private Text textStateInfoJoin;
    [Space(8)]
    [SerializeField] private Button buttonPlus;
    [SerializeField] private Button buttonMinus;
    [SerializeField] private Text textNbPlayerMax;
    [Space(8)]
    [SerializeField] private GameObject serverPrefab;

    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    bool isGamePrivate = false;
    List<ServerInfo> listServer = new List<ServerInfo>();
    List<RoomInfo> listRoom = new List<RoomInfo>();

    List<string> listRoomName = new List<string>();

    int serverId = 0;
    int serverIdSelected;
    byte nbPlayerMax = 4;

    bool isRoomCreatedHasPassword = false;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        canvas.Add("create", canvasCreateGame);
        canvas.Add("join", canvasListServer);
        canvas.Add("parameter", canvasParameter);
        CloseCanvas("all");

        canvasEnterPassword.SetActive(false);
    }

    void Update()
    {
        //print("nb player in rooms : " + PhotonNetwork.CountOfPlayersInRooms);
        if(canvasCreateGame.activeSelf)
            UpdateButtonNbPlayer();
    }

    void SetupCustomProperty()
    {
        //Hashtable hash = new Hashtable();
        //hash.Add("password", impFieldPasswordText.text);
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("password", impFieldPasswordText.text);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
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
        if (canvasEnterPassword.activeSelf)
        {
            impFieldPasswordToEnter.text = "";
            canvasEnterPassword.SetActive(false);
        }
        else
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

	public override void OnConnectedToMaster()
	{
        PhotonNetwork.JoinLobby();
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

    public void OnClickButtonNbPlayer(bool _isPlus)
	{
        UpdateButtonNbPlayer();
        if (_isPlus)
        {
            if (nbPlayerMax < 5)
            {
                nbPlayerMax+=2;
            }
        }
        else
        {
            if (nbPlayerMax > 3)
            {
                nbPlayerMax-=2;
            }
        }
    }

    void UpdateButtonNbPlayer()
	{
        if(nbPlayerMax == 2)
            textNbPlayerMax.text =  "1v1 (" + nbPlayerMax.ToString() + ")";
        if (nbPlayerMax == 4)
            textNbPlayerMax.text = "2v2 (" + nbPlayerMax.ToString() + ")";
        if (nbPlayerMax == 6)
            textNbPlayerMax.text = "3v3 (" + nbPlayerMax.ToString() + ")";

        if (nbPlayerMax < 6)
            buttonPlus.interactable = true;
        else
            buttonPlus.interactable = false;

        if (nbPlayerMax > 2)
            buttonMinus.interactable = true;
        else
            buttonMinus.interactable = false;
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

        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = nbPlayerMax;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("pm", nbPlayerMax);
        hash.Add("pw", impFieldPasswordText.text);
        roomOption.CustomRoomProperties = hash;

        string[] lobbySettings = new string[2];
        lobbySettings[0] = "pm";
        lobbySettings[1] = "pw";
        roomOption.CustomRoomPropertiesForLobby = lobbySettings;

        PhotonNetwork.CreateRoom(impFieldRoomName.text, roomOption);
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

    public void OnClickOnServer()
	{
        print("cliick on server");
        ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        for (int y = 0; y < currentServersFind.Length; y++)
            if (currentServersFind[y].IsSelected())
                serverIdSelected = currentServersFind[y].GetServerId();

        print("server id selected : " + serverIdSelected);
    }

    public void UndrawAllServer()
	{
        for (int i = 0; i < listServer.Count; i++)
        {
            listServer[i].UndrawBackground();
        }
    }

    public void OnClickJoinServer()
	{
        bool isCadenasToActivate = false;
        ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        for (int y = 0; y < currentServersFind.Length; y++)
        {
            if (serverIdSelected == currentServersFind[y].GetServerId())
            {
                if (currentServersFind[y].IsCadenasActive())
                    isCadenasToActivate = true;
            }
        }

        if (isCadenasToActivate)
            canvasEnterPassword.SetActive(true);
        else
            JoinSelectedRoom();
    }

    public void JoinSelectedRoom()
	{
        ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        for (int y = 0; y < currentServersFind.Length; y++)
        {
            if (serverIdSelected == currentServersFind[y].GetServerId())
            {
                string roomName = currentServersFind[y].GetRoomName();
                PhotonNetwork.JoinRoom(roomName);
                print("joining room name : " + roomName);
            }
        }
    }

    public void OnClickOnRefresh()
	{
        /*listServer.Clear();
        ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        for (int y = 0; y < currentServersFind.Length; y++)
            listServer.Add(currentServersFind[y]);

        print("nb server : " + listServer.Count);*/

        /*ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        for (int y = 0; y < currentServersFind.Length; y++)
		{
            listServer[y].SetServerInfo(currentServersFind[y].GetServerId(), currentServersFind[y].GetRoomName(), currentServersFind[y].)
            currentServersFind

        }*/
    }

    public ServerInfo[] GetServersInfo()
	{
        ServerInfo[] currentServersFind = FindObjectsOfType<ServerInfo>();
        return currentServersFind;
    }

    public void OnClickQuikJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
	{
        SendText("join", "<color=#00ff00> Room joined, loading lobby </color>");
        if (canvasCreateGame.activeSelf)
            SendText("create", "<color=#00ff00> Room joined, loading lobby </color>");
        SetupCustomProperty();
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

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
        bool isSameRoom = true;
        if(listRoom.Count <= 0)
		{
            listRoom = roomList;
        }

        if (listRoomName.Count > 0)
        {
            print("room list count : " + roomList.Count);
            print("room list name count : " + listRoomName.Count);

            if (PhotonNetwork.CountOfRooms > listRoomName.Count)
			{
                isSameRoom = false;
            }

            if(isSameRoom)
                return;
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            for (int y = 0; y < listRoom.Count; y++)
            {
                if (roomList[i] != listRoom[y] || listRoom.Count == 1)
                {
                    GameObject newServer = Instantiate(serverPrefab);
                    serverId++;
                    newServer.transform.parent = GameObject.Find("PanelListServer").transform;
                    newServer.transform.localScale = new Vector3(1, 1, 1);

                    ServerInfo serverInfo = newServer.GetComponent<ServerInfo>();
                    print("room custom password : " + (string)roomList[0].CustomProperties["pw"]);
                    string password = (string)roomList[0].CustomProperties["pw"];
                    bool hasPassword = false;
                    if (password != "")
                        hasPassword = true;

                    serverInfo.SetServerInfo(serverId, roomList[0].Name, roomList[0].PlayerCount, roomList[0].MaxPlayers, hasPassword);
                    listServer.Add(serverInfo);
                    listRoomName.Add(roomList[i].Name);
                }
                else
                {
                    //if(listRoom.Count == 1)
                    //print("roomList == listRoom");
                }
            }
        }
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

    public string GetRoomPassword()
	{
        return impFieldPasswordText.text;
	}

    public bool GetIsRoomCreatedHasPassword()
	{
        return isRoomCreatedHasPassword;
	}
}
