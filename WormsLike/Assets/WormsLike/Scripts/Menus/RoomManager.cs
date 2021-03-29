using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static RoomManager instance;

    [SerializeField] private Text textRoomName;
    [SerializeField] private Text textRoomPassword;
    [SerializeField] private Text textPlayerMax;
    [SerializeField] private Text textGamemode;
    [SerializeField] private Text textMap;
    [Space(10)]
    [SerializeField] private Button buttonPlusPlayerMax;
    [SerializeField] private Button buttonMinusPlayerMax;
    [Space(5)]
    [SerializeField] private Button buttonPlusGamemode;
    [SerializeField] private Button buttonMinusGamemode;
    [Space(5)]
    [SerializeField] private Button buttonPlusMap;
    [SerializeField] private Button buttonMinusMap;
    [Space(10)]
    [SerializeField] private GameObject RoomPlayer;

    int indexGamemode;
    byte playerMax;
    GameObject currentPlayer;
    List<Photon.Realtime.Player> listPlayer = new List<Photon.Realtime.Player>();
    List<GameObject> listGoPlayer = new List<GameObject>();

    public enum TeamState
	{
        SPECTATE = 0,
        BLUE,
        RED
	}

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerMax = PhotonNetwork.CurrentRoom.MaxPlayers;

        currentPlayer = PhotonNetwork.Instantiate("RoomPlayer", Vector3.zero, Quaternion.identity);
        currentPlayer.transform.parent = GameObject.Find("BgSpectate").transform;
        currentPlayer.transform.localScale = new Vector3(1, 1, 1);

        currentPlayer.GetComponent<LobbyPlayer>().SetupPlayerInfo(PhotonNetwork.LocalPlayer.NickName);

        SetRoomInfo();
    }

    //[PunRPC]
    /*void CreateRoomPlayer(string _team)
	{
        GameObject newGoPlayer = Instantiate(RoomPlayer);

        if(_team == "spectate")
            newGoPlayer.transform.parent = GameObject.Find("BgSpectate").transform;
        else if(_team == "blue")
            newGoPlayer.transform.parent = GameObject.Find("PanelPlayersBlue").transform;
        else if (_team == "red")
            newGoPlayer.transform.parent = GameObject.Find("PanelPlayersRed").transform;

        newGoPlayer.transform.localScale = new Vector3(1, 1, 1);
        listGoPlayer.Add(newGoPlayer);
    }

    void CreateRoomPlayer()
    {
        GameObject newGoPlayer = Instantiate(RoomPlayer);
        newGoPlayer.transform.parent = GameObject.Find("BgSpectate").transform;
        newGoPlayer.transform.localScale = new Vector3(1, 1, 1);
        listGoPlayer.Add(newGoPlayer);
    }*/

    void Update()
    {
        UpdateParameters();
        print("room gm index : " + PhotonNetwork.CurrentRoom.CustomProperties["gm"].ToString());
    }

    void SetRoomInfo()
	{
        print("room info set");
        textRoomName.text = PhotonNetwork.CurrentRoom.Name;
        textPlayerMax.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            textRoomPassword.text = MenuManager.instance.GetRoomPassword();
        else
            textRoomPassword.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["pw"];

        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 0)
            indexGamemode = 0;
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 1)
            indexGamemode = 1;
    }

    void UpdateParameters()
	{
        UpdateButtons();

        textPlayerMax.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

        if (indexGamemode == 0)
        {
            textGamemode.text = "Team deathmatch";
            print("gamemode = 0 : " + textGamemode.text);
        }
        else if (indexGamemode == 1)
        {
            textGamemode.text = "Forts";
            print("gamemode = 1 : " + textGamemode.text);
        }

        /*if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 0)
        {
            textGamemode.text = "Team deathmatch";
            print("gamemode = 0 : " + textGamemode.text);
        }
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 1)
        {
            textGamemode.text = "Forts";
            print("gamemode = 1 : " + textGamemode.text);
        }*/
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

        if (indexGamemode == 0)
        {
            buttonMinusGamemode.interactable = false;
            buttonPlusGamemode.interactable = true;
        }
        else if (indexGamemode == 1)
		{
            buttonMinusGamemode.interactable = true;
            buttonPlusGamemode.interactable = false;
        }
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

    public void SwitchGamemode()
	{
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 0)
        {
            PhotonNetwork.CurrentRoom.CustomProperties["gm"] = 1;
            indexGamemode = 0;
        }
        else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 1)
        {
            PhotonNetwork.CurrentRoom.CustomProperties["gm"] = 0;
            indexGamemode = 1;
        }

        UpdateServerInfo();
    }

    public void NextMap()
	{

    }

    public void PreviousMap()
	{

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

                currentServersFind[y].SetServerInfo(currentServersFind[y].GetServerId(), PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, playerMax, hasPassword, 
                    currentServersFind[y].GetPassword(), indexGamemode);

            }
        }
    }

    public void OnClickJoinTeam(string _colorTeam)
	{
        if (_colorTeam == "blue")
        {
            currentPlayer.GetComponent<LobbyPlayer>().SetCurrentTeam((int)TeamState.BLUE);
        }
        else if (_colorTeam == "red")
        {
            currentPlayer.GetComponent<LobbyPlayer>().SetCurrentTeam((int)TeamState.RED);
        }
    }

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
        print("player entered room");
        //CreateRoomPlayer("spectate");
        //CreateRoomPlayer();
        listPlayer.Add(newPlayer);
	}

	public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if(changedProps.ContainsKey("team"))
		{
            /*for(int i = 0; i < listPlayer.Count; i++)
			{
                if((string)listPlayer[i].CustomProperties["team"] == "blue")
				{
                    listGoPlayer[i].transform.parent = GameObject.Find("PanelPlayersBlue").transform;
                }
                else if ((string)listPlayer[i].CustomProperties["team"] == "red")
                {
                    listGoPlayer[i].transform.parent = GameObject.Find("PanelPlayersRed").transform;
                }
            }*/

            print("players team updated");
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        if (stream.IsWriting)
        {
            int gm = indexGamemode;
            stream.SendNext(gm);
        }
        else
        {
            indexGamemode = (int)stream.ReceiveNext();
        }
    }
}
