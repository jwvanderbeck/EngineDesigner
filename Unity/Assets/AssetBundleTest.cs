using System;
using EngineDesignerUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace omg
{
    public class AssetBundleTest : MonoBehaviour
    {
        private AssetBundle uiAssetBundle;

        private void Start()
        {
            UIAssetBundle = AssetBundle.LoadFromFile("Assets/StreamingAssets/enginedesigner_ui");
            if (UIAssetBundle == null)
            {
                Debug.Log("Failed to load asset bundle");
            }
            else
            {
                var asset = UIAssetBundle.LoadAsset<GameObject>("Designer Window");
                if (asset == null)
                {
                    Debug.Log("Couldn't load TMP Test Panel");
                    foreach (var assetName in UIAssetBundle.GetAllAssetNames())
                    {
                        Debug.Log(assetName);
                    }
                }
                var testAsset = Instantiate(asset, gameObject.transform);
                var t = testAsset.GetComponent<RectTransform>();
                if (t == null)
                {
                    Debug.Log("RectTransform null!");
                }
                else
                {
                    t.SetParent(gameObject.transform);
                }
                var textHandlers = testAsset.GetComponentsInChildren<OMGText>();
                foreach (var handler in textHandlers)
                {
                    var go = handler.gameObject;
                    var text = go.GetComponent<Text>();
                    var fontName = handler.fontName;
                    var content = text.text;
                    var size = text.fontSize;
                    var color = text.color;
                    var alignment = text.alignment;
                    var style = text.fontStyle;

                    var rectTransform = go.GetComponent<RectTransform>();
                    var rectTransformCopy = new RectTransformHolder();
                    rectTransformCopy.anchorMin = rectTransform.anchorMin;
                    rectTransformCopy.anchorMax = rectTransform.anchorMax;
                    rectTransformCopy.anchoredPosition = rectTransform.anchoredPosition;
                    rectTransformCopy.sizeDelta = rectTransform.sizeDelta;
                    rectTransformCopy.pivot = rectTransform.pivot;
                    
                    DestroyImmediate(text);

                    var tmpText = go.AddComponent<TextMeshProUGUI>();
                    if (tmpText == null) continue;
                    tmpText.text = content;
                    tmpText.fontSize = size;
                    // var font = EngineDesignerKSP.Instance.GetFontNamed(fontName);
                    // if (font == null) LogError($"Failed to load font named {fontName}");
                    // tmpText.font = font;
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
                    }

                    rectTransform.anchorMin = rectTransformCopy.anchorMin;
                    rectTransform.anchorMax = rectTransformCopy.anchorMax;
                    rectTransform.anchoredPosition = rectTransformCopy.anchoredPosition;
                    rectTransform.sizeDelta = rectTransformCopy.sizeDelta;
                    rectTransform.pivot = rectTransformCopy.pivot;
                }

                // testAsset.transform.SetParent(gameObject.transform);
                // testAsset.SetActive(true);
            }
        }

        public AssetBundle UIAssetBundle
        {
            get { return uiAssetBundle; }
            set { uiAssetBundle = value; }
        }
        
        public struct RectTransformHolder
        {
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Vector2 pivot;
        }
    }
}
