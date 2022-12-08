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
    public WeatherSettings weathers;
    public Seasons seasons;
    public OptionsMenuController options;
    [SerializeField] private SaveDataManager _saveManager;

    public RectTransform LoadScreen;

    private SessionVariables sessionVariables;

    public static bool GameCanAdvance = true;
    public static bool TutorialActive => PlayerPrefs.GetInt("SkipTutorial", 0) == 0;

    public Tutorial Tutorial;

    [SerializeField] private GameValue<int> _svDay;
    [SerializeField] private GameValue<float> _svExperience;
    [SerializeField] private GameValue<int> _svFollowers;
    [SerializeField] private GameValue<float> _svReputation;
    [SerializeField] private GameValue<float> _svIncomeMult;
    [SerializeField] private GameValue<float> _svMaxIncome;
    [SerializeField] private ListGameValue<Expense> _svExpenses;
    [SerializeField] private GameValue<ValidColors> _svColors;
    [SerializeField] private ListGameValue<Weather> _svWeather;
    [SerializeField] private ListGameValue<Mechanics> _svTutorials;
    [SerializeField] private ListGameValue<EventData> _svEvents;
    [SerializeField] private GameValue<float> _svSavings;

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

        weathers.Init();
        SessionVariables.weatherSettings = weathers;
        SessionVariables.seasonSettings = seasons;

        SessionVariables.Day = _svDay;
        SessionVariables.Experience = _svExperience;
        SessionVariables.Followers = _svFollowers;
        SessionVariables.Reputation = _svReputation;
        SessionVariables.IncomeMultiplier = _svIncomeMult;
        SessionVariables.MaxIncomeBase = _svMaxIncome;
        SessionVariables.Expenses = _svExpenses;
        SessionVariables.Colors = _svColors;
        SessionVariables.UpcomingWeathers = _svWeather;
        SessionVariables.TutorialsDone = _svTutorials;
        SessionVariables.Events = _svEvents;
        SessionVariables._savings = _svSavings;

        GameCanAdvance = true;
        int i = 0;
        foreach (Event ev in events.events)
        {
            ev.data.idx = i;
            i++;
            ev.Initialize();
        }

        sessionVariables = ScriptableObject.CreateInstance<SessionVariables>();
    }

    private void Start()
    {
        sessionVariables.ResetAll();
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
        sessionVariables.ResetAll();
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
                SaveSaveData();
            } else
            {
                LoadSaveData();
                sessionVariables.Initialize();
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

    public void StartNewGame()
    {
        ClearSaveData();
        TransitionToPreGameScene();
    }

    public void LoadSaveData()
    {
        _saveManager.Restore();
    }

    public void ClearSaveData()
    {
        _saveManager.Clear();
    }

    public void SaveSaveData()
    {
        _saveManager.Save();
    }

    public bool HasSaveData()
    {
        return _saveManager.Exists();
    }

    public int ReadDayFromSave()
    {
        return _saveManager.ReadDay();
    }
}
