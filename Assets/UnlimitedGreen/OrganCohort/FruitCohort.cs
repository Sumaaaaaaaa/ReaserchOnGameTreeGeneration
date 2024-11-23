using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace UnlimitedGreen
{
    
    // 存储用数据格式
    internal class FruitCohort
    {
        private struct FruitCohortData
        {
            public HashSet<EntityFruit> EntityFruits;
            public int BirthCycle;
        }
        
        // 存储数据
        private readonly Queue<FruitCohortData> _data;
        private FruitCohortData _newData;
        private readonly Fruit _fruit;
        
        // 实例化方法
        public FruitCohort([NotNull] Fruit fruit)
        {
            _data = new Queue<FruitCohortData>(fruit.ValidCycles);
            _newData = new FruitCohortData() { EntityFruits = new HashSet<EntityFruit>(), BirthCycle = 1 };
            _fruit = fruit;
        }
        
        // 增加对象
        public void Add( [NotNull] EntityFruit entityFruit)
        {
            //OPT: 我还是觉得这个推入的解决方案不是最好的，参考LeafCohort的解决方法

            _newData.EntityFruits.Add(entityFruit);
            entityFruit.StoragePointer = _newData.EntityFruits;
        }
        
        // 新数据的推入
        private void PushIn()
        {
            if (_newData.EntityFruits.Count > 0)
            {
                _data.Enqueue(_newData); 
            }
        }
        
        // 计算汇总和
        public float CalculateSinkSum(int plantAge)
        {
            PushIn();

            var array = _data.ToArray();
            var sinkSum = .0f;
            foreach (var i in array)
            {
                sinkSum += i.EntityFruits.Count *
                           _fruit.SinkFunction(GenericFunctions.CalculateAge(plantAge, i.BirthCycle));
            }
            return sinkSum;
        }
        
        // 分配
        public void Allocate(int plantAge, float producedBiomass, float sinkSum)
        {
            var array = _data.ToArray();
            foreach (var i in array)
            {
                var sinkStrength = _fruit.SinkFunction(GenericFunctions.CalculateAge(plantAge, i.BirthCycle));
                var allocateBiomass = producedBiomass * sinkStrength / sinkSum;
                var entityFruits = i.EntityFruits.ToArray();

                var newBiomass = .0f;
                for (var j = 0; j < entityFruits.Length; j++)
                {
                    if (j == 0)
                    {
                        newBiomass = entityFruits[j].Biomass + allocateBiomass;
                    }

                    entityFruits[j].Biomass = newBiomass;
                }
            }
        }
        
        // 年龄增长
        public void IncreaseAge(int plantAge)
        {
            _newData = new FruitCohortData() 
                { BirthCycle = plantAge + 1 , EntityFruits = new HashSet<EntityFruit>()};
            
            if (!_data.TryPeek(out var fruitCohortData)) return;
            if (GenericFunctions.CalculateAge(plantAge, fruitCohortData.BirthCycle) < _fruit.ValidCycles) return;

            var array = fruitCohortData.EntityFruits.ToArray();
            foreach (var i in array)
            {
                i.StoragePointer = null;
            }

            _data.Dequeue();
        }
        
#if UNITY_EDITOR
        public override string ToString()
        {
            var returnString = "";
            returnString += "FRUIT\n";
            for (var i = 1; i <= _fruit.ValidCycles; i++)
            {
                returnString += $"\t{i}......{_fruit.SinkFunction(i)}\n";
            }

            returnString += "DATA\n";
            returnString += "\tNEWDATAS\n";
            returnString += $"\t\tbirthCycles={_newData.BirthCycle};contents=";
            if (!_newData.EntityFruits.Any())
            {
                returnString += "'NULL'\n";
            }
            else
            {
                var a = _newData.EntityFruits.ToArray();
                foreach (var i in a)
                {
                    returnString += $"{i}";
                }

                returnString += "\n";

            }

            returnString += "\tDATAS\n";
            var array = _data.ToArray();
            foreach (var i in array)
            {
                returnString += $"\t\tbirthCycles={i.BirthCycle};contents=";
                if (!i.EntityFruits.Any())
                {
                    returnString += "'NULL'\n";
                }
                else
                {
                    var a = i.EntityFruits.ToArray();
                    foreach (var j in a)
                    {
                        returnString += $"{j}";
                    }
                    returnString += "\n";
                }
            }
            

            return returnString;
        }
#endif
    }
}