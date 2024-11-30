using System;
using UnityEditor;
using UnityEngine;

namespace UnlimitedGreen
{
    [RequireComponent(typeof(Transform))]
    public class PlantRenderer : MonoBehaviour
    {
        // AXIS
        [SerializeField] private Material axisMaterial;
        [SerializeField][Range(3,64)] private int axisRes=3;
        // LEAF
        [SerializeField] private Material leafMaterial;
        [SerializeField][Range(0.01f,10.0f)] private float leafSizeMul=1f;
        // FLOWER
        [SerializeField] private Material flowerMaterial;
        [SerializeField] private Mesh flowerMesh;
        // FRUIT
        [SerializeField] private Material fruitMaterial;
        [SerializeField] private Mesh fruitMesh;
        // Gizmos
        [SerializeField] private float drawRadius;
        [SerializeField] private bool showData;
        
        // Render()
        public void Render(Plant plant)
        {
#if  UNITY_EDITOR
            // 数据检查      
            if (axisMaterial is null)
            {
                throw new Exception("ToMesh时'axisMaterial'参数不可以为空");
                // TODO: 但是如果我要的是无材质渲染呢？
                // TODO: 英文
            }
            if (leafMaterial is null)
            {
                throw new Exception("ToMesh时'leafMaterial'参数不可以为空");
            }
            if (plant.HasFlower & (flowerMaterial is null | flowerMesh is null))
            {
                throw new Exception("植物具有花数据，但是Render没有对花相关的Mesh和Material设定");
            }
            if (plant.HasFruit & (fruitMaterial is null | fruitMesh is null))
            {
                throw new Exception("植物具有果数据，但是Render没有对果相关的Mesh和Material设定");
            }
            //FIXME: 好像无效，可能因为Unity默认是有一个值在里面
#endif
            
            // 获取要创建的Mesh的顶点的数量等信息
            var sectionCount = 0;
            var transitionCount = 0;
            
            foreach (var axis in plant._axisWithBud)
            {
               
                if (axis.EntityPhytomers.Count == 0) continue;
                sectionCount += axis.EntityPhytomers.Count + 1;
                transitionCount += axis.EntityPhytomers.Count;
            }
            foreach (var axis in plant._axisNoBud)
            {
                if(axis.EntityPhytomers.Count == 0) continue;
                sectionCount += axis.EntityPhytomers.Count + 1;
                transitionCount += axis.EntityPhytomers.Count;
            }
            // 创建Mesh相关的字段
            var vertices = new Vector3[sectionCount * axisRes]; // 顶点
            var verticesIndex = 0;
            void AddVertex(Vector3 position)
            {
                vertices[verticesIndex] = position;
                verticesIndex++;
            }
            
            var uvs = new Vector2[vertices.Length]; // UV 
            
            var trangles = new int[transitionCount * axisRes * 6]; // 面
            var facesIndex = 0;
            // 遵从：a-↙, b-↘, c-↖, d-↗ 的规则
            void AddFace(int a, int b, int c, int d)
            {
                trangles[facesIndex] = a;
                facesIndex++;
                trangles[facesIndex] = c;
                facesIndex++;
                trangles[facesIndex] = b;
                facesIndex++;
                trangles[facesIndex] = c;
                facesIndex++;
                trangles[facesIndex] = d;
                facesIndex++;
                trangles[facesIndex] = b;
                facesIndex++;
            }
            
            // 根据轴的叶元数量、（当前顶点Index、面解析度）预测性的进行面Index计算设置
            void GenerateFace(int phytomerCount)
            {
                for (var i = 0; i < phytomerCount; i++)
                {
                    for (var j = 0; j < axisRes - 1; j++)
                    {
                        AddFace(
                            a: verticesIndex+axisRes*i+j,
                            b: verticesIndex+axisRes*i+j+1,
                            c: verticesIndex+axisRes*(i+1)+j,
                            d: verticesIndex+axisRes*(i+1)+j+1
                            );
                    }
                    AddFace(
                        a: verticesIndex+axisRes*(i+1)-1,
                        b: verticesIndex+axisRes*i,
                        c: verticesIndex+axisRes*(i+2)-1,
                        d: verticesIndex+axisRes*(i+1)
                        );
                }
            }
            
            // 根据方向、位置、半径 计算顶点
            Vector3[] CalVerticesPosition(Vector3 direction,Vector3 position, float radius)
            {
                var result = new Vector3[axisRes];
                var quat = Quaternion.FromToRotation(Vector3.up, direction);
                for (var i = 0; i < axisRes; i++)
                {
                    var alpha = ((float)i / axisRes) * Mathf.PI * 2f; // 角度
                    var pos = new Vector3(Mathf.Cos(alpha) * radius, 0, Mathf.Sin(alpha) * radius);
                    pos = quat * pos;
                    pos += position;
                    result[i] = pos;
                }
                return result;
            }
            
            // 对轴处理方法
            void AxisProcess(Axis axis)
            {
                if (axis.EntityPhytomers.Count == 0) return;

                GenerateFace(axis.EntityPhytomers.Count);
                
                foreach (
                    var vertex in CalVerticesPosition(
                        (axis.EntityPhytomers[0].Position - axis.Position).normalized,
                        axis.Position,
                        axis.EntityPhytomers[0].Radius)
                )
                {
                    AddVertex(vertex);
                }

                var prePosition = axis.Position;
                
                // 遍历 Phytomer
                for (var index = 0; index < axis.EntityPhytomers.Count; index++)
                {
                    if (index + 1 == axis.EntityPhytomers.Count)
                    {
                        // 当前index所指的叶元是List中最后一个叶元
                        
                        var results = CalVerticesPosition(
                            (axis.EntityPhytomers[index].Position - prePosition).normalized,
                            axis.EntityPhytomers[index].Position,
                            0
                            //axis.EntityPhytomers[index].Radius
                            );
                        foreach (var vertex in results)
                        {
                            AddVertex(vertex);
                        }
                    }
                    else
                    
                    {
                        // 当前index所指的叶元是List中中间的叶元
                        
                        var middleDirection = ((axis.EntityPhytomers[index].Position - prePosition).normalized +
                         (axis.EntityPhytomers[index+1].Position - axis.EntityPhytomers[index].Position).normalized)
                            .normalized;
                        var results = CalVerticesPosition(middleDirection, axis.EntityPhytomers[index].Position,
                            axis.EntityPhytomers[index].Radius);
                        foreach (var vertex in results)
                        {
                            AddVertex(vertex);
                        }

                        prePosition = axis.EntityPhytomers[index].Position;
                    }
                }
            }

            // 对轴进行处理
            foreach (var axis in plant._axisNoBud)
            {
                AxisProcess(axis);
            }
            foreach (var axis in plant._axisWithBud)
            {
                AxisProcess(axis);
            }
                
            //测试
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = trangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
        }
        
        // GizmosDraw()
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