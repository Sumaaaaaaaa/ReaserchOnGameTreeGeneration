using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    public class PruningTarget : MonoBehaviour
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
            print($"{Axis}...{Index}...{ratio}");
            Plant.Pruning(Axis,Index,ratio);
            PlantPruner.Generate(Plant);
            Renderer?.Render(Plant);
        }
    }
    public class PlantPruner : MonoBehaviour
    {
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
                    var xz = phy.Radius * 2;
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
                    var pt = go.AddComponent<PruningTarget>();
                    pt.Plant = plant;
                    pt.Axis = axis;
                    pt.Index = i;
                    pt.PlantPruner = this;
                    if (TryGetComponent<PlantRenderer>(out var renderer))
                    {
                        pt.Renderer = renderer;
                    }
                    
                    // 加入到存储集当中
                    _colliders.Add(go);
                    prePosition = phy.Position;
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