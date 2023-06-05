using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVisualize : MonoBehaviour
{
    [Range(0f, 20f)]
    public float frequency = 1f;
    
    [Range(0f, 2f)]
    public float damping = 0.5f;

    [Range(-5f, 5f)]
    public float Responsiveness = 0f;
}
