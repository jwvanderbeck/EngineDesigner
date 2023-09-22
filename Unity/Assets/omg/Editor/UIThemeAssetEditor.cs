using UnityEditor;
using UnityEngine;

namespace omg.Editor
{
    [CustomEditor(typeof(UIThemeAsset))]
    public class UIThemeAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty themeName;
        private SerializedProperty light;
        private SerializedProperty dark;
        private SerializedProperty accent1;
        private SerializedProperty accent2;
        private SerializedProperty good;
        private SerializedProperty bad;

        private void OnEnable()
        {
            themeName = serializedObject.FindProperty(nameof(themeName));
            light = serializedObject.FindProperty(nameof(light));
            dark = serializedObject.FindProperty(nameof(dark));
            accent1 = serializedObject.FindProperty(nameof(accent1));
            accent2 = serializedObject.FindProperty(nameof(accent2));
            good = serializedObject.FindProperty(nameof(good));
            bad = serializedObject.FindProperty(nameof(bad));
        }

        public override void OnInspectorGUI()
        {
            var theme = target as UIThemeAsset;
            if (theme == null) return;
        
            serializedObject.Update();
            EditorGUILayout.PropertyField(themeName);
            EditorGUILayout.PropertyField(light);
            EditorGUILayout.PropertyField(dark);
            EditorGUILayout.PropertyField(accent1);
            EditorGUILayout.PropertyField(accent2);
            EditorGUILayout.PropertyField(good);
            EditorGUILayout.PropertyField(bad);
            if (GUILayout.Button("Export CFG", GUILayout.Width(150)))
            {
                theme.WriteDataToConfigNode();
            }
            serializedObject.ApplyModifiedProperties();
        }    
    }
}
