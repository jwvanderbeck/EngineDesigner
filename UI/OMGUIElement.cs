using System;
using UnityEngine;

namespace EngineDesignerUI
{
    public class OMGUIElement : MonoBehaviour
    {
        public enum ElementType
        {
            Invalid = 0,
            BackgroundLight,
            BackgroundDark,
            TextLight,
            TextDark,
            ControlBackground,
            ControlLabel
        }
        public OMGUITheme.ElementColorStyle themeColor;

        public virtual void SetColorFromTheme(OMGUITheme theme)
        {
        }

        public virtual Color GetThemeColor(OMGUITheme.ElementColorStyle style, OMGUITheme theme)
        {
            switch (style)
            {
                case OMGUITheme.ElementColorStyle.Light:
                    return theme.light;
                case OMGUITheme.ElementColorStyle.Dark:
                    return theme.dark;
                case OMGUITheme.ElementColorStyle.Accent1:
                    return theme.accent1;
                case OMGUITheme.ElementColorStyle.Accent2:
                    return theme.accent2;
                case OMGUITheme.ElementColorStyle.Bad:
                    return theme.bad;
                case OMGUITheme.ElementColorStyle.Good:
                    return theme.good;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }
    }
}
