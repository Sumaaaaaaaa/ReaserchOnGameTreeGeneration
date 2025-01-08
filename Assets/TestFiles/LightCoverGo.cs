using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCoverGo : MonoBehaviour
{
    [Range(0f, 3f)] public float speed;
    private Coroutine _coroutine;
    
    public void GoCenter()
    {
        if (_coroutine is not null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Go(new Vector3(0, 4.32f, 0)));
    }

    public void GoLeft()
    {
        //0.92
        if (_coroutine is not null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Go(new Vector3(0.92f, 4.32f, 0)));
    }

    public void GoRight()
    {
        if (_coroutine is not null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Go(new Vector3(-0.92f, 4.32f, 0)));
    }

    public void Leave()
    {
        if (_coroutine is not null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Go(new Vector3(7.31f, 4.32f, 0)));
    }

    private IEnumerator Go(Vector3 position)
    {
        while (true)
        {
            if (transform.position == position)
            {
                yield break;
            }
            transform.position += (position - transform.position) * speed * Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     GoLeft();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     GoRight();
        // }
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     GoCenter();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     Leave();
        // }
    }
}
