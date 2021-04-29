using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace DTerrain
{
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
        }
        GamePhaseState gamePhaseState = GamePhaseState.START;

        enum TurnState
        {
            TEAM,
            MAP
        }
        TurnState turnState = TurnState.TEAM;

        enum PlayerPhaseState
        {
            START_PHASE,
            ACTION,
            MOVEMENTS_LEFT,
            DAMAGE,
            MAP
        }
        PlayerPhaseState playerPhaseState = PlayerPhaseState.START_PHASE;

        [SerializeField] Text currentGameStateText;
        [SerializeField] Text currentTurnStateText;
        [SerializeField] Text dataPos;
        [SerializeField] Text itsHisTurnText;
        [SerializeField] GameObject healthBoxPrefab;
        [SerializeField] GameObject damageBoxPrefab;
        [SerializeField] Text uiTimer;
        [SerializeField] Text uiTurn;
        //[SerializeField] Text textGameTime;
        [SerializeField] MapDestroy mapDestroy;
        [SerializeField] GameObject victoryScreen;
        [SerializeField] Text victoryScreenText;

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
        bool isCanvasClosed = false;

        ///////////////
        float timerStart = 5f;
        List<Photon.Realtime.Player> listPlayerBlue = new List<Photon.Realtime.Player>();
        SortedList<int, Photon.Realtime.Player> sortedlistPlayerBlue = new SortedList<int, Photon.Realtime.Player>();
        List<Photon.Realtime.Player> listPlayerRed = new List<Photon.Realtime.Player>();
        SortedList<int, Photon.Realtime.Player> sortedlistPlayerRed = new SortedList<int, Photon.Realtime.Player>();
        List<Player> listPlayers = new List<Player>();
        List<Player> listPlayersBlue = new List<Player>();
        List<Player> listPlayersRed = new List<Player>();

        Dictionary<string, bool> allSlimesPlaced = new Dictionary<string, bool>();
        //////////////

        float timerPlayerStart = 3f;
        float timerMovementsLeft = 3f;
        float timerSendInfo = 0.1f;
        float timerPlayerTurn = 60f;
        float timerAllGame = 0;
        float timerMap = 3f;
        bool isPlayerTurnSetup = false;
        bool crateSpawned = false;
        bool mortSubiteSet = false;
        bool isPassTurn = false;
        int slimeIndex = 0;
        int slimeIndexMax;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            BasicPaintableLayer.id = (int)PhotonNetwork.CurrentRoom.CustomProperties["m"];
        }

        void SetupStartValue()
        {
            //BasicPaintableLayer.id = 1;
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

            gamemode = (int)PhotonNetwork.CurrentRoom.CustomProperties["gm"];
            nbPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
            //nbSlimePerPlayer = 6 / nbPlayer;
            nbSlimePerPlayer = (int)PhotonNetwork.CurrentRoom.CustomProperties["nbs"];
            slimeIndexMax = nbSlimePerPlayer; // - 1
        }

        void Start()
        {
            victoryScreen.SetActive(false);

            SetupStartValue();
            uiTurn.text = "";
            uiTimer.text = "";
            //dataPos.text = "master";
            Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                if ((string)players[i].CustomProperties["t"] == "blue")
                {
                    allSlimesPlaced.Add(players[i].NickName, false);
                    listPlayerBlue.Add(players[i]);
                    //sortedlistPlayerBlue.Add(i, players[i]);
                    GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                    Player player = newPlayer.GetComponent<Player>();
                    player.SetupPlayerState("blue", nbSlimePerPlayer);
                    listPlayersBlue.Add(player);
                }

                if ((string)players[i].CustomProperties["t"] == "red")
                {
                    allSlimesPlaced.Add(players[i].NickName, false);
                    listPlayerRed.Add(players[i]);

                    GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                    Player player = newPlayer.GetComponent<Player>();
                    player.SetupPlayerState("red", nbSlimePerPlayer);
                    listPlayersRed.Add(player);
                }
            }

            listPlayerBlue.Sort(SortByNickname);
            listPlayerRed.Sort(SortByNickname);

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
            TeamVictory();

            if (!isCanvasClosed)
            {
                victoryScreen.SetActive(false);
                isCanvasClosed = true;
            }

            if (IsLocalPlayerMaster())
            {
                if (timerSendInfo <= 0)
                {
                    MasterSendToOthers("currentPlayer");
                    MasterSendToOthers("currentPlayTeam");
                    //MasterSendToOthers("gameState");

                    if (gamePhaseState == GamePhaseState.POINT_PLACEMENT)
                    {
                        MasterSendToOthers("bluePointPlaced");
                        MasterSendToOthers("redPointPlaced");
                    }

                    MasterSendToOthers("bluePlayerIndex");
                    MasterSendToOthers("redPlayerIndex");
                    timerSendInfo = 0.1f;
                }
                else
                    timerSendInfo -= Time.deltaTime;
            }

            /*Debug.LogError("blue list 0 : " + listPlayerBlue[0].NickName);
            Debug.LogError("blue list 1 : " + listPlayerBlue[1].NickName);
            Debug.LogError("red list 0 : " + listPlayerRed[0].NickName);
            Debug.LogError("red list 1 : " + listPlayerRed[1].NickName);*/

            switch (gamePhaseState)
            {
                case GamePhaseState.START:

                    //currentGameStateText.text = "start";
                    if (timerStart <= 0)
                    {
                        if (gamemode == 1)
                            SetGamePhaseState(GamePhaseState.POINT_PLACEMENT, false);
                        else if (gamemode == 0)
                            SetGamePhaseState(GamePhaseState.SLIME_PLACEMENT, false);

                        timerStart = 5f;
                        //dataPos.text = timerStart.ToString();
                    }
                    else
                    {
                        if (IsLocalPlayerMaster())
                        {
                            timerStart -= Time.deltaTime;
                            //dataPos.text = timerStart.ToString();
                        }
                    }
                    break;

                case GamePhaseState.POINT_PLACEMENT:

                    //currentGameStateText.text = "point placement";

                    if (IsLocalPlayerMaster())
                        if (bluePointPlaced && redPointPlaced)
                            SetGamePhaseState(GamePhaseState.SLIME_PLACEMENT, true);

                    if (IsLocalPlayerTurn())
                    {
                        //itsHisTurnText.text = "true";
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
                    //currentGameStateText.text = "slime placement";

                    for (int i = 0; i < allSlimesPlaced.Count; i++)
                    {
                        Debug.LogError("all slime placed of " + PhotonNetwork.PlayerList[i].NickName + ", state : " + allSlimesPlaced[PhotonNetwork.PlayerList[i].NickName]);
                    }

                    if (IsLocalPlayerTurn())
                    {
                        uiTurn.text = "place your slime :";
                        //itsHisTurnText.text = "true";
                        SetCurrentPlayerPlayingName();
                        if (Input.GetMouseButtonUp(0))
                        {
                            GetCurrentPlayer().PlaceSlime();
                            SetNextPlayerNTeamTurn();

                            SendValueToMaster("currentPlayer");
                            SendValueToMaster("currentPlayTeam");
                        }

                        if (GetCurrentPlayer().GetAllSlimePlaced())
                        {
                            allSlimesPlaced[PhotonNetwork.LocalPlayer.NickName] = true;
                            SendValueToMaster("allSlimePlaced", PhotonNetwork.LocalPlayer.NickName);
                        }
                    }
                    else
                        uiTurn.text = "";
                    //itsHisTurnText.text = "false";

                    if (IsLocalPlayerMaster())
                    {
                        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                        {
                            if (allSlimesPlaced[PhotonNetwork.PlayerList[i].NickName] == false)
                                return;
                        }
                        SetGamePhaseState(GamePhaseState.GAME, true);
                    }
                    break;

                case GamePhaseState.GAME:
                    //currentGameStateText.text = "game";

                    if (IsLocalPlayerMaster())
                        timerAllGame += Time.deltaTime;
                    //textGameTime.text = timerAllGame.ToString();

                    switch (turnState)
                    {
                        case TurnState.TEAM:

                            switch (playerPhaseState)
                            {
                                case PlayerPhaseState.START_PHASE:

                                    if (IsLocalPlayerTurn())
                                    {
                                        uiTurn.text = "It's your turn";
                                        SetCurrentPlayerPlayingName();
                                    }
                                    else
                                    {
                                        uiTurn.text = "";
                                    }

                                    if (!isPlayerTurnSetup)
                                    {
                                        if (slimeIndex == slimeIndexMax)
                                            slimeIndex = 0;

                                        ResetTimers(true);
                                        isPlayerTurnSetup = true;
                                    }
                                    else
                                    {
                                        if (timerPlayerStart <= 0)
                                        {
                                            timerPlayerStart = 3f;
                                            ResetTimers(true);
                                            SendValueToMaster("timerPlayerStart");
                                            SetPlayerPhaseState(PlayerPhaseState.ACTION, false, true); //true
                                        }
                                        else
                                        {
                                            if (IsLocalPlayerTurn())
                                            {
                                                timerPlayerStart -= Time.deltaTime;

                                                GetCurrentPlayer().SetCharacterControlled(slimeIndex);
                                                CameraManager.instance.MoveCamOnTarget(GetCurrentPlayer().GetCurrentCharacter().GetPos());

                                                SendValueToMaster("camera");
                                            }
                                        }
                                    }

                                    break;

                                case PlayerPhaseState.ACTION:
                                    //currentTurnStateText.text = "actions";

                                    if (timerPlayerTurn <= 0 || IsPassTurn())
                                    {
                                        timerPlayerTurn = 60f;
                                        ResetTimers(true);
                                        SendValueToMaster("timerPlayerTurn");
                                        uiTimer.text = ((int)timerPlayerTurn).ToString();
                                        SendValueToMaster("uiTimer");
                                        SetPlayerPhaseState(PlayerPhaseState.MAP, false, true); //DAMAGE //true
                                    }
                                    else
                                    {
                                        if (IsLocalPlayerTurn())
                                        {
                                            if (!isPlayerTurnSetup)
                                            {
                                                GetCurrentPlayer().SetCharacterControlled(slimeIndex);
                                                slimeIndex++;
                                                isPlayerTurnSetup = true;
                                            }

                                            timerPlayerTurn -= Time.deltaTime;
                                            uiTimer.text = ((int)timerPlayerTurn).ToString();
                                            SendValueToMaster("uiTimer");
                                            if (!GetCurrentPlayer().GetHasTurnDone())
                                            {
                                                GetCurrentPlayer().ControlCharacter();
                                                CameraManager.instance.SetCamOnTarget(GetCurrentPlayer().GetCurrentCharacter().GetPos());
                                            }
                                            else
                                            {
                                                GetCurrentPlayer().turnDone = false;
                                                SetPlayerPhaseState(PlayerPhaseState.MOVEMENTS_LEFT, false, true); //true
                                            }
                                        }
                                    }

                                    break;

                                case PlayerPhaseState.MOVEMENTS_LEFT:
                                    //currentTurnStateText.text = "movements left";

                                    if (timerMovementsLeft <= 0)
                                    {
                                        GetCurrentPlayer().UnSetCharacterControlled();
                                        GetCurrentPlayer().SetHasAttacked(false);
                                        timerMovementsLeft = 3f;
                                        ResetTimers(true);
                                        CameraManager.instance.ResetCam();
                                        SetPlayerPhaseState(PlayerPhaseState.MAP, false, true); //true
                                    }
                                    else
                                    {
                                        if (IsLocalPlayerTurn())
                                        {
                                            timerMovementsLeft -= Time.deltaTime;
                                            uiTimer.text = ((int)timerMovementsLeft).ToString();
                                            SendValueToMaster("uiTimer");
                                            GetCurrentPlayer().ControlCharacter();
                                            CameraManager.instance.SetCamOnTarget(GetCurrentPlayer().GetCurrentCharacter().GetPos());
                                            SendValueToMaster("camera");
                                            if (!IsLocalPlayerMaster())
                                                SendValueToMaster("timerMovementsLeft");
                                        }

                                        /*if (PhotonNetwork.IsMasterClient)
                                            if (int.Parse(uiTimer.text) <= 0.3)
                                                playerPhaseState = PlayerPhaseState.MAP;*/
                                    }
                                    break;

                                case PlayerPhaseState.DAMAGE:
                                    //currentTurnStateText.text = "damage";

                                    if (IsLocalPlayerMaster())
                                        SetPlayerPhaseState(PlayerPhaseState.MAP, true, false); //only true
                                    break;

                                case PlayerPhaseState.MAP:
                                    //currentTurnStateText.text = "map";

                                    if (timerMap <= 0)
                                    {
                                        SetNextPlayerNTeamTurn();
                                        SendValueToMaster("currentPlayer");
                                        SendValueToMaster("currentPlayTeam");
                                        ResetTimers(true);
                                        SetPlayerPhaseState(PlayerPhaseState.START_PHASE, false, true);
                                    }
                                    else
                                    {
                                        CameraManager.instance.ResetCam();

                                        if(IsLocalPlayerMaster())
										{
                                            if(!mortSubiteSet)
											{
                                                if (timerAllGame >= 40)
                                                {
                                                    timerMap = 7f;
                                                    mapDestroy.SetMortSubite();
                                                }
                                                else
                                                    timerMap = 3f;
                                                mortSubiteSet = true;
                                            }
										}

                                        if (IsLocalPlayerTurn())
                                        {
                                            if (!crateSpawned)
                                            {
                                                /*if (timerAllGame >= 40)
                                                {
                                                    timerMap = 7f;
                                                    mapDestroy.SetMortSubite();
                                                }
                                                else
                                                    timerMap = 3f;*/
                                                SpawnCrate();
                                                crateSpawned = true;
                                            }
                                            timerMap -= Time.deltaTime;
                                        }
                                        ResetTimers(false);
                                        GetCurrentPlayer().UnSetCharacterControlled();
                                    }

                                    break;


                                default:
                                    break;
                            }

                            break;
                        case TurnState.MAP:
                            //currentTurnStateText.text = "map";
                            break;

                        default:
                            break;
                    }
                    break;

                case GamePhaseState.END_GAME:
                    //currentGameStateText.text = "end game";

                    break;

                default:
                    break;
            }
        }

        void ResetTimers(bool _resetTimerMap)
        {
            timerPlayerStart = 3f;
            timerPlayerTurn = 60f;
            isPlayerTurnSetup = false;
            timerMovementsLeft = 3f;
            if (_resetTimerMap)
            {
                if (timerAllGame >= 40)
                    timerMap = 7f;
                else
                    timerMap = 3f;
                crateSpawned = false;
                mortSubiteSet = false;
            }

            //dataPos.text = "all timers reset";
            uiTimer.text = "";
            uiTurn.text = "";
            SendValueToMaster("uiTimer");
        }

        void ResetTimer()
        {
            timerPlayerStart = 3f;
            timerPlayerTurn = 60f;
            //isPlayerTurnSetup = false;
            timerMovementsLeft = 3f;
            if (timerAllGame >= 40)
                timerMap = 5f;
            else
                timerMap = 3f;
            crateSpawned = false;

            uiTimer.text = "";
            uiTurn.text = "";
            SendValueToMaster("uiTimer");
        }

        void SpawnCrate()
        {
            int randCrate = Random.Range(0, 2);
            float posX = Random.Range(5, 61);
            if (randCrate == 0)
            {
                GameObject healthBox = PhotonNetwork.Instantiate(healthBoxPrefab.name, new Vector3(posX, 25f, 0), Quaternion.identity);
            }
            else if (randCrate == 1)
            {
                GameObject damageBox = PhotonNetwork.Instantiate(damageBoxPrefab.name, new Vector3(posX, 25f, 0), Quaternion.identity);
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

            if (_valueToSend == "bluePlayerIndex")
                photonView.RPC("UpdateBluePlayerIndex", RpcTarget.MasterClient, currentBluePlayerIndex);
            if (_valueToSend == "redPlayerIndex")
                photonView.RPC("UpdateRedPlayerIndex", RpcTarget.MasterClient, currentRedPlayerIndex);

            if (_valueToSend == "allSlimePlaced")
                photonView.RPC("UpdateAllSlimePlaced", RpcTarget.MasterClient, GetCurrentPlayer().GetAllSlimePlaced(), PhotonNetwork.LocalPlayer.NickName);

            if (_valueToSend == "timerPlayerStart")
                photonView.RPC("UpdateTimerPlayerStart", RpcTarget.MasterClient, timerPlayerStart);
            if (_valueToSend == "timerMovementsLeft")
                photonView.RPC("UpdateTimerMovementsLeft", RpcTarget.MasterClient, timerMovementsLeft);
            if (_valueToSend == "timerPlayerTurn")
                photonView.RPC("UpdateTimerPlayerTurn", RpcTarget.MasterClient, timerPlayerTurn);
            if (_valueToSend == "timerMap")
                photonView.RPC("UpdateTimerMap", RpcTarget.MasterClient, timerMap);

            if (_valueToSend == "camera")
                photonView.RPC("UpdateCamera", RpcTarget.MasterClient, Camera.main.transform.position, Camera.main.orthographicSize);

            if (_valueToSend == "uiTimer")
                photonView.RPC("UpdateUiTimer", RpcTarget.MasterClient, uiTimer.text);
            if (_valueToSend == "uiTurn")
                photonView.RPC("UpdateUiTurn", RpcTarget.MasterClient, uiTurn.text);
        }

        void SendValueToMaster(string _valueToSend, string _sender)
        {
            if (_valueToSend == "allSlimePlaced")
                photonView.RPC("UpdateAllSlimePlaced", RpcTarget.MasterClient, true, _sender);
        }

        void SendValue(string _valueToSend, RpcTarget _target)
        {
            if (_valueToSend == "currentPlayer")
                photonView.RPC("UpdateCurrentPlayer", _target, currentlocalPlayerPlayingName);
            if (_valueToSend == "currentPlayTeam")
                photonView.RPC("UpdateCurrentPlayTeam", _target, currentPlayTeam);

            if (_valueToSend == "gameState")
                photonView.RPC("UpdateCurrentGameState", _target, gamePhaseState);
            if (_valueToSend == "playerState")
                photonView.RPC("UpdateCurrentPlayerState", _target, playerPhaseState);

            if (_valueToSend == "bluePointPlaced")
                photonView.RPC("UpdateBluePointPlaced", _target, bluePointPlaced);
            if (_valueToSend == "redPointPlaced")
                photonView.RPC("UpdateRedPointPlaced", _target, redPointPlaced);

            if (_valueToSend == "bluePlayerIndex")
                photonView.RPC("UpdateBluePlayerIndex", _target, currentBluePlayerIndex);
            if (_valueToSend == "redPlayerIndex")
                photonView.RPC("UpdateRedPlayerIndex", _target, currentRedPlayerIndex);

            if (_valueToSend == "uiTimer")
                photonView.RPC("UpdateUiTimer", _target, uiTimer.text);
            if (_valueToSend == "uiTurn")
                photonView.RPC("UpdateUiTurn", _target, uiTurn.text);
        }

        void MasterSendToOthers(string _valueToSend)
        {
            if (_valueToSend == "currentPlayer")
                SendValue("currentPlayer", RpcTarget.OthersBuffered);
            if (_valueToSend == "currentPlayTeam")
                SendValue("currentPlayTeam", RpcTarget.OthersBuffered);

            if (_valueToSend == "gameState")
                SendValue("gameState", RpcTarget.OthersBuffered);
            if (_valueToSend == "playerState")
                SendValue("playerState", RpcTarget.OthersBuffered);

            if (_valueToSend == "bluePointPlaced")
                SendValue("bluePointPlaced", RpcTarget.OthersBuffered);
            if (_valueToSend == "redPointPlaced")
                SendValue("redPointPlaced", RpcTarget.OthersBuffered);

            if (_valueToSend == "bluePlayerIndex")
                SendValue("bluePlayerIndex", RpcTarget.OthersBuffered);
            if (_valueToSend == "redPlayerIndex")
                SendValue("redPlayerIndex", RpcTarget.OthersBuffered);

            if (_valueToSend == "uiTimer")
                SendValue("uiTimer", RpcTarget.OthersBuffered);
            if (_valueToSend == "uiTurn")
                SendValue("uiTurn", RpcTarget.OthersBuffered);
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
        void UpdateCurrentPlayerState(PlayerPhaseState _playerState)
        {
            playerPhaseState = _playerState;
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

        [PunRPC]
        void UpdateBluePlayerIndex(int _index)
        {
            currentBluePlayerIndex = _index;
        }

        [PunRPC]
        void UpdateRedPlayerIndex(int _index)
        {
            currentRedPlayerIndex = _index;
        }

        [PunRPC]
        void UpdateAllSlimePlaced(bool _state, string _nickname)
        {
            allSlimesPlaced[_nickname] = _state;
        }

        [PunRPC]
        void UpdateTimerPlayerStart(float _value)
        {
            timerPlayerStart = _value;
        }

        [PunRPC]
        void UpdateTimerMovementsLeft(float _value)
        {
            timerMovementsLeft = _value;
        }

        [PunRPC]
        void UpdateTimerPlayerTurn(float _value)
        {
            timerPlayerTurn = _value;
        }

        [PunRPC]
        void UpdateTimerMap(float _value)
        {
            timerMap = _value;
        }

        [PunRPC]
        void UpdateCamera(Vector3 _pos, float _size)
        {
            Camera.main.transform.position = _pos;
            Camera.main.orthographicSize = _size;
        }

        [PunRPC]
        void UpdateUiTimer(string _value)
        {
            uiTimer.text = _value;
        }

        [PunRPC]
        void UpdateUiTurn(string _value)
        {
            uiTurn.text = _value;
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
                    stream.SendNext(playerPhaseState);
                    stream.SendNext(currentBluePlayerIndex);
                    stream.SendNext(currentRedPlayerIndex);
                    stream.SendNext(allSlimesPlaced);
                    //stream.SendNext(timerAllGame);

                    if (gamePhaseState == GamePhaseState.START)
                    {
                        stream.SendNext(timerStart);
                        stream.SendNext(startTeamIndex);
                        stream.SendNext(strStartTeam);
                    }

                    if (playerPhaseState == PlayerPhaseState.START_PHASE)
                    {
                        stream.SendNext(timerPlayerStart);
                    }
                    if (playerPhaseState == PlayerPhaseState.ACTION)
                    {
                        stream.SendNext(timerPlayerTurn);
                    }
                    if (playerPhaseState == PlayerPhaseState.MOVEMENTS_LEFT)
                    {
                        stream.SendNext(timerMovementsLeft);
                    }
                    if (playerPhaseState == PlayerPhaseState.MAP)
                    {
                        stream.SendNext(timerMap);
                    }

                    if (playerPhaseState == PlayerPhaseState.ACTION || playerPhaseState == PlayerPhaseState.MOVEMENTS_LEFT)
                    {
                        stream.SendNext(uiTimer.text);
                        stream.SendNext(uiTurn.text);
                        stream.SendNext(Camera.main.transform.position);
                        //stream.SendNext(Camera.main.orthographicSize);
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
                    playerPhaseState = (PlayerPhaseState)stream.ReceiveNext();
                    currentBluePlayerIndex = (int)stream.ReceiveNext();
                    currentRedPlayerIndex = (int)stream.ReceiveNext();
                    allSlimesPlaced = (Dictionary<string, bool>)stream.ReceiveNext();
                    //timerAllGame = (float)stream.ReceiveNext();

                    if (gamePhaseState == GamePhaseState.START)
                    {
                        timerStart = (float)stream.ReceiveNext();
                        startTeamIndex = (int)stream.ReceiveNext();
                        strStartTeam = (string)stream.ReceiveNext();

                        //dataPos.text = timerStart.ToString();
                    }
                    if (playerPhaseState == PlayerPhaseState.START_PHASE)
                    {
                        timerPlayerStart = (float)stream.ReceiveNext();
                    }
                    if (playerPhaseState == PlayerPhaseState.ACTION)
                    {
                        timerPlayerTurn = (float)stream.ReceiveNext();
                    }
                    if (playerPhaseState == PlayerPhaseState.MOVEMENTS_LEFT)
                    {
                        timerMovementsLeft = (float)stream.ReceiveNext();
                    }
                    if (playerPhaseState == PlayerPhaseState.MAP)
                    {
                        timerMap = (float)stream.ReceiveNext();
                    }

                    if (playerPhaseState == PlayerPhaseState.ACTION || playerPhaseState == PlayerPhaseState.MOVEMENTS_LEFT)
                    {
                        uiTimer.text = (string)stream.ReceiveNext();
                        uiTurn.text = (string)stream.ReceiveNext();
                        Camera.main.transform.position = (Vector3)stream.ReceiveNext();
                        //Camera.main.orthographicSize = (float)stream.ReceiveNext();
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

        public void SetIsPassTurn()
        {
            isPassTurn = true;
        }

        bool IsPassTurn()
        {
            if (Input.GetKeyUp(KeyCode.N) || isPassTurn)
            {
                isPassTurn = false;
                return true;
            }

            return false;
        }

        Player GetCurrentPlayer()
        {
            if (IsRedTurn())
            {
                if (listPlayersRed[currentRedPlayerIndex] != null)
                    return listPlayersRed[currentRedPlayerIndex];
                else
                {
                    isPassTurn = true;
                    Debug.LogError("red player dead, pass turn");
                    return null;
                }
            }
            else if (IsBlueTurn())
            {
                if (listPlayersBlue[currentBluePlayerIndex] != null)
                    return listPlayersBlue[currentBluePlayerIndex];
                else
                {
                    isPassTurn = true;
                    Debug.LogError("blue player dead, pass turn");
                    return null;
                }
            }
            else
            {
                if (strStartTeam == "red")
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

                SendValueToMaster("redPlayerIndex");

                currentPlayTeam = "blue";
            }
            else if (IsBlueTurn())
            {

                if (currentBluePlayerIndex < listPlayerBlue.Count - 1)
                    currentBluePlayerIndex++;
                else
                    currentBluePlayerIndex = 0;

                SendValueToMaster("bluePlayerIndex");

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

        string GetCurrentPlayerName()
        {
            return currentlocalPlayerPlayingName;
        }

        void SetGamePhaseState(GamePhaseState _state, bool _sendToOther)
        {
            gamePhaseState = _state;
            if (_sendToOther)
                MasterSendToOthers("gameState");
        }

        void SetPlayerPhaseState(PlayerPhaseState _state, bool _sendToOther, bool _sendToMaster)
        {
            playerPhaseState = _state;
            if (_sendToOther)
                MasterSendToOthers("playerState");
            if (_sendToMaster)
                SendValueToMaster("playerState");
        }

        public void OnClickReturnToMenu()
        {
            PhotonNetwork.NetworkStatisticsReset();
            PhotonNetwork.LoadLevel("Lobby");
            /*if(IsLocalPlayerMaster())
            {
                PhotonNetwork.LoadLevel("Lobby");
            }
            else
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.LoadLevel("MainMenu");
            }*/
        }

        public void SetSlimeIndexMax(int _nb)
        {
            slimeIndexMax = _nb;
        }

        static int SortByNickname(Photon.Realtime.Player p1, Photon.Realtime.Player p2)
        {
            return p1.NickName.CompareTo(p2.NickName);
        }

        public bool GetIsMortSubite()
        {
            if (timerAllGame >= 40)
                return true;

            return false;
        }

        public List<Slime> GetEverySlimes()
        {
            List<Slime> slimes = new List<Slime>();

            foreach (Player player in listPlayersBlue)
            {
                foreach (Slime slime in player.slimes)
                {
                    Debug.LogError("add slime blue");
                    slimes.Add(slime);
                }
            }

            foreach (Player player in listPlayersRed)
            {
                foreach (Slime slime in player.slimes)
                {
                    Debug.LogError("add slime red");
                    slimes.Add(slime);
                }
            }

            Debug.LogError("Slimes count : " + slimes.Count);

            return slimes;
        }

        public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
        {
            Debug.LogError(cause);
            for (int i = 0; i < listPlayerBlue.Count; i++)
            {
                if (listPlayerBlue[i] == PhotonNetwork.LocalPlayer)
                {
                    listPlayersBlue[i].KillAllSlimes();
                    photonView.RPC("RemovePlayer", RpcTarget.All, "blue", i);
                }
            }

            for (int i = 0; i < listPlayerRed.Count; i++)
            {
                if (listPlayerRed[i] == PhotonNetwork.LocalPlayer)
                {
                    listPlayersRed[i].KillAllSlimes();
                    photonView.RPC("RemovePlayer", RpcTarget.All, "red", i);
                }
            }
        }

        [PunRPC]
        void RemovePlayer(string _team, int _index)
        {
            if (_team == "blue")
            {
                listPlayersBlue.Remove(listPlayersBlue[_index]);
            }
            else if (_team == "red")
            {
                listPlayersRed.Remove(listPlayersRed[_index]);
            }
        }

        void SetVictoryScreen(bool _state, bool _hasLose)
        {
            if (_state == true)
            {
                victoryScreen.SetActive(true);
                gamePhaseState = GamePhaseState.END_GAME;
                if (_hasLose)
                    victoryScreenText.text = "Lose";
                else
                    victoryScreenText.text = "Victory";
            }
            else
                victoryScreen.SetActive(false);
        }

        bool GetBlueLose()
        {
            bool remove = false;
            int removeIndex = -1;
            for (int i = 0; i < listPlayersBlue.Count; i++)
            {
                if (!listPlayersBlue[i].GetHasLose() && !remove)
                    return false;
                else
                {
                    remove = true;
                    removeIndex = i;
                }
                //photonView.RPC("RemovePlayer", RpcTarget.All, "blue", i);
            }

            if (remove)
            {
                photonView.RPC("RemovePlayer", RpcTarget.All, "blue", removeIndex);
                return false;
            }

            return true;
        }

        bool GetRedLose()
        {
            bool remove = false;
            int removeIndex = -1;
            for (int i = 0; i < listPlayersRed.Count; i++)
            {
                if (!listPlayersRed[i].GetHasLose() && !remove)
                    return false;
                else
                {
                    remove = true;
                    removeIndex = i;
                }
                //photonView.RPC("RemovePlayer", RpcTarget.All, "red", i);
            }

            if (remove)
            {
                photonView.RPC("RemovePlayer", RpcTarget.All, "red", removeIndex);
                return false;
            }

            return true;
        }

        void TeamVictory()
        {
            if (GetBlueLose())
            {
                Debug.LogError("blue loose");
                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["t"] == "blue")
                    SetVictoryScreen(true, true);
                else
                    SetVictoryScreen(true, false);
            }

            if (GetRedLose())
            {
                Debug.LogError("red loose");
                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["t"] == "red")
                    SetVictoryScreen(true, true);
                else
                    SetVictoryScreen(true, false);
            }
        }
    }
}
