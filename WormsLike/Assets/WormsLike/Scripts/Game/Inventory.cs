using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Inventory : MonoBehaviourPunCallbacks
{
    //public List<Item_Weapon> weapons;
    //public List<Item_Defense> defenses;
    //public List<Item_Movement> movements;

    [SerializeField] GameObject ui_Inventory = null;
    [SerializeField] List<Button> items = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (ui_Inventory.activeInHierarchy)
                ui_Inventory.SetActive(true);
        }

        if (/*Input.GetMouseButtonDown(1) ||*/Input.GetKeyDown(KeyCode.Escape))
        {
            if (ui_Inventory.activeInHierarchy)
                ui_Inventory.SetActive(false);
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //        // if mouse click outside of ui inventory
        //    if (ui_Inventory.activeInHierarchy)
        //        ui_Inventory.SetActive(false);
        //}
    }
}