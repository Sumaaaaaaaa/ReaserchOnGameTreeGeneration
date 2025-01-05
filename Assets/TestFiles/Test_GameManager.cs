using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_GameManager : MonoBehaviour
{
    private int _age;

    public tester tester;

    public LightCoverGo LightCoverGo;

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
            tester.Growth();
        }
        if (GUI.Button(new Rect(25, 200, 200, 100), "Restart", buttonStyle))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        GUI.Label(new Rect(25, 330, 100, 300), "遮光",guiStyle);
        if (GUI.Button(new Rect(25, 400, 200, 100), "左", buttonStyle))
        {
            LightCoverGo?.GoLeft();
        }
        if (GUI.Button(new Rect(25, 500, 200, 100), "中心", buttonStyle))
        {
            LightCoverGo?.GoCenter();
        }
        if (GUI.Button(new Rect(25, 600, 200, 100), "右", buttonStyle))
        {
            LightCoverGo?.GoRight();
        }
        if (GUI.Button(new Rect(25, 700, 200, 100), "なし", buttonStyle))
        {
            LightCoverGo?.Leave();
        }

        
        GUI.Label(new Rect(25,25,100,30),$"Age : {_age}",guiStyle);
    }
}
