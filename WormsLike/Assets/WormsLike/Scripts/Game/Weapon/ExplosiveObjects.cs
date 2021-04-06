using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DTerrain
{
    public class ExplosiveObjects : MonoBehaviour
    {
        [Header("Explosion prefab")]
        [SerializeField] GameObject explosionPrefab = null;
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
                        Debug.Log("Explosion : after impact");
                        StartCoroutine(Explosion());
                        Destroy(gameObject);
                        myGo.Remove(gameObject);
                    }
                }
            }

            if (explodeVelocityNull == true)
            {
                if (rb.velocity == new Vector2(0f, 0f))
                {
                    Debug.Log("Explosion : on velocity null");
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    StartCoroutine(Explosion());
                    Debug.Log(gameObject.name);

                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Bullet"))
            {
                if (explodeAfterImpact == true && explodeVelocityNull == false)
                {
                    Debug.Log("Explosion : on trigger");
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    StartCoroutine(Explosion());
                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }

        public IEnumerator Explosion()
        {
            Debug.Log("Explosion");
            GameObject explosion = PhotonNetwork.Instantiate(explosionPrefab.name, gameObject.transform.position, Quaternion.identity);
            yield return null;
        }
    }
}
