using EngineDesignerUI;
using UnityEngine;
using UnityEngine.UI;

namespace EngineDesignerUI
{
public class OMGButton : OMGUIElement
{
    [TextArea(3, 10)][SerializeField]
    private string text = string.Empty;

    public OMGUITheme.ElementColorStyle labelStyle;
    public OMGUITheme.ElementColorStyle iconStyle;
    public OMGUITheme.ElementColorStyle buttonStyle;
    public float highlightValueShift = 20f;
    public float disabledAlphaShift = -50f;
    
    [SerializeField]
    private bool interactable = true;

    public Button button;
    public OMGText label;
    public Image icon;

    private OMGUITheme currentTheme;

    public bool Interactable
    {
        get { return interactable; }
        set
        {
            interactable = value;
            UpdateLabel();
            button.interactable = interactable;
        }
    }

    public string Text
    {
        get { return text; }
        set
        {
            text = value;
        }
    }
    
    void Awake()
    {
        button = gameObject.GetComponent<Button>();
        label = gameObject.GetComponentInChildren<OMGText>();
    }

    private void UpdateLabel()
    {
    }

    private void SetButtonColors()
    {
        if (button == null) return;
        
        // update color from theme
        var mainColor = currentTheme.GetThemeColor(buttonStyle);
        
        var colorBlock = button.colors;
        colorBlock.normalColor = mainColor;
        colorBlock.selectedColor = mainColor;
        colorBlock.pressedColor = mainColor;
        float hue;
        float saturation;
        float value;
        Color.RGBToHSV(mainColor, out hue, out saturation, out value);
        value += highlightValueShift;
        value = Mathf.Min(1.0f, value);
        colorBlock.highlightedColor = Color.HSVToRGB(hue, saturation, value);
        var disabledColor = mainColor;
        disabledColor.a = Mathf.Max(0f, mainColor.a + disabledAlphaShift/100f);
        colorBlock.disabledColor = disabledColor;
        button.colors = colorBlock;
    }
    public override void SetColorFromTheme(OMGUITheme theme)
    {
        currentTheme = theme;
        UpdateLabel();
        SetButtonColors();
    }
}
}