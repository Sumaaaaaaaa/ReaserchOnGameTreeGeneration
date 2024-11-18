using System;
using JetBrains.Annotations;
using UnityEditor.PackageManager;

namespace UnlimitedGreen
{
    public abstract class Organ
    {
        public readonly int ValidCycles;
        public readonly Func<int, float> SinkFunction;

        protected Organ(int validCycles,Func<int, float> sinkFunction)
        {
#if UNITY_EDITOR // 只在Editor下运行的数据检查
            // 有效周期 <= 0
            if (validCycles < 1)
            {
                
                throw new ArgumentException("The input value for 'validCycles' must be within the range [1, infinity].");
            }
            
            // 汇函数返回了不合规的负数
            for (var i = 1; i <= validCycles; i++)
            {
                if (sinkFunction(i) < 0)
                {
                    throw new ArgumentException("'sinkFunction' must return a value greater than 0 for all possible " +
                                                "ages within the range [1, 'validCycles'].");
                }
            }
#endif
            ValidCycles = validCycles;
            SinkFunction = sinkFunction;
            
        }
    }

    public class Fruit : Organ
    {
        public Fruit(int validCycles, [NotNull] Func<int, float> sinkFunction) 
            : base(validCycles, sinkFunction)
        {
        }
    }

    public class Flower : Organ
    {
        public Flower(int validCycles, [NotNull] Func<int, float> sinkFunction) 
            : base(validCycles, sinkFunction)
        {
        }
    }
    
}