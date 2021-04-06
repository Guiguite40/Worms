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

    public void AddItem(Enums.ItemsList _item, int nb)
    {
        items[_item].ammo += nb;
    }

    public void UseItem(Enums.ItemsList _item)
    {
        if (!items[_item].infiniteAmmo)
            if (items[_item].ammo > 0)
                items[_item].ammo -= 1;
    }

    public bool IsItemUseable(Enums.ItemsList _item)
    {
        if (items[_item].infiniteAmmo || items[_item].ammo > 0)
            return true;
        else
            return false;
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