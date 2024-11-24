using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnlimitedGreen
{
    /// <summary>
    /// Phytomer数据载体，用于存储关于Phytomer的各种相关的定义的数据
    /// </summary>
    internal class PhytomerData
    {
        //OPT: 需确认对于该类，是使用Class的模式比较快，还是使用Struct的模式比较快.
        /// <summary>
        /// 最大生理年龄
        /// </summary>
        public int MaxPhysiologicalAge { get; private set; }
        /// <summary>
        /// 有效周期
        /// </summary>
        public int ValidCycles { get; private set; }
        /// <summary>
        /// 汇方程，输入：生理年龄、年龄，返回：汇强度
        /// </summary>
        public Func<int, int, float> SinkFunction { get; private set; }
        /// <summary>
        /// 最大生理年龄数量的异速生长参数{(b1,y1),(b2,y2)...}
        /// </summary>
        public (float, float)[] PhytomerAllometryDatas{ get; private set; }
        /// <summary>
        /// 叶元拓扑学方法，输入：AxisOrder, PrePosition, PreDirection, Length。返回的数据的含义：(NewPosition, NewDirection)
        /// </summary>
        public Func<int,Vector3,Vector3,float,(Vector3,Vector3)> PhytomerTopologyFunc { get; private set; }
        /// <summary>
        /// 叶元侧生轴拓扑学方法，输入：AxisOrder, PreDirection, VerticleDirectionAfterPhyllotaxisRotation,NewDirection。返回：轴的朝向
        /// </summary>
        public Func<int, Vector3, Vector3, Vector3> AxisTopologyFunc { get; private set; }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="maxPhysiologicalAge">最大生理年龄</param>
        /// <param name="validCycles">有效周期</param>
        /// <param name="sinkFunction">汇方程，输入：生理年龄、年龄，返回：汇强度</param>
        /// <param name="phytomerAllometryDatas">最大生理年龄数量的异速生长参数{(b1,y1),(b2,y2)...}</param>
        /// <param name="phytomerTopologyFunc">叶元拓扑学方法，输入：AxisOrder, PrePosition, PreDirection, Length。返回的数据的含义：(NewPosition, NewDirection)</param>
        /// <param name="axisTopologyFunc">叶元侧生轴拓扑学方法，输入：AxisOrdeVer, PreDirection, VerticleDirectionAfterPhyllotaxisRotation,NewDirection。返回：轴的朝向</param>
        /// <exception cref="AggregateException">不符合模型的数值定义</exception>
        public PhytomerData(
            int maxPhysiologicalAge,
            int validCycles,
            [NotNull] Func<int, int, float> sinkFunction,
            [NotNull] (float,float)[] phytomerAllometryDatas, 
            [NotNull] Func<int,Vector3,Vector3,float,(Vector3,Vector3)> phytomerTopologyFunc,
            [NotNull] Func<int,Vector3,Vector3,Vector3> axisTopologyFunc
            )
        {
#if UNITY_EDITOR
            //最大的生理年龄不能小于1
            if (maxPhysiologicalAge < 1)
            {
                throw new AggregateException("The value of 'maxPhysiologicalAge' must be greater than or equal to 1.");
            }
            // 有效周期>=1
            if (validCycles < 1)
            {
                throw new AggregateException("ValidCycles需要>=1");
                //TODO: 翻译成英语
            }
            
            // 测试传入的sinkFunction有效
            for (var i = 1; i <= maxPhysiologicalAge; i++)
            {
                for (var j = 1; j <= validCycles; j++)
                {
                    if (sinkFunction(i, j) < 0)
                    {
                        throw new AggregateException("The value produced by 'sinkFunction' with 输入 1~'maxPhysiologicalAge' must be greater than or equal to 0."); 
                        // TODO: 标准的英语
                    }
                }
            }
            // 异速生长参数的数量不对
            if (phytomerAllometryDatas.Length != maxPhysiologicalAge)
            {
                throw new AggregateException("异速生长参数的数量需与最大升力年龄相同，以对所有的生理年龄的叶元进行定义");
                // 翻译成英语
            }
            // 异速生长的b 要求 > 0
            foreach (var i in phytomerAllometryDatas)
            {
                if (i.Item1 <= 0)
                {
                    throw new AggregateException("异速生长的参数b必须>0。");
                }
            }
            
#endif
            MaxPhysiologicalAge = maxPhysiologicalAge;
            ValidCycles = validCycles;
            SinkFunction = sinkFunction;
            PhytomerAllometryDatas = phytomerAllometryDatas;
            PhytomerTopologyFunc = phytomerTopologyFunc;
            AxisTopologyFunc = axisTopologyFunc;
        }
    }
}