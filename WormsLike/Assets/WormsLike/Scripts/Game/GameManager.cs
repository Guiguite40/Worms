using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab = null;

    bool waitOtherPlayers = true;
    bool connected = false;

    public bool phase_placement = false;
    public bool phase_game = false;

    [SerializeField] Text timerText;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = "test" + Random.Range(0, 1000);
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connection to master..");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && waitOtherPlayers)
        {
            waitOtherPlayers = false;
            Debug.Log("Waiting moment stopped");
        }

        timer -= Time.deltaTime;
        if (timer < 0)
            timer = 60;
        timerText.text = timer.ToString("0");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        Debug.Log("Lobby joining..");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");
        if (PhotonNetwork.CountOfRooms < 1)
            PhotonNetwork.CreateRoom("roomTest");
        else
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
        StartCoroutine(InstantiatePlayer());
    }

    IEnumerator InstantiatePlayer()
    {
        Debug.Log("Waiting other players");
        yield return new WaitWhile(() => waitOtherPlayers);
        if (waitOtherPlayers)
            waitOtherPlayers = false;

        Debug.Log("Players instantiate");
        GameObject newPlayer = playerPrefab;
        newPlayer.GetComponent<Player>().isTurn = true;
        newPlayer.GetComponent<Player>().phase_game = true;
        PhotonNetwork.Instantiate(newPlayer.name, newPlayer.transform.position, newPlayer.transform.rotation);
    }

    public float GetTimer()
    {
        return timer;
    }
}
