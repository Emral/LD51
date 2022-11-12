using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HotkeyButton : MonoBehaviour
{
    public char defaultHotkey;
    public string playerPrefsKey;
    public Text text;

    public UnityEvent OnHotkeyPressed;

    // Start is called before the first frame update
    void Start()
    {

        UnityEngine.InputSystem.Keyboard.current.onTextInput += OnHotkey;
        string s = PlayerPrefs.GetString(playerPrefsKey, defaultHotkey.ToString());
        if (s != text.text)
        {
            text.text = s.ToUpper();

            if (text.text == " ")
            {
                text.text = "_";
            }
        }
        text.gameObject.SetActive(PlayerPrefs.GetInt("ShowHotkeys", 1) == 1);
    }

    private void OnHotkey(char c)
    {
        string s = PlayerPrefs.GetString(playerPrefsKey, defaultHotkey.ToString());
        if (c == s.ToCharArray()[0])
        {
            OnHotkeyPressed.Invoke();
        }
    }
}
