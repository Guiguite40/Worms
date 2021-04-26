using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static bool parachuteOpen = false;

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
        parachuteOpen = true;

        rb.drag = 12;
        sp.GetComponent<SpriteRenderer>().enabled = true;
    }
}
