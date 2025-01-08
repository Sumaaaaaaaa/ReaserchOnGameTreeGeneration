using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMove : MonoBehaviour
{
    public float speed;

    private Color _originalColor;

    private void Start()
    {
        _originalColor = RenderSettings.fogColor;
    }

    private void Update()
    {
        transform.Rotate(speed*Time.deltaTime,0,0);

        var lambertsResult= Vector3.Dot(transform.forward, Vector3.up);
        lambertsResult /= 2;
        lambertsResult += 0.5f;
        lambertsResult = 1 - lambertsResult;
        RenderSettings.fogColor = Color.Lerp(Color.black, _originalColor, lambertsResult);

    }
}
