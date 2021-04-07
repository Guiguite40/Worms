using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Inventory : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<GameObject> itemPrefabs = null;
    public Dictionary<Enums.ItemsList, Item> items = null;

    private void Start()
    {
        items = new Dictionary<Enums.ItemsList, Item>();

        foreach (GameObject item in itemPrefabs)
        {
            items.Add(item.GetComponent<Item>().itemsList, item.GetComponent<Item>());
        }

        //items[Enums.ItemsList.RocketLauncher].ammo = 0;
        //items[Enums.ItemsList.Grenade].ammo = 0;
        items[Enums.ItemsList.SaintGrenade].ammo = 5;
        items[Enums.ItemsList.Banana].ammo = 5;
        items[Enums.ItemsList.AirStrike].ammo = 5;
        items[Enums.ItemsList.Teleportation].ammo = 5;
        items[Enums.ItemsList.Parachute].ammo = 5;
        items[Enums.ItemsList.JetPack].ammo = 5;
        items[Enums.ItemsList.Shield].ammo = 5;
        //items[Enums.ItemsList.SkipTurn].ammo = 0;
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