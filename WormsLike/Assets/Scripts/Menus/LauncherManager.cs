using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;

public class LauncherManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject goChoose;
    [SerializeField] private GameObject goLogin;
    [SerializeField] private GameObject goSignUp;
    [Space(10)]
    [SerializeField] private InputField ifLoginPseudo;
    [SerializeField] private InputField ifLoginMdp;
    [Space(6)]
    [SerializeField] private InputField ifSignUpPseudo;
    [SerializeField] private InputField ifSignUpMdp;
    [SerializeField] private InputField ifSignUpMdpConfirm;
    [Space(10)]
    [SerializeField] private Text textLoginPseudo;
    [SerializeField] private Text textLoginMdp;
    [Space(4)]
    [SerializeField] private Text textLoginPseudoError;
    [SerializeField] private Text textLoginMdpError;
    [Space(6)]
    [SerializeField] private Text textSignUpPseudo;
    [SerializeField] private Text textSignUpMdp;
    [SerializeField] private Text textSignUpMdpConfirm;
    [Space(4)]
    [SerializeField] private Text textSignUpPseudoError;
    [SerializeField] private Text textSignUpMdpConfirmError;
    [SerializeField] private Text textColorError;

    string accountsFilePath = "";
    string nickname = "";
    int accountColorIndex = -1;
    bool[] accountCreationStep;
    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    List<InputField> listInputFieldsLogin = new List<InputField>();
    List<InputField> listInputFieldsSignUp = new List<InputField>();
    void Start()
    {
        accountsFilePath = Application.dataPath + "/Assets/Resources/Accounts.txt";
        canvas.Add("choose", goChoose);
        canvas.Add("login", goLogin);
        canvas.Add("signUp", goSignUp);

        accountCreationStep = new bool[3];
        for (int i = 0; i < 3; i++)
            accountCreationStep[i] = false;

        listInputFieldsLogin.Add(ifLoginPseudo);
        listInputFieldsLogin.Add(ifLoginMdp);

        listInputFieldsSignUp.Add(ifSignUpPseudo);
        listInputFieldsSignUp.Add(ifSignUpMdp);
        listInputFieldsSignUp.Add(ifSignUpMdpConfirm);

        DesactiveErrorLogin();

        if (!File.Exists(Application.dataPath + "/Assets/Resources/Accounts.txt"))
		{
            print("accounts file not exist, create one...");
            File.Create(Application.dataPath + "/Assets/Resources/Accounts.txt");
        }

        OpenCanvas("choose");
    }

    void Update()
    {
        if (canvas["login"].activeSelf)
            if (Input.GetKeyDown(KeyCode.Return))
                OnClickLogin();

        if (canvas["signUp"].activeSelf)
            if (Input.GetKeyDown(KeyCode.Return))
                OnClickSignUp();

        if (Input.GetKeyDown(KeyCode.Tab))
            NextInputFieldActivate();
    }

    public void OpenCanvas(string _keyName)
	{
        DesactiveErrorLogin();

        canvas["choose"].SetActive(false);
        canvas["login"].SetActive(false);
        canvas["signUp"].SetActive(false);

        canvas[_keyName].SetActive(true);
        ResetInputsFields();
    }

    void DesactiveErrorLogin()
	{
        textLoginPseudoError.gameObject.SetActive(false);
        textLoginMdpError.gameObject.SetActive(false);
        textSignUpPseudoError.gameObject.SetActive(false);
        textSignUpMdpConfirmError.gameObject.SetActive(false);
        textColorError.gameObject.SetActive(false);
    }

    void NextInputFieldActivate()
    {
        if (canvas["login"].activeSelf)
        {
            if(listInputFieldsLogin[0].IsActive())
			{
                listInputFieldsLogin[0].DeactivateInputField();
                listInputFieldsLogin[1].ActivateInputField();
                return;
            }
            
            if(listInputFieldsLogin[1].IsActive())
			{
                listInputFieldsLogin[1].DeactivateInputField();
                listInputFieldsLogin[0].ActivateInputField();
                return;
            }
        }
    }

    void ResetInputsFields()
	{
        for (int i = 0; i < 2; i++)
            listInputFieldsLogin[i].text = "";

        for (int i = 0; i < 3; i++)
            listInputFieldsSignUp[i].text = "";

        accountColorIndex = -1;
    }

    public void OnClickLogin()
	{
        if (LoginAccountIsExist())
            Connect();
        else
            return;
	}

    public void OnClickSignUp()
	{
        if (AccountCreation())
            Connect();
        else
            return;
    }

    void Connect()
	{
        PhotonNetwork.LocalPlayer.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    bool AccountCreation()
	{
        string[] line = File.ReadAllLines(accountsFilePath);

        for (int i = 0; i < line.Length; i++)
        {
            string pseudo = line[i].Substring(0, line[i].IndexOf(':'));
            if (textSignUpPseudo.text == pseudo)
            {
                textSignUpPseudoError.gameObject.SetActive(true);
                accountCreationStep[0] = false;
                break;
            }
            else
            {
                textSignUpPseudoError.gameObject.SetActive(false);
                accountCreationStep[0] = true;
            }    
        }

        if (textSignUpMdp.text != textSignUpMdpConfirm.text)
        {
            textSignUpMdpConfirmError.gameObject.SetActive(true);
            accountCreationStep[1] = false;
        }
        else
        {
            textSignUpMdpConfirmError.gameObject.SetActive(false);
            accountCreationStep[1] = true;
        }

        if (accountColorIndex == -1)
        {
            textColorError.gameObject.SetActive(true);
            accountCreationStep[2] = false;
        }
        else
        {
            textColorError.gameObject.SetActive(false);
            accountCreationStep[2] = true;
        }

        if (accountCreationStep[0] && accountCreationStep[1] && accountCreationStep[2])
        {
            WriteFileNewAccount();
            return true;
        }

        ResetAccountCreationStep();
        return false;
	}

    public void SetImgColorIndex(int _index)
	{
        accountColorIndex = _index;
	}

    void ResetAccountCreationStep()
	{
        for (int i = 0; i < 3; i++)
            accountCreationStep[i] = false;
    }

    void WriteFileNewAccount()
	{
        string strToWrite = textSignUpPseudo.text + ":" + textSignUpMdp.text + "/" + accountColorIndex;
        File.AppendAllText(accountsFilePath, "\n" + strToWrite);
    }

	bool LoginAccountIsExist()
	{
        bool isPseudoFind = false;
        string[] line = File.ReadAllLines(accountsFilePath);
        for (int i = 0; i < line.Length; i++)
        {
            string pseudo = line[i].Substring(0, line[i].IndexOf(':'));
            string mdp = line[i].Substring(line[i].IndexOf(':') + 1, line[i].IndexOf('/') - line[i].IndexOf(':') - 1);
            string imgIndexStr = line[i].Substring(line[i].IndexOf('/') + 1);
            int imgIndex = int.Parse(imgIndexStr);

            if (textLoginPseudo.text == pseudo)
                isPseudoFind = true;

            if (textLoginPseudo.text == pseudo && textLoginMdp.text == mdp)
            {
                print("account find : pseudo : " + pseudo + ", mdp : " + mdp + ", img index : " + imgIndex.ToString());
                DesactiveErrorLogin();
                nickname = pseudo;
                return true;
            }
        }

        if (isPseudoFind)
        {
            print("mot de passe incorrect");
            textLoginPseudoError.gameObject.SetActive(false);
            textLoginMdpError.gameObject.SetActive(true);
            ifLoginMdp.text = "";
        }
        else
        {
            print("pseudo introuvable");
            textLoginPseudoError.gameObject.SetActive(true);
            ifLoginPseudo.text = "";
            ifLoginMdp.text = "";
        }
        return false;
	}

    void SetAccountState(string _pseudo, string _mdp)
	{
        PhotonNetwork.LocalPlayer.NickName = _pseudo;

        /*string Password = _mdp;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("PassWord", Password);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        PhotonNetwork.LocalPlayer.CustomProperties.*/
    }
}
