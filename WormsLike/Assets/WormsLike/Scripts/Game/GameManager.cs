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
    List<Player> listPlayers = new List<Player>();
    List<Player> listPlayersBlue = new List<Player>();
    List<Player> listPlayersRed = new List<Player>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
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

                GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                Player player = newPlayer.GetComponent<Player>();
                player.SetupPlayerState("blue", nbSlimePerPlayer);
                listPlayersBlue.Add(player);

                print("player no : " + i + ", added to list player blue");
            }

            if ((string)players[i].CustomProperties["t"] == "red")
            {
                listPlayerRed.Add(players[i]);

                GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                Player player = newPlayer.GetComponent<Player>();
                player.SetupPlayerState("red", nbSlimePerPlayer);
                listPlayersRed.Add(player);

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

    void Update()
    {
        nextStateinputs();

        if (IsLocalPlayerMaster())
        {
            MasterSendToOthers("currentPlayer");
            MasterSendToOthers("currentPlayTeam");
            MasterSendToOthers("gameState");
            MasterSendToOthers("bluePointPlaced");
            MasterSendToOthers("redPointPlaced");
        }
        Debug.LogError("current player : " + currentlocalPlayerPlayingName);

        switch (gamePhaseState)
		{
			case GamePhaseState.START:

                currentGameStateText.text = "start";
                if (timerStart <= 0)
                {
                    if (gamemode == 1)
                        SetGamePhaseState(GamePhaseState.POINT_PLACEMENT, false);
                    else if (gamemode == 0)
                        SetGamePhaseState(GamePhaseState.SLIME_PLACEMENT, false);

                    timerStart = 5f;
                    dataPos.text = timerStart.ToString();
                }
                else
                {
                    if (IsLocalPlayerMaster())
                    {
                        timerStart -= Time.deltaTime;
                        dataPos.text = timerStart.ToString();
                    }
                }
				break;

			case GamePhaseState.POINT_PLACEMENT:

                currentGameStateText.text = "point placement";
                Debug.LogError("blue point placed : " + bluePointPlaced);
                Debug.LogError("red point placed : " + redPointPlaced);

                if (IsLocalPlayerMaster())
                    if(bluePointPlaced && redPointPlaced)
                        SetGamePhaseState(GamePhaseState.SLIME_PLACEMENT, true);

                if (IsLocalPlayerTurn())
                {
                    SetCurrentPlayerPlayingName();
                    if (Input.GetKeyUp(KeyCode.P))
                    {
                        if (IsRedTurn())
                        {
                            redPointPlaced = true;
                            SendValueToMaster("redPointPlaced");
                        }
                        else
                        {
                            bluePointPlaced = true;
                            SendValueToMaster("bluePointPlaced");
                        }
                        SetNextPlayerNTeamTurn();

                        SendValueToMaster("currentPlayer");
                        SendValueToMaster("currentPlayTeam");
                    }
                }
                break;

			case GamePhaseState.SLIME_PLACEMENT:
                currentGameStateText.text = "slime placement";

                if (IsLocalPlayerTurn())
                {
                    SetCurrentPlayerPlayingName();
                    if (Input.GetMouseButtonUp(0))
                    {
                        GetCurrentPlayer().PlaceSlime();
                        SetNextPlayerNTeamTurn();

                        SendValueToMaster("currentPlayer");
                        SendValueToMaster("currentPlayTeam");
                    }
                }
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
	}

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

    void SetPlayerTurn(string _playerTeam, int _playerListIndex)
	{
        if(_playerTeam == "blue")
		{
            listPlayersBlue[_playerListIndex].SetIsTurn(true);
		}
        else if(_playerTeam == "red")
		{
            listPlayersRed[_playerListIndex].SetIsTurn(true);
        }
	}

    void SendValueToMaster(string _valueToSend)
	{
        if (_valueToSend == "currentPlayer")
            photonView.RPC("UpdateCurrentPlayer", RpcTarget.MasterClient, currentlocalPlayerPlayingName);
        if (_valueToSend == "currentPlayTeam")
            photonView.RPC("UpdateCurrentPlayTeam", RpcTarget.MasterClient, currentPlayTeam);

        if (_valueToSend == "gameState")
            photonView.RPC("UpdateCurrentGameState", RpcTarget.MasterClient, gamePhaseState);

        if (_valueToSend == "bluePointPlaced")
            photonView.RPC("UpdateBluePointPlaced", RpcTarget.MasterClient, bluePointPlaced);
        if (_valueToSend == "redPointPlaced")
            photonView.RPC("UpdateRedPointPlaced", RpcTarget.MasterClient, redPointPlaced);
    }

    void SendValue(string _valueToSend, RpcTarget _target)
    {
        if (_valueToSend == "currentPlayer")
            photonView.RPC("UpdateCurrentPlayer", _target, currentlocalPlayerPlayingName);
        if (_valueToSend == "currentPlayTeam")
            photonView.RPC("UpdateCurrentPlayTeam", _target, currentPlayTeam);

        if (_valueToSend == "gameState")
            photonView.RPC("UpdateCurrentGameState", _target, gamePhaseState);

        if (_valueToSend == "bluePointPlaced")
            photonView.RPC("UpdateBluePointPlaced", _target, bluePointPlaced);
        if (_valueToSend == "redPointPlaced")
            photonView.RPC("UpdateRedPointPlaced", _target, redPointPlaced);
    }

    void MasterSendToOthers(string _valueToSend)
	{
        if (_valueToSend == "currentPlayer")
            SendValue("currentPlayer", RpcTarget.OthersBuffered);
        if (_valueToSend == "currentPlayTeam")
            SendValue("currentPlayTeam", RpcTarget.OthersBuffered);

        if (_valueToSend == "gameState")
            SendValue("gameState", RpcTarget.OthersBuffered);

        if (_valueToSend == "bluePointPlaced")
            SendValue("bluePointPlaced", RpcTarget.OthersBuffered);
        if (_valueToSend == "redPointPlaced")
            SendValue("redPointPlaced", RpcTarget.OthersBuffered);
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

    [PunRPC]
    void UpdateCurrentGameState(GamePhaseState _gameState)
    {
        gamePhaseState = _gameState;
    }

    [PunRPC]
    void UpdateBluePointPlaced(bool _isPlaced)
    {
        bluePointPlaced = _isPlaced;
    }

    [PunRPC]
    void UpdateRedPointPlaced(bool _isPlaced)
    {
        redPointPlaced = _isPlaced;
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

    bool IsRedTurn()
    {
        if (currentPlayTeam == "red")
            return true;
        return false;
    }

    bool IsBlueTurn()
    {
        if (currentPlayTeam == "blue")
            return true;
        return false;
    }

    bool IsLocalPlayerTurn()
	{
        if (IsRedTurn())
        {
            if (listPlayerRed[currentRedPlayerIndex].IsLocal)
                return true;
            else
                return false;
        }
        else if (IsBlueTurn())
        {
            if (listPlayerBlue[currentBluePlayerIndex].IsLocal)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    Player GetCurrentPlayer()
	{
        if (IsRedTurn())
            return listPlayersRed[currentRedPlayerIndex];
        else if (IsBlueTurn())
            return listPlayersBlue[currentBluePlayerIndex];
        else
		{
            Debug.LogError("is the turn of none");
            if(strStartTeam == "red")
                return listPlayersRed[0];
            else
                return listPlayersBlue[0];
        }
	}

    bool IsLocalPlayerMaster()
	{
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            return true;
        else
            return false;
    }

    void SetNextPlayerNTeamTurn()
    {
        if (IsRedTurn())
        {
            if (currentRedPlayerIndex < listPlayerRed.Count - 1)
                currentRedPlayerIndex++;
            else
                currentRedPlayerIndex = 0;

            currentPlayTeam = "blue";
        }
        else if (IsBlueTurn())
        {
            if (currentBluePlayerIndex < listPlayerBlue.Count - 1)
                currentBluePlayerIndex++;
            else
                currentBluePlayerIndex = 0;

            currentPlayTeam = "red";
        }
    }

    void SwitchTeamPlay()
	{
        if (IsRedTurn())
            currentPlayTeam = "blue";
        else if (IsBlueTurn())
            currentPlayTeam = "red";
    }

    void SetCurrentPlayerPlayingName()
	{
        currentlocalPlayerPlayingName = PhotonNetwork.LocalPlayer.NickName;
    }

    void SetGamePhaseState(GamePhaseState _state, bool _sendToOther)
	{
        gamePhaseState = _state;
        if (_sendToOther)
            MasterSendToOthers("gameState");
    }
}
