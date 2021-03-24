using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab = null;
    List<GameObject> players = null;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        GameObject newPlayer = playerPrefab;
        newPlayer.GetComponent<Player>().isTurn = true;
        newPlayer.GetComponent<Player>().phase_game = true;
        players.Add(Instantiate(newPlayer));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
