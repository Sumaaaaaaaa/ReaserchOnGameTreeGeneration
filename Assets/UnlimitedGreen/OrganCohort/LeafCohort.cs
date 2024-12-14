using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnlimitedGreen
{
    internal class LeafCohort
    {
        // 数据结构
        private struct LeafCohortData
        {
            public HashSet<EntityLeaf> EntityLeaves;
            public int BirthCycle;
            public LeafCohortData(HashSet<EntityLeaf> entityLeaves,int birthCycle)
            {
                EntityLeaves = entityLeaves;
                BirthCycle = birthCycle;
            }
        }
        
        // 数据存储
        private readonly Queue<LeafCohortData>[] _sinkSourceData;
        private readonly Queue<LeafCohortData>[] _sourceData;
        private readonly HashSet<EntityLeaf>[] _newData;
        private readonly LeafData _leafData;

        public LeafCohort(LeafData leafData)
        {
            _leafData = leafData;
            _sourceData = new Queue<LeafCohortData>[_leafData.MaxPhysiologicalAge];
            for (var i = 0; i < _sourceData.Length; i++)
            {
                //OPT: 其实如果在年龄增加迭代过程中，使源、汇相同时直接pop掉，而不是转移到源存储中，能省很多事
                _sourceData[i] = new Queue<LeafCohortData>(
                    _leafData.SourceValidCycles == _leafData.SinkValidCycles
                        ? 1 : _leafData.SourceValidCycles - _leafData.SinkValidCycles);
            }
            _sinkSourceData = new Queue<LeafCohortData>[_leafData.MaxPhysiologicalAge];
            for (var i = 0; i < _sinkSourceData.Length; i++)
            {
                _sinkSourceData[i] = new Queue<LeafCohortData>(_leafData.SinkValidCycles);
            }

            _newData = new HashSet<EntityLeaf>[_leafData.MaxPhysiologicalAge];
            for (var i = 0; i < _newData.Length; i++)
            {
                _newData[i] = new HashSet<EntityLeaf>();
            }
        }

        public void Add(int physiologicalAge, EntityLeaf entityLeaf)
        {
            _newData[physiologicalAge - 1].Add(entityLeaf);
            entityLeaf.StoragePointer = _newData[physiologicalAge - 1];
        }

        private void PushIn(int plantAge)
        {
            for (var i = 0; i < _newData.Length; i++)
            {
                if (_newData[i].Count <= 0) continue;
                _sinkSourceData[i].Enqueue(
                    new LeafCohortData() { BirthCycle = plantAge, EntityLeaves = _newData[i] }
                );
                _newData[i] = new HashSet<EntityLeaf>();
            }
        }

        public float CalculateSinkSum(int plantAge)
        {
            PushIn(plantAge);
            
            var sinkSum = 0.0f;
            
            // 遍历生理年龄
            for (var phi = 0; phi < _leafData.MaxPhysiologicalAge; phi++)
            {
                // 汇队列
                var array = _sinkSourceData[phi].ToArray();
                for (var i = 0; i < array.Length; i++)
                {
                    // 遍历这个队列中每一组数据
                    var age = GenericFunctions.CalculateAge(plantAge, array[i].BirthCycle); // 年龄
                    var sinkStrength = _leafData.SinkFunction(phi + 1, age); // 汇函数
                    var count = array[i].EntityLeaves.Count;//数量
                    sinkSum += sinkStrength * count;
                }
            }
            return sinkSum;
        }

        public void Allocate(int plantAge, float producedBiomass, float sinkSum)
        {
            // 遍历生理年龄
            for (var phi = 0; phi < _leafData.MaxPhysiologicalAge; phi++)
            {
                var array = _sinkSourceData[phi].ToArray();
                // 遍历到队列中的每一组数据
                for (var i = 0; i < array.Length; i++)
                {
                    var age = GenericFunctions.CalculateAge(plantAge, array[i].BirthCycle); // 年龄
                    var sinkStrength = _leafData.SinkFunction(phi + 1, age); // 汇函数
                    var allocateBiomass = producedBiomass * sinkStrength / sinkSum; // 分配到的生物质
                    var entityLeaves = array[i].EntityLeaves.ToArray();
                    
                    var newBiomass = .0f;
                    for (var index = 0; index < entityLeaves.Count(); index++)
                    {
                        if (index == 0)
                        {
                            newBiomass = entityLeaves[index].Biomass + allocateBiomass;
                        }
                        entityLeaves[index].Biomass = newBiomass;
                    }
                }
            }
        }

        public float Production(float environmentParameter)
        {
            var biomassSum = 0.0f;
            foreach (var i in _sourceData)
            {
                var array = i.ToArray();
                foreach (var j in array)
                {
                    if (j.EntityLeaves.Count == 0) continue;
                    biomassSum += j.EntityLeaves.Count * j.EntityLeaves.First().Biomass;
                }
            }
            foreach (var i in _sinkSourceData)
            {
                var array = i.ToArray();
                foreach (var j in array)
                {
                    if (j.EntityLeaves.Count == 0) continue;
                    biomassSum += j.EntityLeaves.Count * j.EntityLeaves.First().Biomass;
                }
            }
            var totalArea = biomassSum / _leafData.LeafAllometryE;

            var productBiomass = environmentParameter * _leafData.ProjectionArea
                                 / _leafData.WaterUseEfficiency
                                 * (1 - Mathf.Exp(-_leafData.ExtinctionCoefficient * totalArea / _leafData.ProjectionArea));
            
            return productBiomass;
        }

        public void IncreaseAge(int plantAge)
        {
            for (var i = 0; i < _sinkSourceData.Length; i++) // i 是生理年龄（函数用的时候需要+1）
            {
                if(!_sinkSourceData[i].TryPeek(out var leafCohortData)) continue;
                if(GenericFunctions.CalculateAge(plantAge,leafCohortData.BirthCycle)
                   <_leafData.SinkValidCycles)continue;
                _sourceData[i].Enqueue(_sinkSourceData[i].Dequeue());
            }

            foreach (var t in _sourceData)
            {
                if(!t.TryPeek(out var leafCohortData)) continue;
                if(GenericFunctions.CalculateAge(plantAge,leafCohortData.BirthCycle)
                   <_leafData.SourceValidCycles)continue;
                var array = leafCohortData.EntityLeaves.ToArray();
                foreach (var entityLeaf in array)
                {
                    entityLeaf.StoragePointer = null;
                }

                t.Dequeue();
            }
        }

        public override string ToString()
        {
            // 基本参数
            var s = "";
            s += "LEAF\n";
            s +=
                $"\tBASEDATA\n\t\te={_leafData.LeafAllometryE}, k={_leafData.ProjectionArea}, Sp={_leafData.ProjectionArea}, " +
                $"r={_leafData.WaterUseEfficiency}, sourceValidCycle={_leafData.SourceValidCycles}\n";
            s += "\tSINK\n";
            
            // cycle 的 表格头
            s += "\t\t\t";
            for (var phi = 1; phi <= _leafData.SinkValidCycles; phi++)
            {
                s += $"c{phi}\t\t";
            }
            
            // 生理年龄和具体的值
            s += "\n";
            for (var phi = 1; phi <= _leafData.MaxPhysiologicalAge; phi++)
            {
                s += $"\t\tp{phi}\t";
                for (var cycle = 1; cycle <= _leafData.SinkValidCycles; cycle++)
                {
                    s += $"{_leafData.SinkFunction(phi, cycle):F2}\t";
                }
                s += "\n";
            }
            
            // 新
            s += "\tNEW\n";

            for (var phi = 1; phi <= _leafData.MaxPhysiologicalAge; phi++)
            {
                s += $"\t\tPhy={phi}; contents=";
                var array = _newData[phi-1];
                foreach (var entityLeaf in array)
                {
                    s += $"{entityLeaf} ";
                }
                s += "\n";
                
            }
            
            // 源汇
            s += "\tSINK-SOURCE\n";
            for (var phi = 1; phi <= _leafData.MaxPhysiologicalAge; phi++)
            {
                s += $"\t\tPhy={phi}; content=\n";
                var queue = _sinkSourceData[phi-1];
                foreach (var leafCohortData in queue)
                {
                    s += $"\t\t\tBirthCycle={leafCohortData.BirthCycle}: ";
                    var leafs = leafCohortData.EntityLeaves;
                    foreach (var leaf in leafs)
                    {
                        s+=$"{leaf} ";
                    }
                    s += "\n";
                }
            }
            
            // 源
            s += "\tSOURCE\n";
            for (var phi = 1; phi <= _leafData.MaxPhysiologicalAge; phi++)
            {
                s += $"\t\tPhy={phi}; content=\n";
                var queue = _sourceData[phi - 1];
                foreach (var leafCohortData in queue)
                {
                    s += $"\t\t\tBirthCycle={leafCohortData.BirthCycle}: ";
                    var leafs = leafCohortData.EntityLeaves;
                    foreach (var leaf in leafs)
                    {
                        s += $"{leaf} ";
                    }
                    s += "\n";
                }
            }
            
            
            return s;
        }
    }
}