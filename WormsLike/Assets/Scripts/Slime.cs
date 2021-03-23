using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    [Header("Colliders")]
    [SerializeField] private GameObject feet = null;
    [SerializeField] private SpriteRenderer SpRenderer = null;
    [SerializeField] private ContactFilter2D filter; // ramps
    [SerializeField] private Rigidbody2D rb = null;

    /*** SPEED & VELOCITY ***/
    [Header("Speed & Velocity")]
    private bool isGrounded = false;
    [SerializeField] private float jumpForce = 0;
    [SerializeField] private float maxSpeed = 0;
    [SerializeField] private float moveAcceleration = 0;
    private Vector2 velocity = Vector2.zero;
    private Vector2 dir = Vector2.right;
    private float move = 0;

    /******** HEALTH ********/
    [Header("Health")]
    [SerializeField] private Text healthText = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Image healthBarDiff = null;
    public int maxHealth = 0;
    [SerializeField] private float curHealth = 0;
    float healthDisplayed = 0;
    float healthCd = 0;
    public bool isDead = false;
    [SerializeField] Color greenColor = Color.clear;
    [SerializeField] Color orangeColor = Color.clear;
    [SerializeField] Color redColor = Color.clear;

    [SerializeField] Color characterColor = Color.clear;
    public bool isControlled = false;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        healthDisplayed = curHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Health_Management();


        /* DEBUG */
        if (isControlled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                curHealth -= 10;
        }

        if (transform.position.y < -2)
        {
            curHealth = 0;
        }
    }

    void Movement()
    {
        if (!isDead)
        {
            if (isControlled)
            {
                move = Input.GetAxisRaw("Horizontal");
            }
            else
            {
                if (move != 0)
                    move = 0;
            }
        }
        else
        {
            if (move != 0)
                move = 0;
        }

        Flip();

        velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * move, moveAcceleration * Time.deltaTime);
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
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
        return transform.position;
    }

    private void Health_Management()
    {
        healthText.text = healthDisplayed.ToString("0");

        /* HEALTH */
        if (healthDisplayed < curHealth)
            healthCd += 0.001f * Time.deltaTime;
        else
            healthCd += 0.001f * Time.deltaTime;
        healthDisplayed = Mathf.Lerp(healthDisplayed, curHealth, healthCd);
        if (healthCd >= 1f)
            healthCd = 0f;
        healthBar.fillAmount = curHealth / maxHealth;
        healthBarDiff.fillAmount = healthDisplayed / maxHealth;

        /* COLORS */
        if (healthDisplayed >= 30 && healthBar.color != greenColor)
        {
            healthBar.color = greenColor;
        }
        else if (healthDisplayed < 30 && healthBar.color != redColor)
        {
            healthBar.color = redColor;
        }
        // Health diff color
        if (healthBarDiff.color != orangeColor)
        {
            healthBarDiff.color = orangeColor;
        }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
        else if (collision.gameObject.tag != "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "HealBox")
        {
            if (curHealth != maxHealth)
            {
                curHealth += 20;
                Destroy(collision.gameObject);
            }
        }
        if (collision.gameObject.tag == "DamageBox")
        {
            curHealth -= 25;
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}


