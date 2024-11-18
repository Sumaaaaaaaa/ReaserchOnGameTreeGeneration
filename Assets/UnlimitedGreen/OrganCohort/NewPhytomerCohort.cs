using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

using UnityEngine;

namespace UnlimitedGreen
{
    internal class NewPhytomerCohort
    {
        //TODO: 测试
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
        private Queue<ProcessData> _processQueue;
        private HashSet<EntityPhytomer>[] _data;
        private Func<int, int, float> _sinkFunction;
        
        // 实例化方法
        public NewPhytomerCohort(int maxPhysiologicalAge, [NotNull] Func<int,int,float> sinkFunction)
        {
            #if UNITY_EDITOR
            //最大的生理年龄不能小于1
            if (maxPhysiologicalAge < 1)
            {
                throw new AggregateException("The value of 'maxPhysiologicalAge' must be greater than or equal to 1.");
            }
            
            // 测试传入的sinkFunction有效
            for (var i = 1; i <= maxPhysiologicalAge; i++)
            {
                if (sinkFunction(i, 1) < 0)
                {
                    throw new AggregateException("The value produced by 'sinkFunction' with 输入 1~'maxPhysiologicalAge' must be greater than or equal to 0."); 
                    // TODO: 标准的英语
                }
            }
            #endif
            
            _processQueue = new Queue<ProcessData>();
            _data = new HashSet<EntityPhytomer>[maxPhysiologicalAge];
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = new HashSet<EntityPhytomer>();
            }
            _sinkFunction = sinkFunction;

        }
        
        // 增加对象
        public void Add([NotNull] EntityPhytomer entityPhytomer, [NotNull] Phytomer phytomer,
            int physiologicalAge, [NotNull] Axis axis,int indexOnAxis)
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
                    sinkSum += _data[setIndex].Count * _sinkFunction(setIndex + 1, 1);
                }
            }
            return sinkSum;
        }
        
        /// <summary>
        /// 年龄增长——对应向普通叶元器官列的数据角符
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
        public void Allocate(/*int plantAge,*/ float producedBiomass, float sinkSum,
            (float,float)[] phytomerAllometryDatas, 
            Func<int,Vector3,Vector3,float,(Vector3,Vector3)> phytomerTopologyFunc,
            Func<int,Vector3,Vector3,Vector3> axisTopologyFunc)
        {
            var allocateArray = new float[_data.Length];
            for (var i = 0; i < _data.Length; i++)
            {
                var sinkStrength = _sinkFunction(i + 1, 1);
                var allocateBiomass = producedBiomass * sinkStrength / sinkSum;
                allocateArray[i] = allocateBiomass;
            }

            foreach (var processData in _processQueue)
            {
                var allometryB = phytomerAllometryDatas[processData.PhysiologicalAge - 1].Item1;
                var allometryY = phytomerAllometryDatas[processData.PhysiologicalAge - 1].Item2;
                var allocateBiomass = allocateArray[processData.PhysiologicalAge - 1];
                var length = Mathf.Sqrt(allometryB) * Mathf.Pow(allocateBiomass, (1f + allometryY) / 2f);
                var radius = Mathf.Sqrt(Mathf.Pow(allometryB, -0.5f) * Mathf.Pow(allocateBiomass, (1 - allometryY) / 2) /
                                      Mathf.PI);
                
                var prePosition = Vector3.zero;
                var preDirection = Vector3.zero;
                var preSubDirection = Vector3.zero;
                
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

                var result = phytomerTopologyFunc(processData.Axis.AxisOrder, prePosition, preDirection, length);
                var newPosition = result.Item1;
                var newDirection = result.Item2.normalized;
                
                //TODO: 需要在显式处表明、当前使用的转换方式最好不要让新的朝向过大，特别是接近旧朝向的几乎反面，这可能会让副朝向的计算出现奇怪的结果
                var newSubdirection = GenericFunctions.NewSubDirection(preDirection, newDirection, preSubDirection).normalized;

                processData.EntityPhytomer.Direction = newDirection;
                processData.EntityPhytomer.SubDirection = newSubdirection;
                processData.EntityPhytomer.Length = length;
                processData.EntityPhytomer.Position = newPosition;
                processData.EntityPhytomer.Radius = radius;
                foreach (var axis in processData.EntityPhytomer.AxillaryAxis)
                {
                    //newDirection
                    //newSubdirection
                    // 旋转后垂直朝向
                    var verticalDirection = GenericFunctions.PhyllotaxisToVerticalDirection(axis.Direction.x, 
                        newDirection, newSubdirection);
                    // TODO: 根据输入不可信的原则，这里得到的朝向绝对需要被Normalized！
                    
                    // 自定义方法的执行
                    // AxisOrder, PreDirection, VerticleDirectionAfterPhyllotaxisRotation,NewDirection。返回：轴的朝向
                    var axisDirection = axisTopologyFunc(axis.AxisOrder, newDirection, verticalDirection);
                    axis.SetTopology(newPosition,axisDirection);
                }
            }
            
            
        }
        //结束占位符
        
    }
}