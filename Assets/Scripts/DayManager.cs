using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public GuessableImage imageCandidates;

    private GlobalVars _vars = new GlobalVars();

    public static GlobalVars Globals;

    public GuySprites guys;

    private GuySprite _currentGuy;

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
    public static UnityEvent<GuySprite> OnNewGuy = new UnityEvent<GuySprite>();

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

    // Start is called before the first frame update
    void Start()
    {
        CoroutineManager.Start(StartNextDay());
    }

    private void Awake()
    {
        Globals = _vars;
    }

    private IEnumerator StartNextDay()
    {
        _dayActive = true;
        _timerActive = true;
        AudioManager.ChangeMusic(SessionVariables.UpcomingWeathers[0] == Weather.Rain ? BGM.GameRain : BGM.GameRegular);
        NewGuessable();
        yield return null;
    }

    float subtrackVolume = 0;

    // Update is called once per frame
    void Update()
    {
        if (_dayActive)
        {
            _dayTimeElapsed = _dayTimeElapsed + Time.deltaTime;

            if (_dayTimeElapsed/100 >= Globals.RushHourStart && _dayTimeElapsed/100 <= Globals.RushHourEnd)
            {
                subtrackVolume += Time.deltaTime;
            } else
            {
                subtrackVolume -= Time.deltaTime;
            }

            subtrackVolume = Mathf.Clamp01(subtrackVolume);

            AudioManager.SetSubtrackVolume(subtrackVolume);

            OnDayProgress.Invoke(_dayTimeElapsed/100.0f);

            if (_dayTimeElapsed >= 100)
            {
                _dayActive = false;
                Sell();
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
    }

    public void Sell()
    {
        if (_timerActive)
        {
            CoroutineManager.Start(SellRoutine());
        }
    }

    public IEnumerator SellRoutine()
    {
        yield return null;
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
        SessionVariables.todaysDrawings.Add(tex);
        SessionVariables.allDrawings.Add(tex);
        CanvasSurface.DOMove(CanvasSurface.transform.position + Vector3.up * 500, 0.25f, true);
        DrawController.Clear();
        yield return new WaitForSeconds(0.5f);

        if (_dayActive)
        {
            NewGuessable();
            CanvasSurface.DOMove(CanvasSurface.transform.position + Vector3.up * -500, 0.25f, true).OnComplete(() =>
            {
                _timerActive = true;
                DrawController.EnableDrawing();

            });
        }
    }

    private void NewGuessable()
    {
        _currentGuy = guys.GetRandom();
        _currentGuessable = imageCandidates.GetRandom();
        OnNewGuy.Invoke(_currentGuy);
    }

    private void CompareImages(Texture2D drawn, Texture2D wanted)
    {
        RenderTexture rt = RenderTexture.GetTemporary(64, 64);
        rt.Create();
        Graphics.Blit(drawn, rt);

        Texture2D smallerTex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rt;

        smallerTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        smallerTex.Apply();

        RenderTexture.active = old_rt;
        
        var pixels1 = smallerTex.GetPixels();
        var pixels2 = wanted.GetPixels();
        var divisor = 1 / (64.0f * 64.0f);
        float totalPenalty = 0;
        for (int i = 0; i < 64 * 64; i++)
        {

            Color diff = pixels1[i] - pixels2[i];

            var colorDiff = Mathf.Sqrt(diff.r * diff.r + diff.g * diff.g + diff.b * diff.b) * 0.25f;

            if (pixels1[i].a > 0 && pixels2[i].a > 0)
            {
                totalPenalty += colorDiff * divisor - 0.25f;
            } else if (pixels1[i].a == 0 && pixels2[i].a > 0)
            {
                totalPenalty += 5 * divisor;
            }
            else if (pixels1[i].a > 0 && pixels2[i].a == 0)
            {
                totalPenalty += 2.5f * divisor + (colorDiff * 4f) * divisor;
            }
        }

        totalPenalty = (0.6f - 0.025f * SessionVariables.Reputation - 0.1f * _currentGuy.bias - totalPenalty) * 0.5f;

        Debug.Log(totalPenalty);

        var reward = Mathf.Clamp(totalPenalty, 0.01f, SessionVariables.IncomeMultiplier * SessionVariables.MaxIncomeBase);

        reward = Mathf.Ceil(reward * 100) / 100.0f;

        SessionVariables.TodaysEarnings += reward;

        OnSubmit.Invoke(reward);
    }
}
