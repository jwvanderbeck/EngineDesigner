using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OMGHorizontalSelect : MonoBehaviour
{
    public List<string> options;
    public Color normalButtonColor;
    public Color highlightedButtonColor;
    public Color normalIconColor;
    public Color highlightedIconColor;
    public Color textColor;

    [SerializeField]
    private int selectedIndex;
    
    public Button previousButton;
    private Image previousButtonNormalImage;
    private Image previousButtonHighlightedImage;
    private Image previousButtonNormalIconImage;
    private Image previousButtonHighlightedIconImage;

    public Button nextButton;
    private Image nextButtonNormalImage;
    private Image nextButtonHighlightedImage;
    private Image nextButtonNormalIconImage;
    private Image nextButtonHighlightedIconImage;

    public Text label;

    public OptionChangedEvent onSelectionChanged;

    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            if (value >= options.Count)
            {
                value = 0;
            }

            if (value < 0)
            {
                value = options.Count - 1;
            }
            selectedIndex = value;
            UpdateLabel();
        }
    }

    void Awake()
    {
        if (onSelectionChanged == null)
        {
            onSelectionChanged = new OptionChangedEvent();
        }
        
        label = gameObject.GetComponentInChildren<Text>();
        FindImages();
        HookUpButtons();
    }

    private void HookUpButtons()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(OnPreviousButtonClicked);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
    }

    private void OnNextButtonClicked()
    {
        SelectedIndex++;
    }

    private void OnPreviousButtonClicked()
    {
        SelectedIndex--;
    }

    private void OnValidate()
    {
        FindImages();
        SetColors();
        UpdateLabel();
    }

    private void FindImages()
    {
        if (previousButton != null)
        {
            previousButtonNormalImage = previousButton.transform.Find("Normal").GetComponent<Image>();
            previousButtonHighlightedImage = previousButton.transform.Find("Filled").GetComponent<Image>();
            previousButtonNormalIconImage = previousButton.transform.Find("Icon").GetComponent<Image>();
            previousButtonHighlightedIconImage = previousButton.transform.Find("Icon Highlighted").GetComponent<Image>();
        }
        if (nextButton != null)
        {
            nextButtonNormalImage = nextButton.transform.Find("Normal").GetComponent<Image>();
            nextButtonHighlightedImage = nextButton.transform.Find("Filled").GetComponent<Image>();
            nextButtonNormalIconImage = nextButton.transform.Find("Icon").GetComponent<Image>();
            nextButtonHighlightedIconImage = nextButton.transform.Find("Icon Highlighted").GetComponent<Image>();
        }
    }

    private void SetColors()
    {
        // previousButtonNormalImage.color = normalButtonColor;
        // previousButtonHighlightedImage.color = highlightedButtonColor;
        // previousButtonNormalIconImage.color = normalIconColor;
        // previousButtonHighlightedIconImage.color = highlightedIconColor;
        //
        // nextButtonNormalImage.color = normalButtonColor;
        // nextButtonHighlightedImage.color = highlightedButtonColor;
        // nextButtonNormalIconImage.color = normalIconColor;
        // nextButtonHighlightedIconImage.color = highlightedIconColor;
        //
        // label.color = textColor;
    }

    private void UpdateLabel()
    {
        var newOption = string.Empty;
        
        if (SelectedIndex >= 0 && SelectedIndex < options.Count)
        {
            newOption = options[SelectedIndex];
        }

        label.text = newOption;
        if (onSelectionChanged != null)
        {
            onSelectionChanged.Invoke(newOption);
        }
    }

    public class OptionChangedEvent : UnityEvent<string> { }
}
