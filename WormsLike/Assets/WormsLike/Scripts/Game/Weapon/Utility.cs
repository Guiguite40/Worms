using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public static void OpenParachute(Rigidbody2D rb, GameObject sp)
    {
        if (rb.velocity.y != 0)
        {
            rb.drag = 12;
            sp.GetComponent<SpriteRenderer>().color = new Color(sp.GetComponent<SpriteRenderer>().color.r, sp.GetComponent<SpriteRenderer>().color.g, sp.GetComponent<SpriteRenderer>().color.b, 1f);
        }
        else
        {
            rb.drag = 1;
            sp.GetComponent<SpriteRenderer>().color = new Color(sp.GetComponent<SpriteRenderer>().color.r, sp.GetComponent<SpriteRenderer>().color.g, sp.GetComponent<SpriteRenderer>().color.b, 0f);
        }
    }
}
