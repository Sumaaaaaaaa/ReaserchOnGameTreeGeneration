using System;
using UnityEngine;

namespace Model.Automaton
{
    // 面向使用者的定义用的类
    public abstract class Automaton
    {
        public readonly int[] RepeatTimes; // 个体
        public readonly float[,] AdjMat; // 邻接矩阵
        public readonly int EnterStateIndex; // 入口态的序号

        protected Automaton(int[] repeatTimes, float[,] adjMat,int entranceIndex)
        {
            RepeatTimes = repeatTimes;
            AdjMat = adjMat;
            EnterStateIndex = entranceIndex;
        }
        
        // 数据检查，如途中查找到错误会报错
        protected static void DataCheck<T>(T[] vertices, int[] repeatTimes, float[,] adjMat, int entranceIndex)
        {
            // 长度检查
            // 是否为空
            if (vertices.Length == 0 | repeatTimes.Length == 0 | adjMat.Length == 0)
            {
                throw new Exception("数据为空");
            }
            // 长度匹配
            if ((repeatTimes.Length != vertices.Length) | (adjMat.Length != vertices.Length * vertices.Length))
            {
                throw new Exception("数据大小不匹配");
            }
        
            // 重复次数为负数
            foreach (var i in repeatTimes)
            {
                if (i < 0)
                {
                    throw new Exception("重复次数不该为负数");
                }
            }
        
            // 邻接矩阵 不能为负数 
        
            for (var x = 0; x < vertices.Length; x++)
            {
                var sumValue = 0.0f;
                for (var y = 0; y < vertices.Length; y++)
                {
                    sumValue += adjMat[x, y];
                    if (adjMat[x, y] < 0)
                    {
                        throw new Exception("邻接举证参数不能为负数");
                    }
                }
                // 邻接矩阵 可能性 总值 > 1
                if (sumValue > 1.0f)
                {
                    throw new Exception("可能性的总值不能大于1.0");
                }
                if (!Mathf.Approximately(sumValue, 1.0f))
                {
                    // 邻接矩阵 可能性 总值 = 1
                    Debug.LogWarning("注意有顶点的出度之和不为1，可能发生因为自动机的芽死亡");
                }
            }
            // 入口序列号不在范围内
            if (entranceIndex >= vertices.Length)
            {
                throw new Exception("入口序号不在范围内");
            }
        }
    }
    
}