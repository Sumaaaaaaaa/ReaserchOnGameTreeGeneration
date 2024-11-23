using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace UnlimitedGreen
{
    internal class FlowerCohort
    {
        
        // 存储用数据格式
        private struct FlowerCohortData
        {
            public HashSet<EntityFlower> EntityFlowers;
            public int BirthCycle;
        }
        
        // 存储数据
        private readonly Queue<FlowerCohortData> _data;
        private FlowerCohortData _newData;
        private readonly Flower _flower;
        
        // 实例化方法
        public FlowerCohort([NotNull] Flower flower)
        {
            _data = new Queue<FlowerCohortData>(flower.ValidCycles);
            _newData = new FlowerCohortData(){EntityFlowers = new HashSet<EntityFlower>(),BirthCycle = 1};
            _flower = flower;
        }
        
        // 增加对象
        public void Add([NotNull] EntityFlower entityFlower)
        {
            //OPT: 我还是觉得这个推入的解决方案不是最好的，参考LeafCohort的解决方法
            _newData.EntityFlowers.Add(entityFlower);
            entityFlower.StoragePointer = _newData.EntityFlowers;
        }
        
        // 将本回中的数据推入_data中
        private void PushIn()
        {
            if (_newData.EntityFlowers.Count > 0)
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
                sinkSum += i.EntityFlowers.Count *
                    _flower.SinkFunction(GenericFunctions.CalculateAge(plantAge, i.BirthCycle));
            }
            return sinkSum;
        }
        
        // 分配
        public void Allocate(int plantAge,float producedBiomass,float sinkSum)
        {
            var array = _data.ToArray();
            foreach (var i in array)
            {
                var sinkStrength = _flower.SinkFunction(GenericFunctions.CalculateAge(plantAge, i.BirthCycle));
                var allocateBiomass = producedBiomass * sinkStrength / sinkSum ;
                var entityFlowers = i.EntityFlowers.ToArray();

                var newBiomass = .0f;
                for (var j = 0; j < entityFlowers.Length; j++)
                {
                    if (j == 0)
                    {
                        newBiomass = entityFlowers[j].Biomass + allocateBiomass;
                    }
                    entityFlowers[j].Biomass = newBiomass;
                }
            }
        }
        
        // 年龄增长
        public void IncreaseAge(int plantAge)
        {
            _newData = new FlowerCohortData()
                { BirthCycle = plantAge + 1, EntityFlowers = new HashSet<EntityFlower>() };
            if (!_data.TryPeek(out var flowerCohortData)) return;
            if (GenericFunctions.CalculateAge(plantAge, flowerCohortData.BirthCycle) < _flower.ValidCycles) return;
            
            var array = flowerCohortData.EntityFlowers.ToArray();
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
            returnString += "FLOWER\n";
            for (var i = 1; i <= _flower.ValidCycles; i++)
            {
                returnString += $"\t{i}......{_flower.SinkFunction(i)}\n";
            }

            returnString += "DATA\n";
            returnString += "\tNEWDATAS\n";
            returnString += $"\t\tbirthCycles={_newData.BirthCycle};contents=";
            if (!_newData.EntityFlowers.Any())
            {
                returnString += "'NULL'\n";
            }
            else
            {
                var a = _newData.EntityFlowers.ToArray();
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
                if (!i.EntityFlowers.Any())
                {
                    returnString += "'NULL'\n";
                }
                else
                {
                    var a = i.EntityFlowers.ToArray();
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