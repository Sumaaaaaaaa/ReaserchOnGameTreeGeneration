using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    internal class Axis
    {
        public Vector3 Position { get; private set; }
        public Vector3 Direction{ get; private set; }
        public Vector3 SubDirection{ get; private set; }

        public void SetTopology(Vector3 position,Vector3 direction)
        {
            Position = position;
            Direction = direction;
            
            // 从 direction 计算出一个与它垂直且在水平面的向量
            SubDirection = new Vector3(direction.z, 0, -direction.x).normalized;

        }
        
        public float PhyllotaxisRotation = .0f;
        public int AxisOrder;
        
        [CanBeNull] public EntityBud ApicalBud;

        public List<EntityPhytomer> EntityPhytomers;

        public Axis(int axisOrder)
        {
            AxisOrder = axisOrder;
        }
    }
}