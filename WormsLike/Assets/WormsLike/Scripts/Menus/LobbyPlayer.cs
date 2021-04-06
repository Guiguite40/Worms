using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
	[SerializeField] private Text textName;
	[SerializeField] private Image img;

	int oldTeam;
	int currentTeam;
	string name;
	int ppIndex;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			int team = currentTeam;
			stream.SendNext(team);
			stream.SendNext(name);
			stream.SendNext((int)PhotonNetwork.LocalPlayer.CustomProperties["pp"]);
		}
		else
		{
			currentTeam = (int)stream.ReceiveNext();
			name = (string)stream.ReceiveNext();
			ppIndex = (int)stream.ReceiveNext();
			UpdateInfo();
		}
	}

	void Start()
    {
		currentTeam = (int)RoomManager.TeamState.SPECTATE;
		oldTeam = currentTeam;
		//img.sprite = ProfilePictureManager.instance.GetPicture((int)PhotonNetwork.LocalPlayer.CustomProperties["pp"]);
		UpdateTeam();
	}


    void Update()
    {

    }

	void UpdateInfo()
	{
		textName.text = name;
		img.sprite = ProfilePictureManager.instance.GetPicture(ppIndex);
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

	public void SetupPlayerInfo(string _name, int _ppIndex)
	{
		name = _name;
		textName.text = name;
		img.sprite = ProfilePictureManager.instance.GetPicture(_ppIndex);
	}
}
