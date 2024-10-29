using System;
using UnityEngine;

public class Tester_firstLevel:MonoBehaviour
{
    private InAutomaton _inAutomaton;

    private void Awake()
    {
        // 一个单尺度自动机的例子
        var p1 = new Phytomer(){Name = "p1"};
        var p2 = new Phytomer(){Name = "p2"};
        var p3 = new Phytomer(){Name = "p3"};
        Phytomer[] vertices = new Phytomer[] {p1,p2,p3};
        int[] repeatTimes = new[] { 1, 0, 2 };
        float[,] adjMat = new float[,] { {0,0.5f,0.5f},{0,0.5f,0.5f},{0,0,0}};
        int entranceIndex = 0;
        _inAutomaton = new InAutomaton(vertices, repeatTimes, adjMat, entranceIndex,0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            print(_inAutomaton.Expansion().Value.Name);
        }
    }
}