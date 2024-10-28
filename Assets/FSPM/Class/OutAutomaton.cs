

using System.Collections.Generic;

public class OutAutomaton : DualAutomaton
{
    
    private readonly InAutomaton[] _vertices; // 顶点列表
    
    public OutAutomaton(InAutomaton[] vertices, int[] repeatTimes, float[,] adjMat, int entranceIndex,int randomSeed,
        bool dataCheck = true) :
        base(repeatTimes, adjMat, entranceIndex, randomSeed,dataCheck)
    {
        if (dataCheck)
        {
            DataCheck<InAutomaton>(vertices,repeatTimes,adjMat,entranceIndex);
        }
        _vertices = vertices;
    }
    
    //public Phytomer?[] Expansion(float randomValue,int times)
    //{
        //TODO: 完成这个
    //}
}


