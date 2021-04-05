using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatMessage : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text textPlayerName;
    [SerializeField] private Text textMessage;
    [SerializeField] private Image img;
    [SerializeField] private float sizeY = 100;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform rectTransformText;
    [SerializeField] private RectTransform rectTransformTextBg;

    void Start()
    {
        UpdateSize();
        img.sprite = ProfilePictureManager.instance.GetPicture((int)PhotonNetwork.LocalPlayer.CustomProperties["pp"]);
    }

    void Update()
    {
        sizeY++;
        UpdateSize();
    }

    public void SetupMessage(string _playerName, string _message)
	{
        textPlayerName.text = _playerName;
        textMessage.text = _message;
	}

    void UpdateSize()
	{
        //transform.localScale = new Vector3(transform.localScale.x, sizeY, transform.localScale.z);
        //rectTransform.rect.Set(rectTransform.rect.x, rectTransform.rect.y, rectTransform.rect.width, sizeY);
        //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, sizeY);
        //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransformText.sizeDelta.y);
        rectTransformTextBg.sizeDelta =  new Vector2(rectTransformTextBg.sizeDelta.x, rectTransformText.sizeDelta.y + 70);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransformTextBg.sizeDelta.y);
    }
}
