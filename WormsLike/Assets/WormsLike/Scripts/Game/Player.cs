using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject slimePrefab = null;
    [SerializeField] List<Slime> slimes = new List<Slime>();
    [SerializeField] Inventory inv = null;

    [HideInInspector] public int slimeLimit = 3;
    public int team = 0;
    private string strTeam = "none";

    public bool phase_placement = false;
    public bool phase_game = false;
    public bool isTurn = false;
    Enums.ItemsList itemSelected = 0;

    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    [SerializeField] Slime currentCharacter = null;

    [SerializeField] float charge = 0;
    [SerializeField] float chargeMax = 10;
    [SerializeField] float speed = 0;

    float timeToRelease = 0;
    bool isAllSlimePlaced = false;
    bool hasAttacked = false;

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


        if (currentCharacter != null)
        {
            currentCharacter.charge = charge;
            if (currentCharacter.chargeMax != chargeMax)
                currentCharacter.chargeMax = chargeMax;
        }
    }

    void Debuging()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (PhotonNetwork.IsMasterClient)
                strTeam = "blue";
            else
                strTeam = "red";

            PlaceSlime();
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
            SelectWeapon(Enums.ItemsList.Parachute);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectWeapon(Enums.ItemsList.JetPack);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectWeapon(Enums.ItemsList.Shield);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectWeapon(Enums.ItemsList.SkipTurn);
        }
    }

    public void SetupPlayerState(string _currentTeam, int _nbCharacterLimit)
    {
        strTeam = _currentTeam;
        slimeLimit = _nbCharacterLimit;
    }

    public bool GetIsTurn()
    {
        return isTurn;
    }

    public void SetIsTurn(bool _turnState)
    {
        isTurn = _turnState;
    }

    public void SetCharacterControlled(int _index)
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

    public void UnSetCharacterControlled()
    {
        currentCharacter = null;
        foreach (var item in slimes)
        {
            if (item.isControlled == true)
                item.isControlled = false;
        }
    }

    public void PlaceSlime()
    {
        if (slimes.Count < slimeLimit)
        {
            if (photonView.IsMine == true)
            {
                GameObject newGoSlime = PhotonNetwork.Instantiate(slimePrefab.name, MousePos(), Quaternion.identity);
                int id = newGoSlime.GetPhotonView().ViewID;
                int parentId = gameObject.GetPhotonView().ViewID;

                int myTeam = 0;
                if (strTeam == "blue")
                    myTeam = 1;
                else if (strTeam == "red")
                    myTeam = 2;

                photonView.RPC("SpawnSlime", RpcTarget.AllBuffered, id, parentId, MousePos().x, MousePos().y, myTeam);
            }
        }
        else
            isAllSlimePlaced = true;
    }

    public bool GetAllSlimePlaced()
    {
        return isAllSlimePlaced;
    }

    public void SetAllSlimePlaced(bool _state)
    {
        isAllSlimePlaced = _state;
    }

    public void ControlCharacter()
    {
        if (photonView.IsMine == true)
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

            if (!hasAttacked)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (currentCharacter != null)
                    {
                        timeToRelease = 0;
                        UseItem(itemSelected);
                    }
                }
            }
        }
    }

    public bool GetHasAttacked()
    {
        return hasAttacked;
    }

    public void SetHasAttacked(bool _state)
    {
        hasAttacked = _state;
    }

    void UseItem(Enums.ItemsList _itemSelected)
    {
        if (inv.IsItemUseable(_itemSelected))
        {
            inv.UseItem(_itemSelected);

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

            hasAttacked = true;
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    Vector3 MousePos()
    {
        Vector3 tmp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tmp.z = 0;
        return tmp;
    }

    IEnumerator ChargeCalculation(Enums.ItemsList _attack)
    {
        if (currentCharacter != null)
        {
            Debug.Log("ChargeCalculation");
            while (!Input.GetMouseButtonUp(0))
            {
                timeToRelease += Time.deltaTime * 5f;
                charge = timeToRelease;
                if (charge > chargeMax)
                    break;
                yield return null;
            }
            charge = Mathf.Clamp(timeToRelease + 3, 3f, chargeMax);
            StartCoroutine(LaunchExplosiveCharged(_attack, charge));
            timeToRelease = 0;
            charge = 0;
        }
        yield return null;
    }

    IEnumerator LaunchExplosiveCharged(Enums.ItemsList _item, float _charge)
    {
        if (currentCharacter != null)
        {
            Debug.Log("LaunchAttackCharged");
            //if (_item == Enums.ItemsList.RocketLauncher
            //    || _item == Enums.ItemsList.Grenade
            //    || _item == Enums.ItemsList.SaintGrenade
            //    || _item == Enums.ItemsList.Banana)
            //{
            Vector3 targetPos = MousePos();
            targetPos.z = 0;
            Vector3 startPos = new Vector3(currentCharacter.transform.position.x, currentCharacter.transform.position.y, 0);
            if (MousePos().x < currentCharacter.transform.position.x)
                startPos.x -= 1.5f;
            else
                startPos.x += 1.5f;

            Explosive explosive;
            explosive = PhotonNetwork.Instantiate(inv.itemPrefabs[(int)_item].name, startPos, Quaternion.identity).GetComponent<Explosive>();
            explosive.shooter = currentCharacter.gameObject;
            explosive.startPos = startPos;
            explosive.targetPos = targetPos;
            explosive.charge = _charge;
            //}
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

    IEnumerator UsingUtilitary(Enums.ItemsList _utilitary)
    {
        if (currentCharacter != null)
        {
            if (_utilitary == Enums.ItemsList.AirStrike)
            {
                Vector3 startPos = MousePos();
                startPos.y = 25;
                startPos.z = 0;

                Explosive utilitary = PhotonNetwork.Instantiate(inv.itemPrefabs[(int)_utilitary].name, startPos, Quaternion.identity).GetComponent<Explosive>();
                utilitary.startPos = startPos;
            }

            else if (_utilitary == Enums.ItemsList.Teleportation)
            {
                currentCharacter.SetPos(MousePos());
            }
        }
        yield return null;
    }

    [PunRPC]
    public void SpawnSlime(int id, int parentId, float posX, float posY, int team)
    {
        GameObject newGoSlime = PhotonView.Find(id).gameObject;

        Slime newSlime = newGoSlime.GetComponent<Slime>();
        newSlime.transform.parent = PhotonView.Find(parentId).gameObject.transform;
        newSlime.SetPos(new Vector3(posX, posY, 0f));
        newSlime.team = team;
        slimes.Add(newSlime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}