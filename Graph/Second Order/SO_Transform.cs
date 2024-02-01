using System;
using UnityEngine;
using Sora_Ults;
using Unity.Collections;

public class SO_Transform : MonoBehaviour
{
    public SelectedAlgorithm ASelector;
    private GraphVisualize GV;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float responsiveness = 0f;
    [SerializeField] private float deltaTimeScale = 1f;
    [SerializeField] private Transform target;
    [SerializeField] private SO_Position_Handler positionHandler = new SO_Position_Handler();
    private Vector3 initialPosition;
    private void Awake()
    {
        //if (GV != null) ASelector = GV.selectedAlgorithm;
        GV = this.gameObject.GetComponent<GraphVisualize>();
        
        initialPosition = transform.position;
        positionHandler.Initialize(frequency, damping, responsiveness, deltaTimeScale, initialPosition, ASelector,target.transform.position);
    }

    private void OnValidate()
    {
        initialPosition = transform.position;
        positionHandler.Initialize(frequency, damping, responsiveness, deltaTimeScale, initialPosition, ASelector, target.transform.position);
    }

    private void LateUpdate()
    {
        transform.position =  positionHandler.UpdatePosition_2(target.position, transform);
    }
    
    
}