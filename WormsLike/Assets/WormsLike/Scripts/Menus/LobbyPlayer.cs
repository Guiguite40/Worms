using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
	int oldTeam;
	int currentTeam;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			int team = currentTeam;
			stream.SendNext(team);
		}
		else
		{
			currentTeam = (int)stream.ReceiveNext();
		}
	}

	void Start()
    {
		currentTeam = (int)RoomManager.TeamState.SPECTATE;
		oldTeam = currentTeam;
		UpdateTeam();
	}


    void Update()
    {
        if(currentTeam != oldTeam)
		{
			UpdateTeam();
			oldTeam = currentTeam;
		}
    }

	void UpdateTeam()
	{
		if (currentTeam == (int)RoomManager.TeamState.SPECTATE)
			gameObject.transform.parent = GameObject.Find("BgSpectate").transform;
		else if (currentTeam == (int)RoomManager.TeamState.BLUE)
			gameObject.transform.parent = GameObject.Find("PanelPlayersBlue").transform;
		else if (currentTeam == (int)RoomManager.TeamState.RED)
			gameObject.transform.parent = GameObject.Find("PanelPlayersRed").transform;
	}

	public void SetCurrentTeam(int _index)
	{
		currentTeam = _index;
		UpdateTeam();
	}
}
