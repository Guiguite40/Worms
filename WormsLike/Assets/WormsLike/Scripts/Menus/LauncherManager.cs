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
    [SerializeField] private Text textLoginMdpCache;
    [Space(4)]
    [SerializeField] private Text textLoginPseudoError;
    [SerializeField] private Text textLoginMdpError;
    [Space(6)]
    [SerializeField] private Text textSignUpPseudo;
    [SerializeField] private Text textSignUpMdp;
    [SerializeField] private Text textSignUpMdpConfirm;
    [SerializeField] private Text textSignUpMdpCache;
    [SerializeField] private Text textSignUpMdpConfirmCache;
    [Space(4)]
    [SerializeField] private Text textSignUpPseudoError;
    [SerializeField] private Text textSignUpMdpConfirmError;
    [SerializeField] private Text textColorError;
    [Space(4)]
    [SerializeField] private Image ppSelected;
    [SerializeField] private Button pp0;
    [SerializeField] private Button pp1;
    [SerializeField] private Button pp2;
    [SerializeField] private Button pp3;
    [SerializeField] private Button pp4;
    [SerializeField] private Button pp5;
    [SerializeField] private GameObject parameters;

    string accountsFilePath = "";
    string nickname = "";
    int accountColorIndex = -1;
    int ifSignUpIndex = 0;
    int ifLoginIndex = 0;
    bool fileEmpty = false;
    bool hasNoLine = false;
    bool[] accountCreationStep;
    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    List<InputField> listInputFieldsLogin = new List<InputField>();
    List<InputField> listInputFieldsSignUp = new List<InputField>();
    List<Button> listPp = new List<Button>();
    void Start()
    {
#if UNITY_EDITOR
        accountsFilePath = Application.dataPath + "/WormsLike/Resources/Accounts.txt";
#elif UNITY_STANDALONE_WIN
        accountsFilePath = Application.dataPath + "/Resources/Accounts.txt";
#endif
        canvas.Add("choose", goChoose);
        canvas.Add("login", goLogin);
        canvas.Add("signUp", goSignUp);
        canvas.Add("parameter", parameters);

        accountCreationStep = new bool[3];
        for (int i = 0; i < 3; i++)
            accountCreationStep[i] = false;

        listInputFieldsLogin.Add(ifLoginPseudo);
        listInputFieldsLogin.Add(ifLoginMdp);

        listInputFieldsSignUp.Add(ifSignUpPseudo);
        listInputFieldsSignUp.Add(ifSignUpMdp);
        listInputFieldsSignUp.Add(ifSignUpMdpConfirm);

        DesactiveErrorLogin();

        if (!File.Exists(accountsFilePath))
		{
            print("accounts file not exist, create one...");
            File.Create(accountsFilePath);
            fileEmpty = true;
        }

        OpenCanvas("choose");

        ppSelected.gameObject.SetActive(false);
        listPp.Add(pp0);
        listPp.Add(pp1);
        listPp.Add(pp2);
        listPp.Add(pp3);
        listPp.Add(pp4);
        listPp.Add(pp5);

        DontDestroyOnLoad(parameters);
        parameters.SetActive(false);

        SoundManager.instance.PlayMusic("menuOst");
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

        textLoginMdpCache.text = "";
        for (int i = 0; i < textLoginMdp.text.Length; i++)
		{
            textLoginMdpCache.text += "*";
		}

        textSignUpMdpCache.text = "";
        for (int i = 0; i < textSignUpMdp.text.Length; i++)
        {
            textSignUpMdpCache.text += "*";
        }

        textSignUpMdpConfirmCache.text = "";
        for (int i = 0; i < textSignUpMdpConfirm.text.Length; i++)
        {
            textSignUpMdpConfirmCache.text += "*";
        }
    }

    public void OpenCanvas(string _keyName)
	{
        DesactiveErrorLogin();

        canvas["choose"].SetActive(false);
        canvas["login"].SetActive(false);
        canvas["signUp"].SetActive(false);
        canvas["parameter"].SetActive(false);

        canvas[_keyName].SetActive(true);
        ResetInputsFields();
    }

    public void PlayMenuClick(string _clickType)
	{
        if(_clickType == "base")
            SoundManager.instance.PlaySound("menuClick");
        if (_clickType == "confirm")
            SoundManager.instance.PlaySound("menuClickConfirm");
        if (_clickType == "back")
            SoundManager.instance.PlaySound("menuClickError");
        if (_clickType == "error")
            SoundManager.instance.PlaySound("menuClickError");
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
            if (listInputFieldsLogin[0].isFocused)
                ifLoginIndex = 1;
            if (listInputFieldsLogin[1].isFocused)
                ifLoginIndex = 0;

            listInputFieldsLogin[ifLoginIndex].Select();
            ifLoginIndex++;
            if (ifLoginIndex == 2)
                ifLoginIndex = 0;
            return;
        }

        if (canvas["signUp"].activeSelf)
        {
            if (listInputFieldsSignUp[0].isFocused)
                ifSignUpIndex = 1;
            if (listInputFieldsSignUp[1].isFocused)
                ifSignUpIndex = 2;
            if (listInputFieldsSignUp[2].isFocused)
                ifSignUpIndex = 0;

            listInputFieldsSignUp[ifSignUpIndex].Select();
            ifSignUpIndex++;
            if (ifSignUpIndex == 3)
                ifSignUpIndex = 0;
            return;
        }
    }

    void ResetInputsFields()
	{
        for (int i = 0; i < 2; i++)
            listInputFieldsLogin[i].text = "";

        for (int i = 0; i < 3; i++)
            listInputFieldsSignUp[i].text = "";

        accountColorIndex = -1;
        ppSelected.gameObject.SetActive(false);
    }

    public void OnClickLogin()
	{
        if (LoginAccountIsExist())
            Connect(false);
        else
        {
            SoundManager.instance.PlaySound("menuClickError");
            return;
        }
	}

    public void OnClickSignUp()
	{
        if (AccountCreation())
            Connect(true);
        else
        {
            SoundManager.instance.PlaySound("menuClickError");
            Debug.LogError("account creation failed");
            return;
        }
    }

    void Connect(bool _isSignUp)
    {
        SoundManager.instance.PlaySound("menuClickConfirm");
        if (!_isSignUp)
            PhotonNetwork.LocalPlayer.NickName = nickname;
        else
            PhotonNetwork.LocalPlayer.NickName = textSignUpPseudo.text;

        PhotonNetwork.ConnectUsingSettings();

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("pp", accountColorIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        PhotonNetwork.LoadLevel("MainMenu");
    }

    bool AccountCreation()
	{
        if (!fileEmpty)
        {
            string[] line = File.ReadAllLines(accountsFilePath);

            if (line.Length > 0)
            {
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
            }
            else
            {
                accountCreationStep[0] = true;
                hasNoLine = true;
            }
        }
        else
            accountCreationStep[0] = true;

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
            //PhotonNetwork.LocalPlayer.NickName = textSignUpPseudo.text;
            WriteFileNewAccount();
            return true;
        }

        ResetAccountCreationStep();
        SoundManager.instance.PlaySound("menuClickError");
        return false;
	}

    public void OpenParameter()
	{
        parameters.SetActive(true);
	}

    public void CloseParameter()
    {
        parameters.SetActive(false);
    }

    public void SetImgColorIndex(int _index)
	{
        accountColorIndex = _index;
        ppSelected.gameObject.SetActive(true);

        ppSelected.transform.position = listPp[accountColorIndex].transform.position;
    }

    void ResetAccountCreationStep()
	{
        for (int i = 0; i < 3; i++)
            accountCreationStep[i] = false;
    }

    void WriteFileNewAccount()
	{
        string strToWrite = textSignUpPseudo.text + ":" + textSignUpMdp.text + "/" + accountColorIndex;
        if (!fileEmpty && !hasNoLine)
            File.AppendAllText(accountsFilePath, "\n" + strToWrite);
        else
            File.AppendAllText(accountsFilePath, strToWrite);
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
                accountColorIndex = imgIndex;
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
        SoundManager.instance.PlaySound("menuClickError");
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
