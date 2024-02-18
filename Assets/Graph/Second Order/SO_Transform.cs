using System;
using UnityEngine;
using Sora_Ults;
using Unity.Collections;

public class SO_Transform : MonoBehaviour
{
    public SelectedAlgorithm ASelector;
    public GraphVisualize GV;
    [SerializeField] public float frequency = 1f;
    [SerializeField] public float damping = 1f;
    [SerializeField] public float responsiveness = 0.2f;
    [SerializeField] public float deltaTimeScale = 1f;
    [SerializeField] private Transform target;
    [SerializeField] private SO_Position_Handler positionHandler = new SO_Position_Handler();
    private Vector3 initialPosition;
    private void Awake()
    {

        
        GV = this.gameObject.GetComponent<GraphVisualize>();
        
        initialPosition = transform.position;
        positionHandler.Initialize(frequency, damping, responsiveness, deltaTimeScale, initialPosition, ASelector,target.transform.position);


    }

    private void OnValidate()
    {
        initialPosition = transform.position;
        

        positionHandler.Initialize(frequency, damping, responsiveness, deltaTimeScale, initialPosition, ASelector, target.transform.position);
        GV.UpdateOnGUI(frequency, damping, responsiveness, null, ASelector);

    }

    private void LateUpdate()
    {
        transform.position= positionHandler.UpdatePosition_2(target.position);
    }
    
    
}