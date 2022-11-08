using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputOption : MonoBehaviour, IOptionsMenuOption
{
    public InputField input;
    public string PlayerPrefsIndex;
    public string defaultValue; // When merging this into the toybox, turn it into a self-resetting SO reference

    private bool isInitialized = false;

    private string currentValue;
    private string oldValue;
    private string lastTextString;

    private void OnDestroy()
    {
        input.onValidateInput -= OnInputValueChanged;
    }

    private bool midValidation = false;

    char OnInputValueChanged(string text, int charIndex, char addedChar)
    {
        if (midValidation)
        {
            return addedChar;
        }

        midValidation = true;

        if (addedChar == ' ')
        {
            input.SetTextWithoutNotify("Spacebar");
        } else
        {
            input.SetTextWithoutNotify("");
        }

        currentValue = addedChar.ToString();
        PlayerPrefs.SetString(PlayerPrefsIndex, currentValue);

        midValidation = false;

        return addedChar.ToString().ToUpper().ToCharArray()[0];
    }

    public void OnDiscard()
    {
        PlayerPrefs.SetString(PlayerPrefsIndex, oldValue);
        currentValue = oldValue;
        input.SetTextWithoutNotify(oldValue);
    }

    public void OnSave()
    {
        oldValue = currentValue;
    }

    private void SetText(string s)
    {
        if (s == " ")
        {
            s = "Spacebar";
        }

        input.SetTextWithoutNotify(s);
    }

    public void OnRestore()
    {
        if (!isInitialized)
        {
            input.onValidateInput += OnInputValueChanged;
            isInitialized = true;
        }

        string hotkey = PlayerPrefs.GetString(PlayerPrefsIndex, defaultValue);

        SetText(hotkey);

        currentValue = hotkey;
        oldValue = currentValue;
    }
}
