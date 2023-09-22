using System;
using System.Collections.Generic;
using EngineDesignerUI;
#if UNITY_EDITOR
using omg.Editor;
#else
using EngineDesignerUI.Interfaces;
#endif
using UnityEngine;

namespace omg.UI
{
    public class DesignerWindowController : MonoBehaviour
    {
        public string Theme;
        public OMGUITheme activeTheme;
#if UNITY_EDITOR
        public List<UIThemeAsset> availableThemes;
#else            
        public List<IUIThemeProvider> availableThemes;
#endif
        public void ChangeTheme(string newThemeName)
        {
            var newTheme = availableThemes.Find(x => x.ThemeName.Equals(newThemeName, StringComparison.InvariantCultureIgnoreCase));
            if (newTheme != null)
            {
                Debug.Log($"New theme set to {newTheme.ThemeName}");
                activeTheme = OMGUITheme.CreateTheme(newTheme);
                UpdateWindowControlsForTheme();
            }
        }

        private void Start()
        {
            ChangeTheme(Theme);
        }

        private void UpdateWindowControlsForTheme()
        {
            var textComponents = GetComponentsInChildren<OMGText>();
            foreach (var omgText in textComponents)
            {
                if (omgText != null)
                {
                    omgText.SetColorFromTheme(activeTheme);
                }
            }
            var buttonComponents = GetComponentsInChildren<OMGButton>();
            foreach (var omgButton in buttonComponents)
            {
                omgButton.SetColorFromTheme(activeTheme);
            }
        }
    }
}