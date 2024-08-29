using UnityEngine;
using System.Collections;

public class LightIntensityTool : MonoBehaviour 
{
    private void OnDrawGizmos()
    {
        var lightValue = LightIntensity.GetLightIntensity(transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,0.05f);

        var style = new GUIStyle();
        style.normal.textColor = Color.red;
        UnityEditor.Handles.Label(transform.position+Vector3.up*0.2f, lightValue.ToString(),style);
    }
}