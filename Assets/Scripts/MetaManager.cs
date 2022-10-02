using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MetaManager : MonoBehaviour
{
    public static MetaManager instance;

    public RectTransform LoadScreen;

    private SessionVariables sessionVariables;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            return;
        }

        sessionVariables = ScriptableObject.CreateInstance<SessionVariables>();

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            sessionVariables.Initialize();
        }
    }

    public void TransitionToTitleScene()
    {
        ShowLoadScreen(0, a =>
        {
            HideLoadScreen(() =>
            {
                // Initialize Title
            });
        });
    }

    public void TransitionToGameScene()
    {
        ShowLoadScreen(2, a =>
        {
            HideLoadScreen(() =>
            {
                DayManager dm = FindObjectOfType<DayManager>();
                // Initialize Game
            });
        });
    }

    public void TransitionToPostGameScene()
    {
        ShowLoadScreen(3, a =>
        {
            HideLoadScreen(() =>
            {
                // Initialize PostGame
            });
        });
    }

    public void TransitionToPreGameScene()
    {
        sessionVariables.Initialize();
        ShowLoadScreen(1, a =>
        {
            HideLoadScreen(() =>
            {
                // Initialize PreGame
            });
        });
    }

    private void LoadScene(int idx, System.Action<AsyncOperation> callback)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(idx);
        op.completed += callback;
    }

    private void ShowLoadScreen(int idxToLoad, System.Action<AsyncOperation> callback)
    {
        LoadScreen.anchoredPosition = Vector3.up * 20;
        LoadScreen.DOMoveY(180, 1.0f).SetEase(Ease.OutQuint).OnComplete(() => LoadScene(idxToLoad, callback));
    }

    private void HideLoadScreen(System.Action callback)
    {
        LoadScreen.DOMoveY(-180, 1.0f).SetEase(Ease.InQuad).OnComplete(() => callback.Invoke());
    }
}
