using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;
using System;

public class PlayerInfoManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text textNbVictory;
    [SerializeField] private Text textNbLoses;
    [SerializeField] private Text textRatio;
    [SerializeField] private GameObject canvasPlayerInfo;
    [SerializeField] private GameObject canvasModifyTeam;
    [SerializeField] private GameObject canvasTeamManager;

    string saveFilePath;
    int accountIndex = 0;

    [Serializable]
    public class AccountsSaveInfo
	{
        public Account[] account;

	} public AccountsSaveInfo accountInfo;

    [Serializable]
    public class Account
	{
        public string nickname;
        public int dmVictory;
        public int dmLoses;
        public int fortVictory;
        public int fortLoses;
        public Team[] team;
	}

    [Serializable]
    public class Team
	{
        public string teamName;
        public Character[] character;
	}

    [Serializable]
    public class Character
	{
        public string name;
        public int skinIndex;
	}

    void ReadSaveFile()
	{
        if(File.Exists(saveFilePath))
		{
            string json = File.ReadAllText(saveFilePath);
            accountInfo = JsonUtility.FromJson<AccountsSaveInfo>(json);
        }
        else
		{
            File.Create(saveFilePath);
            SetupPlayerInfo();
		}
	}

    void SetupPlayerInfo()
	{
        accountInfo.account = new Account[1];
        accountInfo.account[0] = new Account();
        accountInfo.account[0].nickname = PhotonNetwork.LocalPlayer.NickName;
        accountInfo.account[0].dmVictory = 1;
        accountInfo.account[0].dmLoses = 2;
        accountInfo.account[0].fortVictory = 2;
        accountInfo.account[0].fortLoses = 0;

        accountInfo.account[0].team = new Team[4];
        for (int i = 0; i < 4; i++)
        {
            accountInfo.account[0].team[i] = new Team();
            accountInfo.account[0].team[i].teamName = "empty";
            accountInfo.account[0].team[i].character = new Character[6];
            for (int y = 0; y < 6; y++)
			{
                accountInfo.account[0].team[i].character[y] = new Character();
                accountInfo.account[0].team[i].character[y].name = "name" + i.ToString() + y.ToString();
                accountInfo.account[0].team[i].character[y].skinIndex = 0;
            }
        }
    }

    void WriteFilePlayerInfo()
	{
        string toJson = JsonUtility.ToJson(accountInfo, true);
        File.WriteAllText(saveFilePath, toJson);
    }

    void Start()
    {
        saveFilePath = Application.dataPath + "/Assets/Resources/PlayerSaveInfo.json";
        ReadSaveFile();
        GetActiveAccount();
        SetDeathMatchInfo();
        canvasPlayerInfo.SetActive(false);
        canvasModifyTeam.SetActive(false);
        //canvasTeamManager.SetActive(false);
    }

    void Update()
    {
        
    }

    void GetActiveAccount()
	{
        for (int i = 0; i < accountInfo.account.Length; i++)
            if (PhotonNetwork.LocalPlayer.NickName == accountInfo.account[i].nickname)
                accountIndex = i;
    }

    public void SetDeathMatchInfo()
	{
        float victory = accountInfo.account[accountIndex].dmVictory;
        float loses = accountInfo.account[accountIndex].dmLoses;

        textNbVictory.text = victory.ToString();
        textNbLoses.text = loses.ToString();

        if (loses == 0)
            loses = 1;

        float ratio = victory / loses;
        textRatio.text = ratio.ToString();
    }

    public void SetFortsInfo()
    {
        float victory = accountInfo.account[accountIndex].fortVictory;
        float loses = accountInfo.account[accountIndex].fortLoses;

        textNbVictory.text = victory.ToString();
        textNbLoses.text = loses.ToString();

        if (loses == 0)
            loses = 1;

        float ratio = victory / loses;
        textRatio.text = ratio.ToString();
    }

    public void SavePlayerInfo()
	{
        WriteFilePlayerInfo();
    }

    public void ClosePlayerInfo()
	{

        WriteFilePlayerInfo();
    }

	public void OnApplicationQuit()
	{
        //WriteFilePlayerInfo();
    }

    public void OnClickPlayerImg()
	{
        canvasPlayerInfo.SetActive(true);
    }

    public void OnClickTeamCharacter()
	{
        print("cliiick");
	}
}