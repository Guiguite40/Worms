using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosive : MonoBehaviour
{
    [SerializeField] bool airStrike = false;

    Rigidbody2D rb = null;
    [HideInInspector] public Vector3 startPos = Vector3.zero;
    [HideInInspector] public Vector3 targetPos = Vector3.zero;
    public float speed = 0;

    public float charge = 1;

    private Vector2 direction = Vector2.zero;
    private float angle = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (airStrike == false)
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            transform.position = startPos;
            direction = targetPos - transform.position;
            angle = Vector3.SignedAngle(Vector3.right, targetPos - transform.position, Vector3.forward);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            rb.AddForce(direction.normalized * charge, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        if (airStrike == false)
        {
            angle = Vector3.SignedAngle(Vector3.right, rb.velocity, Vector3.forward);
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}
