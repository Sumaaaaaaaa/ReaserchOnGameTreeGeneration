using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tester: MonoBehaviour
{
    private InAutomaton _inAutomaton;
    private OutAutomaton _outAutomaton;
    
    private void Awake()
    {
        // 一个单尺度自动机的例子
        
        var p1 = new Phytomer(){Name = "p1"};
        var p2 = new Phytomer(){Name = "p2"};
        var p3 = new Phytomer(){Name = "p3"};
        Phytomer[] vertices = new Phytomer[] {p1,p2,p3};
        int[] repeatTimes = new[] { 2, 0, 3 };
        float[,] adjMat = new float[,] { {0,0.5f,0.5f},{0,0.5f,0.5f},{0,0,0}};
        int entranceIndex = 0;
        _inAutomaton = new InAutomaton(vertices, repeatTimes, adjMat, entranceIndex,0);
        
        
        // 一个双尺度自动机的例子
        /*
        var p1 = new Phytomer() { Name = "P1" };
        var p2 = new Phytomer() { Name = "P2" };
        Phytomer[] vertices = new Phytomer[] {p1,p2};
        int[] repeatTimes = new[] { 2, 0 };
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
        
        //TODO: 完成这个双尺度的例子

        for (int i = 0; i < 6; i++)
        {
            print(automatonB.Expansion().Value.Name);
        }
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var lll = _inAutomaton.Expansion();
            print($"{lll.Value.Name}");
        }
    }
}
