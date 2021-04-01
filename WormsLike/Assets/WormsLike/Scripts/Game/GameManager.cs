using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance;

    [SerializeField] GameObject playerPrefab = null;
    
    public bool phase_placement = false;
    public bool phase_game = false;

    [SerializeField] Text timerText;
    float timer = 0;


    enum GamePhaseState
	{
        START = 0,
        POINT_PLACEMENT,
        SLIME_PLACEMENT,
        GAME,
        END_GAME
	} GamePhaseState gamePhaseState = GamePhaseState.START;

    enum TurnState
	{
        TEAM,
        MAP
	} TurnState turnState = TurnState.TEAM;

    enum PlayerPhaseState
    {
        START_PHASE,
        ACTION,
        MOVEMENTS_LEFT,
        DAMAGE
    } PlayerPhaseState playerPhaseState = PlayerPhaseState.START_PHASE;

    [SerializeField] Text currentGameStateText;
    [SerializeField] Text currentTurnStateText;
    [SerializeField] Text dataPos;

    int currentBluePlayerIndex = 0;
    int currentRedPlayerIndex = 0;

    int startTeamIndex = -1;
    string strStartTeam = "none";
    string currentPlayTeam = "none";

    int gamemode;
    int nbPlayer;
    int nbSlimePerPlayer;
    int localPlayerSlimePlaced = 0;

    string currentlocalPlayerPlayingName = "none";
    bool bluePointPlaced = false;
    bool redPointPlaced = false;

    float timerStart = 5f;
    List<Photon.Realtime.Player> listPlayerBlue = new List<Photon.Realtime.Player>();
    List<Photon.Realtime.Player> listPlayerRed = new List<Photon.Realtime.Player>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        SetupStartValue();

        dataPos.text = "master";
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if ((string)players[i].CustomProperties["t"] == "blue")
            {
                listPlayerBlue.Add(players[i]);
                print("player no : " + i + ", added to list player blue");
            }

            if ((string)players[i].CustomProperties["t"] == "red")
            {
                listPlayerRed.Add(players[i]);
                print("player no : " + i + ", added to list player red");
            }
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (currentPlayTeam == "blue")
                currentlocalPlayerPlayingName = listPlayerBlue[currentBluePlayerIndex].NickName;
            else if (currentPlayTeam == "red")
                currentlocalPlayerPlayingName = listPlayerRed[currentRedPlayerIndex].NickName;
            SendValue("currentPlayer", RpcTarget.OthersBuffered);
        }
    }

    void SetupStartValue()
	{
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startTeamIndex = Random.Range(0, 2);
            if (startTeamIndex == 0)
                strStartTeam = "blue";
            else if (startTeamIndex == 1)
                strStartTeam = "red";

            currentPlayTeam = strStartTeam;
            SendValue("currentPlayTeam", RpcTarget.OthersBuffered);
        }

        Debug.LogError("current play team start : " + currentPlayTeam);

        gamemode = (int)PhotonNetwork.CurrentRoom.CustomProperties["gm"];
        nbPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        nbSlimePerPlayer = 12 / nbPlayer;

        Debug.LogError("gamemode : " + gamemode);
        Debug.LogError("nb player : " + nbPlayer);
        Debug.LogError("nb slimes per player : " + nbSlimePerPlayer);
    }

    void nextStateinputs()
	{
        if (Input.GetKeyDown(KeyCode.Keypad7))
            gamePhaseState--;
        if (Input.GetKeyDown(KeyCode.Keypad9))
            gamePhaseState++;
        if (Input.GetKeyDown(KeyCode.Keypad4))
            turnState--;
        if (Input.GetKeyDown(KeyCode.Keypad6))
            turnState++;
        if (Input.GetKeyDown(KeyCode.Keypad1))
            playerPhaseState--;
        if (Input.GetKeyDown(KeyCode.Keypad3))
            playerPhaseState++;
    }

    // Update is called once per frame
    void Update()
    {
        nextStateinputs();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            MasterSendToOthers();
        Debug.LogError("current player : " + currentlocalPlayerPlayingName);

        switch (gamePhaseState)
		{
			case GamePhaseState.START:

                currentGameStateText.text = "start";
                if (timerStart <= 0)
                {
                    if(gamemode == 1)
                        gamePhaseState = GamePhaseState.POINT_PLACEMENT;
                    else if (gamemode == 0)
                        gamePhaseState = GamePhaseState.SLIME_PLACEMENT;

                    timerStart = 5f;
                    dataPos.text = timerStart.ToString();
                }
                else
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        timerStart -= Time.deltaTime;
                        dataPos.text = timerStart.ToString();
                    }
                }
				break;

			case GamePhaseState.POINT_PLACEMENT:

                currentGameStateText.text = "point placement";
                if (currentPlayTeam == "blue")
                {
                    Debug.LogError("current play team : blue");
                    if (listPlayerBlue[currentBluePlayerIndex].IsLocal)
                    {
                        currentlocalPlayerPlayingName = PhotonNetwork.LocalPlayer.NickName;
                        if (Input.GetKeyUp(KeyCode.P))
                        {
                            currentBluePlayerIndex++;
                            bluePointPlaced = true;
                            currentPlayTeam = "red";
                            SendValueToMaster("currentPlayer");
                            SendValueToMaster("currentPlayTeam");
                            Debug.LogError("blue point placed");

                        }
                    }
                }

                if (currentPlayTeam == "red")
                {
                    Debug.LogError("current play team : red");
                    if (listPlayerRed[currentRedPlayerIndex].IsLocal)
                    {
                        currentlocalPlayerPlayingName = PhotonNetwork.LocalPlayer.NickName;
                        if (Input.GetKeyUp(KeyCode.P))
                        {
                            currentRedPlayerIndex++;
                            redPointPlaced = true;
                            currentPlayTeam = "blue";
                            SendValueToMaster("currentPlayer");
                            SendValueToMaster("currentPlayTeam");
                            Debug.LogError("red point placed");
                        }
                    }
                }
                break;

			case GamePhaseState.SLIME_PLACEMENT:
                currentGameStateText.text = "slime placement";

                break;

			case GamePhaseState.GAME:
                currentGameStateText.text = "game";
                switch (turnState)
				{
					case TurnState.TEAM:

						switch (playerPhaseState)
						{
							case PlayerPhaseState.START_PHASE:
                                currentTurnStateText.text = "start";
                                break;
							case PlayerPhaseState.ACTION:
                                currentTurnStateText.text = "actions";
                                break;
							case PlayerPhaseState.MOVEMENTS_LEFT:
                                currentTurnStateText.text = "movements left";
                                break;
							case PlayerPhaseState.DAMAGE:
                                currentTurnStateText.text = "damage";
                                break;
							default:
								break;
						}

						break;
					case TurnState.MAP:
                        currentTurnStateText.text = "map";
                        break;
					default:
						break;
				}

				break;
			case GamePhaseState.END_GAME:
                currentGameStateText.text = "end game";
                break;
			default:
				break;
		}


		/*timer -= Time.deltaTime;
        if (timer < 0)
            timer = 60;
        timerText.text = timer.ToString("0");*/
	}

    /*public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        Debug.Log("Lobby joining..");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");
        PhotonNetwork.CreateRoom("roomTest");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
        StartCoroutine(InstantiatePlayer());
    }*/

    IEnumerator InstantiatePlayer()
    { 
        Debug.Log("Player instantiation1");
        //Debug.Log("Waiting other players");
        yield return new WaitForSeconds(1);
        Debug.Log("Player instantiation2");

        GameObject newPlayer = playerPrefab;
        PhotonNetwork.Instantiate(newPlayer.name, newPlayer.transform.position, newPlayer.transform.rotation);
        newPlayer.GetComponent<Player>().isTurn = true;
        newPlayer.GetComponent<Player>().phase_game = true;
        newPlayer.GetComponent<Player>().team = 1;

        yield return null;
    }

    void SendValueToMaster(string _valueToSend)
	{
        if (_valueToSend == "currentPlayer")
            photonView.RPC("UpdateCurrentPlayer", RpcTarget.MasterClient, currentlocalPlayerPlayingName);
        if (_valueToSend == "currentPlayTeam")
            photonView.RPC("UpdateCurrentPlayTeam", RpcTarget.MasterClient, currentPlayTeam);
    }

    void SendValue(string _valueToSend, RpcTarget _target)
    {
        if (_valueToSend == "currentPlayer")
            photonView.RPC("UpdateCurrentPlayer", _target, currentlocalPlayerPlayingName);
        if (_valueToSend == "currentPlayTeam")
            photonView.RPC("UpdateCurrentPlayTeam", _target, currentPlayTeam);
    }

    void MasterSendToOthers()
	{
        SendValue("currentPlayer", RpcTarget.OthersBuffered);
        SendValue("currentPlayTeam", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    void UpdateCurrentPlayer(string _name)
	{
        currentlocalPlayerPlayingName = _name;
	}

    [PunRPC]
    void UpdateCurrentPlayTeam(string _currentTeam)
    {
        currentPlayTeam = _currentTeam;
    }

    public float GetTimer()
    {
        return timer;
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        if (stream.IsWriting)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                stream.SendNext(currentlocalPlayerPlayingName);
                stream.SendNext(currentPlayTeam);
                stream.SendNext(gamePhaseState);

                if (gamePhaseState == GamePhaseState.START)
                {
                    stream.SendNext(timerStart);
                    stream.SendNext(startTeamIndex);
                    stream.SendNext(strStartTeam);
                }
            }
        }
        else
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                currentlocalPlayerPlayingName = (string)stream.ReceiveNext();
                currentPlayTeam = (string)stream.ReceiveNext();
                gamePhaseState = (GamePhaseState)stream.ReceiveNext();

                if (gamePhaseState == GamePhaseState.START)
                {
                    timerStart = (float)stream.ReceiveNext();
                    startTeamIndex = (int)stream.ReceiveNext();
                    strStartTeam = (string)stream.ReceiveNext();

                    dataPos.text = timerStart.ToString();
                }
            }
        }
	}
}
