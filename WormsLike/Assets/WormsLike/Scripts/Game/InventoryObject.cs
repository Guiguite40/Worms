using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObject : MonoBehaviour
{
    public GameObject itemPrefabs = null;
    public int ammo = 0;

    public bool selected = false;

    public void SelectItem()
    {
        if (!selected)
            selected = true;
    }
}
