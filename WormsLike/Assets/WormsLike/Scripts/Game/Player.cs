using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject slimePrefab = null;
    [SerializeField] List<GameObject> slimes = new List<GameObject>();
    Inventory inv = null;

    [HideInInspector] public int slimeLimit = 3;

    public int team = 0;

    public bool phase_placement = false;
    public bool phase_game = false;

    public bool isTurn = false;


    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        // DEBUG
        team = 1;
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
        else if (Input.GetKeyDown(KeyCode.Alpha0)) // Not available for now
        {
            GameObject healthBox = healthBoxPrefab;
            healthBox.transform.position = new Vector3(MousePos().x, MousePos().y, 0);
            Instantiate(healthBox);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) // Not available for now
        {
            GameObject damageBox = damageBoxPrefab;
            damageBox.transform.position = new Vector3(MousePos().x, MousePos().y, 0);
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
        if (slimes.Count - 1 >= _index)
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
    }

    private void PlaceCharacter()
    {
        if (slimes.Count < slimeLimit)
        {
            Debug.Log(slimes.Count);
            GameObject slime = slimePrefab;
            slime.GetComponent<Slime>().SetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            slimes.Add(Instantiate(slime));
            slimes[slimes.Count - 1].transform.parent = transform;
            slimes[slimes.Count - 1].GetComponent<Slime>().team = team;
        }
    }

    private void ControlCharacter()
    {
        float move = 0;
        foreach (var item in slimes)
        {
            if (!item.GetComponent<Slime>().isDead)
            {
                if (item.GetComponent<Slime>().isControlled)
                {
                    move = Input.GetAxisRaw("Horizontal");
                    if (Input.GetKeyDown(KeyCode.UpArrow) && item.GetComponent<Slime>().isGrounded && !item.GetComponent<Slime>().isDead)
                        item.GetComponent<Slime>().rb.velocity = new Vector2(0, item.GetComponent<Slime>().jumpForce); ;
                }
                else
                    if (move != 0)
                    move = 0;
            }
            else
                if (move != 0)
                move = 0;

            item.GetComponent<Slime>().velocity.x = Mathf.MoveTowards(item.GetComponent<Slime>().velocity.x, item.GetComponent<Slime>().maxSpeed * move, item.GetComponent<Slime>().moveAcceleration * Time.deltaTime);
            item.GetComponent<Slime>().rb.velocity = new Vector2(item.GetComponent<Slime>().velocity.x, item.GetComponent<Slime>().rb.velocity.y);
        }
    }

    Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
