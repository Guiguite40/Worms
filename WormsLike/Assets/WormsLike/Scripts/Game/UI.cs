using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject inv = null;
    [SerializeField] public List<Button> itemButtons = new List<Button>();

    private static UI instance = null;

    public bool inventoryOpened = false;

    private Enums.ItemsList itemSelected = 0;

    // Game Instance Singleton
    public static UI Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpened = !inventoryOpened;
        }

        if (inventoryOpened)
        {
            if (!inv.activeInHierarchy)
                inv.SetActive(true);
        }
        else
        {
            if (inv.activeInHierarchy)
                inv.SetActive(false);
        }
    }

    public void SelectItem(int _itemSelected)
    {
        itemSelected = (Enums.ItemsList)_itemSelected;
    }

    public Enums.ItemsList GetItemSelected()
    {
        return itemSelected;
    }

    public void ActualizeAmmoCount(Inventory _inv)
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (!_inv.items[(Enums.ItemsList)i].infiniteAmmo)
            {
                itemButtons[i].GetComponentInChildren<Text>().text = _inv.items[(Enums.ItemsList)i].ammo.ToString();
            }
        }
    }

    public void CloseInventory()
    {
        if (inventoryOpened)
            inventoryOpened = false;
    }
}
