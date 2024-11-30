using UnityEditor;
using UnityEngine;

namespace UnlimitedGreen
{
    [RequireComponent(typeof(Transform))]
    public class PlantRenderer : MonoBehaviour
    {
        // AXIS
        [SerializeField] private Material axisMaterial;
        [SerializeField] private int axisRes;
        // LEAF
        [SerializeField] private Material leafMaterial;
        [SerializeField] private Mesh leafMesh;
        // FLOWER
        [SerializeField] private Material flowerMaterial;
        [SerializeField] private Mesh flowerMesh;
        // FRUIT
        [SerializeField] private Material fruitMaterial;
        [SerializeField] private Mesh fruitMesh;
        // Gizmos
        [SerializeField] private float drawRadius;
        [SerializeField] private bool showData;
        
        // private 
        
        // GizmosDraw
        public void GizmosDraw(Plant plant)
        {
            
            // 设置Gizmos的矩阵
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            
            // 画种子
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero,Vector3.one*drawRadius);
            if (showData) // 植物的数据
            {
                Handles.Label(Vector3.zero, $"Age={plant._age},\nBiomass={plant._biomassStorage}" +
                                            $"\nAxis_withBud={plant._axisWithBud.Count}\nAxis_noBud={plant._axisNoBud.Count}\n");
            }
            
            // 画各个的轴
            void DrawAxis(Axis axis,bool isLiveAxis)
            {
                // 画种子
                if (isLiveAxis) Gizmos.color = Color.green;
                else Gizmos.color = Color.red;
                Gizmos.DrawWireCube(axis.BudPosition, Vector3.one * drawRadius);
                
                var gizmosDrawer = new GizmosLineDrawer(axis.Position, drawRadius); // 实例化 
                
                // 画叶元
                foreach (var phytomer in axis.EntityPhytomers) // 遍历叶元
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(phytomer.Position,drawRadius/4);
                    if (phytomer.StoragePointer is not null)
                    {
                        // 还在期间内
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    gizmosDrawer.Draw(phytomer.Position); // 画叶元
                    if (showData)
                    {
                        Handles.Label(phytomer.Position,$"r={phytomer.Radius:F2}");
                    }

                    foreach (var entityFlower in phytomer.AxillaryFlowers)
                    {
                        var endPosition = GenericFunctions.PhyllotaxisToVerticalDirection(
                            entityFlower.PhyllotaxisRotation, phytomer.Direction, phytomer.SubDirection)
                            *drawRadius + phytomer.Position;
                        Gizmos.color = entityFlower.StoragePointer is not null ? Color.red : Color.magenta;
                        Gizmos.DrawLine(phytomer.Position,endPosition);
                        if (showData) Handles.Label(endPosition, $"b={entityFlower.Biomass:F2}");
                    }

                    foreach (var entityFruit in phytomer.AxillaryFruits)
                    {
                        var endPosition = GenericFunctions.PhyllotaxisToVerticalDirection(
                            entityFruit.PhyllotaxisRotation, phytomer.Direction, phytomer.SubDirection) 
                            * drawRadius + phytomer.Position;
                        Gizmos.color = entityFruit.StoragePointer is not null ? new Color(255, 127, 0) : Color.black;
                        Gizmos.DrawLine(phytomer.Position,endPosition);
                        if(showData)Handles.Label(endPosition,$"b={entityFruit.Biomass:F2}");
                    }

                    foreach (var leaf in phytomer.AxillaryLeaves)
                    {
                        var endPosition =
                            GenericFunctions.PhyllotaxisToVerticalDirection(leaf.PhyllotaxisRotation,
                                phytomer.Direction, phytomer.SubDirection) * drawRadius + phytomer.Position;
                        Gizmos.color = leaf.StoragePointer is not null ? Color.green : Color.gray;
                        Gizmos.DrawLine(phytomer.Position,endPosition);
                        if (showData) Handles.Label(endPosition, $"b={leaf.Biomass:F2}");
                    }
                }
            }
            
            foreach (var axis in plant._axisWithBud)
            {
                DrawAxis(axis,true);
            }

            foreach (var axis in plant._axisNoBud)
            {
                DrawAxis(axis,false);
            }
            
            // 恢复Gizmos的矩阵
            Gizmos.matrix = Matrix4x4.identity;
        }
        // Render
        private class GizmosLineDrawer
        {
            private Vector3 _prePosition;
            private Vector3 _postPosition;
            public GizmosLineDrawer(Vector3 beginPosition,float radius)
            {
                _postPosition = beginPosition;
            }

            public void Draw(Vector3 position)
            {
                _prePosition = _postPosition;
                _postPosition = position;
                Gizmos.DrawLine(_prePosition,_postPosition);
            }
        }
    }
}