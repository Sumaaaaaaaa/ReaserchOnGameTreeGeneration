using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightType = UnityEngine.LightType;

public class LightIntensity
{
    static public float GetLightIntensity(Vector3 worldPosition)
    {
        // 设置Layer
        int mask = ~0; 
        mask &= ~(1 << 3); 
        
        var intensity = 0.0f;
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        foreach (var light in lights)
        {
            switch (light.type)
            {
                case LightType.Directional:
                    // 若射线检测到碰撞，跳过
                    if (Physics.Raycast(origin: worldPosition, direction: -light.transform.forward,maxDistance:float.MaxValue,layerMask:mask))
                    {
                        Debug.DrawRay(worldPosition,-light.transform.forward,Color.red,1.0f);
                        continue; 
                    }
                    Debug.DrawRay(worldPosition,-light.transform.forward,Color.green,1.0f);
                    intensity += CalculateLightIntensity(light, 1); // 计算光量
                    break;
                
                case LightType.Point:
                    
                    // 若超过距离，跳过
                    if ((light.transform.position - worldPosition).sqrMagnitude > light.range*light.range)
                    {
                        continue;
                    }
                    // 若射线检测到碰撞，跳过
                    if (Physics.Raycast(origin: worldPosition, 
                            direction: (light.transform.position - worldPosition).normalized,maxDistance:float.MaxValue,layerMask:mask))
                    {
                        Debug.DrawLine(worldPosition,light.transform.position,Color.red,1.0f);
                        continue; 
                    }
                    Debug.DrawLine(worldPosition,light.transform.position,Color.green,1.0f);
                    intensity += CalculateLightIntensity(light, CalculateLightAttenuation(worldPosition,light)); // 计算光量
                    
                    break;
                
                case LightType.Spot:
                    
                    // 若超过距离，跳过
                    if ((light.transform.position - worldPosition).sqrMagnitude > light.range*light.range)
                    {
                        continue;
                    }
                    // 若射线检测到碰撞，跳过
                    if (Physics.Raycast(origin: worldPosition, 
                            direction: (light.transform.position - worldPosition).normalized,
                            maxDistance:float.MaxValue,layerMask:mask))
                    {
                        Debug.DrawLine(worldPosition,light.transform.position,Color.red,1.0f);
                        continue; 
                    }
                    
                    // 若不在spot范围内，跳过
                    var toLightVector = (light.transform.position - worldPosition).normalized;
                    var lightVector = -light.transform.forward;
                    var dotValue = Vector3.Dot(toLightVector.normalized, lightVector.normalized);
                    var compareValue = Mathf.Cos(light.spotAngle * Mathf.Deg2Rad * 0.5f);
                    if (dotValue <= compareValue)
                    {
                        Debug.DrawLine(worldPosition,light.transform.position,Color.yellow,1.0f);
                        continue;
                    }
                    
                    Debug.DrawLine(worldPosition,light.transform.position,Color.green,1.0f);
                    intensity += CalculateLightIntensity(light, CalculateLightAttenuation(worldPosition,light)); // 计算光量
                    
                    
                    break;
            }
        }

        return intensity;
    }

    private static float CalculateLightIntensity(Light light, float attenuation)
    {
        /*
         * var lightColor = (light.intensity * light.color);
                    var lightBrightness = lightColor.r * 0.7152f + lightColor.b * 0.0722f;
                    intensity += lightBrightness * 1;
         */
        var lightColor = light.intensity * light.color;
        var lightBrightness = lightColor.r * 0.7152f + lightColor.b * 0.0722f;
        return lightBrightness * attenuation;
    }

    private static float CalculateLightAttenuation(Vector3 positon,Light light)
    {
        var toLightVector = light.transform.position - positon;
        var toLightVectorLength = toLightVector.magnitude;
        var attenuation = Mathf.Clamp01(Mathf.Lerp(1, 0, toLightVectorLength / light.range));
        attenuation *= attenuation;
        return attenuation;
    }
}
