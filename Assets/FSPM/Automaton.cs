using System;
using UnityEngine;

// 注意：因为特殊要求，出度之和可以不为1！

// 可被作为图的元素的对象的接口
public interface Graphable { }

public class Automaton
{
    private int _stateNow ; // 当前处于的位置
    private int _stateRepeatTime = 0 ; // 重复次数
    private bool _budDead = false; // 芽死亡，转True后，无论多少次触发，结果永远为 Null
    
    private readonly Graphable[] _vertices; // 顶点列表
    private readonly int[] _repeatTimes; // 个体
    private readonly float[,] _adjMat; // 邻接矩阵

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="vertices">顶点对象</param>
    /// <param name="repeatTimes">重复次数</param>
    /// <param name="adjMat">邻接矩阵</param>
    /// <param name="dataCheck">是否对输入的数据进行检查</param>
    public Automaton(Graphable[] vertices,int[] repeatTimes, float[,] adjMat,int entranceIndex ,bool dataCheck = true)
    {
        if (dataCheck)
        {
            DataCheck(vertices,repeatTimes,adjMat,entranceIndex);
        }
        _vertices = vertices;
        _repeatTimes = repeatTimes;
        _adjMat = adjMat;
        _stateNow = entranceIndex;
        return;
    }
   
    
    public Graphable Next(float randomValue)
    {
        // 芽死亡时，怎么都不会产生新的对象了。
        if (_budDead) return null;
        
        // 
        var result = _vertices[_stateNow];
        _stateRepeatTime += 1;
        if (_stateRepeatTime < _repeatTimes[_stateNow]) return result;
        // 跳转状态
        var sumValue = 0.0f;
        for (var i = 0; i < _vertices.Length; i++)
        {
            sumValue += _adjMat[_stateNow, i];
            if (randomValue <= sumValue)
            {
                _stateNow = i;
                _stateRepeatTime = 0;
                return result;
            }
        }
        // 返回空，等于芽死亡
        _budDead = true;
        return null;
    }
    // 数据检查，如途中查找到错误会报错
    private static void DataCheck(Graphable[] vertices, int[] repeatTimes, float[,] adjMat, int entranceIndex)
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
        // 邻接矩阵 可能性 总值 = 1
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
            if (!Mathf.Approximately(sumValue, 1.0f))
            {
                Debug.LogWarning("注意有顶点的出度之和不为1，可能发生因为自动机的芽死亡");
            }
        }
        // 入口序列号不在范围内
        if (entranceIndex >= vertices.Length)
        {
            throw new Exception("入口序号不在范围内");
        }
    }
    
    
    
    
    /// <summary>
    /// 进入下一个态
    /// </summary>
    /// <returns></returns>
    /*
    public AbstractPhytomer Next()
    {
        
    }*/
}