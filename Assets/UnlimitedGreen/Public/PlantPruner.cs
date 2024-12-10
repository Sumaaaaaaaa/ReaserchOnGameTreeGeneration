using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    public class PruningPhytomer : MonoBehaviour
    {
        internal Plant Plant;
        internal Axis Axis;
        internal int Index;
        [CanBeNull] internal PlantRenderer Renderer;
        internal PlantPruner PlantPruner;
        public void Pruning(Vector3 point)
        {
            var boxCollider = GetComponent<BoxCollider>();
            var bounds = boxCollider.bounds;
            var heightRange = bounds.max.y - bounds.min.y;
            var pointHeightRelativeToMin = point.y - bounds.min.y;
            var ratio = pointHeightRelativeToMin / heightRange;
            // print($"{Axis}...{Index}...{ratio}");
            Plant.Pruning(Axis,Index,ratio);
            PlantPruner.Generate(Plant);
            Renderer?.Render(Plant);
        }
    }

    public class PruningFruit : MonoBehaviour
    {
        internal Plant Plant;
        internal EntityPhytomer Phytomer;
        internal int Index;
        [CanBeNull] internal PlantRenderer Renderer;
        internal PlantPruner PlantPruner;

        public void Pruning()
        {
            Plant.Pruning(Phytomer,Index);
            PlantPruner.Generate(Plant);
            Renderer?.Render(Plant);
        }
    }
    public class PlantPruner : MonoBehaviour
    {
        [Range(0.01f,10f)]public float FruitSizeMul = 1.0f;
        [Range(0.01f,10f)]public float PhytomerRadiusMul = 1.0f;
        
        private readonly List<GameObject> _colliders = new List<GameObject>();

        public void Generate(Plant plant)
        {
            // 清空原先所有碰撞体
            foreach (var go in _colliders)
            {
                Destroy(go);
            }
            _colliders.Clear();
            
            // 遍历 植物所有轴 创建collider
            void CreateCollider(Axis axis)
            {
                var prePosition = axis.Position;
                for (var i = 0; i < axis.EntityPhytomers.Count; i++)
                {
                    var phy = axis.EntityPhytomers[i];
                    
                    // 计算出 collider 的位置
                    var pos = (prePosition + phy.Position) / 2f;
                    
                    // 计算出这个collider 的尺寸
                    var xz = phy.Radius * 2 * PhytomerRadiusMul;
                    var y = (phy.Position - prePosition).magnitude;
                    
                    // 计算出 collider 的旋转
                    var rot = Quaternion.FromToRotation(Vector3.up, phy.Direction);
                    
                    // 创建子GameObject
                    var go = new GameObject(phy.GetHashCode().ToString());
                    go.transform.parent = transform;
                    
                    // 设置GameObject的 transform
                    go.transform.localRotation = rot;
                    go.transform.localPosition = pos;
                    go.transform.localScale = new Vector3(xz, y, xz);
                    
                    // 为其创建BoxCollider并进行设置
                    go.AddComponent<BoxCollider>();
                    
                    // 创建数据容器
                    var pt = go.AddComponent<PruningPhytomer>();
                    pt.Plant = plant;
                    pt.Axis = axis;
                    pt.Index = i;
                    pt.PlantPruner = this;

                    TryGetComponent<PlantRenderer>(out var plantRenderer);
                    if (plantRenderer)
                    {
                        pt.Renderer = plantRenderer;
                    }
                    
                    // 加入到存储集当中
                    _colliders.Add(go);
                    prePosition = phy.Position;
                    
                    // 水果的部分
                    for (var j = 0; j < phy.AxillaryFruits.Length; j++)
                    {
                        var fruit = phy.AxillaryFruits[j];
                        // 计算半径
                        var radius = GenericFunctions.CalFlowerFruitRadius(fruit.Biomass) * FruitSizeMul;
                        // 计算位置
                        var direction = GenericFunctions.PhyllotaxisToVerticalDirection(fruit.PhyllotaxisRotation,
                            phy.Direction, phy.SubDirection);
                        var length = phy.Radius + radius;
                        var position = phy.Position + direction * length;
                        
                        //创建子GameObject
                        var fruitGo = new GameObject(fruit.GetHashCode().ToString());
                        
                        // Transform 设定
                        fruitGo.transform.parent = transform;
                        fruitGo.transform.localPosition = position;
                        
                        // 创建SphereCollider并进行设置
                        fruitGo.AddComponent<SphereCollider>().radius = radius;
                        
                        // 创建数据容器
                        var pf = fruitGo.AddComponent<PruningFruit>();
                        pf.Plant = plant;
                        pf.Phytomer = phy;
                        pf.Index = j;
                        pf.PlantPruner = this;
                        
                        if (plantRenderer)
                        {
                            pf.Renderer = plantRenderer;
                        }
                        _colliders.Add(fruitGo); 
                    }
                }
            }
            foreach (var axis in plant._axisWithBud)
            {
                CreateCollider(axis);
            }
            foreach (var axis in plant._axisWithoutBud)
            {
                CreateCollider(axis);
            }
        }
    }
}