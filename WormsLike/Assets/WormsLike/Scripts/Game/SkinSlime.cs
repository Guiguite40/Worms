using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SkinSlime : MonoBehaviourPunCallbacks
{
    [SerializeField] Sprite Slime1 = null;
    [SerializeField] Sprite Slime2 = null;
    [SerializeField] Sprite Slime3 = null;
    [SerializeField] Sprite Slime4 = null;
    [SerializeField] Sprite Slime5 = null;
    [SerializeField] Sprite Slime6 = null;

    [SerializeField] SpriteRenderer mySlime = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in PhotonNetwork.PlayerList)
        {
            if (item.CustomProperties.ContainsKey("pp"))
            {
                if (item.UserId == PhotonNetwork.LocalPlayer.UserId)
                {
                    if ((int)item.CustomProperties["pp"] == 0)
                    {
                        mySlime.sprite = Slime1;
                    }
                    else if ((int)item.CustomProperties["pp"] == 1)
                    {
                        mySlime.sprite = Slime2;
                    }
                    else if ((int)item.CustomProperties["pp"] == 2)
                    {
                        mySlime.sprite = Slime3;
                    }
                    else if ((int)item.CustomProperties["pp"] == 3)
                    {
                        mySlime.sprite = Slime4;
                    }
                    else if ((int)item.CustomProperties["pp"] == 4)
                    {
                        mySlime.sprite = Slime5;
                    }
                    else if ((int)item.CustomProperties["pp"] == 5)
                    {
                        mySlime.sprite = Slime6;
                    }
                }
            }
        }
    }
}
