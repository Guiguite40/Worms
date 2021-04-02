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
            if (ExplosiveObjects.myGo != null)
            {
                for (int i = 0; i < ExplosiveObjects.myGo.Count; i++)
                {
                    Vector3 screenPoint = mainCamera.WorldToViewportPoint(ExplosiveObjects.myGo[i].transform.position);

                    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0;
                    if (onScreen == false)
                    {
                        Destroy(ExplosiveObjects.myGo[i]);
                        ExplosiveObjects.myGo.Remove(ExplosiveObjects.myGo[i]);
                    }
                }
            }

            foreach (GameObject gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (gameObj.name == "AirStrike(Clone)" && gameObj.transform.childCount == 0)
                {
                    Destroy(gameObj);
                }
            }
        }
    }
}
