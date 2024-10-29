

using System.Collections.Generic;
using UnityEngine;

public class OutAutomaton : DualAutomaton
{
    
    private readonly InAutomaton[] _vertices; // 顶点列表
    
    public OutAutomaton(InAutomaton[] vertices, int[] repeatTimes, float[,] adjMat, int entranceIndex,int randomSeed,
        bool dataCheck = true) :
        base(repeatTimes, adjMat, entranceIndex, randomSeed)
    {
        if (dataCheck)
        {
            DataCheck<InAutomaton>(vertices,repeatTimes,adjMat,entranceIndex);
        }
        _vertices = vertices;
    }
    
    public Phytomer?[] Expansion(int times)
    {
        List<Phytomer?> results = new List<Phytomer?>();
        
        // 芽死亡时，怎么都不会产生新的对象了。
        if (_budDead) return null;
        
        // 入口
        if (_stateNow == -1)
        {
            _stateNow = _enterStateIndex;
            for (var i = 0; i < times; i++)
            {
                results.Add(_vertices[_stateNow].Expansion());
            }
            return results.ToArray();
        }
        
        //重复
        if (_stateRepeatTime < _repeatTimes[_stateNow])
        {
            _stateRepeatTime ++;
            for (var i = 0; i < times; i++)
            {
                results.Add(_vertices[_stateNow].Expansion());
            }
            return results.ToArray();
        }
        
        // 跳转状态
        var sumValue = 0.0f;
        for (var i = 0; i < _vertices.Length; i++)
        {
            sumValue += _adjMat[_stateNow, i];
            if (_random.NextDouble() <= sumValue)
            {
                _stateNow = i;
                _stateRepeatTime = 0;
                for (var t = 0; t < times; t++)
                {
                    results.Add(_vertices[_stateNow].Expansion());
                }
                return results.ToArray();
            }
        }
        
        // 没有能成功扩展，返回空，等于芽死亡
        _budDead = true;
        return null;
    }
}


