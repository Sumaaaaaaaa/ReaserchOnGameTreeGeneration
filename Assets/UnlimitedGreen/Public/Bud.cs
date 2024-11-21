using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    public class Bud
    {
        public readonly bool[] RhythmRatio; // rhythm ratio - $w_\varphi$ 节律比 w
        public readonly Func<int,float> RandomRatio;// Non-periodic random aspect - $B(i)$ - $b$ 随机比 b, 输入：生理年龄
        public readonly Func<int,float> MortalityRatio; // Mortality - $C(i)$ - c 生存率 c，输入：生理年龄
        public readonly Func<int, int, int,float> BranchingIntensity;// Branching intensity - F(i) - a 分支强度 a，输入：父体生理年龄，本体生理年龄，个体年龄，返回概率值，如果小于0则为死亡
        public readonly Func<Vector3,float> LightRatio; // Light ratio - $L()$ 光线随机比 l

        public Bud(
            [NotNull] bool[] rhythmRatio,
            [NotNull] Func<int, float> randomRatio,
            [NotNull] Func<int, float> mortalityRatio,
            [NotNull] Func<int, int, int,float> branchingIntensity,
            [NotNull] Func<Vector3, float> lightRatio)
        {
            RandomRatio = randomRatio;
            MortalityRatio = mortalityRatio;
            RhythmRatio = rhythmRatio;
            BranchingIntensity = branchingIntensity;
            LightRatio = lightRatio;
        }
        
    }
    
}