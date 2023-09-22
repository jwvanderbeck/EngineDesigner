using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PresetManager : MonoBehaviour
{
    public GameObject presetLabelGroup;
    public GameObject presetInputFieldGroup;

    public Button renameButton;

    public PresetWindow presetWindow;

    public string PresetName
    {
        get { return presetName; }
    }

    private TextMeshProUGUI presetLabel;
    private TMP_InputField presetInputField;

    private string presetName;
    private string presetOldName;
    
    private bool isRenaming;
    private bool discardEdit;
    
    // Start is called before the first frame update
    void Start()
    {
        presetLabel = presetLabelGroup.GetComponent<TextMeshProUGUI>();
        presetInputField = presetInputFieldGroup.GetComponent<TMP_InputField>();
        if (presetInputField.textComponent.textInfo == null)
        {
            Debug.Log("Missing TextInfo!");
            presetInputField.textComponent.ForceMeshUpdate(true);
            Debug.Log($"{presetInputField.textComponent.textInfo == null}");
        }
        if (presetInputField.placeholder == null)
        {
            List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>(presetInputFieldGroup.GetComponentsInChildren<TextMeshProUGUI>());
            foreach (var textComponent in textComponents)
            {
                if (textComponent.gameObject.name == "Placeholder")
                {
                    presetInputField.placeholder = textComponent;
                }
            }
        }
        presetName = presetLabel.text;
        // SetRenameStatus(isRenaming);
    }

    public void OnLoadButton()
    {
        
    }

    public void OnDeleteButton()
    {
        presetWindow.DeletePreset(this);
        if (Application.isEditor)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetRenameStatus(bool rename)
    {
        if (rename)
        {
            Debug.Log($"Setting input text {presetLabel.text}");
            presetLabelGroup.SetActive(false);
            presetInputFieldGroup.SetActive(true);
            renameButton.interactable = false;
            presetInputField.text = presetLabel.text;
        }
        else
        {
            presetLabelGroup.SetActive(true);
            presetInputFieldGroup.SetActive(false);
            StartCoroutine(EnableRenameButtonDelayed());
        }
    }

    public void Deselect(string foo)
    {
        Debug.Log($"Deselect {foo}");
        presetName = presetOldName;
        Debug.Log($"Old name: {presetOldName}");
        presetLabel.text = presetName;
    }

    public void OnRenamePresetComplete(string newName)
    {
        presetName = presetInputField.text;
        presetLabel.text = presetName;
        isRenaming = false;
        SetRenameStatus(isRenaming);
    }
    
    public void OnRenameButton()
    {
        presetOldName = presetLabel.text;
        isRenaming = !isRenaming;
        SetRenameStatus(isRenaming);
    }

    public IEnumerator EnableRenameButtonDelayed()
    {
        int frameCount = 10;
        while (frameCount > 0)
        {
            frameCount--;
            yield return new WaitForEndOfFrame();
        }
        renameButton.interactable = true;
    }
}
