using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class MyExplosiveObjects : MonoBehaviour
    {
        [SerializeField]
        protected int circleSize = 0;
        [SerializeField]
        protected bool explodeAfterImpact = false;
        [SerializeField]
        protected float delay = 0f;

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
                    Destroy(gameObject);
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
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
