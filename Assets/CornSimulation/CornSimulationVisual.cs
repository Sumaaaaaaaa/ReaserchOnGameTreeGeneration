using System;
using System.Collections;
using System.Collections.Generic;
using TestFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CornSimulationVisual : MonoBehaviour
{
    private CornSimulation.CornSimulation[] _corns;
    private TestB[] _testBs;
    public float e;
    private void Awake()
    {
        _corns = Object.FindObjectsOfType<CornSimulation.CornSimulation>();
        foreach (var corn in _corns)
        {
            corn.randomSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        _testBs = Object.FindObjectsOfType<TestB>();

    }

    private void Start()
    {
        StartCoroutine(IEGrowth());
        print("start");
    }

    IEnumerator IEGrowth()
    {
        print("IEGrowth");
        var i = 0;
        while (i < 45)
        {
            if (i <= 33)
            {
                foreach (var corn in _corns)
                {
                    corn.Growth(e);
                    yield return null;
                }
            }

            
            foreach (var tree in _testBs)
            {
                tree.Growth(e);
                yield return null;
            }
            i++;
            print(i);
        }
        yield break;
    }
}
