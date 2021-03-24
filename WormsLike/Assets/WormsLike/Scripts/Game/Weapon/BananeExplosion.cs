using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class BananeExplosion : MonoBehaviour
    {
        [Header("Prefab weapons")]
        [SerializeField]
        protected GameObject ChildBanane = null;

        public static bool firstExplode = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (firstExplode == true)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 mPos = gameObject.transform.position;
                    GameObject newBanana = Instantiate(ChildBanane, mPos, new Quaternion(0, 0, 0, 0));
                    newBanana.name = "BananaChilds";
                }

                firstExplode = false;
                Destroy(gameObject);
                MyExplosiveObjects.myGo.Remove(gameObject);
            }
        }
    }
}
