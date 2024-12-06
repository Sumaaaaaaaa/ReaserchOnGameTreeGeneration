using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnlimitedGreen
{
    public enum LeafDirection
    {
        Up,
        Parallel,
        Axis
    }
    [RequireComponent(typeof(Transform),typeof(MeshFilter),typeof(MeshRenderer))]
    public class PlantRenderer : MonoBehaviour
    {
        [FormerlySerializedAs("axisMaterial")]
        [Header("Axis")]
        [SerializeField] private Material axisRoundMaterial;
        [SerializeField] private Material axisLidMaterial;
        [SerializeField][Range(3,64)] private int axisRes=3;
        [Header("Mesh")]
        [SerializeField] private Mesh leafMesh;
        [SerializeField] private Material leafMaterial;
        [SerializeField][Range(0.01f,10.0f)] private float leafSizeMul=1f;
        [SerializeField] private LeafDirection leafDirection;
        [Header("Flower")]
        [SerializeField] private Material flowerMaterial;
        [SerializeField] private Mesh flowerMesh;
        [SerializeField][Range(0.01f,10.0f)] private float flowerSizeMultiply = 1;
        [Header("Fruit")]
        [SerializeField] private Material fruitMaterial;
        [SerializeField] private Mesh fruitMesh;
        [SerializeField][Range(0.01f,10.0f)] private float fruitSizeMultiply=1;
        [Header("Gizmos")]
        [SerializeField] private float drawRadius;
        [SerializeField] private bool showData;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        // Render()
        public void Render(Plant plant)
        {
            //OPT: 关于这些List有什么办法给它一个预设长度，以解决扩大时产生的O(n)的计算量
            // Mesh相关的字段
            var vertices = new List<Vector3>(); // 顶点
            var normals = new List<Vector3>(); // 法线
            var uvs = new List<Vector2>(); // UV
            var roundFaces = new List<int>(); // 围绕面
            void AddRoundFace(int a, int b, int c, int d)
            {
                roundFaces.Add(a);
                roundFaces.Add(c);
                roundFaces.Add(b);

                roundFaces.Add(c);
                roundFaces.Add(d);
                roundFaces.Add(b);
            }// 遵从：a-↙, b-↘, c-↖, d-↗ 的规则
            var lidFaces = new List<int>();// 顶面
            void AddLidTriangle(int a, int b, int c)
            {
                lidFaces.Add(a);
                lidFaces.Add(b);
                lidFaces.Add(c);
            }
            
            // 叶相关
            var leafVertices = new List<Vector3>(); // 顶点
            var leafNormal = new List<Vector3>(); // 法线
            var leafUV = new List<Vector2>(); // UV
            var leafFaces = new List<int>(); // 面
            
            // 果相关
            var fruitVertices = new List<Vector3>();
            var fruitNormal = new List<Vector3>();
            var fruitUV = new List<Vector2>();
            var fruitFaces = new List<int>();
            
            // 花相关
            var flowerVertices = new List<Vector3>();
            var flowerNormal = new List<Vector3>();
            var flowerUV = new List<Vector2>();
            var flowerFaces = new List<int>();

            void Add<T>(EntityOrgan<T> entityOrgan,
                EntityPhytomer entityPhytomer,
                Vector3 phytomerDirection,
                Mesh mesh,
                float sizeMultiply,
                List<Vector3> verticesList,
                List<Vector3> normalList,
                List<Vector2> uvList)
            {
                float organRadius;
                if (entityOrgan is EntityFruit | entityOrgan is EntityFlower)
                {
                    organRadius = Mathf.Pow((entityOrgan.Biomass * 0.75f / Mathf.PI), 1f / 3f);
                }
                else if (entityOrgan is EntityLeaf)
                {
                    organRadius = Mathf.Sqrt(entityOrgan.Biomass / (4 * plant.LeafAllometryE));
                }
                else
                {
                    organRadius = 0;
                }
                
                var rotatedDirection = GenericFunctions.PhyllotaxisToVerticalDirection(entityOrgan.PhyllotaxisRotation,
                    entityPhytomer.Direction, entityPhytomer.SubDirection);
                
                Vector3 basePosition = entityPhytomer.Position + rotatedDirection * entityPhytomer.Radius;

                Vector3 newY = rotatedDirection;
                Vector3 newZ;
                
                if (entityOrgan is EntityFruit | entityOrgan is EntityFlower)
                {
                    newZ = phytomerDirection;
                }
                else if (entityOrgan is EntityLeaf)
                {
                    switch (leafDirection)
                    {
                        case LeafDirection.Up:
                            newZ = Vector3.up;
                            break;
                        case LeafDirection.Parallel:
                            newZ = phytomerDirection;
                            break;
                        case LeafDirection.Axis:
                            newZ = Vector3.Cross(phytomerDirection, rotatedDirection);
                            if (newZ.y < 0) newZ *= -1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    newZ = Vector3.zero;
                }
                
                Vector3 newX = Vector3.Cross(newZ,newY);

                Matrix4x4 transformMatrix = new Matrix4x4();
                transformMatrix.SetColumn(0, newX);
                transformMatrix.SetColumn(1, newY);
                transformMatrix.SetColumn(2, newZ);
                transformMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));
                
                // 顶点变换、添加
                foreach (var pos in mesh.vertices)
                {
                    var newPos = pos * organRadius; // 器官生物质缩放
                    newPos *= sizeMultiply; //  用户缩放
                    newPos = transformMatrix.MultiplyPoint(newPos); // 旋转
                    newPos += basePosition; // 移动
                    verticesList.Add(newPos);
                }
                
                // 法线 变换、添加
                foreach (var normal in mesh.normals)
                {
                    var newNormal = transformMatrix.MultiplyPoint(normal);
                    normalList.Add(newNormal);
                }
                
                // UV 添加
                uvList.AddRange(mesh.uv);
            }
            void ToTriangles(Mesh organMesh, List<Vector3> organVertices, List<int> organTriangles ,
                List<Vector3> baseVertices)
            {
                var organCount = organVertices.Count / organMesh.vertices.Length;
                var beginIndex = baseVertices.Count;
                for (var organIndex = 0; organIndex < organCount; organIndex++)
                {
                    foreach (var verticesIndex in organMesh.triangles)
                    {
                        organTriangles.Add(beginIndex + organIndex * organMesh.vertices.Length + verticesIndex);
                    }
                }
            }
            
            // 根据轴的叶元数量、（当前顶点Index、面解析度）预测性的进行面Index计算设置
            void PhytomerGenerateFace(int phytomerCount)
            {
                for (var i = 0; i < phytomerCount; i++)
                {
                    for (var j = 0; j < axisRes ; j++)
                    {
                        AddRoundFace(
                            a: vertices.Count+(axisRes+1)*i+j,
                            b: vertices.Count+(axisRes+1)*i+j+1,
                            c: vertices.Count+(axisRes+1)*(i+1)+j,
                            d: vertices.Count+(axisRes+1)*(i+1)+j+1
                            );
                    }
                }

                var finalIndex = vertices.Count + (phytomerCount + 2) * (axisRes + 1);
                for (var i = -1; i >= -axisRes; i--)
                {
                    AddLidTriangle(finalIndex + i, finalIndex + i - 1, finalIndex);
                }
            }
            
            // 根据方向、位置、半径 计算顶点
            void PhytomerAlVerticesPosition(Vector3 direction,Vector3 position, float radius,float V)
            {
                var result = new Vector3[axisRes+1];
                var quat = Quaternion.FromToRotation(Vector3.up, direction);
                for (var i = 0; i <= axisRes; i++)
                {
                    var ratio = (float)i / axisRes;
                    var alpha = ratio * Mathf.PI * 2f; // 角度
                    var normal = new Vector3(Mathf.Cos(alpha), 0, Mathf.Sin(alpha));
                    var pos = normal*radius;
                    pos = quat * pos;
                    pos += position;
                    normal = quat * normal;
                    
                    vertices.Add(pos);
                    normals.Add(normal);
                    uvs.Add(new Vector2(ratio,V));
                    
                    result[i] = pos;
                }
            }
            
            // 对轴处理方法
            void AxisProcess(Axis axis)
            {
                if (axis.EntityPhytomers.Count == 0) return;

                var vValue = 0.0f;

                PhytomerGenerateFace(axis.EntityPhytomers.Count);

                PhytomerAlVerticesPosition(
                    (axis.EntityPhytomers[0].Position - axis.Position).normalized,
                    axis.Position,
                    axis.EntityPhytomers[0].Radius,
                    vValue);

                var prePosition = axis.Position;
                
                // 遍历 Phytomer
                for (var index = 0; index < axis.EntityPhytomers.Count; index++)
                {
                    vValue += axis.EntityPhytomers[index].Length;
                    Vector3 phytomerDirection;
                    if (index + 1 == axis.EntityPhytomers.Count)
                    {
                        // 当前index所指的叶元是List中最后一个叶元
                        
                        phytomerDirection = (axis.EntityPhytomers[index].Position - prePosition).normalized;
                        
                        PhytomerAlVerticesPosition(
                            phytomerDirection,
                            axis.EntityPhytomers[index].Position,
                            axis.EntityPhytomers[index].Radius,
                            vValue
                        );
                        
                        var quat = Quaternion.FromToRotation(Vector3.up, axis.EntityPhytomers[index].Position - prePosition);
                        for (var i = 0; i <= axisRes; i++)
                        {
                            var ratio = (float)i / axisRes;
                            var alpha = ratio * Mathf.PI * 2f; // 角度
                            var normal = new Vector3(Mathf.Cos(alpha), 0, Mathf.Sin(alpha));
                            var pos = normal*axis.EntityPhytomers[index].Radius;
                            pos = quat * pos;
                            pos += axis.EntityPhytomers[index].Position;
                            var uv = new Vector2(normal.x, normal.z);
                            uv *= 0.5f;
                            uv += new Vector2(0.5f, 0.5f);
                    
                            vertices.Add(pos);
                            normals.Add(axis.EntityPhytomers[index].Direction);
                            uvs.Add(uv);
                        }
                        
                        vertices.Add(axis.EntityPhytomers[index].Position);
                        normals.Add(axis.EntityPhytomers[index].Direction);
                        uvs.Add(new Vector2(0.5f,0.5f));
                    }
                    else
                    
                    {
                        // 当前index所指的叶元是List中中间的叶元
                        
                        phytomerDirection = ((axis.EntityPhytomers[index].Position - prePosition).normalized +
                                             (axis.EntityPhytomers[index+1].Position - axis.EntityPhytomers[index].Position).normalized)
                            .normalized;
                        PhytomerAlVerticesPosition(phytomerDirection, axis.EntityPhytomers[index].Position,
                            axis.EntityPhytomers[index].Radius,vValue);
                        prePosition = axis.EntityPhytomers[index].Position;
                    }

                    foreach (var leaf in axis.EntityPhytomers[index].AxillaryLeaves)
                    {
                        Add(leaf,axis.EntityPhytomers[index],phytomerDirection,leafMesh,leafSizeMul,leafVertices,leafNormal,leafUV);
                    }

                    foreach (var fruit in axis.EntityPhytomers[index].AxillaryFruits)
                    {
                        Add(fruit,axis.EntityPhytomers[index],phytomerDirection,fruitMesh,fruitSizeMultiply,fruitVertices,fruitNormal,fruitUV);
                    }

                    foreach (var flower in axis.EntityPhytomers[index].AxillaryFlowers)
                    {
                        Add(flower,axis.EntityPhytomers[index],phytomerDirection,flowerMesh,flowerSizeMultiply,flowerVertices,flowerNormal,flowerUV);
                    }
                }
            }

            // 对轴进行处理
            foreach (var axis in plant._axisWithoutBud)
            {
                AxisProcess(axis);
            }
            foreach (var axis in plant._axisWithBud)
            {
                AxisProcess(axis);
            }
                
            //转为Mesh
            var mesh = new Mesh();
            mesh.subMeshCount = 5;
            
            // 叶
            ToTriangles(leafMesh,leafVertices,leafFaces,vertices);
            vertices.AddRange(leafVertices);
            normals.AddRange(leafNormal);
            uvs.AddRange(leafUV);
            
            // 果
            ToTriangles(fruitMesh,fruitVertices,fruitFaces,vertices);
            vertices.AddRange(fruitVertices);
            normals.AddRange(fruitNormal);
            uvs.AddRange(fruitUV);
            
            // 花
            ToTriangles(flowerMesh,flowerVertices,flowerFaces,vertices);
            vertices.AddRange(flowerVertices);
            normals.AddRange(flowerNormal);
            uvs.AddRange(flowerUV);
            
            mesh.SetVertices(vertices);
            
            mesh.SetTriangles(roundFaces,0);
            mesh.SetTriangles(lidFaces,1);
            mesh.SetTriangles(leafFaces,2);
            mesh.SetTriangles(fruitFaces,3);
            mesh.SetTriangles(flowerFaces,4);
            
            mesh.SetNormals(normals);
            mesh.SetUVs(0,uvs);
            
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            
            
            // 组件更新
            _meshFilter.mesh = mesh;
            _meshRenderer.materials = new[]
                { axisRoundMaterial, axisLidMaterial, leafMaterial, fruitMaterial, flowerMaterial };
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
                                            $"\nAxis_withBud={plant._axisWithBud.Count}\nAxis_noBud={plant._axisWithoutBud.Count}\n");
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

            foreach (var axis in plant._axisWithoutBud)
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