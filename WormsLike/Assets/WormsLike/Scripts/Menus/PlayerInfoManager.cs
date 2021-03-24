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
    [SerializeField] private GameObject canvasEnterTeamName;
    [Space(10)]
    [SerializeField] private Text characterName1;
    [SerializeField] private Text characterName2;
    [SerializeField] private Text characterName3;
    [SerializeField] private Text characterName4;
    [SerializeField] private Text characterName5;
    [SerializeField] private Text characterName6;
    [Space(8)]
    [SerializeField] private Text teamNameEntered;
    [Space(5)]
    [SerializeField] private Text teamName1;
    [SerializeField] private Text teamName2;
    [SerializeField] private Text teamName3;
    [SerializeField] private Text teamName4;

    List<Text> listCharacterName = new List<Text>();
    List<Text> listTeamName = new List<Text>();
    string saveFilePath;
    int accountIndex = 0;
    int currentTeamSelected = 0;

    [Serializable]
    public class AccountsSaveInfo
    {
        public Account[] account;

    }
    public AccountsSaveInfo accountInfo;

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
        if (File.Exists(saveFilePath))
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
        canvasTeamManager.SetActive(false);
        canvasEnterTeamName.SetActive(false);

        listCharacterName.Add(characterName1);
        listCharacterName.Add(characterName2);
        listCharacterName.Add(characterName3);
        listCharacterName.Add(characterName4);
        listCharacterName.Add(characterName5);
        listCharacterName.Add(characterName6);

        listTeamName.Add(teamName1);
        listTeamName.Add(teamName2);
        listTeamName.Add(teamName3);
        listTeamName.Add(teamName4);
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

    public void OnClickPlayerImg()
    {
        if (!canvasPlayerInfo.activeSelf)
            canvasPlayerInfo.SetActive(true);
        else
            canvasPlayerInfo.SetActive(false);
    }

    public void OnClickTeamCharacter()
    {
        print("cliiick");
    }

    public void OnClickOnTeam(int _index)
	{
        canvasModifyTeam.SetActive(true);
	}

    public void OnClickValidateTeam()
    {
        canvasEnterTeamName.SetActive(true);
        for (int i = 0; i < 6; i++)
        {
            if (listCharacterName[i].text == "")
                listCharacterName[i].text = "character" + UnityEngine.Random.Range(10, 100);
        }
    }

    public void OnClickCreateTeam()
    {
        for (int i = 0; i < 6; i++)
        {
            accountInfo.account[accountIndex].team[currentTeamSelected].character[i].name = listCharacterName[i].text;
            listCharacterName[i].text = "";
        }

        if (teamNameEntered.text == "")
            teamNameEntered.text = "empty";

        accountInfo.account[accountIndex].team[currentTeamSelected].teamName = teamNameEntered.text;
        listTeamName[currentTeamSelected].text = teamNameEntered.text;

        canvasModifyTeam.SetActive(false);
        canvasTeamManager.SetActive(false);
        canvasEnterTeamName.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        //WriteFilePlayerInfo();
    }
}