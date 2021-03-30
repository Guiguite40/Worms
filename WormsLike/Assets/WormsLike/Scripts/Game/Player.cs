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

    bool isShooting = false;

    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    [SerializeField] Slime currentCharacter = null;

    [SerializeField] Vector2 strenght = Vector2.zero;
    [SerializeField] float speed = 0;

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
        ControlCharacter();
        Rocket();
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
            //Debug.Log(slimes.Count);
            slimes.Add(Instantiate(slimePrefab).GetComponent<Slime>());
            slimes[slimes.Count - 1].team = team;
            slimes[slimes.Count - 1].SetPos(MousePos());
            slimes[slimes.Count - 1].transform.parent = transform;
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
    }

    void Rocket()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            if (currentCharacter != null)
            {
                Vector3 targetPos = MousePos();
                targetPos.z = 0;

                Rocket rocket = Instantiate(inv.RocketPrefab).GetComponent<Rocket>();
                rocket.shooter = currentCharacter.gameObject;
                rocket.startPos = currentCharacter.transform.position;
                rocket.targetPos = targetPos;
                rocket.strenght = strenght;
            }
        }
    }

    Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    IEnumerator ShootStrenght()
    {
        //while (!Input.GetMouseButtonUp(0)) { }

        //yield return WaitWhile(!Input.GetMouseButtonUp(0));


        yield return null;


    }
}
