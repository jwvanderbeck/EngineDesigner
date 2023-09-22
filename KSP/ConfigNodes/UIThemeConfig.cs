using EngineDesignerUI.Interfaces;
using UnityEngine;

namespace EngineDesignerKSP
{
    public class UIThemeConfig : IUIThemeProvider
    {
        #region Interface Properties

        public bool IsValidTheme { get; private set; }

        public string ThemeName => themeName;
        public Color Light => light;
        public Color Dark => dark;
        public Color Accent1 => accent1;
        public Color Accent2 => accent2;
        public Color Good => good;
        public Color Bad => bad;

        #endregion

        private string themeName;
        private Color light;
        private Color dark;
        private Color accent1;
        private Color accent2;
        private Color good;
        private Color bad;
        
        public UIThemeConfig(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            IsValidTheme = true;
            IsValidTheme = IsValidTheme && node.TryGetValue("name", ref themeName);
            IsValidTheme = IsValidTheme && node.TryGetValue("light", ref light);
            IsValidTheme = IsValidTheme && node.TryGetValue("dark", ref dark);
            IsValidTheme = IsValidTheme && node.TryGetValue("accent1", ref accent1);
            IsValidTheme = IsValidTheme && node.TryGetValue("accent2", ref accent2);
            IsValidTheme = IsValidTheme && node.TryGetValue("good", ref good);
            IsValidTheme = IsValidTheme && node.TryGetValue("bad", ref bad);
        }
    }
}