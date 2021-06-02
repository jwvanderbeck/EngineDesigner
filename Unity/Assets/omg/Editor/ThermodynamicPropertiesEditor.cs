using System;
using omg.Runtime;
using UnityEditor;
using UnityEngine;

namespace omg.Editor
{
    [CustomEditor(typeof(ThermodynamicPropertiesAsset))]
    public class ThermodynamicPropertiesEditor : UnityEditor.Editor
    {
        private SerializedProperty oxidizer;
        private SerializedProperty fuel;

        private void OnEnable()
        {
            oxidizer = serializedObject.FindProperty(nameof(oxidizer));
            fuel = serializedObject.FindProperty(nameof(fuel));
        }

        public override void OnInspectorGUI()
        {
            var asset = target as ThermodynamicPropertiesAsset;
            
            if (asset == null) return;

            serializedObject.Update();
            
            EditorGUILayout.PropertyField(oxidizer);
            EditorGUILayout.PropertyField(fuel);
            
            EditorGUILayout.Space();

            if (asset.engineTransients != null)
            {
                foreach (var transient in asset.engineTransients)
                {
                    EditorGUILayout.LabelField($"O:F Ratio {transient.ofRatio:F}:1");
                    GetMinMaxValues(transient.combustionTemperature, out float min, out float max);
                    EditorGUILayout.LabelField($"Chamber Pressure {min / 100000f:N}bar to {max / 100000f:N}bar");
                    var step = transient.combustionTemperature[1].time - transient.combustionTemperature[0].time;
                    EditorGUILayout.LabelField($"Chamber Pressure step is {step / 100000f:N}bar");
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        public void GetMinMaxValues(AnimationCurve curve, out float min, out float max)
        {
            min = curve[0].time;
            max = curve[curve.length - 1].time;
        }
    }
}
