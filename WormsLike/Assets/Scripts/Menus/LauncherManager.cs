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
    [Space(6)]
    [SerializeField] private Text textSignUpPseudo;
    [SerializeField] private Text textSignUpMdp;
    [SerializeField] private Text textSignUpMdpConfirm;

    string accountsFilePath = "";
    string nickname = "";
    int accountColorIndex = -1;
    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    List<InputField> listInputFieldsLogin = new List<InputField>();
    List<InputField> listInputFieldsSignUp = new List<InputField>();
    void Start()
    {
        accountsFilePath = Application.dataPath + "/Assets/Resources/Accounts.txt";
        canvas.Add("choose", goChoose);
        canvas.Add("login", goLogin);
        canvas.Add("signUp", goSignUp);

        listInputFieldsLogin.Add(ifLoginPseudo);
        listInputFieldsLogin.Add(ifLoginMdp);

        listInputFieldsSignUp.Add(ifSignUpPseudo);
        listInputFieldsSignUp.Add(ifSignUpMdp);
        listInputFieldsSignUp.Add(ifSignUpMdpConfirm);

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
        canvas["choose"].SetActive(false);
        canvas["login"].SetActive(false);
        canvas["signUp"].SetActive(false);

        canvas[_keyName].SetActive(true);
        ResetInputsFields();
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
        PhotonNetwork.LoadLevel("Menu");
    }

    bool AccountCreation()
	{


        return true;
	}

	bool LoginAccountIsExist()
	{
        //if (File.Exists(Application.dataPath + "/Assets/Resources/Accounts.txt"))
        //{
            bool isPseudoFind = false;
            string[] line = File.ReadAllLines(accountsFilePath);
            for (int i = 0; i < line.Length; i++)
            {
                string pseudo = line[i].Substring(0, line[i].IndexOf(':'));
                string mdp = line[i].Substring(line[i].IndexOf(':')+1, line[i].IndexOf('/') - line[i].IndexOf(':')-1);
                string imgIndexStr = line[i].Substring(line[i].IndexOf('/')+1);
                int imgIndex = int.Parse(imgIndexStr);

                //print("pseudo : " + pseudo);
                //print("mdp : " + mdp);
                //print("img index : " + imgIndex);

                if (textLoginPseudo.text == pseudo)
                    isPseudoFind = true;

                if (textLoginPseudo.text == pseudo && textLoginMdp.text == mdp)
				{
                    print("account find : pseudo : " + pseudo + ", mdp : " + mdp + ", img index : " + imgIndex.ToString());
                    nickname = pseudo;
                    return true;
				}
            }

            if (isPseudoFind)
            {
                print("mot de passe incorrect");
                ifLoginMdp.text = "";
            }
            else
            {
                print("pseudo introuvable");
                ifLoginPseudo.text = "";
                ifLoginMdp.text = "";
            }
            return false;
        /*}
        else
        {
            print("accounts file not exist, create one...");
            File.Create(Application.dataPath + "/Assets/Resources/Accounts.txt");
            return false;
        }*/
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
