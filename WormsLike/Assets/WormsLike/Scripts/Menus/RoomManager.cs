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
	[SerializeField] private Text textNbPlayerMax;
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
	[Space(5)]
	[SerializeField] private GameObject chatMessageParent;
	[SerializeField] private GameObject chatMessage;
	[SerializeField] private InputField inputFieldChatMessage;
	[SerializeField] private Button buttonStart;
	[SerializeField] private GridLayoutGroup grid;

	int indexGamemode;
	int nbSlimes = 2;
	int mapIndex = 0;
	int lastMapIndex = 3;
	byte playerMax;
	bool isClientButtonSetup = false;
	bool isGridSetup = false;
	float timerStart = 0.1f;
	string playerTeam = "none";
	GameObject currentPlayer;
	List<Photon.Realtime.Player> listPlayer = new List<Photon.Realtime.Player>();
	List<GameObject> listGoPlayer = new List<GameObject>();
	List<GameObject> listChatMessage = new List<GameObject>();
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
		currentPlayer.GetComponent<LobbyPlayer>().SetupPlayerInfo(PhotonNetwork.LocalPlayer.NickName, (int)PhotonNetwork.LocalPlayer.CustomProperties["pp"]);

		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
		hash.Add("t", playerTeam);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

		SetRoomInfo();

		SoundManager.instance.StopMusic("menuOst");
		SoundManager.instance.PlayMusic("lobbyOst");
	}

	void Update()
	{
		/*if (timerStart <= 0)
		{
			currentPlayer.GetComponent<LobbyPlayer>().SetCurrentTeam((int)TeamState.SPECTATE);
			currentPlayer.transform.localScale = new Vector3(1, 1, 1);
		}
		else
			timerStart -= Time.deltaTime;*/

		UpdateNbPlayerText();
		UpdateParameters();

		if (inputFieldChatMessage.IsActive())
		{
			if (Input.GetKeyUp(KeyCode.Return))
			{
				if (inputFieldChatMessage.text == "" || inputFieldChatMessage.text == "\n")
				{
					inputFieldChatMessage.text = "";
					return;
				}
				else
					OnClickSendMessage();

				/*if (inputFieldChatMessage.text.Length < 1)
                {
                    inputFieldChatMessage.text = "";
                    return;
                }
                else
                    OnClickSendMessage();*/
			}
		}
	}

	void SetRoomInfo()
	{
		textRoomName.text = PhotonNetwork.CurrentRoom.Name;
		textPlayerMax.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
			textRoomPassword.text = MenuManager.instance.GetRoomPassword();
		else
		{
			textRoomPassword.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["pw"];
			buttonStart.interactable = false;
		}

		if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 0)
			indexGamemode = 0;
		else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["gm"] == 1)
			indexGamemode = 1;

		nbSlimes = (int)PhotonNetwork.CurrentRoom.CustomProperties["nbs"];
		mapIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["m"];
	}

	void UpdateParameters()
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			UpdateButtons();
			buttonStart.interactable = true;
		}
		else
		{
			if (!isClientButtonSetup)
			{
				SetAllButtonIninteractable();
				isClientButtonSetup = true;
			}
		}

		textPlayerMax.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

		UpdateGamemode();
		UpdateMap();
	}

	void UpdateMap()
	{
		if (mapIndex == 0)
			textMap.text = "map 1";
		if (mapIndex == 1)
			textMap.text = "map 2";
		if (mapIndex == 2)
			textMap.text = "map 3";
		if (mapIndex == lastMapIndex)
			textMap.text = "map 4";

		PhotonNetwork.CurrentRoom.CustomProperties["m"] = mapIndex;
	}

	void UpdateGamemode()
	{
		/*if (indexGamemode == 0)
		{
			textGamemode.text = "Team deathmatch";
			PhotonNetwork.CurrentRoom.CustomProperties["gm"] = 0;
			//print("gamemode = 0 : " + textGamemode.text);
		}
		else if (indexGamemode == 1)
		{
			textGamemode.text = "Forts";
			PhotonNetwork.CurrentRoom.CustomProperties["gm"] = 1;
			//print("gamemode = 1 : " + textGamemode.text);
		}*/

		PhotonNetwork.CurrentRoom.CustomProperties["nbs"] = nbSlimes;
		textGamemode.text = nbSlimes.ToString();
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

		if (/*indexGamemode == 0*/ nbSlimes == 2)
		{
			buttonMinusGamemode.interactable = false;
			buttonPlusGamemode.interactable = true;
		}
		else if (/*indexGamemode == 1*/ nbSlimes == 6)
		{
			buttonMinusGamemode.interactable = true;
			buttonPlusGamemode.interactable = false;
		}
		else
		{
			buttonMinusGamemode.interactable = true;
			buttonPlusGamemode.interactable = true;
		}

		if (mapIndex == lastMapIndex)
			buttonPlusMap.interactable = false;
		else
			buttonPlusMap.interactable = true;

		if (mapIndex == 0)
			buttonMinusMap.interactable = false;
		else
			buttonMinusMap.interactable = true;
	}

	void SetAllButtonIninteractable()
	{
		buttonPlusPlayerMax.interactable = false;
		buttonMinusPlayerMax.interactable = false;
		buttonPlusGamemode.interactable = false;
		buttonMinusGamemode.interactable = false;
		buttonPlusMap.interactable = false;
		buttonMinusMap.interactable = false;
	}

	public void IncrementPlayerMax()
	{
		if (playerMax < 6)
			playerMax += 2;

		PhotonNetwork.CurrentRoom.MaxPlayers = playerMax;
		UpdateServerInfo();
		SoundManager.instance.PlaySound("menuClick");
	}

	public void DecrementPlayerMax()
	{
		if (playerMax > 2)
			playerMax -= 2;

		PhotonNetwork.CurrentRoom.MaxPlayers = playerMax;
		UpdateServerInfo();
		SoundManager.instance.PlaySound("menuClick");
	}

	public void SwitchGamemode()
	{
		if (indexGamemode == 0)
		{
			indexGamemode = 1;
		}
		else if (indexGamemode == 1)
		{
			indexGamemode = 0;
		}

		SoundManager.instance.PlaySound("menuClick");

		UpdateServerInfo();
	}

	public void IncrementNbSlimes()
	{
		nbSlimes++;
		SoundManager.instance.PlaySound("menuClick");
	}

	public void DecrementNbSlimes()
	{
		nbSlimes--;
		SoundManager.instance.PlaySound("menuClick");
	}

	public void NextMap()
	{
		if (mapIndex < lastMapIndex)
			mapIndex++;
		SoundManager.instance.PlaySound("menuClick");
	}

	public void PreviousMap()
	{
		if (mapIndex > 0)
			mapIndex--;
		SoundManager.instance.PlaySound("menuClick");
	}

	public void UpdateServerInfo()
	{
		ServerInfo[] currentServersFind = MenuManager.instance.GetServersInfo();
		for (int y = 0; y < currentServersFind.Length; y++)
		{
			if (currentServersFind[y].GetRoomName() == PhotonNetwork.CurrentRoom.Name)
			{
				print("uptade server info, name : " + currentServersFind[y].GetRoomName());
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

			playerTeam = "blue";
			ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
			hash.Add("t", playerTeam);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
		else if (_colorTeam == "red")
		{
			currentPlayer.GetComponent<LobbyPlayer>().SetCurrentTeam((int)TeamState.RED);

			playerTeam = "red";
			ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
			hash.Add("t", playerTeam);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
		SoundManager.instance.PlaySound("menuClick");
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		print("player entered room");
		listPlayer.Add(newPlayer);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			int gm = indexGamemode;
			int map = mapIndex;
			int nbSlimesMax = nbSlimes;
			stream.SendNext(gm);
			stream.SendNext(map);
			stream.SendNext(nbSlimesMax);
		}
		else
		{
			indexGamemode = (int)stream.ReceiveNext();
			mapIndex = (int)stream.ReceiveNext();
			nbSlimes = (int)stream.ReceiveNext();
		}
	}

	public void OnClickSendMessage()
	{
		photonView.RPC("CreateChatMessage", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, inputFieldChatMessage.text, (int)PhotonNetwork.LocalPlayer.CustomProperties["pp"]);
		inputFieldChatMessage.text = "";
		inputFieldChatMessage.ActivateInputField();
		SoundManager.instance.PlaySound("menuClick");
	}

	[PunRPC]
	void CreateChatMessage(string _senderName, string _message, int _ppIndex)
	{
		GameObject newMessage = Instantiate(chatMessage);
		newMessage.GetComponent<ChatMessage>().SetupMessage(_senderName, _message, _ppIndex);
		newMessage.transform.parent = chatMessageParent.transform;
		newMessage.transform.localScale = new Vector3(1, 1, 1);
		listChatMessage.Add(newMessage);
	}

	void UpdateNbPlayerText()
	{
		textNbPlayerMax.text = "Players in lobby : " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
	}

	public void LaunchGame()
	{
		PhotonNetwork.LoadLevel("TurnSystemTest");
	}

	public void OnClickReturnToMenu()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.LoadLevel("MainMenu");
		SoundManager.instance.PlaySound("menuClickError");
	}
}
