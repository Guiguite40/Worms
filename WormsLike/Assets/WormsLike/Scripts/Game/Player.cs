using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject slimePrefab = null;
    [SerializeField] public List<Slime> slimes = new List<Slime>();
    [SerializeField] Inventory inv = null;
    [SerializeField] GameObject teleportationPS = null;

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

    bool actionDone = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Slime slime in slimes)
        {
            if (slime.GetComponent<Slime>().isDead)
            {
                slimes.Remove(slime);
                Destroy(slime);
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

        if (UI.Instance.inventoryOpened)
        {
            UI.Instance.ActualizeAmmoCount(inv);
        }


        if (itemSelected != UI.Instance.GetItemSelected())
            itemSelected = UI.Instance.GetItemSelected();
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
            foreach (Slime slime in slimes)
            {
                if (slime.isControlled == true)
                {
                    slime.isControlled = false;
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
        foreach (Slime slime in slimes)
        {
            if (slime.isControlled == true)
                slime.isControlled = false;
        }
    }

    public Slime GetCurrentCharacter()
    {
        return currentCharacter;
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

            foreach (Slime slime in slimes)
            {
                if (!slime.isDead)
                {
                    if (slime.isControlled)
                    {
                        move = Input.GetAxisRaw("Horizontal");
                        if (Input.GetKeyDown(KeyCode.UpArrow) && slime.GetComponent<Slime>().isGrounded && !slime.GetComponent<Slime>().isDead)
                            slime.rb.velocity = new Vector2(0, slime.jumpForce);

                        if (UI.Instance.isItemSelected)
                        {
                            if (!hasAttacked)
                            {
                                if (UI.Instance.inventoryOpened == false)
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
                    }
                    else
                        if (move != 0)
                        move = 0;
                }
                else
                    if (move != 0)
                    move = 0;

                slime.velocity.x = Mathf.MoveTowards(slime.velocity.x, slime.maxSpeed * move, slime.moveAcceleration * Time.deltaTime);
                slime.rb.velocity = new Vector2(slime.velocity.x, slime.rb.velocity.y);
            }

            if (!hasAttacked)
            {
                if (UI.Instance.inventoryOpened == false)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (currentCharacter != null && currentCharacter.parachuteOpen == false)
                        {
                            timeToRelease = 0;
                            UseItem(itemSelected);
                        }
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
        if (!actionDone)
        {
            if (inv.IsItemUseable(_itemSelected))
            {
                inv.UseItem(_itemSelected);

                switch (inv.items[_itemSelected].type)
                {
                    case Enums.Type.Weapon:
                        StartCoroutine(LaunchAttack(_itemSelected));
                        hasAttacked = true;

                        break;

                    case Enums.Type.ChargableWeapon:
                        StartCoroutine(ChargeCalculation(_itemSelected));
                        hasAttacked = true;

                        break;

                    case Enums.Type.Utility:
                        StartCoroutine(UsingUtilitary(_itemSelected));
                        break;

                    default:
                        break;
                }
            }
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
        if (photonView.IsMine)
        {
            if (currentCharacter != null)
            {
                Vector3 targetPos = MousePos();
                targetPos.z = 0;
                Vector3 startPos = new Vector3(currentCharacter.transform.position.x, currentCharacter.transform.position.y, 0);
                if (MousePos().x < currentCharacter.transform.position.x)
                    startPos.x -= 0.5f;
                else
                    startPos.x += 0.5f;

                GameObject explosive = PhotonNetwork.Instantiate(inv.itemPrefabs[(int)_item].name, startPos, Quaternion.identity);
                int idweapon = explosive.GetPhotonView().ViewID;

                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC("SpawnWeapon", RpcTarget.AllBuffered, idweapon, startPos.x, startPos.y, targetPos.x, targetPos.y, _charge);
                else
                    photonView.RPC("SpawnWeaponClient", RpcTarget.MasterClient, idweapon, startPos.x, startPos.y, targetPos.x, targetPos.y, _charge);


                EndTurn(); // End turn call
            }
            yield return null;
        }
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
        if (photonView.IsMine)
        {
            if (currentCharacter != null)
            {
                if (_utilitary == Enums.ItemsList.AirStrike)
                {
                    Vector3 startPos = MousePos();
                    startPos.y = 25;
                    startPos.z = 0;

                    GameObject utilitary = PhotonNetwork.Instantiate(inv.itemPrefabs[(int)_utilitary].name, startPos, Quaternion.identity);
                    int idutilitary = utilitary.GetPhotonView().ViewID;

                    photonView.RPC("SpawnUtility", RpcTarget.AllBuffered, idutilitary, startPos.x, startPos.y);
                    hasAttacked = true;

                    EndTurn(); // End turn call
                }
                else if (_utilitary == Enums.ItemsList.Teleportation)
                {
                    StartCoroutine(Teleportation());
                }
                else if (_utilitary == Enums.ItemsList.Parachute)
                {
                    int idSlime = currentCharacter.gameObject.GetPhotonView().ViewID;
                    photonView.RPC("DisplayParachute", RpcTarget.AllBuffered, idSlime);
                }
                else if (_utilitary == Enums.ItemsList.SkipTurn)
                {
                    EndTurn(); // End turn call
                }
            }
        }
        yield return null;
    }

    IEnumerator Teleportation()
    {
        GameObject ps = PhotonNetwork.Instantiate("PS/" + teleportationPS.name, currentCharacter.transform.position, teleportationPS.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        currentCharacter.SetPos(MousePos());
        GameObject ps1 = PhotonNetwork.Instantiate("PS/" + teleportationPS.name, currentCharacter.transform.position, teleportationPS.transform.rotation);
        EndTurn(); // End turn call
        yield return null;
        hasAttacked = true;
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

    [PunRPC]
    public void SpawnWeapon(int idWeapon, float posX, float posY, float targetPosX, float targetPosY, float _charge)
    {
        GameObject newGoExplosive = PhotonView.Find(idWeapon).gameObject;

        Explosive explosive = newGoExplosive.GetComponent<Explosive>();

        explosive.startPos = new Vector3(posX, posY, 0f);
        explosive.targetPos = new Vector3(targetPosX, targetPosY, 0f);
        explosive.charge = _charge;
    }

    [PunRPC]
    public void SpawnWeaponClient(int idWeapon, float posX, float posY, float targetPosX, float targetPosY, float _charge)
    {
        photonView.RPC("SpawnWeapon", RpcTarget.AllBuffered, idWeapon, posX, posY, targetPosX, targetPosY, _charge);
    }

    [PunRPC]
    public void DisplayParachute(int idSlime)
    {
        GameObject mySlime = PhotonView.Find(idSlime).gameObject;
        Slime curSlime = mySlime.GetComponent<Slime>();
        curSlime.parachuteOpen = true;
        curSlime.rb.drag = 12;
    }

    [PunRPC]
    public void SpawnUtility(int idutilitary, float posX, float posY)
    {
        GameObject newGoUtilitary = PhotonView.Find(idutilitary).gameObject;

        Explosive utilitary = newGoUtilitary.GetComponent<Explosive>();

        utilitary.startPos = new Vector3(posX, posY, 0f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public void EndTurn()
    {
        UI.Instance.CloseInventory();
        UI.Instance.isItemSelected = false;
        itemSelected = 0;
        UI.Instance.SetCursor(Enums.CursorType.Normal);
    }
}