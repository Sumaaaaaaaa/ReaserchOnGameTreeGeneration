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

        //TODO: 我感觉与生产相关的内容一会也需要加进来。
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
                throw new AggregateException("'MaxPhysiologicalAge'需要是一个大于等于0的值");
            }
            // 叶子的异速生长参数e 必须 > 0 
            if (leafAllometryE <= 0)
            {
                throw new AggregateException("'leafAllometryE'必须传入一个大于0的数以描述叶子的厚度");
            }

            if (extinctionCoefficient <= 0)
            {
                throw new AggregateException("'extinctionCoefficient'(即K)需要是一个>0的值");
            }
            if (projectionArea <= 0)
            {
                throw new AggregateException("'projectionArea'(即Sp)需要是一个>0的值");
            }
            if (waterUseEfficiency <= 0)
            {
                throw new AggregateException("'waterUseEfficiency'(即r)需要是一个>0的值");
            }
            // 汇有效周期需要 > 0
            if (sinkValidCycles <= 0)
            {
                throw new AggregateException("'sinkValidCycles'需要是一个大于0的值");
            }
            // 源有效周期需要 >= 汇有效周期
            //TODO: 需要确认源有效周期 = 汇有效周期 时后面的迭代能否正常的运行。
            if (sourceValidCycles < sinkValidCycles)
            {
                throw new AggregateException("'sourceValidCycles'需要大于等于'sinkValidCycles'");
            }
            // 确认汇函数有效
            for (var i = 1; i <= maxPhysiologicalAge; i++) // 生理年龄
            {
                for (var j = 1; j <= sinkValidCycles; j++) // 年龄
                {
                    if (sinkFunction(i, j) < 0)
                    {
                        throw new ArgumentException(
                            "'sinkFunction'对于生理年龄[1,maxPhysiologicalAge]和周期年龄[1,sinkValidCycles]必须都获得有效的大于等于0的汇值");
                    }
                }
            }
            //TODO: 翻译成标准英语
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