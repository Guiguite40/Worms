using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class MyExplosiveObjects : MonoBehaviour
    {
        [Header("Paramétres")]
        [SerializeField]
        protected int circleSize = 0;
        [SerializeField]
        protected bool explodeVelocityNull = false;
        [SerializeField]
        protected bool explodeAfterImpact = false;
        [SerializeField]
        protected float delay = 0f;       
        private float timer = 0f;

        private Rigidbody2D rb = null;
        public static List<GameObject> myGo = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0f, -1f); // Debug

            timer = 0f;          
            myGo.Add(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (explodeAfterImpact == false && explodeVelocityNull == false)
            {
                if (delay <= timer)
                {
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);

                    if (gameObject.name.Contains("FirstBanana"))
                    {
                        BananeExplosion.firstExplode = true;
                    }
                    else
                    {
                        Destroy(gameObject);
                        myGo.Remove(gameObject);
                    }
                }
            }

            if (explodeVelocityNull == true)
            {
                if (rb.velocity == new Vector2(0f,0f))
                {
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Bullet"))
            {
                if (explodeAfterImpact == true && explodeVelocityNull == false)
                {
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }
    }
}
