using UnityEngine;
using UnityEngine.UI;

namespace EngineDesignerUI.Interfaces
{
    public class OMGTextProvider : Text, IUITextProvider
    {
        public string Content { get; set; }
        public Color Color { get; set; }
        public TextAlignment Alignment { get; set; }
        public TextStyle Style { get; set; }
        public float FontSize { get; set; }
        public string FontName { get; set; }
        public string UIControlType => "Text";
    }
}