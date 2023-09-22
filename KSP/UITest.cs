using System;
using System.Collections;
using EngineDesignerUI;
using KSP.UI;
using KSP.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EngineDesignerKSP
{
    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class UITest : MonoBehaviour
    {
        private ApplicationLauncherButton appLauncherButton;
        private GameObject window;
        private GameObject testPanel;
        private GameObject[] loadedAssets;

        public void Awake()
        {
            CurrentLogLevel = LogLevel.Info;
        }

        public void Start()
        {
            LogInfo("Starting coroutine to load UI");
            StartCoroutine(LoadUIAssets());
        }

        private struct RectTransformHolder
        {
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Vector2 pivot;
        }

        private RectTransformHolder rectTransformHolder;

        private void SaveRectTransform(RectTransform rectTransform, RectTransformHolder holder)
        {
            holder.anchorMin = rectTransform.anchorMin;
            holder.anchorMax = rectTransform.anchorMax;
            holder.anchoredPosition = rectTransform.anchoredPosition;
            holder.sizeDelta = rectTransform.sizeDelta;
            holder.pivot = rectTransform.pivot;
        }

        private void RestoreRectTransform(RectTransform rectTransform, RectTransformHolder holder)
        {
            rectTransform.anchorMin = holder.anchorMin;
            rectTransform.anchorMax = holder.anchorMax;
            rectTransform.anchoredPosition = holder.anchoredPosition;
            rectTransform.sizeDelta = holder.sizeDelta;
            rectTransform.pivot = holder.pivot;
        }

        IEnumerator LoadUIAssets()
        {
            // Wait for main 
            yield return new WaitUntil(() => EngineDesignerKSP.Ready);
            
            var bundle = EngineDesignerKSP.Instance.UIAssetBundle;
            if (bundle != null)
            {
                loadedAssets = bundle.LoadAllAssets<GameObject>();
                rectTransformHolder = new RectTransformHolder();

                for (int i = 0; i < loadedAssets.Length; i++)
                {
                    LogInfo($"Loaded asset named {loadedAssets[i].name}");
                    if (loadedAssets[i].name == "Designer Window")
                    {
                        LogInfo("Loading Designer Window");
                        var testAsset = loadedAssets[i];
                        window = Instantiate(testAsset);
                        window.transform.SetParent(MainCanvasUtil.MainCanvas.transform);
                        window.transform.localScale *= UIMasterController.Instance.uiScale;
                        var textHandlers = window.GetComponentsInChildren<OMGText>();
                        foreach (var handler in textHandlers)
                        {
                            var go = handler.gameObject;
                            LogInfo($"Processing TextHandler on {go.name}");
                            var text = go.GetComponent<Text>();
                            if (text != null) LogInfo("Found text");
                            var fontName = handler.fontName;
                            var content = text.text;
                            var size = text.fontSize;
                            var color = text.color;
                            var alignment = text.alignment;
                            var style = text.fontStyle;
                            LogInfo("Saving RectTransform values");
                            var rectTransform = go.GetComponent<RectTransform>();
                            var rectTransformCopy = new RectTransformHolder();
                            rectTransformCopy.anchorMin = rectTransform.anchorMin;
                            rectTransformCopy.anchorMax = rectTransform.anchorMax;
                            rectTransformCopy.anchoredPosition = rectTransform.anchoredPosition;
                            rectTransformCopy.sizeDelta = rectTransform.sizeDelta;
                            rectTransformCopy.pivot = rectTransform.pivot;
                            
                            DestroyImmediate(text);

                            LogInfo("Adding TMP");
                            var tmpText = go.AddComponent<TextMeshProUGUI>();
                            if (tmpText != null) LogInfo("TMP Added");
                            tmpText.text = content;
                            tmpText.fontSize = size;
                            var font = EngineDesignerKSP.Instance.GetFontNamed(fontName);
                            if (font == null) LogError($"Failed to load font named {fontName}");
                            tmpText.font = font;
                            tmpText.color = color;
                            switch (alignment)
                            {
                                case TextAnchor.UpperLeft:
                                    tmpText.alignment = TextAlignmentOptions.TopLeft;
                                    break;
                                case TextAnchor.UpperCenter:
                                    tmpText.alignment = TextAlignmentOptions.Top;
                                    break;
                                case TextAnchor.UpperRight:
                                    tmpText.alignment = TextAlignmentOptions.TopRight;
                                    break;
                                case TextAnchor.MiddleLeft:
                                    tmpText.alignment = TextAlignmentOptions.MidlineLeft;
                                    break;
                                case TextAnchor.MiddleCenter:
                                    tmpText.alignment = TextAlignmentOptions.Center;
                                    break;
                                case TextAnchor.MiddleRight:
                                    tmpText.alignment = TextAlignmentOptions.MidlineRight;
                                    break;
                                case TextAnchor.LowerLeft:
                                    tmpText.alignment = TextAlignmentOptions.BottomLeft;
                                    break;
                                case TextAnchor.LowerCenter:
                                    tmpText.alignment = TextAlignmentOptions.Bottom;
                                    break;
                                case TextAnchor.LowerRight:
                                    tmpText.alignment = TextAlignmentOptions.BottomRight;
                                    break;
                                default:
                                    LogError("Unsupported text alignment option");
                                    break;
                            }

                            switch (style)
                            {
                                case FontStyle.Normal:
                                    tmpText.fontStyle = FontStyles.Normal;
                                    break;
                                case FontStyle.Bold:
                                    tmpText.fontStyle = FontStyles.Bold;
                                    break;
                                case FontStyle.Italic:
                                    tmpText.fontStyle = FontStyles.Italic;
                                    break;
                                default:
                                    LogError("Unsupported Font Style option.  Only Normal, Bold, and Italic are supported.");
                                    break;
                            }
                            LogInfo("Restoring RectTransform Values");
                            rectTransform.anchorMin = rectTransformCopy.anchorMin;
                            rectTransform.anchorMax = rectTransformCopy.anchorMax;
                            rectTransform.anchoredPosition = rectTransformCopy.anchoredPosition;
                            rectTransform.sizeDelta = rectTransformCopy.sizeDelta;
                            rectTransform.pivot = rectTransformCopy.pivot;
                            LogInfo(tmpText.font.name);
                        }
                    }

                    if (loadedAssets[i].name == "Settings Window")
                    {
                        LogInfo("Loading Settings Window");
                        var testAsset = loadedAssets[i];
                        window = Instantiate(testAsset);
                        window.transform.SetParent(MainCanvasUtil.MainCanvas.transform);
                        window.transform.localScale *= UIMasterController.Instance.uiScale;
                    }
                }
                LogInfo("Starting couroutine to add icon to toolbar");
                StartCoroutine("AddToToolbar");
            }
        }
        
        IEnumerator AddToToolbar()
        {
            while (!ApplicationLauncher.Ready)
            {
                LogInfo("Application launcher not ready..waiting");
                yield return null;
            }
            try
            {
                // Load the icon for the button
                Texture iconTexture = GameDatabase.Instance.GetTexture("EngineDesigner/Resources/AppLauncherIcon", false);
                if (iconTexture == null)
                {
                    LogError("Failed to load icon texture");
                }
                else
                {
                    LogInfo("Creating icon on toolbar");
                    appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                        OpenWindow,
                        CloseWindow,
                        HoverInButton,
                        HoverOutButton,
                        null,
                        null,
                        ApplicationLauncher.AppScenes.FLIGHT,
                        iconTexture);
                    ApplicationLauncher.Instance.AddOnHideCallback(HideButton);
                    ApplicationLauncher.Instance.AddOnRepositionCallback(RepostionWindow);
                }
            }
            catch (Exception e)
            {
                LogError("Unable to add button to application launcher: " + e.Message);
                throw e;
            }
        }

        private void RepostionWindow()
        {
        }

        private void HideButton()
        {
            ApplicationLauncher.Instance.RemoveOnHideCallback(HideButton);
            ApplicationLauncher.Instance.RemoveOnRepositionCallback(RepostionWindow);
            ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
        }

        private void HoverOutButton()
        {
        }

        private void HoverInButton()
        {
        }

        private void CloseWindow()
        {
            LogInfo("Close Window");
            window.SetActive(false);
            testPanel.SetActive(false);
        }

        private void OpenWindow()
        {
            LogInfo("Open Window");
            window.SetActive(true);
            testPanel.gameObject.SetActive(true);
            if (!testPanel.activeInHierarchy)
            {
                var parent = testPanel.transform.parent;
                while (parent != null)
                {
                    LogInfo($"{parent.name}, hierarchy {parent.gameObject.activeInHierarchy}, self {parent.gameObject.activeSelf}");
                    parent = parent.parent;
                }
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
        private const string k_debugName = "[Engine Designer UI]";

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
