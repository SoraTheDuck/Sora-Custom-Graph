using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sora_Ults;

[CustomEditor(typeof(GraphVisualize))]
public class GraphInspector : Editor
{
    private AlgorithmSelector_SO algorithmSelector = new AlgorithmSelector_SO();
    private ISecondOrderSystem _selectedAlgorithm = new SO_Calc_None();

    public Graph _graph;
    //private SO_Calc_ZeroPole _soCalcZeroPole = new SO_Calc_ZeroPole();
    private SerializedObject _serializedObject;
    private SerializedProperty _frequencyProperty;
    private SerializedProperty _dampingProperty;
    private SerializedProperty _ResponsivenessProperty;
    private SerializedProperty _DeltaTimeScaleProperty;

    [Range(0, 50)] [SerializeField] float _graphMaxX = 10;
    [Range(0, 30)] [SerializeField] float _graphMaxY = 1;

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _frequencyProperty = _serializedObject.FindProperty("frequency");
        _dampingProperty = _serializedObject.FindProperty("damping");
        _ResponsivenessProperty = _serializedObject.FindProperty("Responsiveness");
        _DeltaTimeScaleProperty = _serializedObject.FindProperty("DeltaTime");
    }
    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUILayout.LabelField("Graphic Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        GraphDetail = EditorGUILayout.IntSlider("Graph Detail", GraphDetail, 0, 10000);
        _graphMaxX = EditorGUILayout.Slider("Max X", _graphMaxX, 0, 50);
        _graphMaxY = EditorGUILayout.Slider("Max Y", _graphMaxY, 0, 30);

        _graph = new Graph(0, _graphMaxX, 0, _graphMaxY);

        _graph.HorizontalAxisUnits = "s";
        _graph.LabelStyle = "label";

        EditorGUILayout.Space(5);
        // Draw the sine graph in the Inspector
        UpdateGraphLine();
        DrawGraph();

        GUILayout.Space(15);
        // Access and modify the selected algorithm directly from the target object
        GraphVisualize graphVisualize = (GraphVisualize)target;
        graphVisualize.selectedAlgorithm = (SelectedAlgorithm)EditorGUILayout.EnumPopup("Selected Algorithm", graphVisualize.selectedAlgorithm);
        GUILayout.Space(10);

        // Display the properties below the graph
        EditorGUILayout.PropertyField(_frequencyProperty);
        EditorGUILayout.PropertyField(_dampingProperty);
        EditorGUILayout.PropertyField(_ResponsivenessProperty);
        EditorGUILayout.PropertyField(_DeltaTimeScaleProperty);

        _serializedObject.ApplyModifiedProperties();
    }
    private void DrawGraph()
    {
        GUILayout.Label("Graph", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        Rect graphRect = GUILayoutUtility.GetRect(0, 100, GUILayout.ExpandWidth(true));
        _graph.Draw(graphRect);
        if (GUI.changed) Repaint();
    }

    [Range(0, 10000)]
    [SerializeField] int GraphDetail = 1000;
    private float[] ySamples;
    private int currentIndex = 0;

    public void UpdateGraphLine()
    {
        //Example 
        //ySamples = CalculateLine_Sin();
        //_graph.UpdateLine("line1", ySamples, Color.yellow);

        ySamples = CalculateLine_SelectedAlgorithm();
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

    ISecondOrderSystem selectedAlgorithm;
    private GraphVisualize graphVisualize;
    void SelectAlgorithm()
    {
        graphVisualize = (GraphVisualize)target;
        switch (graphVisualize.selectedAlgorithm)
        {
            case SelectedAlgorithm.ZeroPole:
                selectedAlgorithm = new SO_Calc_ZeroPole();
                break;
            case SelectedAlgorithm.StableForcedIterations:
                selectedAlgorithm = new SO_Calc_StableForcedIterations();
                break;
            case SelectedAlgorithm.Euler:
                selectedAlgorithm = new SO_Calc_Euler();
                break;
            case SelectedAlgorithm.None:
                selectedAlgorithm = new SO_Calc_None();
                break;
            default:
                Debug.Log("pick plz");
                selectedAlgorithm = new SO_Calc_None();
                break;
        }
    }

    private float[] CalculateLine_SelectedAlgorithm()
    {
        SelectAlgorithm();

        float[] LineArray = new float[GraphDetail];
        SecondOrderState state = new SecondOrderState
        {
            F = _frequencyProperty.floatValue,
            Z = _dampingProperty.floatValue,
            R = _ResponsivenessProperty.floatValue,
            InitialValue = 0.0f,
            DeltaTime = _DeltaTimeScaleProperty.floatValue,
            TargetValue = 1.0f,
        };

        selectedAlgorithm.RecalculateConstants(ref state, state.InitialValue);

        for (int i = 0; i < GraphDetail; i++)
        {
            LineArray[i] = selectedAlgorithm.UpdateStrategy(ref state, state.DeltaTime, state.TargetValue, null);
        }

        return LineArray;
    }
}
