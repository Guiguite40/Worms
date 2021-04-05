using UnityEngine;
using Photon.Pun;

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
            if (Input.GetKeyDown(KeyCode.B) && PhotonNetwork.IsMasterClient)
            {
                Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mPos.z = 0;
                PhotonNetwork.Instantiate("Weapon/" + weapon.name, mPos, new Quaternion(0, 0, 0, 0));
                Debug.Log("error 404");
            }
        }
    }
}