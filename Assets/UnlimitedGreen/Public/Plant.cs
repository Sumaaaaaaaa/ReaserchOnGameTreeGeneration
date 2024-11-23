using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace UnlimitedGreen
{
    public class Plant
    {
        // OPT: 将项目中的所有中文解释改为英语
        // TODO: 撰写完整的程序README
        
        // 参数
        private readonly int _maxPhysiologicalAge;
        private readonly Random _random;
        
        private readonly Flower _flower;
        private readonly Fruit _fruit;
        private readonly LeafData _leafData;
        private readonly PhytomerData _phytomerData;
        private readonly DualScaleAutomaton _dualScaleAutomaton;
        private readonly Bud[] _buds;
        
        // 器官列
        private readonly NewPhytomerCohort _newPhytomerCohort;
        private readonly PhytomerCohort _phytomerCohort;
        private readonly LeafCohort _leafCohort;
        private readonly FruitCohort _fruitCohort;
        private readonly FlowerCohort _flowerCohort;
        
        // 变量
        private int _age = 0;
        private float _biomassStorage = 0.0f;
        private List<Axis> _axisWithBud = new List<Axis>();
        private List<Axis> _axisNoBud = new List<Axis>(); //TODO: 确认这个实现方式是不是最好的
        //private List<Axis> _updatedAxis; //TODO: 确认这个实现方式是不是最好的


        /// <summary>
        /// 以GreenLab模型参数为基础，实例化植物。
        /// </summary>
        /// <param name="randomSeed">随机种</param>
        /// <param name="maxPhysiologicalAge">最大的生理年龄</param>
        /// <param name="initialBiomass">初始的生物质量，即种子所持有的生物质量</param>
        /// <param name="waterUseEfficiency">水利用率 r</param>
        /// <param name="projectionArea">投影面积Sp</param>
        /// <param name="extinctionCoefficient">消光系数 K</param>
        /// <param name="leafAllometryE">叶子的厚度 e</param>
        /// <param name="leafSourceValidCycles">叶子的源有效周期数</param>
        /// <param name="leafSinkValidCycles">叶子的汇有效周期数</param>
        /// <param name="leafSinkFunction">叶子的汇函数——输入：生理年龄、年龄；返回：汇强度</param>
        /// <param name="phytomerValidcycles">叶元的汇有效周期数</param>
        /// <param name="phytomerSinkFunction">叶元的汇函数——输入：生理年龄、年龄，返回：汇强度</param>
        /// <param name="phytomerAllometryDatas">叶元的异速生长参数集，根据生理年龄{(b1,y1),(b2,y2)...}</param>
        /// <param name="phytomerTopologyFunc">叶元拓扑学方法，输入：AxisOrder, PrePosition, PreDirection, Length。返回的数据的含义：(NewPosition, NewDirection)</param>
        /// <param name="axisTopologyFunc">叶元侧生轴拓扑学方法，输入：AxisOrder, PreDirection, VerticleDirectionAfterPhyllotaxisRotation。返回：轴的朝向NewDirection</param>
        /// <param name="dualScaleAutomaton">双尺度自动机，其内部的自动机数量需要与生理年龄相匹配</param>
        /// <param name="buds">芽的数据定义，定义的数量需与生理年龄相同</param>
        /// <param name="flower">定义花的参数</param>
        /// <param name="fruit">定义果的参数</param>
        /// <exception cref="AggregateException">不符合模型的输入要求</exception>
        public Plant(
            int randomSeed,
            int maxPhysiologicalAge, 
            float initialBiomass,
            // 启动时的拓扑信息
            Vector3 startDireciton,
            // 源计算相关参数
            float waterUseEfficiency,
            float projectionArea,
            float extinctionCoefficient,
            // 叶子
            float leafAllometryE,
            int leafSourceValidCycles,
            int leafSinkValidCycles,
            [NotNull] Func<int,int,float> leafSinkFunction,
            // 叶元数据
            int phytomerValidcycles,
            [NotNull] Func<int,int,float> phytomerSinkFunction,
            [NotNull] (float,float)[] phytomerAllometryDatas,
            [NotNull] Func<int,Vector3,Vector3,float,(Vector3,Vector3)> phytomerTopologyFunc,
            [NotNull] Func<int,Vector3,Vector3,Vector3> axisTopologyFunc,
            // 自动机
            DualScaleAutomaton dualScaleAutomaton,
            // 芽
            Bud[] buds,
            // 器官
            Flower flower = null,Fruit fruit = null
            )
        {
#if UNITY_EDITOR
            if (maxPhysiologicalAge <= 0)
            {
                throw new AggregateException("'maxPhysiologicalAge'最大生理年龄要>=0");
            }

            if (initialBiomass <= 0)
            {
                throw new AggregateException("初始的生物质量需要>0，以能够使植物能够开始生长");
                // TODO: 英语
            }

            if (dualScaleAutomaton.Count != maxPhysiologicalAge)
            {
                throw new AggregateException("'dualScaleAutomaton'的内自动机尺度数量应该等于与最大生理年龄");
            }

            if (buds.Length != maxPhysiologicalAge)
            {
                throw new AggregateException("'buds'的定义数量应该等于最大生理年龄");
            }
#endif
            _random = new Random(randomSeed);
            
            _maxPhysiologicalAge = maxPhysiologicalAge;

            _biomassStorage = initialBiomass;
            
            _leafData = new LeafData(
                maxPhysiologicalAge:_maxPhysiologicalAge,
                leafAllometryE:leafAllometryE,
                extinctionCoefficient : extinctionCoefficient,
                projectionArea : projectionArea,
                waterUseEfficiency : waterUseEfficiency,
                sourceValidCycles:leafSourceValidCycles,
                sinkValidCycles:leafSinkValidCycles,
                sinkFunction:leafSinkFunction
                );
            _phytomerData = new PhytomerData(
                maxPhysiologicalAge:_maxPhysiologicalAge,
                validCycles:phytomerValidcycles,
                sinkFunction:phytomerSinkFunction,
                phytomerAllometryDatas:phytomerAllometryDatas,
                phytomerTopologyFunc:phytomerTopologyFunc,
                axisTopologyFunc:axisTopologyFunc
                );
            
            _flower = flower;
            _fruit = fruit;

            _dualScaleAutomaton = dualScaleAutomaton;

            _buds = buds;

            //TODO: 有芽轴、无芽轴的 初始化
            //TODO: 更新轴的 初始化
            //TODO: 起始轴是否是需要的。
            
            // 创建一个生理年龄为1的新芽，
            var firstBud = new EntityBud(
                bud: _buds[0],
                physiologicalAge: 1,
                parentPhysiologicalAge: 1,
                birthCycle: 1,
                random: _random,
                dualScaleAutomaton: _dualScaleAutomaton
            );
            // 把这个芽放到一个轴当中，
            var firstAxis = new Axis(
                axisOrder: 1,
                entityBud: firstBud,
                phyllotaxisRotation: 0
            );
            firstAxis.SetTopology(Vector3.zero, startDireciton);
            // 将这个轴放入有芽轴当中，
            _axisWithBud.Add(firstAxis);
            
            //器官列
            _newPhytomerCohort = new NewPhytomerCohort(_phytomerData);
            _phytomerCohort = new PhytomerCohort(_phytomerData);
            _leafCohort = new LeafCohort(_leafData);
            if (_fruit is not null)
            {
              _fruitCohort = new FruitCohort(_fruit);
            }
            if (_flower is not null)
            {
                _flowerCohort = new FlowerCohort(_flower);
            }

        }

        public void Growth(float environmentParameter)
        {
            _age++;
            Organogenesis();
            
            // 获取所有的汇强度之和
            var sumSink = 0.0f;
            if (_flower is not null) sumSink += _flowerCohort.CalculateSinkSum(_age);
            if (_fruit is not null) sumSink += _fruitCohort.CalculateSinkSum(_age);
            sumSink += _leafCohort.CalculateSinkSum(_age);
            sumSink += _newPhytomerCohort.CalculateSinkSum(_age);
            sumSink += _phytomerCohort.CalculateSinkSum(_age);
            Debug.Log($"汇总值为 {sumSink}。");
            
            // 主要生长
            _newPhytomerCohort.Allocate(_biomassStorage,sumSink);
            
            // 次要生长
            _phytomerCohort.Allocate(_age,_biomassStorage,sumSink);
            _leafCohort.Allocate(_age,_biomassStorage,sumSink);
            if (_flower is not null) _flowerCohort.Allocate(_age,_biomassStorage,sumSink);
            if (_fruit is not null) _fruitCohort.Allocate(_age,_biomassStorage,sumSink);
            
            // 生产
            var producedBiomass = _leafCohort.Production(environmentParameter);
            Debug.Log($"生产的Biomass = {producedBiomass}");
            _biomassStorage = producedBiomass;
            
            // 年龄增长操作
            if (_fruit is not null) _fruitCohort.IncreaseAge(_age);
            if (_flower is not null) _flowerCohort.IncreaseAge(_age);
            _leafCohort.IncreaseAge(_age);
            _phytomerCohort.IncreaseAge(_age);
            _phytomerCohort.Add(_age,_newPhytomerCohort.IncreaseAge());
        }
        
        // 器官发生 
        private void Organogenesis()
        {
            var newAxisWithBud = new List<Axis>();
            
            // 遍历所有的有芽轴
            foreach (var axis in _axisWithBud)
            {
                if (axis.ApicalBud.Expansion(axis.Position, _age, out var result))
                    // 注意这里IndexNow指的是 程序序号，而不是 生物学上的生理年龄描述。
                {
                    // 芽活着
                    newAxisWithBud.Add(axis);
                }
                else
                {
                    // 芽死了
                    _axisNoBud.Add(axis);
                    axis.ApicalBud = null;
                }

                if (result is not null)
                {
                    var newPhytomer = result.Value.phytomer;
                    var physiologicalAge = result.Value.indexNow + 1; // TODO: 确认这样用可以吗
                    
                    var entityLeavesList = new List<EntityLeaf>(newPhytomer.Phyllotaxis.Length);
                    var entityFruitsList = new List<EntityFruit>(newPhytomer.Phyllotaxis.Length);
                    var entityFlowersList = new List<EntityFlower>(newPhytomer.Phyllotaxis.Length);
                    var entityAxisList = new List<Axis>(newPhytomer.Phyllotaxis.Length);
                    
                    var phyllotaxisArray  = newPhytomer.Phyllotaxis;
                    foreach (var phyllotaxis in phyllotaxisArray)
                    {
                        axis.PhyllotaxisRotation += phyllotaxis.Rotation;
                        axis.PhyllotaxisRotation %= 360;
                        var rotationResult = (
                            axis.PhyllotaxisRotation
                            + newPhytomer.PhyllotaxisRandomValue * 180 *
                            (_random.Next() * 2 - 1)
                        ) % 360;
                        //TODO: 我才发现的Random正确的调用出一个随机float的方法是使用Next()，而不是使用NextDouble(),需要将所有错误使用的部分全部修正。
                        
                        if (phyllotaxis.HasLeaf)
                        {
                            var newLeaf = new EntityLeaf(){PhyllotaxisRotation = rotationResult};
                            entityLeavesList.Add(newLeaf);
                            _leafCohort.Add(physiologicalAge,newLeaf);
                        }
                        
                        if(phyllotaxis.BeerOrgan == BeerOrgan.None) continue;
                        if (phyllotaxis.BeerOrgan == BeerOrgan.Fruit)
                        {
                            var newFruit = new EntityFruit() { PhyllotaxisRotation = rotationResult };
                            entityFruitsList.Add(newFruit);
                            _fruitCohort.Add(newFruit);
                        }
                        else if (phyllotaxis.BeerOrgan == BeerOrgan.Flower)
                        {
                            var newFlower = new EntityFlower() { PhyllotaxisRotation = rotationResult };
                            entityFlowersList.Add(newFlower);
                            _flowerCohort.Add(newFlower);
                        }
                        else if (phyllotaxis.BeerOrgan == BeerOrgan.Bud)
                        {
                            var newPhysiologicalAge = physiologicalAge + phyllotaxis.AddPhysiologicalAge;
                            if (newPhysiologicalAge > _maxPhysiologicalAge)
                            {
                                Debug.LogError("在Organogenesis时，出现了计算后的生理年龄超过了最大生理年龄的情况，请确认自动机中的Phytomer是否有设计错误的地方");
                                //OPT: 这边的这个DEBUG信息可以更加的优化。以表达更明确的报错信息
                                //TODO: 翻译成英语
                                continue;
                            }

                            var newBud = new EntityBud(
                                bud: _buds[newPhysiologicalAge - 1],
                                physiologicalAge: newPhysiologicalAge,
                                parentPhysiologicalAge:physiologicalAge,
                                birthCycle: _age,
                                random: _random,
                                dualScaleAutomaton: _dualScaleAutomaton
                            );

                            var newAxis = new Axis(
                                axisOrder: axis.AxisOrder + 1,
                                entityBud: newBud,
                                phyllotaxisRotation: rotationResult
                            );
                            
                            entityAxisList.Add(newAxis);
                            newAxisWithBud.Add(newAxis);
                        }
                    }

                    var newEntityPhytomer = new EntityPhytomer()
                    {
                        AxillaryAxis = entityAxisList.ToArray(),
                        AxillaryFlowers = entityFlowersList.ToArray(),
                        AxillaryLeaves = entityLeavesList.ToArray(),
                        AxillaryFruits = entityFruitsList.ToArray(),
                    };
                    
                    var indexOnAxis = axis.EntityPhytomers.Count;
                    axis.EntityPhytomers.Add(newEntityPhytomer);
                    _newPhytomerCohort.Add(
                        entityPhytomer: newEntityPhytomer,
                        phytomer: newPhytomer,
                        physiologicalAge: physiologicalAge,
                        axis: axis,
                        indexOnAxis: indexOnAxis
                    );
                }
            }

            _axisWithBud = newAxisWithBud;
        }

        public void GizmosDraw()
        {
            var drawRadius = 0.1f;
            // 画种子
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Vector3.zero,drawRadius*1.2f);
            // 画各个的轴
            foreach (var axis in _axisWithBud) // 【轴】
            {
                var gizmosDrawer = new GizmosDrawer(axis.Position,drawRadius);
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(axis.Position,axis.Position+axis.Direction);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(axis.Position,axis.Position+axis.SubDirection);
                
                foreach (var phytomer in axis.EntityPhytomers) // 【叶元】
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(phytomer.Position,phytomer.Position+phytomer.Direction);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(phytomer.Position,phytomer.Position+phytomer.SubDirection);
                    
                    gizmosDrawer.Draw(phytomer.Position);
                    foreach (var entityLeaf in phytomer.AxillaryLeaves)
                    {
                        var direction = GenericFunctions.PhyllotaxisToVerticalDirection(entityLeaf.PhyllotaxisRotation,
                            phytomer.Direction, phytomer.SubDirection);
                        var position = phytomer.Position + direction * 0.2f;
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(position,new Vector3(drawRadius*0.7f,drawRadius*0.7f,drawRadius*0.7f));

                    }
                    Handles.Label(phytomer.Position,
                        $"Storage:{phytomer.StoragePointer}\n" +
                        $"radius:{phytomer.Radius}\n" +
                        $"");
                }
            }
        }

        private class GizmosDrawer
        {
            private Vector3 _prePosition;
            private Vector3 _postPosition;
            private float _raduis;
            public GizmosDrawer(Vector3 beginPosition,float radius)
            {
                _postPosition = beginPosition;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(beginPosition,radius*1.1f);
                _raduis = radius;
            }

            public void Draw(Vector3 position)
            {
                _prePosition = _postPosition;
                _postPosition = position;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_prePosition,_postPosition);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(position,_raduis);
            }
        }
    }
}