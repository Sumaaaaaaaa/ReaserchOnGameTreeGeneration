using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoCloud : MonoBehaviour
{
    public float speed;
    public Vector2 speedRange;
    
    public Vector2 sizeRangeX;
    public Vector2 sizeRangeY;
    public Vector2 sizeRangeZ;
    
    private Transform[] _cloudsGameObjects;
    private float[] _targetSpeed;
    public float repeatDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取所有子云对象
        _cloudsGameObjects = new Transform[transform.childCount];
        for (var i = 0; i < transform.childCount; i++)
        {
            _cloudsGameObjects[i] = transform.GetChild(i);
        }

        _targetSpeed = new float[transform.childCount];
        for (var i = 0; i < transform.childCount; i++)
        {
            _targetSpeed[i] = Random.Range(speedRange.x, speedRange.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < _cloudsGameObjects.Length; i++)
        {
            var cloud = _cloudsGameObjects[i];
            var sp = _targetSpeed[i];
            
            cloud.Translate(Vector3.left * sp * Time.deltaTime);
            if (cloud.localPosition.x < sizeRangeX.x)
            {
                cloud.localPosition = new Vector3(sizeRangeX.y, Random.Range(sizeRangeY.x, sizeRangeY.y),
                    Random.Range(sizeRangeZ.x, sizeRangeZ.y));
                _targetSpeed[i] = Random.Range(speedRange.x, speedRange.y);
            }
        }
    }

    public void CustomUpdate()
    {
        
    }
}
