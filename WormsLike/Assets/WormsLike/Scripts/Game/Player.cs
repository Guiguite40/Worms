using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject slimePrefab = null;
    [SerializeField] List<Slime> slimes = new List<Slime>();
    [SerializeField] Inventory inv = null;

    [HideInInspector] public int slimeLimit = 3;
    public int team = 0;

    public bool phase_placement = false;
    public bool phase_game = false;
    public bool isTurn = false;

    Enums.ItemsList itemSelected = 0;

    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    [SerializeField] Slime currentCharacter = null;

    [SerializeField] float charge = 0;
    [SerializeField] float speed = 0;

    float timeToRelease = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("player start");
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
        ControlCharacter();
    }

    void Debuging()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            PlaceCharacter();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameObject healthBox = Instantiate(healthBoxPrefab);
            healthBox.transform.position = new Vector3(MousePos().x, MousePos().y, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameObject damageBox = Instantiate(damageBoxPrefab);
            damageBox.transform.position = new Vector3(MousePos().x, MousePos().y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetCharacterControlled(0);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetCharacterControlled(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetCharacterControlled(2);
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(Enums.ItemsList.RocketLauncher);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(Enums.ItemsList.Grenade);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectWeapon(Enums.ItemsList.SaintGrenade);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectWeapon(Enums.ItemsList.Banana);
        }    
       else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectWeapon(Enums.ItemsList.AirStrike);
        }       
       else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectWeapon(Enums.ItemsList.Teleportation);
        }       
       else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectWeapon(Enums.ItemsList.JetPack);
        }       
       else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectWeapon(Enums.ItemsList.FlameThrower);
        }
    }

    private void SetCharacterControlled(int _index)
    {
        currentCharacter = null;
        if (slimes.Count - 1 >= _index)
        {
            foreach (var item in slimes)
            {
                if (item.isControlled == true)
                {
                    item.isControlled = false;
                }
            }
            slimes[_index].isControlled = true;
            currentCharacter = slimes[_index];
        }
    }

    public void SelectWeapon(Enums.ItemsList _item)
    {
        itemSelected = _item;
    }

    private void UnSetCharacterControlled()
    {
        currentCharacter = null;
        foreach (var item in slimes)
        {
            if (item.isControlled == true)
                item.isControlled = false;
        }
    }

    private void PlaceCharacter()
    {
        if (slimes.Count < slimeLimit)
        {
            //Debug.Log(slimes.Count);
            slimes.Add(PhotonNetwork.Instantiate(slimePrefab.name, MousePos(), Quaternion.identity).GetComponent<Slime>());
            slimes[slimes.Count - 1].transform.parent = transform;
            slimes[slimes.Count - 1].SetPos(MousePos());
            slimes[slimes.Count - 1].team = team;
        }
    }

    private void ControlCharacter()
    {
        float move = 0;
        foreach (var item in slimes)
        {
            if (!item.isDead)
            {
                if (item.isControlled)
                {
                    move = Input.GetAxisRaw("Horizontal");
                    if (Input.GetKeyDown(KeyCode.UpArrow) && item.GetComponent<Slime>().isGrounded && !item.GetComponent<Slime>().isDead)
                        item.rb.velocity = new Vector2(0, item.jumpForce);
                }
                else
                    if (move != 0)
                    move = 0;
            }
            else
                if (move != 0)
                move = 0;

            item.velocity.x = Mathf.MoveTowards(item.velocity.x, item.maxSpeed * move, item.moveAcceleration * Time.deltaTime);
            item.rb.velocity = new Vector2(item.velocity.x, item.rb.velocity.y);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (currentCharacter != null)
            {
                timeToRelease = 0;
                UseItem(itemSelected);
            }
        }
    }

    void UseItem(Enums.ItemsList _itemSelected)
    {
        if (inv.items[_itemSelected].ammo > 0)
        {
            if (inv.items[_itemSelected].itemsList != Enums.ItemsList.RocketLauncher
                && inv.items[_itemSelected].itemsList != Enums.ItemsList.Grenade)
                inv.items[_itemSelected].ammo--;

            switch (inv.items[_itemSelected].type)
            {
                case Enums.Type.Weapon:
                    StartCoroutine(LaunchAttack(_itemSelected));
                    break;

                case Enums.Type.ChargableWeapon:
                    StartCoroutine(ChargeCalculation(_itemSelected));
                    break;

                case Enums.Type.Utility:
                    StartCoroutine(UsingUtilitary(_itemSelected));
                    break;

                default:
                    break;
            }
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    IEnumerator ChargeCalculation(Enums.ItemsList _attack)
    {
        if (currentCharacter != null)
        {
            Debug.Log("ChargeCalculation");
            while (!Input.GetMouseButtonUp(0))
            {
                timeToRelease += Time.deltaTime * 6f;
                yield return null;
            }
            charge = Mathf.Clamp(timeToRelease + 3, 3f, 20);
            StartCoroutine(LaunchExplosiveCharged(_attack, charge));
            timeToRelease = 0;
            charge = 0;
        }
        yield return null;
    }

    IEnumerator LaunchExplosiveCharged(Enums.ItemsList _attack, float _charge)
    {
        if (currentCharacter != null)
        {
            Debug.Log("LaunchAttackCharged");
            if (_attack == Enums.ItemsList.RocketLauncher
                || _attack == Enums.ItemsList.Grenade
                || _attack == Enums.ItemsList.SaintGrenade
                || _attack == Enums.ItemsList.Banana)
            {
                Vector3 targetPos = MousePos();
                targetPos.z = 0;
                Vector3 startPos = new Vector3(currentCharacter.transform.position.x + 1.5f, currentCharacter.transform.position.y, 0);

                Explosive explosive;
                explosive = PhotonNetwork.Instantiate(inv.itemPrefabs[(int)_attack].name, startPos, Quaternion.identity).GetComponent<Explosive>();
                explosive.shooter = currentCharacter.gameObject;
                explosive.startPos = startPos;
                explosive.targetPos = targetPos;
                explosive.charge = _charge;
            }
        }
        yield return null;
    }
    IEnumerator LaunchAttack(Enums.ItemsList _attack)
    {
        if (currentCharacter != null)
        {
            Debug.Log("LaunchAttack");
        }
        yield return null;
    }

    IEnumerator UsingUtilitary(Enums.ItemsList itemsList)
    { 
        yield return null;
    }
}
