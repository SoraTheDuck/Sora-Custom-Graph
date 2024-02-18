using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sora_Ults;

public class GraphVisualize : MonoBehaviour
{
    public SelectedAlgorithm selectedAlgorithm;
    
    [Range(0f, 20f)]
    public float frequency = 1f;
    
    [Range(0f, 2f)]
    public float damping = 0.5f;

    [Range(-5f, 5f)]
    public float Responsiveness = 0f;
    
    [Range(0, 100f)]
    public float DeltaTime = 0f;

    public void UpdateOnGUI(float? frequency , float? damping , float? Responsiveness,float? DeltaTime,SelectedAlgorithm selectedAlgorithm)
    {
        this.frequency = frequency.GetValueOrDefault(this.frequency);
        this.damping = damping.GetValueOrDefault(this.damping);
        this.Responsiveness = Responsiveness.GetValueOrDefault(this.Responsiveness);
        this.selectedAlgorithm = selectedAlgorithm;
       // this.DeltaTime = DeltaTime.GetValueOrDefault(this.DeltaTime);

    }
}
