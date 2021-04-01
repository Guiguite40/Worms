using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject invPanel = null;
    [SerializeField] public List<Button> itemButtons = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Button button in invPanel.GetComponentsInChildren<Button>())
        {
            itemButtons.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SelectItem(int _itemIndex)
    {

    }
}
