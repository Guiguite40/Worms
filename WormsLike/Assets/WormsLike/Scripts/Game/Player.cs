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

    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    [SerializeField] Slime currentCharacter = null;

    [SerializeField] float charge = 0;
    [SerializeField] float speed = 0;

    float timeToRelease = 0;
    private int myteam;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("player start");
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            myteam = 1;
        else
            myteam = 2;

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
            if (photonView.IsMine == true)
            {
                PlaceCharacter();
            }
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
            SetCharacterControlled(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCharacterControlled(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCharacterControlled(2);
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
            slimes.Add(PhotonNetwork.Instantiate(slimePrefab.name, MousePos(), Quaternion.identity).GetComponent<Slime>());
            slimes[slimes.Count - 1].transform.parent = transform;
            slimes[slimes.Count - 1].SetPos(MousePos());
            slimes[slimes.Count - 1].team = myteam;
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
            timeToRelease = 0;
            UseItem(Enums.ItemsList.RocketLauncher);
        }
    }

    void UseItem(Enums.ItemsList _itemSelected)
    {
        if (_itemSelected == Enums.ItemsList.RocketLauncher)
        {
            StartCoroutine(ChargeCalculation(_itemSelected));
        }
    }

    Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    IEnumerator ChargeCalculation(Enums.ItemsList _attack)
    {
        while (!Input.GetMouseButtonUp(0))
        {
            timeToRelease += Time.deltaTime * 6f;
            yield return null;
        }
        charge = Mathf.Clamp(timeToRelease + 3, 3f, 20);
        StartCoroutine(LaunchAttackCharged(_attack, charge));
        timeToRelease = 0;
        charge = 0;

        yield return null;
    }

    IEnumerator LaunchAttackCharged(Enums.ItemsList _attack, float _charge)
    {
        if (currentCharacter != null)
        {
            if (_attack == Enums.ItemsList.RocketLauncher)
            {
                Vector3 targetPos = MousePos();
                targetPos.z = 0;

                Rocket rocket = PhotonNetwork.Instantiate(inv.RocketPrefab.name, currentCharacter.transform.position, Quaternion.identity).GetComponent<Rocket>();
                rocket.shooter = currentCharacter.gameObject;
                rocket.startPos = currentCharacter.transform.position;
                rocket.targetPos = targetPos;
                rocket.charge = _charge;
            }
        }
        yield return null;
    }
    IEnumerator LaunchAttack(Enums.ItemsList _attack)
    {
        Debug.Log("LaunchAttack");
        if (currentCharacter != null)
        {
        }
        yield return null;
    }
}
