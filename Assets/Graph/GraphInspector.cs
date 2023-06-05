using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using Sora_Ults;

[CustomEditor(typeof(GraphVisualize))]
public class GraphInspector : Editor
{
    public Graph _graph;
    private SO_Calc soCalc = new SO_Calc();
    private SerializedObject _serializedObject;
    private SerializedProperty _frequencyProperty;
    private SerializedProperty _dampingProperty;
    private SerializedProperty _ResponsivenessProperty;
    
    private void OnEnable()
    {
        _graph = new Graph(0, 10, 0, 1);
        _graph.HorizontalAxisUnits = "s";
        _graph.LabelStyle = "label";
        
        _serializedObject = new SerializedObject(target);
        _frequencyProperty = _serializedObject.FindProperty("frequency");
        _dampingProperty = _serializedObject.FindProperty("damping");
        _ResponsivenessProperty = _serializedObject.FindProperty("Responsiveness");
        
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();
        GraphDetail = EditorGUILayout.IntSlider("Graph Detail", GraphDetail, 0, 10000);

        // Draw the sine graph in the Inspector
        UpdateGraphLine();
        DrawGraph();
        
        _serializedObject.ApplyModifiedProperties();
    }
    private void DrawGraph()
    {
        EditorGUILayout.Space(10);
        // Use the labelStyle here:
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        GUILayout.Label("Graph", labelStyle);
        Rect graphRect = GUILayoutUtility.GetRect(0, 100, GUILayout.ExpandWidth(true));
        _graph.Draw(graphRect);
        if (GUI.changed)  Repaint();
        GUILayout.Space(20);
        
        // Display the properties below the graph
        EditorGUILayout.PropertyField(_frequencyProperty);
        EditorGUILayout.PropertyField(_dampingProperty);
        EditorGUILayout.PropertyField(_ResponsivenessProperty);
    }
    
    [Range(0,10000)]
    [SerializeField] int GraphDetail = 1000;
    private float[] ySamples;
    private int currentIndex = 0;

    public void UpdateGraphLine()
    {
        //Example 
        //ySamples = CalculateLine_Sin();
        //_graph.UpdateLine("line1", ySamples, Color.yellow);
        
        ySamples = CalculateLine_SO_ZeroPole();
        _graph.UpdateLine("line2", ySamples, Color.yellow);
    }
    
    //example, to print anything owo
    private float[] CalculateLine_Sin()
    {
        if (ySamples == null || ySamples.Length != GraphDetail)
        {
            ySamples = new float[GraphDetail];
        }
    
        float frequency = _frequencyProperty.floatValue;
        // Calculate new y samples for the graph
        for (int i = 0; i < GraphDetail; i++)
        {
            float x = i / 10f * frequency;
            ySamples[(currentIndex + i) % GraphDetail] = Mathf.Sin(x);
        }
        currentIndex = (currentIndex + GraphDetail) % GraphDetail;

        return ySamples;
    }
    private float[] CalculateLine_SO_ZeroPole()
    {
        float[] LineArray = new float[GraphDetail];
        SecondOrderState state = new SecondOrderState
        {
            F = _frequencyProperty.floatValue,
            Z = _dampingProperty.floatValue,
            R = _ResponsivenessProperty.floatValue,
            InitialValue = 0.0f,
            DeltaTime = 0.01f,
            TargetValue = 1.0f,
        };

        soCalc.RecalculateConstants(out state.W, out state.D, out state.K1, out state.K2, out state.K3, state.Z, state.F, state.R);

        for (int i = 0; i < GraphDetail; i++)
        {
            LineArray[i] = soCalc.UpdateStrategy(ref state, state.DeltaTime, state.TargetValue, null);
        }

        return LineArray;
    }
}

