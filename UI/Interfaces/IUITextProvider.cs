using UnityEngine;

namespace EngineDesignerUI.Interfaces
{
    public interface IUITextProvider
    {
        string Content { get; set; }
        Color Color { get; set; }
        TextAlignment Alignment { get; set; }
        TextStyle Style { get; set; }
        float FontSize { get; set; }
        string FontName { get; set; }
        string UIControlType { get; }
    }

    public enum TextAlignment
    {
        TopLeft,
        TopCenter,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum TextStyle
    {
        Normal,
        Bold,
        Italic
    }
}