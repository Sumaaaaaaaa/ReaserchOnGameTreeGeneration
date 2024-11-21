﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    /// <summary>
    /// 描述叶元上本叶轴节点所带器官,
    /// None - 无携带器官， Fruit - 携带水果 ， Flower - 携带花朵。
    /// Bud - 携带侧芽，并定义生理年龄增长量
    /// </summary>
    public enum BeerOrgan
    {
        None,
        Fruit,
        Flower,
        Bud_SamePhysiologicalAge = 0,
        Bud_Add1PhysiologicalAge = 1,
        Bud_Add2PhysiologicalAge = 2,
        Bud_Add3PhysiologicalAge = 3,
        Bud_Add4PhysiologicalAge = 4,
        Bud_Add5PhysiologicalAge = 5,
        Bud_Add6PhysiologicalAge = 6,
        Bud_Add7PhysiologicalAge = 7,
        Bud_Add8PhysiologicalAge = 8,
        Bud_Add9PhysiologicalAge = 9,
    }
    
    /// <summary>
    /// 描述叶元上本叶轴节点的参数
    /// </summary>
    public struct Phyllotaxis
    {
        public float Rotation;
        public bool HasLeaf;
        public BeerOrgan BeerOrgan;
        public Phyllotaxis(float rotation, bool hasLeaf,BeerOrgan beerOrgan)
        {
            Rotation = rotation;
            HasLeaf = hasLeaf;
            BeerOrgan = beerOrgan;
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
        /// <exception cref="AggregateException">定义不符合要求</exception>
        public Phytomer(float phyllotaxisRandomValue,[NotNull] Phyllotaxis[] phyllotaxis)
        {
            
            #if UNITY_EDITOR
            // 'phyllotaxisRandomValue'旋转随机值需要在0~1之间
            if (phyllotaxisRandomValue > 1.0f | phyllotaxisRandomValue < 0.0f)
            {
                throw new AggregateException("'phyllotaxisRandomValue'需要在0~1的范围内，以说明随机比例");
                //TODO: 翻译成英语
            }
            #endif

            PhyllotaxisRandomValue = phyllotaxisRandomValue;
            Phyllotaxis = phyllotaxis;
        }
    }
}