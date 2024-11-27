using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    /// <summary>
    /// 描述叶元上本叶轴节点所带器官,
    /// None - 无携带器官， Fruit - 携带水果 ， Flower - 携带花朵。
    /// Bud - 携带侧芽
    /// </summary>
    public enum BeerOrgan
    {
        None,
        Fruit,
        Flower,
        Bud
    }
    
    /// <summary>
    /// 描述叶元上本叶轴节点的参数
    /// </summary>
    public struct Phyllotaxis
    {
        public float Rotation;
        public bool HasLeaf;
        public BeerOrgan BeerOrgan;
        public int AddPhysiologicalAge;
        public Phyllotaxis(float rotation, bool hasLeaf,BeerOrgan beerOrgan,int addPhysiologicalAge = 0)
        {
            if (rotation == 0)
            {
                throw new ArgumentException("'rotation' value cannot be zero. It must create a valid rotation.");
            }
            Rotation = rotation;
            HasLeaf = hasLeaf;
            BeerOrgan = beerOrgan;
            AddPhysiologicalAge = addPhysiologicalAge;
        }
    }
    public class Phytomer
    {
        internal readonly float PhyllotaxisRandomValue;
        internal readonly Phyllotaxis[] Phyllotaxis;
        
        /// <summary>
        /// 定义叶元
        /// </summary>
        /// <param name="phyllotaxisRandomValue">对于所有的出现在植物上的叶轴的旋转量做出随机量定义</param>
        /// <param name="phyllotaxis">叶轴定义</param>
        /// <exception cref="ArgumentException">定义不符合要求</exception>
        public Phytomer(float phyllotaxisRandomValue,[NotNull] Phyllotaxis[] phyllotaxis)
        {
            
            #if UNITY_EDITOR
            // 'phyllotaxisRandomValue'旋转随机值需要在0~1之间
            if (phyllotaxisRandomValue > 1.0f | phyllotaxisRandomValue < 0.0f)
            {
                throw new ArgumentException("'phyllotaxisRandomValue' must be within the range of 0 to 1 to specify a valid random proportion.");
            }
            #endif

            PhyllotaxisRandomValue = phyllotaxisRandomValue;
            Phyllotaxis = phyllotaxis;
        }

        public override string ToString()
        {
            var s = $"[R{PhyllotaxisRandomValue:F2}->";
            foreach (var p in Phyllotaxis)
            {
                s += $"(R:{p.Rotation:F2},L:{(p.HasLeaf?"⚪":"×")},B:{p.BeerOrgan}{(p.BeerOrgan==BeerOrgan.Bud?$"+{p.AddPhysiologicalAge}":"")})";
            }
            s += "]";
            return s;
        }
    }
}