using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(Renderer))]
public class LightTester : MonoBehaviour
{

    public Material material;


    private void Awake()
    {
    }
    private void Update()
    {
        //print(Time.time);
        //material.SetFloat("_MyDynamicValue", Time.time);
        float currentShaderValue = material.GetFloat("_MyDynamicValue");
        print(currentShaderValue);
    }
    private void OnGUI()
    {
        //GUILayout.Label(transform.position.ToString());
        //GUILayout.Label(Shader.GetGlobalFloat("testValue").ToString());
    }
    private void OnDrawGizmos()
    {
    }
}
