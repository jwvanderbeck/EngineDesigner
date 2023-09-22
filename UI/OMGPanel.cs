using System;
using UnityEngine;
using UnityEngine.UI;

namespace EngineDesignerUI
{
    public class OMGPanel : OMGUIElement
    {
        public Image backgroundImage;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
        }

        public override void SetColorFromTheme(OMGUITheme theme)
        {
            if (backgroundImage == null) return;

            backgroundImage.color = GetThemeColor(themeColor, theme);
        }
        
    }
}
