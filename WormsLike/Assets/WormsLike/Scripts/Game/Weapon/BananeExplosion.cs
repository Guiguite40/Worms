using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
                if (PhotonNetwork.IsMasterClient)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 mPos = gameObject.transform.position;
                        GameObject newBanana = PhotonNetwork.InstantiateRoomObject("Weapon/" + ChildBanane.name, mPos, new Quaternion(0, 0, 0, 0));
                        newBanana.name = "BananaChilds";
                    }
                }

                firstExplode = false;
                Destroy(gameObject);
                MyExplosiveObjects.myGo.Remove(gameObject);
            }
        }
    }
}
