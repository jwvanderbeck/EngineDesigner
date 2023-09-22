using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OMGSlider : MonoBehaviour
{
    public float minValue;
    public float maxValue;
    public float currentValue;

    public Slider slider;
    public InputField inputBox;

    private void Awake()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;

        inputBox.text = $"{currentValue:F}";
        
        slider.onValueChanged.AddListener(OnSliderChanged);
        inputBox.onEndEdit.AddListener(OnInputChanged);
    }

    private void OnInputChanged(string arg0)
    {
        if (float.TryParse(arg0, out var newValue))
        {
            currentValue = newValue;
            slider.value = newValue;
        }
    }

    private void OnSliderChanged(float arg0)
    {
        currentValue = arg0;
        inputBox.text = $"{currentValue:F}";
    }
}
