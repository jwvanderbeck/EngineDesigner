using EngineDesignerUI;
using TMPro;
using UnityEngine;

namespace EngineDesignerKSP.UI
{
    public class OMGTextSwapper
    {
        private const string k_debugName = "[Engine Designer KSP/UI]";
        
        public void ConvertText(OMGText textControl)
        {
            var textProvider = textControl.TextProvider;
            if (textProvider.UIControlType != "Text")
            {
                LogError($"{textControl.name} can't be converted because it doesn't have the expected Text provider");
                return;
            }
            var font = EngineDesignerKSP.Instance.GetFontNamed(textProvider.FontName);
            if (font == null)
            {
                LogError($"Unable to find font named {textProvider.FontName} in KSP resources.  Aborting text conversion.");
                return;
            }
            
            LogInfo($"Converting text on {textControl.name} from standard Text to TextMesh Pro");
            var content = textProvider.Content;
            var size = textProvider.FontSize;
            var color = textProvider.Color;
            var alignment = textProvider.Alignment;
            var style = textProvider.Style;
            var rectTransform = textControl.GetComponent<RectTransform>();
            var rectTransformCopy = new RectTransformHolder();
            rectTransformCopy.anchorMin = rectTransform.anchorMin;
            rectTransformCopy.anchorMax = rectTransform.anchorMax;
            rectTransformCopy.anchoredPosition = rectTransform.anchoredPosition;
            rectTransformCopy.sizeDelta = rectTransform.sizeDelta;
            rectTransformCopy.pivot = rectTransform.pivot;
            
            textControl.DestroyTextProvider();
            
            var textMeshProvider = textControl.gameObject.AddComponent<OMGTextMeshProvider>();
            textMeshProvider.Content = content;
            textMeshProvider.FontSize = size;
            textMeshProvider.Color = color;
            textMeshProvider.Alignment = alignment;
            textMeshProvider.Style = style;

            textControl.TextProvider = textMeshProvider;

            rectTransform.anchorMin = rectTransformCopy.anchorMin;
            rectTransform.anchorMax = rectTransformCopy.anchorMax;
            rectTransform.anchoredPosition = rectTransformCopy.anchoredPosition;
            rectTransform.sizeDelta = rectTransformCopy.sizeDelta;
            rectTransform.pivot = rectTransformCopy.pivot;
        }
        private struct RectTransformHolder
        {
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Vector2 pivot;
        }
        #region Logging
    
        public LogLevel CurrentLogLevel { get; set; }
    
        public enum LogLevel
        {
            None,
            Error,
            Warning,
            Info
        }
    
        private void LogInfo(string message)
        {
            if (CurrentLogLevel != LogLevel.Info) return;
    
            Debug.Log($"{k_debugName} [INFO] {message}");
        }
        
        private void LogWarning(string message)
        {
            if (CurrentLogLevel == LogLevel.None || CurrentLogLevel == LogLevel.Error) return;
    
            Debug.Log($"{k_debugName} [WARNING] {message}");
        }
        
        private void LogError(string message)
        {
            if (CurrentLogLevel == LogLevel.None) return;
    
            Debug.Log($"{k_debugName} [ERROR] {message}");
        }
        
        private void LogAlways(string message)
        {
            Debug.Log($"{k_debugName} [ALWAYS] {message}");
        }
    
    
        #endregion
    }
}