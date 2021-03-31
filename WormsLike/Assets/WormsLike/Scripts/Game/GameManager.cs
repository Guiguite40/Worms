using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject playerPrefab = null;
    
    //bool connected = false;

    public bool phase_placement = false;
    public bool phase_game = false;

    [SerializeField] Text timerText;
    float timer = 0;

    float timerStart = 3f;
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

    int currentBluePlayerIndex = 0;
    int currentRedPlayerIndex = 0;
    int startTeamIndex = 0;
    List<Photon.Realtime.Player> listPlayerBlue = new List<Photon.Realtime.Player>();
    List<Photon.Realtime.Player> listPlayerRed = new List<Photon.Realtime.Player>();
    void Start()
    {
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

        switch (gamePhaseState)
		{
			case GamePhaseState.START:
                currentGameStateText.text = "start";
                if (timerStart <= 0)
                {
                    gamePhaseState = GamePhaseState.POINT_PLACEMENT;
                    timerStart = 3f;
                }
                else
                    if(PhotonNetwork.LocalPlayer.IsMasterClient)
                        timerStart -= Time.deltaTime;

				break;
			case GamePhaseState.POINT_PLACEMENT:
                currentGameStateText.text = "point placement";
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

    public float GetTimer()
    {
        return timer;
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (gamePhaseState == GamePhaseState.START)
                {
                    float masterTimerStart = timerStart;
                    stream.SendNext(masterTimerStart);
                }
            }
		}
        else
		{
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (gamePhaseState == GamePhaseState.START)
                {
                    timerStart = (float)stream.ReceiveNext();
                }
            }
		}
	}
}
