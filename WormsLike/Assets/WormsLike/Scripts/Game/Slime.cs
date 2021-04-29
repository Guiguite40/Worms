using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Slime : MonoBehaviourPunCallbacks
{
    [Header("Colliders")]
    [SerializeField] private GameObject head = null;
    [SerializeField] private SpriteRenderer SpRenderer = null;
    [SerializeField] private ContactFilter2D filter; // ramps
    [SerializeField] public Rigidbody2D rb = null;

    /*** SPEED & VELOCITY ***/
    [Header("Speed & Velocity")]
    public bool isGrounded = false;
    [SerializeField] public float jumpForce = 0;
    [SerializeField] public float maxSpeed = 0;
    [SerializeField] public float moveAcceleration = 0;
    public Vector2 velocity = Vector2.zero;
    float fallingDamage = 0;
    float VyMaxBeforeDamage = 0;
    private Vector2 dir = Vector2.right;
    public float move = 0;

    /******** CHARGE ********/
    [SerializeField] public Image charge_ImgBG = null;
    [SerializeField] public Image chargeImg = null;
    public float charge = 0;
    public float chargeMax = 0;

    /******** HEALTH ********/
    [Header("Health")]
    [SerializeField] private Text healthText = null;
    public int maxHealth = 0;
    [SerializeField] private float curHealth = 0;
    float healthDisplayed = 0;
    float healthCd = 0;
    public bool isDead = false;
    public bool isInvicible = false;
    [SerializeField] Color blueColor = Color.clear;
    [SerializeField] Color greenColor = Color.clear;
    [SerializeField] Color orangeColor = Color.clear;
    [SerializeField] Color redColor = Color.clear;

    [SerializeField] Color characterColor = Color.clear;
    public bool isControlled = false;
    public int team = 0;

    [SerializeField] GameObject parachute = null;
    public bool parachuteOpen = false;
    public bool jetpackOn = false;
    public float timerJetpack = 0;
    public bool fallDamageActived = true;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        healthDisplayed = curHealth;

        if (team == 1)
        {
            healthText.color = blueColor;
        }
        else if (team == 2)
            healthText.color = redColor;
        else
            healthText.color = greenColor;
    }

    // Update is called once per frame
    void Update()
    {
        Flip();
        Health_Management();
        Charge_Management();
        FallDmg_Management();

        /* DEBUG */
        if (isControlled)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
                curHealth -= 10;
        }
        if (transform.position.y < -2)
        {
            curHealth = 0;
        }

        if (parachuteOpen == true)
        {
            parachute.SetActive(true);
        }

        if (isGrounded == true && parachuteOpen == true)
        {
            parachuteOpen = false;
            rb.drag = 0;
            parachute.SetActive(false);
            GetComponentInParent<Player>().SetHasAttacked(false);
        }

        if (Input.GetKey(KeyCode.Space) && jetpackOn == true)
        {
            timerJetpack += Time.deltaTime;
            rb.AddForce(new Vector2(0, 2f));
        }

        if (jetpackOn == true && timerJetpack >= 5f)
        {
            jetpackOn = false;
            GetComponentInParent<Player>().SetHasAttacked(false);
        }

        if (!isControlled)
            jetpackOn = false;
    }

    public void SetMove(float _move)
    {
        if (!isDead)
            if (isControlled && !isDead)
            {
                move = _move;
            }
    }

    public void Jump(bool _value)
    {
        if (_value)
        {
            if (isGrounded && !isDead)
            {
                rb.velocity = new Vector2(0, jumpForce);
            }
        }
    }

    private void FallDmg_Management()
    {
        if (fallDamageActived)
        {
            if (rb.velocity.y < -20)
            {
                fallingDamage = Mathf.Abs(rb.velocity.y) - 3;
            }
        }
    }

    private void Flip()
    {
        if (move == 1)
        {
            if (SpRenderer.flipX)
                SpRenderer.flipX = false;
            //if (transform.rotation.y != 0f)
            //    transform.rotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, transform.rotation.w);
        }
        else if (move == -1)
        {
            if (!SpRenderer.flipX)
                SpRenderer.flipX = true;
            //if (transform.rotation.y != 180f)
            //    transform.rotation = new Quaternion(transform.rotation.x, 180f, transform.rotation.z, transform.rotation.w);
        }
    }

    public void SetPos(Vector3 _pos)
    {
        transform.position = new Vector3(_pos.x, _pos.y, 0);
    }

    public void SetPos(Vector2 _pos)
    {
        transform.position = new Vector3(_pos.x, _pos.y, 0);
    }

    public Vector3 GetPos()
    {
        if (this == null)
        {
            DTerrain.GameManager.instance.SetIsPassTurn();
            return Vector3.zero;
        }
        else
            return transform.position;
    }

    private void Health_Management()
    {
        healthText.text = healthDisplayed.ToString("0");

        if (healthDisplayed < curHealth)
            healthCd += 0.001f * Time.deltaTime;
        else
            healthCd += 0.001f * Time.deltaTime;

        healthDisplayed = Mathf.Lerp(healthDisplayed, curHealth, healthCd);

        if (healthCd >= 1f)
            healthCd = 0f;

        if (curHealth <= 0)
        {
            if (curHealth != 0)
                curHealth = 0;
            if (!isDead)
                isDead = true;
        }
        else if (curHealth > 0)
        {
            if (curHealth > maxHealth)
                curHealth = maxHealth;
        }
    }

    private void Charge_Management()
    {
        if (charge > 0)
        {
            charge_ImgBG.gameObject.SetActive(true);
        }
        else if (charge <= 0)
        {
            charge_ImgBG.gameObject.SetActive(false);
        }

        chargeImg.fillAmount = charge / chargeMax;
    }

    public void Hit(float damage)
    {
        if (isInvicible)
        {
            isInvicible = false;
        }
        else
        {
            curHealth -= damage;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jumpable" && collision.gameObject != head.gameObject)
        {
            if (fallingDamage != 0)
            {
                Hit(fallingDamage);
                fallingDamage = 0;
            }
            isGrounded = true;
        }

        if (collision.gameObject.tag == "DamageBox")
        {
            Debug.Log("damage");
            Hit(25);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jumpable" && collision.gameObject != head.gameObject)
            isGrounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jumpable" && collision.gameObject != head.gameObject)
            isGrounded = true;

        if (collision.gameObject.tag == "HealBox")
        {
            if (curHealth != maxHealth)
            {
                Debug.Log("heal");
                curHealth += 20;
                Destroy(collision.gameObject);
            }
        }
    }
}


