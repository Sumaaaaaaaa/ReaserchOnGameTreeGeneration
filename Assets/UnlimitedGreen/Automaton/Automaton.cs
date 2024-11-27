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
            if (vertices.Length == 0 | repeatTimes.Length == 0 | adjMat.Length == 0)
            {
                throw new Exception("Input data is empty. Please ensure all inputs contain valid data.");
            }
            // 数据的数量要相互之间匹配
            if ((repeatTimes.Length != vertices.Length) | (adjMat.Length != vertices.Length * vertices.Length))
            {
                throw new Exception("Data size mismatch. Ensure repeat times, vertices, and adjacency matrix are properly aligned.");
            }
            // 重复次数不能为负数
            foreach (var i in repeatTimes)
            {
                if (i < 0)
                {
                    throw new Exception("Negative repeat count encountered. Repeat times must be non-negative.");
                }
            }
            // 邻接矩阵中的可能性不能为负数 
            for (var x = 0; x < vertices.Length; x++)
            {
                var sumValue = 0.0f;
                for (var y = 0; y < vertices.Length; y++)
                {
                    sumValue += adjMat[x, y];
                    if (adjMat[x, y] < 0)
                    {
                        throw new Exception("Negative value detected in adjacency matrix. All probabilities must be non-negative.");
                    }
                }
                // 邻接矩阵 可能性 总值 不能  > 1
                if (sumValue > 1.0f)
                {
                    throw new Exception("Total probability exceeds 1.0. Ensure sum of probabilities for each vertex does not exceed 1.0.");
                }
            }
#endif
            RepeatTimes = repeatTimes;
            AdjMat = adjMat;
            Vertices = vertices;
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