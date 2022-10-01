using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private List<Texture2D> _oldTextures = new List<Texture2D>();
    private int _texturesToday;
    private int _moneyToday;

    private float _dayTimeElapsed;
    private float _timerTimeElapsed;

    private bool _dayActive;
    private bool _timerActive;

    public static bool DayActive;
    public static bool TimerActive;

    public static UnityEvent<float> OnSubmit = new UnityEvent<float>();
    public static UnityEvent<GuySprite> OnNewGuy = new UnityEvent<GuySprite>();

    private Texture2D __currentGuessable;
    private Texture2D _currentGuessable
    {
        get => __currentGuessable;
        set {
            __currentGuessable = value;
            GuessableImage.sprite = Sprite.Create(__currentGuessable, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
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
        NewGuessable();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dayActive)
        {
            _dayTimeElapsed = _dayTimeElapsed + Time.deltaTime;
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

            _timerTimeElapsed = Mathf.Max(_timerTimeElapsed - 20 * Time.deltaTime, isRushHour ? 1 : 0);
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
        CompareImages(tex, _currentGuessable);
        _oldTextures.Add(tex);
        _texturesToday++;
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

        totalPenalty = (0.6f - totalPenalty) * 0.5f;

        Debug.Log(totalPenalty);

        var reward = Mathf.Clamp(totalPenalty, 0.01f, Globals.MaxIncome);

        reward = Mathf.Ceil(reward * 100) / 100.0f;

        Globals.Money += reward;

        OnSubmit.Invoke(reward);
    }
}
