using omg.Unity.Runtime;
using Simulation.Engines.Liquid;
using UnityEditor;
using UnityEngine;

namespace omg.Editor
{
    [CustomEditor(typeof(LiquidEngine))]
    public class LiquidEngineSolverEditor : UnityEditor.Editor
    {
        // private SerializedProperty efficiencyFactor;
        private SerializedProperty propellant;
        private SerializedProperty chamberPressure;
        private SerializedProperty propellantRatio;
        private SerializedProperty nozzleDiameter;
        private SerializedProperty expansionRatio;
        private SerializedProperty inputThrust;

        public void OnEnable()
        {
            // efficiencyFactor = serializedObject.FindProperty(nameof(efficiencyFactor));
            propellant = serializedObject.FindProperty(nameof(propellant));
            chamberPressure = serializedObject.FindProperty(nameof(chamberPressure));
            propellantRatio = serializedObject.FindProperty(nameof(propellantRatio));
            nozzleDiameter = serializedObject.FindProperty(nameof(nozzleDiameter));
            expansionRatio = serializedObject.FindProperty(nameof(expansionRatio));
            inputThrust = serializedObject.FindProperty(nameof(inputThrust));
        }
        public override void OnInspectorGUI()
        {
            var solver = target as LiquidEngine;
            if (solver == null) return;

            // if (GUILayout.Button("Re-Initialize", GUILayout.Width(100)))
            // {
            //     solver.Initialize();
            // }
            //
            serializedObject.Update();
            // EditorGUILayout.PropertyField(efficiencyFactor);
            EditorGUILayout.PropertyField(propellant);
            if (GUILayout.Button("Update Propellant", GUILayout.Width(200)))
            {
                solver.UpdatePropellant();
            }
            
            EditorGUILayout.PropertyField(chamberPressure);
            EditorGUILayout.PropertyField(propellantRatio);
            EditorGUILayout.PropertyField(nozzleDiameter);
            EditorGUILayout.PropertyField(expansionRatio);
            EditorGUILayout.PropertyField(inputThrust);
            
            GUILayout.Space(25);
            
            EditorGUILayout.HelpBox($"Propellant Transients for O/F Ratio {solver.actualPropellantRatio}", MessageType.None);
            EditorGUILayout.LabelField("Actual O/F Ratio", $"{solver.actualPropellantRatio}");
            EditorGUILayout.LabelField("Combustion Temperature", $"{solver.combustionTemperature} K");
            EditorGUILayout.LabelField("Specific Heat Ratio", $"{solver.specificHeatRatio}");
            EditorGUILayout.LabelField("Molecular Weight", $"{solver.molecularWeight}");

            GUILayout.Space(25);
            
            EditorGUILayout.HelpBox($"Engine Transients", MessageType.None);
            EditorGUILayout.LabelField("Pressure Ratio", $"{solver.pressureRatio}");
            EditorGUILayout.LabelField("Mass Flow Rate", $"{solver.massFlowRate} kg/s");
            EditorGUILayout.LabelField("Exhaust Velocity", $"{solver.exhaustVelocity} m/s");
            EditorGUILayout.LabelField("Mach Number", $"{solver.machNumber}");
            EditorGUILayout.LabelField("Throat Diameter", $"{solver.throatDiameter*100:N2} cm");
            
            GUILayout.Space(25);
            
            EditorGUILayout.HelpBox($"Solved Output", MessageType.None);
            EditorGUILayout.LabelField("Sea Level Thrust", $"{solver.seaLevelThrust:N} N");
            EditorGUILayout.LabelField("Sea Level Specific Impulse", $"{solver.seaLevelIsp} s");
            EditorGUILayout.LabelField("Vacuum Thrust", $"{solver.vacuumThrust:N} N");
            EditorGUILayout.LabelField("Vacuum Specific Impulse", $"{solver.vacuumIsp} s");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
