using System;
using EngineDesignerUI.Interfaces;
using TMPro;
using UnityEngine;
using TextAlignment = EngineDesignerUI.Interfaces.TextAlignment;

namespace EngineDesignerKSP.UI
{
    public class OMGTextMeshProvider : TextMeshProUGUI, IUITextProvider
    {
        public string Content { get; set; }
        public Color Color { get; set; }
        public TextAlignment Alignment
        {
            get
            {
                switch (alignment)
                {
                    case TextAlignmentOptions.TopLeft:
                        return TextAlignment.TopLeft;
                    case TextAlignmentOptions.Top:
                        return TextAlignment.TopCenter;
                    case TextAlignmentOptions.TopRight:
                        return TextAlignment.TopRight;
                    case TextAlignmentOptions.Center:
                        return TextAlignment.Center;
                    case TextAlignmentOptions.BottomLeft:
                        return TextAlignment.BottomLeft;
                    case TextAlignmentOptions.Bottom:
                        return TextAlignment.BottomCenter;
                    case TextAlignmentOptions.BottomRight:
                        return TextAlignment.BottomRight;
                    case TextAlignmentOptions.MidlineLeft:
                        return TextAlignment.Left;
                    case TextAlignmentOptions.MidlineRight:
                        return TextAlignment.Right;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (value)
                {
                    case TextAlignment.TopLeft:
                        alignment = TextAlignmentOptions.TopLeft;
                        break;
                    case TextAlignment.TopCenter:
                        alignment = TextAlignmentOptions.Top;
                        break;
                    case TextAlignment.TopRight:
                        alignment = TextAlignmentOptions.TopRight;
                        break;
                    case TextAlignment.Left:
                        alignment = TextAlignmentOptions.MidlineLeft;
                        break;
                    case TextAlignment.Center:
                        alignment = TextAlignmentOptions.Center;
                        break;
                    case TextAlignment.Right:
                        alignment = TextAlignmentOptions.MidlineRight;
                        break;
                    case TextAlignment.BottomLeft:
                        alignment = TextAlignmentOptions.BottomLeft;
                        break;
                    case TextAlignment.BottomCenter:
                        alignment = TextAlignmentOptions.Bottom;
                        break;
                    case TextAlignment.BottomRight:
                        alignment = TextAlignmentOptions.BottomRight;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            } 
        }
        public TextStyle Style { get; set; }
        public float FontSize { get; set; }
        public string FontName { get; set; }
        public string UIControlType => "TextMesh Pro";
    }
}