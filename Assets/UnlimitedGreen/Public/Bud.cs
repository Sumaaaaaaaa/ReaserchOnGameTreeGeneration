using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    public class Bud
    {
        public readonly bool[] RhythmRatio; 
        public readonly Func<int,float> RandomRatio;
        public readonly Func<int,float> ViabilityRatio;
        public readonly Func<int, int,float> BranchingIntensity;
        public readonly Func<Vector3,float> LightRatio;

        /// <summary>
        /// 定义一个（生理年龄的）芽的各种概率参数。
        /// </summary>
        /// <param name="rhythmRatio">节律比率(rhythm ratio)</param>
        /// <param name="randomRatio">非周期随机值函数(non-periodic random aspect)：(年龄)=>概率值 </param>
        /// <param name="viabilityRatio">生存随机值函数(Viability)：(年龄)=>概率值</param>
        /// <param name="branchingIntensity">(年龄, 父叶元生理年龄)=>概率值 当概率值小于0的话，将直接判定为芽的死亡</param>
        /// <param name="lightRatio">通过位置进行光之类的随机值的计算方程：(芽位置)=>随机值</param>
        public Bud(
            [NotNull] bool[] rhythmRatio,
            [NotNull] Func<int, float> randomRatio,
            [NotNull] Func<int, float> viabilityRatio,
            [NotNull] Func<int, int, float> branchingIntensity,
            [NotNull] Func<Vector3, float> lightRatio)
        {
            RandomRatio = randomRatio;
            ViabilityRatio = viabilityRatio;
            RhythmRatio = rhythmRatio;
            BranchingIntensity = branchingIntensity;
            LightRatio = lightRatio;
        }
        
    }
    
}