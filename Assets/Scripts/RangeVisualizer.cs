using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeVisualizer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int subdivisions;

    public void ShowRange(float radius)
    {
        float angleStep = 2f * Mathf.PI / subdivisions;
        lineRenderer.positionCount = subdivisions + 1;
        
        for (int i = 0; i < subdivisions + 1; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }
}
