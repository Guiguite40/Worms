using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Enums.Type type = 0;
    public Enums.ItemsList itemsList = 0;
    public int ammo = 0;
    [HideInInspector] public bool selected = false;

    private void Update()
    {
        ActualizeAmmo();
    }

    private void SelectItem()
    {
        if (!selected)
            selected = true;
    }

    private void ActualizeAmmo()
    {

    }
}
