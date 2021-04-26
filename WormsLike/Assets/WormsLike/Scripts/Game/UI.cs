using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private List<Texture2D> CursorTextures; // 0=red, 1=blue
    [Header("Others")]
    [SerializeField] public GameObject inv = null;
    [SerializeField] public List<Button> itemButtons = new List<Button>();

    private static UI instance = null;

    public bool inventoryOpened = false;

    private Enums.ItemsList itemSelected = 0;
    public bool isItemSelected = false;

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

        if (_itemSelected >= (int)Enums.ItemsList.RocketLauncher && _itemSelected <= (int)Enums.ItemsList.AirStrike)
        {
            UI.Instance.SetCursor(Enums.CursorType.Red);
        }
        else
        {
            UI.Instance.SetCursor(Enums.CursorType.Blue);
        }

        isItemSelected = true;
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

    public void OpenInventory()
    {
        if (!inventoryOpened)
            inventoryOpened = true;
    }

    public void CloseInventory()
    {
        if (inventoryOpened)
            inventoryOpened = false;
    }

    public void SetCursor(Enums.CursorType _cur)
    {
        if (_cur == Enums.CursorType.Normal)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //Debug.LogError("Cursor set to normal");
        }
        else
        {
            Cursor.SetCursor(CursorTextures[(int)_cur - 1], new Vector2(CursorTextures[(int)_cur - 1].width / 2, CursorTextures[(int)_cur - 1].height / 2), CursorMode.Auto);
            //if (_cur == Enums.CursorType.Red)
            //    Debug.LogError("Cursor set to red");
            //else
            //    Debug.LogError("Cursor set to blue");

        }
    }
}
