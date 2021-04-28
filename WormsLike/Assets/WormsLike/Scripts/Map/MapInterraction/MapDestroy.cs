using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace DTerrain
{
    public class MapDestroy : MonoBehaviourPunCallbacks, IPunObservable
    {
        private int outlineSize = 4;

        protected Shape destroyCircle;
        protected Shape outlineCircle;

        [Header("LayerMap")]
        [SerializeField]
        protected BasicPaintableLayer primaryLayer;
        [SerializeField]
        protected BasicPaintableLayer secondaryLayer;

        public static List<Vector3> ExplosiveObjectsPosition = new List<Vector3>();
        public static List<int> ExplosiveObjectsSize = new List<int>();
        public static List<float> ExplosiveObjectsDamage = new List<float>();

        [Header("MapDeathMove")]
        [SerializeField]
        protected GameObject LeftMapKiller = null;
        [SerializeField]
        protected GameObject RightMapKiller = null;
        [SerializeField]
        protected GameObject LeftLaser = null;
        [SerializeField]
        protected GameObject RightLaser = null;

        private bool mapTurn = false;
        private bool shipDescent = false;
        private float leftPosX = 0f, RightPosX = 0f, PosY = 0f;

        void Start()
        {

        }

        void Update()
        {
            if (mapTurn == true)
            {
                MapMortSubite();
            }

            if (ExplosiveObjectsPosition.Count() != 0)
            {
                for (int i = 0; i < ExplosiveObjectsPosition.Count(); i++)
                {
                    if (PhotonNetwork.IsMasterClient)
                        photonView.RPC("MapSync", RpcTarget.AllBuffered, ExplosiveObjectsPosition[i], ExplosiveObjectsSize[i], ExplosiveObjectsDamage[i]);
                    else
                        photonView.RPC("MapSyncClient", RpcTarget.MasterClient, ExplosiveObjectsPosition[i], ExplosiveObjectsSize[i], ExplosiveObjectsDamage[i]);

                    ExplosiveObjectsPosition.RemoveAt(i);
                    ExplosiveObjectsSize.RemoveAt(i);
                }
            }
        }

        public void SetMortSubite()
		{
            if (mapTurn == false /*&& PhotonNetwork.IsMasterClient*/)
            {
                mapTurn = true;
                shipDescent = true;

                leftPosX = LeftMapKiller.transform.position.x;
                RightPosX = RightMapKiller.transform.position.x;

                PosY = LeftMapKiller.transform.position.y;

                photonView.RPC("SyncMortSubite", RpcTarget.AllBuffered);
            }
        }

        private void ZoneDamage(Vector3 pos, int size, float damage)
        {
            Vector2 myPos = new Vector2(pos.x, pos.y);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(myPos, size);
            foreach (Collider2D Collider in hitColliders)
            {
                if (Collider.gameObject.tag == "Player")
                {
                    //Debug.LogError(Collider.gameObject.GetPhotonView().ViewID);
                }
            }
        }

        private void DestroyMapCircle(Vector3 position, int size)
        {
            destroyCircle = Shape.GenerateShapeCircle(size);
            outlineCircle = Shape.GenerateShapeCircle(size + outlineSize);

            Vector3 p = position - primaryLayer.transform.position;

            primaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = new Vector2Int((int)(p.x * primaryLayer.PPU) - size, (int)(p.y * primaryLayer.PPU) - size),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.DESTROY
            });

            secondaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = new Vector2Int((int)(p.x * secondaryLayer.PPU) - size, (int)(p.y * secondaryLayer.PPU) - size),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.NONE
            });
        }

        private void DestroyMapRect(Vector3 position, int width, int height)
        {
            destroyCircle = Shape.GenerateShapeRect(width, height);
            outlineCircle = Shape.GenerateShapeRect(width, height);

            Vector3 p = position - primaryLayer.transform.position;

            primaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = new Vector2Int((int)(p.x * primaryLayer.PPU) - width, (int)(p.y * primaryLayer.PPU) - height),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.DESTROY
            });

            secondaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.clear,
                Position = new Vector2Int((int)(p.x * secondaryLayer.PPU) - width, (int)(p.y * secondaryLayer.PPU) - height),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.NONE
            });
        }

        public void MapMortSubite()
        {
            if (shipDescent == true)
            {
                if (PosY - 10f <= LeftMapKiller.transform.position.y)
                    LeftMapKiller.transform.position += Vector3.down * 5f * Time.deltaTime;

                if (PosY - 10f <= RightMapKiller.transform.position.y)
                    RightMapKiller.transform.position += Vector3.down * 5f * Time.deltaTime;

                if (PosY - 10f >= RightMapKiller.transform.position.y && PosY - 10f >= LeftMapKiller.transform.position.y)
                {
                    shipDescent = false;
                }
            }
            else
            {
                LeftLaser.SetActive(true);
                RightLaser.SetActive(true);

                if (leftPosX + 5f >= LeftMapKiller.transform.position.x)
                    LeftMapKiller.transform.position += Vector3.right * 5f * Time.deltaTime;

                if (RightPosX - 5f <= RightMapKiller.transform.position.x)
                    RightMapKiller.transform.position += Vector3.left * 5f * Time.deltaTime;

                photonView.RPC("MapSyncMortSubite", RpcTarget.AllBuffered, LeftMapKiller.transform.position, 10, 1080);
                photonView.RPC("MapSyncMortSubite", RpcTarget.AllBuffered, RightMapKiller.transform.position, 10, 1080);

                if (RightPosX - 5f >= RightMapKiller.transform.position.x && leftPosX + 5f <= LeftMapKiller.transform.position.x)
                {
                    LeftLaser.SetActive(false);
                    RightLaser.SetActive(false);

                    if (PosY >= LeftMapKiller.transform.position.y)
                        LeftMapKiller.transform.position += Vector3.up * 5f * Time.deltaTime;

                    if (PosY >= RightMapKiller.transform.position.y)
                        RightMapKiller.transform.position += Vector3.up * 5f * Time.deltaTime;

                    if (PosY <= RightMapKiller.transform.position.y && PosY <= LeftMapKiller.transform.position.y)
                    {
                        mapTurn = false;
                    }
                }
            }
        }

        [PunRPC]
        public void MapSync(Vector3 pos, int size, float dmg = 0.0F)
        {
            DestroyMapCircle(pos, size);
            ZoneDamage(pos, size, dmg);
        }

        [PunRPC]
        public void MapSyncClient(Vector3 pos, int size, float dmg = 0.0F)
        {
            photonView.RPC("MapSync", RpcTarget.AllBuffered, pos, size, dmg);
        }

        [PunRPC]
        public void MapSyncMortSubite(Vector3 pos, int width, int height)
        {
            DestroyMapRect(pos, width, height);
        }

        [PunRPC]
        public void SyncMortSubite()
        {
            mapTurn = true;
            shipDescent = true;

            leftPosX = LeftMapKiller.transform.position.x;
            RightPosX = RightMapKiller.transform.position.x;

            PosY = LeftMapKiller.transform.position.y;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }
    }
}
