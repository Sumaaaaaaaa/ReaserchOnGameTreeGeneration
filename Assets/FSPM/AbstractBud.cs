using System;

public struct AbstractBud
{
   public Func<int,float> RandomRatio;// Non-periodic random aspect - $B(i)$ - $b$ 随机比 b
   public Func<int,float> MortalityRatio; // Mortality - $C(i)$ - c 生存率 c
   public bool[] RhythmRatio; // rhythm ratio - $w_\varphi$ 节律比 w
   public Func<int, AbstractBud, float> BranchingIntensity;// Branching intensity - F(i) - a 分支强度 a
   public Func<float> SpecialRatio; // Special ratio - $S()$ 特殊随即比 s
   public InAutomaton InAutomaton; // 自动机
}