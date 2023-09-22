using TMPro;
using UnityEditor;
using TMPro.EditorUtilities;

namespace omg.UI.Unity.Editor
{
    [CustomEditor(typeof(TMP_TextSwapper))]
    public class TMP_TextSwapperEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var editor = UnityEditor.Editor.CreateEditor(target, typeof(TMP_UiEditorPanel));
            editor.OnInspectorGUI();
        }
    }
}
