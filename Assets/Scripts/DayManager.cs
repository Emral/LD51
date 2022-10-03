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

    private int drawingsDone = 0;

    // Start is called before the first frame update
    async void Start()
    {
        var rushHourDuration = Mathf.FloorToInt((SessionVariables.Followers + SessionVariables.Reputation) / 10.0f) * 0.01f;
        Globals.IsRaining = SessionVariables.UpcomingWeathers[0] == Weather.Rain;
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
        rushHourDuration = Mathf.Max(0, rushHourDuration - Random.Range(0.2f, 0.3f), 0);

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

        if (Globals.IsRaining)
        {
            await MetaManager.instance.DoTutorial(Mechanics.SelectColor);
        }
        Globals.RushHourEnd = Random.Range(0.8f, 1);
        Globals.RushHourStart = Globals.RushHourEnd - rushHourDuration;
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
        foreach (var e in SessionVariables.Events)
        {
            yield return e.DailyExecute();
        }
        _dayActive = true;
        _timerActive = true;
        yield return null;
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
                _timerActive = false;
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

    private void NewGuessable()
    {
        _currentGuy = guys.GetRandom();
        _currentGuessable = imageCandidates.GetRandom(_currentGuy.bias, _currentGuy.preferredTags);
        OnNewGuy.Invoke(_currentGuy);
    }

    private void CompareImages(Texture2D drawn, Texture2D wanted)
    {
        RenderTexture rt = RenderTexture.GetTemporary(64, 64);
        rt.Create();
        rt.filterMode = FilterMode.Point;
        Graphics.Blit(drawn, rt);

        Texture2D smallerTex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rt;

        smallerTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        smallerTex.Apply();

        RenderTexture.active = old_rt;
        
        var pixels1 = smallerTex.GetPixels();
        var pixels2 = wanted.GetPixels();
        float totalPenalty = 0;
        float penaltyRange = 0;

        float totalInkA = 0;
        float totalInkB = 0;

        Dictionary<Color, float> colorDifferences = new Dictionary<Color, float>();
        Dictionary<Color, float> bSum = new Dictionary<Color, float>();
        for (int i = 0; i < 64 * 64; i++)
        {
            if (pixels1[i].r + pixels1[i].g + pixels1[i].b == 3)
            {
                pixels1[i].a = 0;
            } else if (pixels1[i].a > 0)
            {
                pixels1[i].a = 1;
            }

            if (pixels1[i].a > 0)
            {
                totalInkA++;
                if (colorDifferences.ContainsKey(pixels1[i]))
                {
                    colorDifferences[pixels1[i]]--;
                }
                else
                {
                    colorDifferences.Add(pixels1[i], -1);
                    if (!bSum.ContainsKey(pixels1[i]))
                    {
                        bSum.Add(pixels1[i], -1);
                    }
                }
            }
            if (pixels2[i].a > 0)
            {
                totalInkB++;
                if (colorDifferences.ContainsKey(pixels2[i]))
                {
                    if (bSum[pixels2[i]] == -1)
                    {
                        bSum[pixels2[i]] = 1;
                    } else
                    {
                        bSum[pixels2[i]]++;
                    }
                    colorDifferences[pixels2[i]]++;
                }
                else
                {
                    bSum.Add(pixels2[i], 1);
                    colorDifferences.Add(pixels2[i], 1);
                }
            }

            Color diff = pixels1[i] - pixels2[i];

            if (pixels1[i].a > 0 && pixels2[i].a > 0)
            {
                totalPenalty += 2;
                penaltyRange++;
            } else if ((pixels1[i].a == 0 && pixels2[i].a > 0) || (pixels1[i].a > 0 && pixels2[i].a == 0))
            {
                totalPenalty -= 1f;
                if (pixels1[i].a == 0)
                {
                    penaltyRange++;
                }
            }
        }

        totalPenalty = totalPenalty / Mathf.Max(penaltyRange, 1);

        totalPenalty = totalPenalty + 2 - 2 * Mathf.Abs(totalInkA/penaltyRange - totalInkB/penaltyRange);

        totalPenalty = totalPenalty + 1;

        foreach (var kvp in colorDifferences)
        {
            if (Mathf.Abs(kvp.Value) < bSum[kvp.Key])
            {
                totalPenalty = totalPenalty + ((kvp.Value) / bSum[kvp.Key]) / colorDifferences.Count;
                Debug.Log(kvp.Key.ToString() + ": " + (kvp.Value) / bSum[kvp.Key]);
            }
            else if (bSum[kvp.Key] == -1)
            {
                totalPenalty = totalPenalty - 2f / colorDifferences.Count;
            }
        }

        totalPenalty = (totalPenalty * 1.1f) + 0.3f;

        if (totalInkA == 0)
        {
            totalPenalty = -999;
        }

        Debug.Log(totalPenalty);

        var reward = Mathf.Max( Mathf.Lerp(0, 1 + _currentGuy.bias * 0.05f, 0.5f + totalPenalty * 0.5f) * SessionVariables.IncomeMultiplier * SessionVariables.MaxIncomeBase, 0);

        reward = reward.MakeDollars();

        Globals.LastGain = reward;

        if (reward > 6 - SessionVariables.Reputation + 0.1f)
        {
            SessionVariables.Followers = SessionVariables.Followers + 1;
        }
        SessionVariables.Experience = SessionVariables.Experience + reward * reward * 0.002f;
        SessionVariables.Reputation = Mathf.Max(0, SessionVariables.Reputation + 0.5f * totalPenalty + 0.25f);

        SessionVariables.TodaysEarnings += reward;

        OnSubmit.Invoke(reward);
    }
}
