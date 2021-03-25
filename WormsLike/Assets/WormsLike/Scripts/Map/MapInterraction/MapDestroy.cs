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
        private float timer = 0f;
        [SerializeField]
        protected GameObject LeftMapKiller = null;
        [SerializeField]
        protected GameObject RightMapKiller = null;
        private bool mapTurn = false;
        private float leftPosX = 0f, RightPosX = 0f;

        void Start()
        {
            
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer > 5f && mapTurn == false)
            {
                mapTurn = true;

                leftPosX = LeftMapKiller.transform.position.x;
                RightPosX = RightMapKiller.transform.position.x;
            }

            if (mapTurn)
            {               
                if (leftPosX + 5f >= LeftMapKiller.transform.position.x)
                    LeftMapKiller.transform.position += Vector3.right * 5f * Time.deltaTime;

                if (RightPosX - 5f <= RightMapKiller.transform.position.x)
                    RightMapKiller.transform.position += Vector3.left * 5f * Time.deltaTime;

                //DestroyMapRect(LeftMapKiller.transform.position, 150, 1080);
                //DestroyMapRect(RightMapKiller.transform.position, 150, 1080);

                if (RightPosX - 5f >= RightMapKiller.transform.position.x && leftPosX + 5f <= LeftMapKiller.transform.position.x)
                {
                    timer = 0f;
                    mapTurn = false;
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

        private void DestroyMap(Vector3 position,int size)
        {
            Vector3 p =  position - primaryLayer.transform.position;

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
            outlineCircle = Shape.GenerateShapeRect(width, height + outlineSize);

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
