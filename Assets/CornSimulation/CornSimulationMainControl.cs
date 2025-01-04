using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CornSimulationMainControl : MonoBehaviour
{
    public CornSimulation.CornSimulation[] CornSimulations;

    private int _age;

    private string _seedRecord = "RandomSeed = ";
    
    private void Awake()
    {
        foreach (var c in CornSimulations)
        {
            var seed = c.randomSeed = Random.Range(int.MinValue, int.MaxValue);
            _seedRecord += $"{seed}, ";
        }
    }

    private void OnGUI()
    {
        var guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;
        guiStyle.fontSize = 50;

        var guiStyle2 = new GUIStyle();
        guiStyle2.normal.textColor = Color.black;
        guiStyle2.fontSize = 35;

        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 50;
        buttonStyle.normal.textColor = Color.black;
        
        
        if (GUI.Button(new Rect(25, 100, 200, 100), "Growth",buttonStyle))
        {
            _age++;
            if (_age >= 43)
            {
                return;
            }

            foreach (var i in CornSimulations)
            {
                i.Growth(1.1f);
                //TODO: 该数值暂时没有办法被控制，需要加入一个控制数值量的输入口。
            }
        }

        if (GUI.Button(new Rect(25, 200, 200, 100), "Restart", buttonStyle))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        GUI.Label(new Rect(25,25,100,30),$"Age : {_age}",guiStyle);
        GUI.Label(new Rect(50,1000,100,30),_seedRecord,guiStyle2);
    }
    
}
