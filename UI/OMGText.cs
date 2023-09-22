using EngineDesignerUI.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace EngineDesignerUI
{
    public class OMGText : OMGUIElement
    {
        public string fontName = "OpenSans SDF";
        
        public IUITextProvider TextProvider;
        
        public class OnTextEvent : UnityEvent<string> { }

        private OnTextEvent _onTextUpdate = new OnTextEvent();

        public UnityEvent<string> OnTextUpdate
        {
            get { return _onTextUpdate; }
        }

        public string Text
        {
            get
            {
                return TextProvider?.Content;
            }
            set
            {
                if (TextProvider != null)
                {
                    TextProvider.Content = value;
                }
                
                // call event
                _onTextUpdate?.Invoke(value);
            }
        }

        public void DestroyTextProvider()
        {
            var components = GetComponents<MonoBehaviour>();
            for (var index = components.Length - 1; index >= 0; index--)
            {
                var component = components[index];
                var textProvider = component as IUITextProvider;
                if (textProvider != null)
                {
                    DestroyImmediate(component);
                }
            }
        }

        private void Awake()
        {
            // Attach to an initial provider if found
            var components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                var textProvider = component as IUITextProvider;
                if (textProvider != null)
                {
                    TextProvider = textProvider;
                    break;
                }
            }
        }

        public override void SetColorFromTheme(OMGUITheme theme)
        {
            if (TextProvider != null)
            {
                TextProvider.Color = GetThemeColor(themeColor, theme);
            }
        }
    }
}
