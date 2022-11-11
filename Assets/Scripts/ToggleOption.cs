using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOption : MonoBehaviour, IOptionsMenuOption
{
    public Toggle toggle;
    public bool defaultValue; // When merging this into the toybox, turn it into a self-resetting SO reference
    public string PlayerPrefsIndex;

    private bool isInitialized = false;

    private bool currentValue;
    private bool oldValue;

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    public void OnDiscard()
    {
        toggle.SetIsOnWithoutNotify(oldValue);
        currentValue = oldValue;
        PlayerPrefs.SetInt(PlayerPrefsIndex, oldValue ? 1 : 0);
    }

    void OnToggleValueChanged(bool newValue)
    {
        currentValue = newValue;
        PlayerPrefs.SetInt(PlayerPrefsIndex, currentValue ? 1 : 0);
    }

    public void OnSave()
    {
        oldValue = currentValue;
    }

    public void OnRestore()
    {
        if (!isInitialized)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            isInitialized = true;
        }

        bool isOn = PlayerPrefs.GetInt(PlayerPrefsIndex, defaultValue ? 1 : 0) == 1;

        toggle.SetIsOnWithoutNotify(isOn);

        currentValue = isOn;
        oldValue = isOn;
    }
}
