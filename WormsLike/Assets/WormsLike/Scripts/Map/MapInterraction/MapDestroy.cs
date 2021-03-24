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

        [SerializeField]
        protected BasicPaintableLayer primaryLayer;
        [SerializeField]
        protected BasicPaintableLayer secondaryLayer;

        public static List<Vector3> ExplosiveObjectsPosition = new List<Vector3>();
        public static List<int> ExplosiveObjectsSize = new List<int>();

        void Start()
        {
            
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                OnLeftMouseButtonClick();
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

        private void OnLeftMouseButtonClick()
        {
            destroyCircle = Shape.GenerateShapeCircle(16);
            outlineCircle = Shape.GenerateShapeCircle(16 + outlineSize);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DestroyMap(mousePos, 16);
        }
    }
}
