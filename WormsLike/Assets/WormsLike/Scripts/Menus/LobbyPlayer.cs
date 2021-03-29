using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
	[SerializeField] private Text textName;

	int oldTeam;
	int currentTeam;
	string name;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			int team = currentTeam;
			stream.SendNext(team);
			stream.SendNext(name);
		}
		else
		{
			currentTeam = (int)stream.ReceiveNext();
			name = (string)stream.ReceiveNext();
			UpdateInfo();
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

    }

	void UpdateInfo()
	{
		textName.text = name;
		if (currentTeam != oldTeam)
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

	public void SetupPlayerInfo(string _name)
	{
		name = _name;
		textName.text = name;
	}
}
