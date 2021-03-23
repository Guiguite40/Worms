using UnityEngine;

namespace DTerrain
{
    public class MouseSpawn : MonoBehaviour
    {
        [SerializeField]
        private GameObject weapon = null;

        void Update()
        {
            CreateObject();
        }

        public void CreateObject()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mPos.z = 0;
                Instantiate(weapon, mPos, new Quaternion(0, 0, 0, 0));
            }
        }
    }
}