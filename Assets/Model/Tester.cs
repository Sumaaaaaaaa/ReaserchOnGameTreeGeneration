using System;
using Model.Class.Automaton;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tester: MonoBehaviour
{
    
    private OutAutomaton _outAutomaton;
    
    private void Awake()
    {
        // 一个双尺度自动机的例子
        
        var p1 = new Phytomer() { Name = "P1" };
        var p2 = new Phytomer() { Name = "P2" };
        Phytomer[] vertices = new Phytomer[] {p1,p2};
        int[] repeatTimes = new[] { 1, 0 };
        float[,] adjMat = new float[,] {{0,1},{1,0}};
        int entranceIndex = 0;
        var automatonA = new InAutomaton(vertices, repeatTimes, adjMat,0, entranceIndex);
        
        var p3 = new Phytomer() { Name = "P3" };
        var p4 = new Phytomer() { Name = "P4" };
        var p5 = new Phytomer() { Name = "P5" };
        vertices = new Phytomer[] { p3, p4, p5 };
        repeatTimes = new[] { 0, 0, 0 };
        adjMat = new float[,] {{0,0.5f,0.5f},{0,0,0},{0,0,0} };
        entranceIndex = 0;
        var automatonB = new InAutomaton(vertices, repeatTimes, adjMat,0, entranceIndex);
        
        _outAutomaton = new OutAutomaton(
            vertices: new InAutomaton[] { automatonA, automatonB },
            repeatTimes:new int[]{2,0},
            adjMat:new float[,]{{0,1},{0,0}},
            entranceIndex:0,
            0
        );
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            var result = _outAutomaton.Expansion(3);
            var printString = "";
            for (var i = 0; i < result.Length; i++)
            {
                printString += result[i] is null ? "null" : $"{result[i].Value.Name}/";
            }
            print(printString);
            
        }
        
        
    }
}
