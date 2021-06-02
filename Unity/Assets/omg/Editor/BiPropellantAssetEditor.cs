using System;
using System.Collections.Generic;
using omg.Runtime;
using Simulation.Engines.Liquid;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BiPropellantAsset))]
public class BiPropellantAssetEditor : Editor
{
    SerializedProperty oxidizer;
    SerializedProperty fuel;
    SerializedProperty hypergolic;
    SerializedProperty datasetFile;

    private GUIStyle iconButtonStyle;
    private GUIContent removeButton;
    
    public void OnEnable()
    {
        oxidizer = serializedObject.FindProperty(nameof(oxidizer));
        fuel = serializedObject.FindProperty(nameof(fuel));
        hypergolic = serializedObject.FindProperty(nameof(hypergolic));
        datasetFile = serializedObject.FindProperty(nameof(datasetFile));
        iconButtonStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("IconButton");
        removeButton = EditorGUIUtility.IconContent("Toolbar Minus");
    }

    public override void OnInspectorGUI()
    {
        var biprop = target as BiPropellantAsset;
        if (biprop == null) return;
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(oxidizer);
        EditorGUILayout.PropertyField(fuel);
        EditorGUILayout.PropertyField(hypergolic);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Contains data for {biprop.sortedKeys.Count} propellant ratios");
        EditorGUILayout.LabelField("Minimum O:F ratio", $"{biprop.GetLowestAvailableRatio():F}:1");
        EditorGUILayout.LabelField("Maximum O:F ratio", $"{biprop.GetHighestAvailableRatio():F}:1");
        EditorGUILayout.LabelField("Minimum Combustion Chamber Pressure", $"{biprop.GetLowestAvailableChamberPressure()/100000:N} Bar");
        EditorGUILayout.LabelField("Maximum Combustion Chamber Pressure", $"{biprop.GetHighestAvailableChamberPressure()/100000:N} Bar");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Re-Initialize Data", GUILayout.Width(150)))
        {
            biprop.Initialize();
        }
        if (GUILayout.Button("Export CFG", GUILayout.Width(150)))
        {
            biprop.WriteDataToConfigNode();
        }
        EditorGUILayout.EndHorizontal();

        if (biprop.thermodynamicData == null)
        {
            biprop.thermodynamicData = new List<ThermodynamicPropertiesAsset>();
        }
        GUILayout.Space(25);
            
        EditorGUILayout.HelpBox($"Thermodynamic datasets based on CEARUN data", MessageType.None);

        for (var index = 0; index < biprop.thermodynamicData.Count; index++)
        {
            var dataset = biprop.thermodynamicData[index];
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(removeButton, iconButtonStyle))
            {
                biprop.thermodynamicData.RemoveAt(index);
                biprop.Initialize();
            }

            EditorGUILayout.LabelField(dataset.name);
            EditorGUILayout.EndHorizontal();
        }

        DropAreaGUI(biprop);

        serializedObject.ApplyModifiedProperties();
    }
    public void DropAreaGUI(BiPropellantAsset biprop)
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
        GUI.Box (drop_area, "Drop additional datasets here");
     
        switch (evt.type) 
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains (evt.mousePosition))
                    return;
             
                foreach (var dragged_object in DragAndDrop.objectReferences)
                {
                    var dataset = dragged_object as ThermodynamicPropertiesAsset;
                    if (dataset == null)
                        return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
                if (evt.type == EventType.DragPerform) 
                {
                    DragAndDrop.AcceptDrag ();
             
                    foreach (var dragged_object in DragAndDrop.objectReferences)
                    {
                        var dataset = dragged_object as ThermodynamicPropertiesAsset;
                        if (dataset != null)
                        {
                            if (!biprop.thermodynamicData.Contains(dataset))
                            {
                                biprop.thermodynamicData.Add(dataset);
                            }
                        }
                    }
                    biprop.Initialize();
                }
                break;
        }
    }    
}
