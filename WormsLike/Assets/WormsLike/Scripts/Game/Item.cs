using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Enums.Type type = 0;
    public Enums.ItemsList itemsList = 0;
    public bool infiniteAmmo = false;
    public int ammo = 0;
}
