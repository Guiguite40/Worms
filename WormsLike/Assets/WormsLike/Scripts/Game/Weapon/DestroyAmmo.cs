using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class DestroyAmmo : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (MyExplosiveObjects.myGo != null)
            {
                for (int i = 0; i < MyExplosiveObjects.myGo.Count; i++)
                {
                    Vector3 screenPoint = mainCamera.WorldToViewportPoint(MyExplosiveObjects.myGo[i].transform.position);

                    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0;
                    if (onScreen == false)
                    {
                        Destroy(MyExplosiveObjects.myGo[i]);
                        MyExplosiveObjects.myGo.Remove(MyExplosiveObjects.myGo[i]);
                    }
                }
            }
        }
    }
}
