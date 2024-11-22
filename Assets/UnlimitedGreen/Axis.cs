using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    internal class Axis
    {
        [CanBeNull] public EntityBud ApicalBud;
        public List<EntityPhytomer> EntityPhytomers = new List<EntityPhytomer>();
        public Vector3 Position { get; private set; }
        public Vector3 Direction{ get; private set; }
        public Vector3 SubDirection{ get; private set; }

        public float PhyllotaxisRotation = .0f;
        public int AxisOrder;
        
        public void SetTopology(Vector3 position,Vector3 direction)
        {
            Position = position;
            Direction = direction;
            
            // 从 direction 计算出一个与它垂直且在水平面的向量
            SubDirection = new Vector3(direction.z, 0, -direction.x).normalized;

        }
        
        


        public Axis(int axisOrder,EntityBud entityBud,float phyllotaxisRotation)
        {
            AxisOrder = axisOrder;
            ApicalBud = entityBud;
            Direction = new Vector3(phyllotaxisRotation, 0, 0);
        }
        
        // 获取芽应该在的位置
        public Vector3 BudPosition => EntityPhytomers.Count == 0 ? Position : EntityPhytomers[^1].Position;
    }
}