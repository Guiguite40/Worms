using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject newPlayer = playerPrefab;
        newPlayer.GetComponent<Player>().isTurn = true;
        newPlayer.GetComponent<Player>().phase_game = true;
        PhotonNetwork.Instantiate(newPlayer.name, newPlayer.transform.position, newPlayer.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
