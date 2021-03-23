using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

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

    Dictionary<string, GameObject> canvas = new Dictionary<string, GameObject>();
    List<InputField> listInputFields = new List<InputField>();
    void Start()
    {
        canvas.Add("choose", goChoose);
        canvas.Add("login", goLogin);
        canvas.Add("signUp", goSignUp);

        listInputFields.Add(ifLoginPseudo);
        listInputFields.Add(ifLoginMdp);

        listInputFields.Add(ifSignUpPseudo);
        listInputFields.Add(ifSignUpMdp);
        listInputFields.Add(ifSignUpMdpConfirm);

        OpenCanvas("choose");
    }

    void Update()
    {
        
    }

    public void OpenCanvas(string _keyName)
	{
        canvas["choose"].SetActive(false);
        canvas["login"].SetActive(false);
        canvas["signUp"].SetActive(false);

        canvas[_keyName].SetActive(true);
        ResetInputsFields();
    }

    void ResetInputsFields()
	{
        for (int i = 0; i < 5; i++)
            listInputFields[i].text = "";
	}
}
