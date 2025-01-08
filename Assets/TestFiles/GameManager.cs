using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameSpeed;
    public bool doRender;

    private void Update()
    {
        Time.timeScale = gameSpeed;
    }
}
