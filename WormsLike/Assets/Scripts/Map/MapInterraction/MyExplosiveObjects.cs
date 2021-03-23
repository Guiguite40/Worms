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
        protected bool explodeAfterImpact = false;
        [SerializeField]
        protected float delay = 0f;

        [Header("Prefab weapons")]
        [SerializeField]
        protected GameObject ChildBanane = null;

        private float timer;

        // Start is called before the first frame update
        void Start()
        {
            timer = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (explodeAfterImpact == false)
            {
                if (delay <= timer)
                {
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);

                    if (gameObject.name.Contains("FirstBanana"))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector3 mPos = gameObject.transform.position;
                            GameObject newBanana = Instantiate(ChildBanane, mPos, new Quaternion(0, 0, 0, 0));
                            newBanana.name = "BananaChilds";
                            // EXPLO FORCE
                        }

                        Destroy(gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.name.Contains("BananaChilds"))
            {
                if (explodeAfterImpact == true)
                {
                    MapDestroy.ExplosiveObjectsPosition.Add(gameObject.transform.position);
                    MapDestroy.ExplosiveObjectsSize.Add(circleSize);
                    Destroy(gameObject);
                }
            }
        }
    }
}
