using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananeChildDispersion : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(new Vector2(Random.Range(-3, 3), Random.Range(5, 10)), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
