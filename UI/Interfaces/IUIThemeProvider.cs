using UnityEngine;

namespace EngineDesignerUI.Interfaces
{
    public interface IUIThemeProvider
    {
        bool IsValidTheme { get; }
        string ThemeName { get; }
        Color Light { get; }
        Color Dark { get; }
        Color Accent1 { get; }
        Color Accent2 { get; }
        Color Good { get; }
        Color Bad { get; }
    }
}