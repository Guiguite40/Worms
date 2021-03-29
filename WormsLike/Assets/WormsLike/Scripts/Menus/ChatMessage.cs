using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Text textPlayerName;
    [SerializeField] private Text textMessage;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetupMessage(string _playerName, string _message)
	{
        textPlayerName.text = _playerName;
        textMessage.text = _message;
	}
}
