using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject slimePrefab = null;
    [SerializeField] List<GameObject> slimes = new List<GameObject>();
    [SerializeField] Inventory inv = null;

    [HideInInspector] public int slimeLimit = 3;

    public int team = 0;

    public bool phase_placement = false;
    public bool phase_game = false;

    public bool isTurn = false;

    /***** DEBUG *****/
    [SerializeField] GameObject healthBoxPrefab = null;
    [SerializeField] GameObject damageBoxPrefab = null;

    [SerializeField] GameObject currentCharacter = null;

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
        currentCharacter = null;
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
            currentCharacter = slimes[_index].gameObject;
        }
    }

    private void PlaceCharacter()
    {
        if (slimes.Count < slimeLimit)
        {
            Debug.Log(slimes.Count);
            GameObject slime = slimePrefab;
            slime.GetComponent<Slime>().team = team;
            slime.GetComponent<Slime>().SetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            slimes.Add(Instantiate(slime));
            slimes[slimes.Count - 1].transform.parent = transform;
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

    void Rocket()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentCharacter != null)
            {
                Debug.Log("Shooooott");
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0;

                GameObject rocket = inv.RocketPrefab;
                rocket.GetComponent<Rocket>().shooter = currentCharacter.gameObject;
                rocket.GetComponent<Rocket>().startPos = currentCharacter.transform.position;
                Debug.Log(currentCharacter.transform.position);
                rocket.GetComponent<Rocket>().targetPos = targetPos;
                rocket.GetComponent<Rocket>().strenght = strenght;
                Instantiate(rocket);
            }
        }
    }

    Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
