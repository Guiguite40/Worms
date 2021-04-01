using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Inventory : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<GameObject> itemPrefabs = null;

    public Dictionary<Enums.ItemsList, Item> items = new Dictionary<Enums.ItemsList, Item>();

    private void Start()
    {
        foreach (GameObject item in itemPrefabs)
        {
            items.Add(item.GetComponent<Item>().itemsList, Instantiate(item.GetComponent<Item>()));
        }                
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    if (ui_Inventory.activeInHierarchy)
        //        ui_Inventory.SetActive(true);
        //}

        //if (/*Input.GetMouseButtonDown(1) ||*/Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (ui_Inventory.activeInHierarchy)
        //        ui_Inventory.SetActive(false);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //        // if mouse click outside of ui inventory
        //    if (ui_Inventory.activeInHierarchy)
        //        ui_Inventory.SetActive(false);
        //}
    }
}