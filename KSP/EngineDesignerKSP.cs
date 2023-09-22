using System;
using System.Collections.Generic;
using System.Linq;
using EngineDesignerKSP.ConfigNodes;
using TMPro;
using UnityEngine;
// ReSharper disable IdentifierTypo

namespace EngineDesignerKSP
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class EngineDesignerKSP : MonoBehaviour
    {
        public static EngineDesignerKSP Instance;
        
        public AssetBundle UIAssetBundle;
        public static bool Ready { get; private set; }
        
        private List<ConfigNode> bipropellantConfigNodes;
        private List<BiPropellantConfig> bipropellants;
        private List<UIThemeConfig> uiThemes;
        private TMP_FontAsset[] fonts;

        private const string k_debugName = "[Engine Designer KSP]";

        public bool TryGetBipropellant(string fuel, string oxidizer, out BiPropellantConfig biprop)
        {
            try
            {
                biprop = bipropellants.First((config => string.Equals(config.fuel, fuel, StringComparison.InvariantCultureIgnoreCase) &&
                    String.Equals(config.oxidizer, oxidizer, StringComparison.InvariantCultureIgnoreCase)));
                return true;
            }
            catch (InvalidOperationException e)
            {
                biprop = new BiPropellantConfig();
                return false;
            }
        }

        public void ModuleManagerPostLoad()
        {
            LogAlways("ModuleManager PostLoad");

            LogInfo("Loading biprop configs");
            bipropellants = new List<BiPropellantConfig>();
            
            ConfigNode[] bipropConfigs = GameDatabase.Instance.GetConfigNodes("ENGINEDESIGNER_BIPROPELLANT");
            foreach (var config in bipropConfigs)
            {
                var bipropConfig = new BiPropellantConfig(config);
                bipropellants.Add(bipropConfig);
            }
            LogInfo($"Loaded {bipropellants.Count} biprop configs");
            
            LogInfo("Loading UI Theme configs");
            uiThemes = new List<UIThemeConfig>();
            
            ConfigNode[] uiThemeConfigs = GameDatabase.Instance.GetConfigNodes("OMG_UITHEME");
            foreach (var config in uiThemeConfigs)
            {
                var uiThemeConfig = new UIThemeConfig(config);
                uiThemes.Add(uiThemeConfig);
            }
            LogInfo($"Loaded {uiThemes.Count} UI Theme configs");

            UIAssetBundle = AssetBundle.LoadFromFile(KSPUtil.ApplicationRootPath + "GameData/EngineDesigner/Resources/enginedesigner_ui");
            if (UIAssetBundle == null)
            {
                LogError("Failed to load UI AssetBundle");
            }
            else
            {
                LogInfo("UI Assets loaded from AssetBundle");
            }
            Debug.Log("[EngineDesignerKSP] Looking for fonts");
            fonts = Resources.LoadAll<TMP_FontAsset>("Fonts");
            foreach (var font in fonts)
            {
                Debug.Log($"Found fount {font.name}");
            }
        }

        public TMP_FontAsset GetFontNamed(string fontName)
        {
            foreach (var font in fonts)
            {
                if (font.name == fontName) return font;
            }

            return null;
        }

        public void Start()
        {
            // FOR TESTING - Make settings later
            CurrentLogLevel = LogLevel.Info;
            LogInfo("Start");
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Ready = true;
        }

        #region Logging

        public LogLevel CurrentLogLevel { get; set; }

        public enum LogLevel
        {
            None,
            Error,
            Warning,
            Info
        }

        private void LogInfo(string message)
        {
            if (CurrentLogLevel != LogLevel.Info) return;

            Debug.Log($"{k_debugName} [INFO] {message}");
        }
        
        private void LogWarning(string message)
        {
            if (CurrentLogLevel == LogLevel.None || CurrentLogLevel == LogLevel.Error) return;

            Debug.Log($"{k_debugName} [WARNING] {message}");
        }
        
        private void LogError(string message)
        {
            if (CurrentLogLevel == LogLevel.None) return;

            Debug.Log($"{k_debugName} [ERROR] {message}");
        }
        
        private void LogAlways(string message)
        {
            Debug.Log($"{k_debugName} [ALWAYS] {message}");
        }

        #endregion
    }
}
