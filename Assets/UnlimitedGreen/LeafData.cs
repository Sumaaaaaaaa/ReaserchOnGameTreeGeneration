using System;
using JetBrains.Annotations;

namespace UnlimitedGreen
{
    internal class LeafData
    {
        public int MaxPhysiologicalAge { get; private set; }
        public float LeafAllometryE { get; private set; }
        public int SourceValidCycles { get; private set; }
        public int SinkValidCycles { get; private set; }
        /// <summary>
        /// 汇方程，输入：生理年龄、年龄；返回：汇强度
        /// </summary>
        public Func<int, int, float> SinkFunction { get; private set; }
        /// <summary>
        /// 消光系数 K
        /// </summary>
        public float ExtinctionCoefficient { get; private set; }
        /// <summary>
        /// 投影面积Sp
        /// </summary>
        public float ProjectionArea { get; private set; }
        /// <summary>
        /// 水利用率 r
        /// </summary>
        public float WaterUseEfficiency { get; private set; }
        
        public LeafData(
            int maxPhysiologicalAge,
            float leafAllometryE,
            float extinctionCoefficient,
            float projectionArea,
            float waterUseEfficiency,
            int sourceValidCycles,
            int sinkValidCycles,
            [NotNull] Func<int,int,float> sinkFunction)
        {
#if UNITY_EDITOR
            // 最大生理年龄需要>=1
            if (maxPhysiologicalAge < 1)
            {
                throw new ArgumentException("'maxPhysiologicalAge' must be greater than or equal to 1.");
            }
            // 叶子的异速生长参数e 必须 > 0 
            if (leafAllometryE <= 0)
            {
                throw new ArgumentException("'leafAllometryE' must be greater than 0 to accurately describe leaf thickness.");
            }

            if (extinctionCoefficient <= 0)
            {
                throw new ArgumentException("'extinctionCoefficient' (K) must be greater than 0.");
            }
            if (projectionArea <= 0)
            {
                throw new ArgumentException("'projectionArea' (Sp) must be greater than 0.");
            }
            if (waterUseEfficiency <= 0)
            {
                throw new ArgumentException("'waterUseEfficiency' (r) must be greater than 0.");
            }
            // 汇有效周期需要 > 0
            if (sinkValidCycles <= 0)
            {
                throw new ArgumentException("'sinkValidCycles' must be greater than 0.");
            }
            // 源有效周期需要 >= 汇有效周期
            if (sourceValidCycles < sinkValidCycles)
            {
                throw new ArgumentException("'sourceValidCycles' must be greater than or equal to 'sinkValidCycles'.");
            }
            // 确认汇函数有效
            for (var i = 1; i <= maxPhysiologicalAge; i++) // 生理年龄
            {
                for (var j = 1; j <= sinkValidCycles; j++) // 年龄
                {
                    if (sinkFunction(i, j) < 0)
                    {
                        throw new ArgumentException(
                            "'sinkFunction' must return valid non-negative values for physiological ages in 1~maxPhysiologicalAge and cycle ages in 1~sinkValidCycles.");
                    }
                }
            }
#endif
            MaxPhysiologicalAge = maxPhysiologicalAge;
            
            LeafAllometryE = leafAllometryE;
            ExtinctionCoefficient = extinctionCoefficient;
            ProjectionArea = projectionArea;
            WaterUseEfficiency = waterUseEfficiency;
            
            SourceValidCycles = sourceValidCycles;
            SinkValidCycles = sinkValidCycles;
            SinkFunction = sinkFunction;
        }
    }
}