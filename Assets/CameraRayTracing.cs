using System;
using UnityEngine;
using UnlimitedGreen;
public class CameraRayTracing : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.TryGetComponent<PruningTarget>(out var p))
                {
                    p.Pruning(raycastHit.point);
                }
            }
        }
    }
}
