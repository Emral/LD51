using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;

public class MetaManager : MonoBehaviour
{
    public static MetaManager instance;

    public EventList events;
    public TutorialsList tutorials;

    public RectTransform LoadScreen;

    private SessionVariables sessionVariables;

    public static bool GameCanAdvance = true;
    public static bool TutorialActive = true;

    public Tutorial Tutorial;

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

        foreach(Event ev in events.events)
        {
            ev.Initialize();
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            TutorialActive = false;
        }

        sessionVariables = ScriptableObject.CreateInstance<SessionVariables>();
        sessionVariables.Initialize();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.ChangeMusic(BGM.MainMenu);
        }
    }

    public IEnumerator DoTutorialRoutine(Mechanics mechanic)
    {
        SFX.Pageturn.Play();
        yield return Tutorial.ShowRoutine(tutorials.Get(mechanic));
    }

    public async Task DoTutorial(Mechanics mechanic)
    {
        await Tutorial.Show(tutorials.Get(mechanic));
    }

    public void TransitionToTitleScene()
    {
        SFX.ButtonPress.Play();
        AudioManager.ChangeMusic(BGM.MainMenu);
        sessionVariables.Initialize();
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
        SFX.ToMainGame.Play();
        AudioManager.ChangeMusic(null, null);
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
        AudioManager.ChangeMusic(BGM.PostGame);
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
        SFX.ToNextDay.Play();
        var oldScene = SceneManager.GetActiveScene().buildIndex;
        AudioManager.ChangeMusic(BGM.PreGame);
        ShowLoadScreen(1, a =>
        {
            if (oldScene == 3)
            {
                SessionVariables.NewDayBegins();
            }
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
        LoadScreen.localPosition = Vector3.up * 20;
        LoadScreen.DOLocalMoveY(0, 1.0f).SetEase(Ease.OutQuint).OnComplete(() => LoadScene(idxToLoad, callback));
    }

    private void HideLoadScreen(System.Action callback)
    {
        LoadScreen.DOLocalMoveY(20, 1.0f).SetEase(Ease.InQuad).OnComplete(() => callback.Invoke());
    }
}
