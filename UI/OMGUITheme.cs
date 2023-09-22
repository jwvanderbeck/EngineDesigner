using System;
using EngineDesignerUI.Interfaces;
using UnityEngine;

namespace EngineDesignerUI
{
    [Serializable]
    public class OMGUITheme
    {
        public static readonly Color k_defaultLight = new Color(0.898f, 0.898f, 0.898f, 1f);
        public static readonly Color k_defaultDark = new Color(0.2588235f, 0.2784314f, 0.3019608f, 1f);
        public static readonly Color k_defaultAccent1 = new Color(0.08627451f, 0.7333333f, 0.8f, 1f);
        public static readonly Color k_defaultAccent2 = new Color(0.04313726f, 0.4117647f, 0.4470588f, 1f);
        public static readonly Color k_defaultBad = new Color(0.8f, 0.08235294f, 0.2627451f, 1f);
        public static readonly Color k_defaultGood = new Color(0.08627451f, 0.8f, 0.5529412f, 1f);

        public string name = "Default Theme";
        public Color light = k_defaultLight;
        public Color dark = k_defaultDark;
        public Color accent1 = k_defaultAccent1;
        public Color accent2 = k_defaultAccent2;
        public Color bad = k_defaultBad;
        public Color good = k_defaultGood;

        public enum ElementColorStyle
        {
            Light,
            Dark,
            Accent1,
            Accent2,
            Bad,
            Good
        }
        
        public Color GetThemeColor(OMGUITheme.ElementColorStyle style)
        {
            switch (style)
            {
                case OMGUITheme.ElementColorStyle.Light:
                    return light;
                case OMGUITheme.ElementColorStyle.Dark:
                    return dark;
                case OMGUITheme.ElementColorStyle.Accent1:
                    return accent1;
                case OMGUITheme.ElementColorStyle.Accent2:
                    return accent2;
                case OMGUITheme.ElementColorStyle.Bad:
                    return bad;
                case OMGUITheme.ElementColorStyle.Good:
                    return good;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }


        public static OMGUITheme CreateTheme(string name,
            Color light, Color dark,
            Color accent1, Color accent2,
            Color bad, Color good)
        {
            return new OMGUITheme
            {
                name = name,
                light = light,
                dark = dark,
                accent1 = accent1,
                accent2 = accent2,
                bad = bad,
                good = good
            };
        }
        public static OMGUITheme CreateTheme(string name,
            string light, string dark,
            string accent1, string accent2,
            string bad, string good)
        {
            var theme = new OMGUITheme();

            theme.name = name;

            if (!ColorUtility.TryParseHtmlString(light, out theme.light))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {light}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultLight)}");
            }
            if (!ColorUtility.TryParseHtmlString(dark, out theme.dark))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {dark}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultDark)}");
            }
            if (!ColorUtility.TryParseHtmlString(accent1, out theme.accent1))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {accent1}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultAccent1)}");
            }
            if (!ColorUtility.TryParseHtmlString(accent2, out theme.accent2))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {accent2}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultAccent2)}");
            }
            if (!ColorUtility.TryParseHtmlString(bad, out theme.bad))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {bad}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultBad)}");
            }
            if (!ColorUtility.TryParseHtmlString(good, out theme.good))
            {
                Debug.LogWarning($"[OMG] Parsing theme: {name}, color {good}, unable to convert to Color value.  Using default {ColorUtility.ToHtmlStringRGBA(k_defaultGood)}");
            }
            
            return theme;
        }

        public static OMGUITheme CreateTheme(IUIThemeProvider themeProvider)
        {
            var theme = new OMGUITheme();

            theme.name = themeProvider.ThemeName;
            theme.light = themeProvider.Light;
            theme.dark = themeProvider.Dark;
            theme.accent1 = themeProvider.Accent1;
            theme.accent2 = themeProvider.Accent2;
            theme.good = themeProvider.Good;
            theme.bad = themeProvider.Bad;

            return theme;
        }
    }
}
