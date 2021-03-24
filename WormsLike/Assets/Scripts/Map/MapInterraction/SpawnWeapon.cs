using UnityEngine;

namespace DTerrain
{
    public class SpawnWeapon : MonoBehaviour
    {
        [SerializeField]
        private GameObject weapon = null;

        void Update()
        {
            CreateObjectMouse();
        }

        public void CreateObjectMouse()
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