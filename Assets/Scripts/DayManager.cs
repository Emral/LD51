using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public GuessableImage imageCandidates;

    private GlobalVars _vars = new GlobalVars();

    public AnimationCurve BalanceCurve;

    public static GlobalVars Globals;

    public GuySprites guys;

    private Guy _currentGuy;

    public Image Test1;
    public Image Test2;
    public Image TimerImage;
    public Image GuessableImage;
    public DrawDriver DrawController;
    public RectTransform CanvasSurface;

    private float _dayTimeElapsed;
    private float _timerTimeElapsed;

    private bool _dayActive;
    private bool _timerActive;

    public static bool DayActive;
    public static bool TimerActive;

    public RectTransform ThatsAWrap;

    public static UnityEvent<float> OnDayProgress = new UnityEvent<float>();
    public static UnityEvent OnDayEnd = new UnityEvent();

    public static UnityEvent<float> OnSubmit = new UnityEvent<float>();
    public static UnityEvent<Guy> OnNewGuy = new UnityEvent<Guy>();

    private Sprite __currentGuessable;
    private Sprite _currentGuessable
    {
        get => __currentGuessable;
        set {
            __currentGuessable = value;
            GuessableImage.sprite = _currentGuessable;
        }
    }

    private bool isRushHour = false;

    private int drawingsDone = 0;

    // Start is called before the first frame update
    async void Start()
    {
        DrawComparison.Curve = BalanceCurve;
        imageCandidates.Initialize();
        var rushHourDuration = Mathf.FloorToInt((SessionVariables.Followers + SessionVariables.Reputation * 2) / 5.0f) * 0.02f;
        Globals.IsRaining = SessionVariables.UpcomingWeathers[0] == Weather.Rain;
        foreach (var e in SessionVariables.Events)
        {
            e.DailyExecute();
        }
        if (Globals.IsRaining)
        {
            DayManager.Globals.tagBiases.Add(Tag.Rain);
        }
        guys.ResetAll();
        NewGuessable();
        if (SessionVariables.Day > 1)
        {
            rushHourDuration = Mathf.Clamp01(rushHourDuration + (Globals.IsRaining ? -0.2f : 0.2f));

            if (SessionVariables.Day == 2)
            {
                rushHourDuration = Mathf.Clamp01(rushHourDuration + 0.3f);
            }
        }

        if (Globals.IsRaining)
        {
            if (SessionVariables.Day > 5)
            {
                rushHourDuration = Mathf.Max(0, rushHourDuration - Random.Range(0.2f, 0.3f), 0);
            }
        }
        Globals.RushHourEnd = Mathf.Clamp(Random.Range(0.8f, 1), rushHourDuration, 1);
        Globals.RushHourStart = Globals.RushHourEnd - rushHourDuration;

        if (SessionVariables.Day == 1)
        {
            await MetaManager.instance.DoTutorial(Mechanics.HowToPlay);
        }

        if (SessionVariables.Day == 2)
        {
            await MetaManager.instance.DoTutorial(Mechanics.SelectColor);
        }

        if (rushHourDuration > 0)
        {
            await MetaManager.instance.DoTutorial(Mechanics.RushHour);
        }

        if (SessionVariables.Day == 4)
        {
            await MetaManager.instance.DoTutorial(Mechanics.MoneyExplanation);
        }
        CoroutineManager.Start(StartNextDay());
        SessionVariables.todaysDrawings.Clear();
    }

    private void Awake()
    {
        Globals = _vars;
    }

    private IEnumerator StartNextDay()
    {
        yield return new WaitForSeconds(0.75f);
        AudioManager.ChangeMusic(Globals.IsRaining ? BGM.GameRain : BGM.GameRegular);
        _dayActive = true;
        _timerActive = true;
    }

    float subtrackVolume = 0;

    // Update is called once per frame
    void Update()
    {
        if (!MetaManager.GameCanAdvance)
        {
            return;
        }

        if (_dayActive)
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                _dayTimeElapsed = 100;
            }
