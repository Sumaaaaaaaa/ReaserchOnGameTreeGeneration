using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    internal class PhytomerCohort
    {
        private struct PhytomerCohortData
        {
            public HashSet<EntityPhytomer> Phytomers;
            public int BirthCycle;
        }

        private readonly Queue<PhytomerCohortData>[] _data;
        private readonly PhytomerData _phytomerData;
        
        // 实例化方法
        public PhytomerCohort([NotNull] PhytomerData phytomerData)
        {
            _phytomerData = phytomerData;
            
            // 存储数据初始化
            _data = new Queue<PhytomerCohortData>[_phytomerData.MaxPhysiologicalAge];
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = new Queue<PhytomerCohortData>(_phytomerData.ValidCycles);
            }
        }
        
        // 计算汇总和
        public float CalculateSinkSum(int plantAge)
        {
            var sinkSum = .0f;
            for (var phi = 0; phi < _data.Length; phi++)
            {
                // 此时的phi 是 生理年龄 - 1
                var array = _data[phi].ToArray();
                foreach (var phytomerCohortData in array)
                {
                    var count = phytomerCohortData.Phytomers.Count; // 数量
                    var age = GenericFunctions.CalculateAge(plantAge, phytomerCohortData.BirthCycle); // 年龄
                    sinkSum += _phytomerData.SinkFunction(phi + 1, age) * count;
                }
            }
            return sinkSum;
        }
        
        // 分配
        public void Allocate(int plantAge,float producedBiomass, float sinkSum)
        {
            // 遍历生理年龄：此时的phi 是 生理年龄 - 1
            for (var phi = 0; phi < _data.Length; phi++)
            {
                var array = _data[phi].ToArray();
                // 遍历出生日期：
                foreach (var phytomerCohortData in array)
                {
                    var age = GenericFunctions.CalculateAge(plantAge, phytomerCohortData.BirthCycle); // 年龄
                    var sinkStength = _phytomerData.SinkFunction(phi + 1, age);
                    var allocateBiomass = producedBiomass * sinkStength / sinkSum;

                    var entityPhytomers = phytomerCohortData.Phytomers.ToArray();
                    var newRadius = .0f;
                    for (var i = 0; i < entityPhytomers.Length; i++)
                    {
                        if (i == 0)
                        {
                            var ro = entityPhytomers[i].Radius;
                            var h = entityPhytomers[i].Length;
                            newRadius = Mathf.Sqrt(allocateBiomass / Mathf.PI * h + ro * ro);
                        }
                        entityPhytomers[i].Radius = newRadius;
                    }
                    
                }
            }
        }
        // 年龄增长
        public void IncreaseAge(int plantAge)
        {
            foreach (var queue in _data)
            {
                if(!queue.TryPeek(out var phytomerCohortData)) continue;
                if(GenericFunctions.CalculateAge(plantAge,phytomerCohortData.BirthCycle) 
                   < _phytomerData.ValidCycles) continue;
                var array = phytomerCohortData.Phytomers.ToArray();
                foreach (var entityPhytomer in array)
                {
                    entityPhytomer.StoragePointer = null;
                }

                queue.Dequeue();
            }
        }
        // 增加对象：对应->移交
        public void Add(int plantAge, HashSet<EntityPhytomer>[] hashSets)
        {
            for (var i = 0; i < hashSets.Length; i++)
            {
                if (hashSets[i] is null) continue;
                var newData = new PhytomerCohortData() 
                    { BirthCycle = plantAge, Phytomers = hashSets[i] };
                _data[i].Enqueue(newData);
            }
        }
    }
}