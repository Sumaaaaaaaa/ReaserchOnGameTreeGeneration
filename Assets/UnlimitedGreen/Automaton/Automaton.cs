using System;
using UnityEngine;

namespace UnlimitedGreen
{
    public abstract class Automaton<T>
    {
        public readonly int[] RepeatTimes; // 个体
        public readonly float[,] AdjMat; // 邻接矩阵
        public readonly T[] Vertices; // 顶点集
        
        protected Automaton(int[] repeatTimes, float[,] adjMat, T[] vertices)
        {
            #if UNITY_EDITOR
            DataCheck();
            #endif
            RepeatTimes = repeatTimes;
            AdjMat = adjMat;
            Vertices = vertices; // TODO: 这个还没有进行检查
        }
  
        // 数据检查，如途中查找到错误会报错
        private void DataCheck()
        {
            var adjMat = AdjMat;
            // 长度检查
            // 三个参数都不可以为空
            if (Vertices.Length == 0 | RepeatTimes.Length == 0 | adjMat.Length == 0)
            {
                //TODO: 标准的英语翻译
                throw new Exception("数据为空");
            }
            // 数据的数量要相互之间匹配
            if ((RepeatTimes.Length != Vertices.Length) | (adjMat.Length != Vertices.Length * Vertices.Length))
            {
                throw new Exception("数据大小不匹配");
            }
        
            // 重复次数不能为负数
            foreach (var i in RepeatTimes)
            {
                if (i < 0)
                {
                    throw new Exception("重复次数不该为负数");
                }
            }
        
            // 邻接矩阵中的可能性不能为负数 
        
            for (var x = 0; x < Vertices.Length; x++)
            {
                var sumValue = 0.0f;
                for (var y = 0; y < Vertices.Length; y++)
                {
                    sumValue += adjMat[x, y];
                    if (adjMat[x, y] < 0)
                    {
                        throw new Exception("邻接举证参数不能为负数");
                    }
                }
                // 邻接矩阵 可能性 总值 不能  > 1
                if (sumValue > 1.0f)
                {
                    throw new Exception("可能性的总值不能大于1.0");
                }
                
                //OPT: 后续可以删掉这部分，不是必要的
                if (!Mathf.Approximately(sumValue, 1.0f))
                {
                    // 邻接矩阵 可能性 总值 = 1
                    Debug.LogWarning("注意有顶点的出度之和不为1，可能发生因为自动机的芽死亡");
                }
            }
        }

        internal int Count => Vertices.Length;
    }
    public class InAutomaton : Automaton<Phytomer>
    {
        public InAutomaton(int[] repeatTimes, float[,] adjMat, Phytomer[] vertices) : base(repeatTimes, adjMat, vertices)
        {
        }
    }

    public class DualScaleAutomaton : Automaton<InAutomaton>
    {
        public DualScaleAutomaton(int[] repeatTimes, float[,] adjMat, InAutomaton[] vertices) : base(repeatTimes, adjMat, vertices)
        {
        }
    }
}