using System.Collections.Generic;
using System.IO;
using EngineDesignerUI.Interfaces;
using UnityEngine;

namespace omg.Editor
{
    [CreateAssetMenu(fileName = "UITheme", menuName = "OMG/UI/Theme", order = 0)]
    public class UIThemeAsset : ScriptableObject, IUIThemeProvider
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

        [SerializeField]
        private string themeName;
        [SerializeField]
        private Color light;
        [SerializeField]
        private Color dark;
        [SerializeField]
        private Color accent1;
        [SerializeField]
        private Color accent2;
        [SerializeField]
        private Color good;
        [SerializeField]
        private Color bad;

        public void WriteDataToConfigNode()
        {
            var fileText = new List<string>();
            // Header
            fileText.Add("OMG_UITHEME");
            fileText.Add("{");
            fileText.Add($"\tthemeName = {themeName}");
            fileText.Add($"\tlight = {light.r},{light.g},{light.b},{light.a}");
            fileText.Add($"\tdark = {dark.r},{dark.g},{dark.b},{dark.a}");
            fileText.Add($"\taccent1 = {accent1.r},{accent1.g},{accent1.b},{accent1.a}");
            fileText.Add($"\taccent2 = {accent2.r},{accent2.g},{accent2.b},{accent2.a}");
            fileText.Add($"\tgood = {good.r},{good.g},{good.b},{good.a}");
            fileText.Add($"\tbad = {bad.r},{bad.g},{bad.b},{bad.a}");
            // Footer
            fileText.Add("}");
            
            File.WriteAllLines($"Assets/Data/Configs/OMG-Theme-{themeName}.cfg", fileText);
        }
    }
}