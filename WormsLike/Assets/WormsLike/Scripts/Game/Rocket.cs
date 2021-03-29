using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [HideInInspector] public GameObject shooter = null;
    Rigidbody2D rb = null;
    [HideInInspector] public Vector3 startPos = Vector3.zero;
    [HideInInspector] public Vector3 targetPos = Vector3.zero;
    public float speed = 0;

    private Vector2 direction = Vector2.zero;
    private float angle = 0;

    [HideInInspector] public Vector2 strenght = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();

        transform.position = startPos;
        direction = targetPos - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

        rb.AddForce(targetPos * speed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        //rb.velocity = transform.up * strenght;

        if (transform.position.y < -5)
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Jumpable")
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Jumpable")
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
    }
}
