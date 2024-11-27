using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

using UnityEngine;

namespace UnlimitedGreen
{
    internal class NewPhytomerCohort
    {
        // 内用结构
        private struct ProcessData
        {
            public Axis Axis;
            public Phytomer Phytomer;
            public EntityPhytomer EntityPhytomer;
            public int PhysiologicalAge;
            public int IndexOnAxis;
        }
        
        // 存储数据
        private readonly Queue<ProcessData> _processQueue;
        private readonly HashSet<EntityPhytomer>[] _data;
        // private Func<int, int, float> _sinkFunction;
        private readonly PhytomerData _phytomerData;
        
        // 实例化方法
        public NewPhytomerCohort(
            //int maxPhysiologicalAge, 
            //[NotNull] Func<int,int,float> sinkFunction,
            [NotNull] PhytomerData phytomerData)
        {
            _phytomerData = phytomerData;
            _processQueue = new Queue<ProcessData>();
            _data = new HashSet<EntityPhytomer>[_phytomerData.MaxPhysiologicalAge];
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = new HashSet<EntityPhytomer>();
            }


        }
        
        // 增加对象
        public void Add([NotNull] EntityPhytomer entityPhytomer, 
            [NotNull] Phytomer phytomer,
            int physiologicalAge, 
            [NotNull] Axis axis,
            int indexOnAxis)
        {
            _processQueue.Enqueue(new ProcessData()
            {
                Axis = axis,
                EntityPhytomer = entityPhytomer,
                IndexOnAxis = indexOnAxis,
                PhysiologicalAge = physiologicalAge,
                Phytomer = phytomer
            });
            
            _data[physiologicalAge - 1].Add(entityPhytomer);
            entityPhytomer.StoragePointer = _data[physiologicalAge - 1];
        }

        public float CalculateSinkSum(int plantAge)
        {
            var sinkSum = .0f;
            for (var setIndex = 0; setIndex < _data.Length; setIndex++)
            {
                if (_data[setIndex].Count > 0)
                {
                    // 这个setIndex+1 就是生理年龄
                    // count获得的是个体的数量
                    sinkSum += _data[setIndex].Count * _phytomerData.SinkFunction(setIndex + 1, 1);
                }
            }
            return sinkSum;
        }
        
        /// <summary>
        /// 年龄增长——对应向普通叶元器官列的数据交付
        /// </summary>
        /// <returns>一个长度为"最大生理年龄"的EntityPhytomer的HashSet的Array，其中内容可能为Null，表明该回合内并没有该生理年龄的实体叶元</returns>
        public HashSet<EntityPhytomer>[] IncreaseAge()
        {
            var result = new HashSet<EntityPhytomer>[_data.Length];
            for (var i = 0; i < _data.Length; i++)
            {
                if (_data[i].Count > 0)
                {
                    result[i] = _data[i];
                    _data[i] = new HashSet<EntityPhytomer>();
                }
                else
                {
                    result[i] = null;
                    //OPT: 照理说这个是不是可以不用写，也是为null的。
                }
            }
            return result;
        }

        /// <summary>
        /// 分配——对应"主要生长"、包括了3D拓扑构型的功能
        /// </summary>
        /// <param name="producedBiomass"></param>
        /// <param name="sinkSum"></param>
        /// <param name="phytomerAllometryDatas">{(b,y)...}</param>
        /// <param name="phytomerTopologyFunc">叶元拓扑学方法，输入：AxisOrder, PrePosition, PreDirection, Length。返回的数据的含义：(NewPosition, NewDirection)</param>
        /// <param name="axisTopologyFunc">叶元侧生轴拓扑学方法，输入：AxisOrder, PreDirection, VerticleDirectionAfterPhyllotaxisRotation,NewDirection。返回：轴的朝向</param>
        public void Allocate(
            float producedBiomass, 
            float sinkSum)
        {
            var allocateArray = new float[_data.Length];
            for (var i = 0; i < _data.Length; i++)
            {
                var sinkStrength = _phytomerData.SinkFunction(i + 1, 1);
                var allocateBiomass = producedBiomass * sinkStrength / sinkSum;
                allocateArray[i] = allocateBiomass;
            }

            while (_processQueue.Count != 0)
            {
                var processData = _processQueue.Dequeue();
                var allometryB = _phytomerData.PhytomerAllometryDatas[processData.PhysiologicalAge - 1].Item1;
                var allometryY = _phytomerData.PhytomerAllometryDatas[processData.PhysiologicalAge - 1].Item2;
                var allocateBiomass = allocateArray[processData.PhysiologicalAge - 1];
                var length = Mathf.Sqrt(allometryB) * Mathf.Pow(allocateBiomass, (1f + allometryY) / 2f);
                var radius = Mathf.Sqrt(Mathf.Pow(allometryB, -0.5f) * Mathf.Pow(allocateBiomass, (1 - allometryY) / 2) /
                                      Mathf.PI);
                
                Vector3 prePosition;
                Vector3 preDirection;
                Vector3 preSubDirection;
                
                if (processData.IndexOnAxis == 0)
                {
                    prePosition = processData.Axis.Position;
                    preDirection = processData.Axis.Direction;
                    preSubDirection = processData.Axis.SubDirection;
                }
                else
                {
                    var p = processData.Axis.EntityPhytomers[processData.IndexOnAxis - 1];
                    prePosition = p.Position;
                    preDirection = p.Direction;
                    preSubDirection = p.SubDirection;
                }
                //拓扑学方法，输入：AxisOrder, PrePosition, PreDirection, Length
                //返回的数据的含义：(NewPosition, NewDirection) 

                var result = _phytomerData.PhytomerTopologyFunc(processData.Axis.AxisOrder, prePosition, preDirection, length);
                var newPosition = result.Item1;
                var newDirection = result.Item2.normalized;
                
                var newSubdirection = GenericFunctions.NewSubDirection(preDirection, newDirection, preSubDirection).normalized;

                processData.EntityPhytomer.Direction = newDirection;
                processData.EntityPhytomer.SubDirection = newSubdirection;
                processData.EntityPhytomer.Length = length;
                processData.EntityPhytomer.Position = newPosition;
                processData.EntityPhytomer.Radius = radius;
                foreach (var axis in processData.EntityPhytomer.AxillaryAxis)
                {
                    // 旋转后垂直朝向
                    var verticalDirection = GenericFunctions.PhyllotaxisToVerticalDirection(axis.Direction.x, 
                        newDirection, newSubdirection);
                    
                    // 自定义方法的执行
                    var axisDirection = _phytomerData.AxisTopologyFunc(axis.AxisOrder, newDirection, verticalDirection).normalized;
                    axis.SetTopology(newPosition,axisDirection);
                }
            }
            
            
        }

        public override string ToString()
        {
            var s = "";
            s += "NEW-PHYTOMER\n\tQUEUE\n";
            foreach (var processData in _processQueue)
            {
                s += $"[A:{processData.Axis.GetHashCode()}, P:{processData.Phytomer.GetHashCode()}, " +
                     $"EP:{processData.EntityPhytomer.GetHashCode()},Phy:{processData.PhysiologicalAge}, " +
                     $"I:{processData.IndexOnAxis}]";
            }
            
            /*for (var phi = 1; phi <= _phytomerData.MaxPhysiologicalAge; phi++)
            {
                _data[phi]
            }*/

            return s;
        }
        
    }
}