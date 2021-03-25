using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class MapDestroy : MonoBehaviour
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
            if (Input.GetKeyDown(KeyCode.Space) && mapTurn == false)
            {
                mapTurn = true;
                shipDescent = true;

                leftPosX = LeftMapKiller.transform.position.x;
                RightPosX = RightMapKiller.transform.position.x;

                PosY = LeftMapKiller.transform.position.y;
            }

            if (mapTurn)
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

                    DestroyMapRect(LeftMapKiller.transform.position, 10, 1080);
                    DestroyMapRect(RightMapKiller.transform.position, 10, 1080);

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

            if (ExplosiveObjectsPosition.Count() != 0)
            {
                for (int i = 0; i < ExplosiveObjectsPosition.Count(); i++)
                {
                    destroyCircle = Shape.GenerateShapeCircle(ExplosiveObjectsSize[i]);
                    outlineCircle = Shape.GenerateShapeCircle(ExplosiveObjectsSize[i] + outlineSize);

                    DestroyMap(ExplosiveObjectsPosition[i], ExplosiveObjectsSize[i]);
                    ExplosiveObjectsPosition.RemoveAt(i);
                    ExplosiveObjectsSize.RemoveAt(i);
                }
            }
        }

        private void DestroyMap(Vector3 position, int size)
        {
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

        private void OnLeftMouseButtonClick()
        {
            destroyCircle = Shape.GenerateShapeCircle(16);
            outlineCircle = Shape.GenerateShapeCircle(16 + outlineSize);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DestroyMap(mousePos, 16);
        }
    }
}
