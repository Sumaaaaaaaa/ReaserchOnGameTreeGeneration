﻿namespace Model.Class.Automaton
{
    public class OutAutomaton : Automaton
    {
        public readonly InAutomaton[] Vertices; // 顶点列表
        
        public OutAutomaton(InAutomaton[] vertices,int[] repeatTimes, float[,] adjMat, int entranceIndex, bool dataCheck = true) 
            : base(repeatTimes, adjMat, entranceIndex)
        {
            if (dataCheck)
            {
                DataCheck(vertices,repeatTimes,adjMat,entranceIndex);
            }
            Vertices = vertices;
        }
    }
}