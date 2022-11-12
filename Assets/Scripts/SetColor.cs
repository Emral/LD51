using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SetColor : MonoBehaviour
{
    public Color color;
    public ValidColors ownColor;
    public char defaultHotkey;
    public string playerPrefsKey;
    public Text text;

    private void Start()
    {
        if (ownColor != (ValidColors)0 && (ownColor & SessionVariables.Colors.Value) == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        UnityEngine.InputSystem.Keyboard.current.onTextInput += OnHotkey;
        string s = PlayerPrefs.GetString(playerPrefsKey, defaultHotkey.ToString());
        if (s != text.text)
        {
            text.text = s.ToUpper();
        }
        text.gameObject.SetActive(PlayerPrefs.GetInt("ShowHotkeys", 1) == 1);

        DayManager.Globals.OnPenColorChanged.AddListener(OnPenColorChanged);
        OnPenColorChanged(DayManager.Globals.PenColor);
    }

    private Tween t;
    private Tween t2;
    private void OnPenColorChanged(Color c)
    {
        if (t != null)
        {
            t.Complete();
            t = null;
        }
        if (t2 != null)
        {
            t2.Complete();
            t2 = null;
        }

        if (c == color)
        {
            t = transform.DOScale(Vector3.one * 1.2f, 0.15f).SetEase(Ease.OutBack);
            t2 = transform.DOLocalRotate(Vector3.forward * Random.Range(-15, 15.0f), 0.175f).SetEase(Ease.OutQuad);
        } else
        {
            t = transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            t2 = transform.DOLocalRotate(Vector3.zero, 0.175f).SetEase(Ease.OutQuad);
        }
    }

    private void OnDestroy()
    {
        UnityEngine.InputSystem.Keyboard.current.onTextInput -= OnHotkey;
        DayManager.Globals.OnPenColorChanged.RemoveListener(OnPenColorChanged);
    }

    private void OnHotkey(char c)
    {
        string s = PlayerPrefs.GetString(playerPrefsKey, defaultHotkey.ToString());
        if (c == s.ToCharArray()[0] && DayManager.Globals.PenColor != color)
        {
            Set();
        }
    }

    public void Set()
    {
        SFX.SelectColor.Play();
        DayManager.Globals.PenColor = color;
    }
}
