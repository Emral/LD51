using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private IOptionsMenuOption[] options;

    // Start is called before the first frame update
    void Start()
    {
        options = transform.GetComponentsInChildren<IOptionsMenuOption>(true);

        foreach(var o in options)
        {
            o.OnRestore();
        }
    }

    public void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseCancel()
    {
        CloseCommon();

        foreach (var o in options)
        {
            o.OnDiscard();
        }
    }

    public void CloseSave()
    {
        CloseCommon();

        foreach (var o in options)
        {
            o.OnSave();
        }

        PlayerPrefs.Save();
    }

    private void CloseCommon()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
