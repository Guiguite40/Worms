using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab = null;
    [SerializeField] private List<GameObject> slimes = new List<GameObject>();
    private Inventory inv = null;

    [HideInInspector] public int slimeLimit = 3;


    /***** DEBUG *****/
    [SerializeField] private GameObject healthBoxPrefab = null;
    [SerializeField] private GameObject damageBoxPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debuging();

        foreach (var item in slimes)
        {
            if (item.GetComponent<Slime>().isDead)
            {
                slimes.Remove(item);
                Destroy(item);
                break;
            }
        }
    }

    void Debuging()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            PlaceCharacter();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) // Not available for now
        {
            GameObject healthBox = healthBoxPrefab;
            healthBox.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(healthBox);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) // Not available for now
        {
            GameObject damageBox = damageBoxPrefab;
            damageBox.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(damageBox);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCharacterControlled(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCharacterControlled(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCharacterControlled(2);
    }


    private void SetCharacterControlled(int _index)
    {
        foreach (var item in slimes)
        {
            if (item.GetComponent<Slime>().isControlled == true)
            {
                item.GetComponent<Slime>().isControlled = false;
            }
        }
        slimes[_index].GetComponent<Slime>().isControlled = true;
    }

    private void PlaceCharacter()
    {
        if (slimes.Count < slimeLimit)
        {
            Debug.Log(slimes.Count);
            GameObject slime = slimePrefab;
            slime.GetComponent<Slime>().SetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            slimes.Add(Instantiate(slime));
        }
    }
}