#endif
            _dayTimeElapsed = _dayTimeElapsed + Time.deltaTime;

            if (_dayTimeElapsed/100 >= Globals.RushHourStart && _dayTimeElapsed/100 <= Globals.RushHourEnd)
            {
                subtrackVolume += Time.deltaTime;
                isRushHour = true;
            } else
            {
                isRushHour = false;
                subtrackVolume -= Time.deltaTime;
            }

            subtrackVolume = Mathf.Clamp01(subtrackVolume);

            AudioManager.SetSubtrackVolume(subtrackVolume);

            OnDayProgress.Invoke(_dayTimeElapsed/100.0f);

            if (_dayTimeElapsed >= 100)
            {
                _dayActive = false;
                Sell();
                SFX.DayEnd.Play();
                OnDayEnd.Invoke();

                AudioManager.ChangeMusic(null, null);

                EventSystem.current.enabled = false;

                ThatsAWrap.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    MetaManager.instance.TransitionToPostGameScene();
                });
            }
        }
        if (_timerActive)
        {
            _timerTimeElapsed = _timerTimeElapsed + Time.deltaTime;

            if (_timerTimeElapsed >= 10)
            {
                Sell();
            }
        } else
        {

            _timerTimeElapsed = Mathf.Max(_timerTimeElapsed - 20 * Time.deltaTime, Mathf.Max(_dayTimeElapsed - 90, isRushHour ? 3 : 0));
        }

        TimerImage.fillAmount = 1 - _timerTimeElapsed / 10;

        DayActive = _dayActive;
        TimerActive = _timerActive;

        Globals.RushHour = isRushHour;
    }

    public void Sell()
    {
        if (_timerActive)
        {
            SFX.Sell.Play();
            CoroutineManager.Start(SellRoutine());
        }
    }

    public IEnumerator SellRoutine()
    {
        yield return null;

        if (_timerActive)
        {
            _timerActive = false;
            var tex = DrawController.FinishDrawing();
            var tex2 = new Texture2D(64, 64);
            var tex3 = _currentGuessable.texture;
            tex2.SetPixels(tex3.GetPixels(
                Mathf.FloorToInt(_currentGuessable.rect.x),
                Mathf.FloorToInt(_currentGuessable.rect.y),
                Mathf.FloorToInt(_currentGuessable.rect.width),
                Mathf.FloorToInt(_currentGuessable.rect.height)));
            tex2.Apply();
            CompareImages(tex, tex2);
            var y = CanvasSurface.transform.localPosition.y;
            CanvasSurface.DOLocalMoveY(160, 0.25f, true);
            DrawController.Clear();
            yield return new WaitForSeconds(0.5f);

            if (SessionVariables.Day == 1)
            {
                drawingsDone++;

                if (drawingsDone == 1)
                {
                    yield return MetaManager.instance.DoTutorial(Mechanics.TimeLimit);
                }
                else if (drawingsDone == 2)
                {
                    yield return MetaManager.instance.DoTutorial(Mechanics.DaySlider);
                }
                else if (drawingsDone == 3)
                {
                    yield return MetaManager.instance.DoTutorial(Mechanics.SellEarly);
                }
            }

            if (_dayActive)
            {
                NewGuessable();
                CanvasSurface.DOLocalMoveY(y, 0.25f, true).OnComplete(() =>
                {
                    _timerActive = true;
                    DrawController.EnableDrawing();

                });
            }
        }
    }

    private void NewGuessable()
    {
        _currentGuessable = null;
        while (_currentGuessable == null)
        {
            _currentGuy = guys.GetRandom();
            _currentGuessable = imageCandidates.GetRandom(_currentGuy);

            if (_currentGuessable == null)
            {
                Debug.Log("Selecting different guy...");
            }
        }
        _currentGuy.ArrivalSound.Play();
        SFX.NewCustomer.Play();
        OnNewGuy.Invoke(_currentGuy);
    }

    private void CompareImages(Texture2D drawn, Texture2D wanted)
    {
        var similarity = DrawComparison.CompareImages(drawn, wanted, out Texture2D a, out Texture2D b);

        Test1.sprite = Sprite.Create(a, new Rect(0, 0, a.width, a.height), Vector2.one * 0.5f);
        Test2.sprite = Sprite.Create(b, new Rect(0, 0, b.width, b.height), Vector2.one * 0.5f);

        var totalPenalty = similarity;

        if (similarity == -1)
        {
            totalPenalty = -999;
        } else
        {
            SessionVariables.todaysDrawings.Add(drawn);
            SessionVariables.allDrawings.Add(drawn);
        }

        var reward = Mathf.Max( Mathf.Lerp(0, 1 + _currentGuy.bias * 0.05f, totalPenalty) * SessionVariables.IncomeMultiplier * SessionVariables.MaxIncomeBase, 0);

        reward = reward.MakeDollars();

        Globals.LastGain = reward;

        if (reward > 6 - SessionVariables.Reputation + 0.1f)
        {
            SessionVariables.Followers = SessionVariables.Followers + 1;
        }
        SessionVariables.Experience = SessionVariables.Experience + reward * reward * 0.002f;
        SessionVariables.Reputation = Mathf.Max(0, SessionVariables.Reputation + Mathf.Clamp(totalPenalty - 0.1f, -0.25f, 0.5f));

        SessionVariables.TodaysEarnings += reward;

        OnSubmit.Invoke(reward);
    }
}
