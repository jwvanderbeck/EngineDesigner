using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PresetWindow : MonoBehaviour
{
    public GameObject scrollerContent;
    public List<PresetManager> presets;
    
    public void OnSearchChanged(string newSearch)
    {
        var search = newSearch.ToLower(CultureInfo.InvariantCulture);
        for (int i = 0; i < presets.Count; i++)
        {
            if (!presets[i].PresetName.ToLower(CultureInfo.InvariantCulture).Contains(search))
            {
                presets[i].gameObject.SetActive(false);
            }
            else
            {
                presets[i].gameObject.SetActive(true);
            }
        }
    }

    public void DeletePreset(PresetManager preset)
    {
        presets.Remove(preset);
    }

    // Start is called before the first frame update
    void Start()
    {
        var childCount = scrollerContent.transform.childCount;
        presets = new List<PresetManager>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            presets.Add(scrollerContent.transform.GetChild(i).GetComponent<PresetManager>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
