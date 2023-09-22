using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EngineDesignerKSP.UI
{
    // [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class UILoader : MonoBehaviour
    {
        public static UILoader Instance;
        public AssetBundle UIAssetBundle;

        public List<TMP_FontAsset> fonts;
        public TMP_FontAsset mainFont;
        public TMP_FontAsset monoFont;

        private const string k_debugName = "[Engine Designer UILoader]";

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        
        public void LoadAssetBundle()
        {
            
        }
        public void ModuleManagerPostLoad()
        {
            return;
            LogInfo("ModuleManager PostLoad");

            UIAssetBundle = AssetBundle.LoadFromFile(KSPUtil.ApplicationRootPath + "GameData/EngineDesigner/Resources/enginedesigner_ui");
            if (UIAssetBundle == null)
            {
                LogError("Failed to load UI AssetBundle");
            }
            else
            {
                LogInfo("UI Assets loaded from AssetBundle");
            }
            LogInfo("[EngineDesignerKSP] Looking for fonts");
            var fonts = UnityEngine.Resources.LoadAll<TMP_FontAsset>("Fonts");
            foreach (var font in fonts)
            {
                LogInfo($"Found fount {font.name}");
            }
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
