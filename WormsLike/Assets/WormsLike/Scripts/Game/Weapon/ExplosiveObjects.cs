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
        [SerializeField]
        protected float damage = 0f;
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
                    MapDestroy.ExplosiveObjectsDamage.Add(damage);

                    if (gameObject.name.Contains("FirstBanana"))
                    {
                        BananeExplosion.firstExplode = true;
                    }
                    else
                    {
                        //Debug.Log("Explosion : after impact");
                        RepulseObjects();

                        StartCoroutine(Explosion());
                        Destroy(gameObject);
                        myGo.Remove(gameObject);
                    }
                }
            }

            if (explodeVelocityNull == true)
            {
                if (rb.velocity.x <= 0.2f && rb.velocity.y <= 0.2f)
                {
                    //Debug.Log("Explosion : on velocity null");
                    RepulseObjects();

                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    MapDestroy.ExplosiveObjectsDamage.Add(damage);
                    StartCoroutine(Explosion());
                    Debug.Log(gameObject.name);

                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }

        public void RepulseObjects()
        {
            //Debug.LogError("RepulseObject Called");

            GameObject[] tmpSlimes;
            tmpSlimes = GameObject.FindGameObjectsWithTag("Player");
            //Debug.LogError("tmpSlimes count : " + tmpSlimes.Length);
            //tmpSlimes = GameManager.instance.GetEverySlimes();

            foreach (GameObject slime in tmpSlimes)
            {
                //Debug.LogError(Vector3.Distance(slime.transform.position, rb.transform.position));

                if (Vector3.Distance(slime.transform.position, rb.transform.position) <= 1 * 1.3f)
                {
                    slime.GetComponent<Rigidbody2D>().AddExplosionForce(30, rb.transform.position, (float)circleSize);

                    Vector2 explosionDir = slime.transform.position - rb.transform.position;
                    float explosionDistance = explosionDir.magnitude;
                    slime.GetComponent<Slime>().Hit((1 - (explosionDistance / (float)circleSize * 2f)) * damage);
                    //Debug.LogError(explosionDistance);
                    //Debug.LogError(circleSize);
                    //Debug.LogError((1 - (explosionDistance / (float)circleSize * 2f)) * damage);
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag("Bullet"))
            {
                if (explodeAfterImpact == true && explodeVelocityNull == false)
                {
                    //Debug.Log("Explosion : on trigger");
                    RepulseObjects();

                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    MapDestroy.ExplosiveObjectsDamage.Add(damage);
                    StartCoroutine(Explosion());

                    Destroy(gameObject);
                    myGo.Remove(gameObject);
                }
            }
        }

        public IEnumerator Explosion()
        {
            //Debug.Log("Explosion");
            GameObject explosion = PhotonNetwork.Instantiate(explosionPrefab.name, gameObject.transform.position, Quaternion.identity);
            yield return null;
        }
    }
}
