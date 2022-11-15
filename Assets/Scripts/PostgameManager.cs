using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PostgameManager : MonoBehaviour
{
    public GameObject EarningsText;
    public GameObject ExpensesText;
    public Text SavingsValueText;
    public Text ResultsText;
    public PostgamePlayerAnimation PlayerPerson;

    public RectTransform Results;
    public RectTransform Canvas;
    public Image CanvasContents;

    public CanvasGroup GalleryOverlay;
    public RectTransform GalleryContent;
    public Transform GalleryLayoutGroup;
    public GalleryImage GalleryImagePrefab;
    public Button OpenGalleryButton;

    public Button LoseButton;
    public Button ContinueButton;
    public Text SuccessText;

    // Start is called before the first frame update
    void Start()
    {
        CoroutineManager.Start(ResultsRoutine());

        ResultsText.text = $"Day {SessionVariables.Day.Value + 1} Summary";

        int i = 0;
        foreach (var drawing in SessionVariables.todaysDrawings)
        {
            var newButton = Instantiate(GalleryImagePrefab, GalleryLayoutGroup);
            newButton.Initialize(drawing, i);
            i++;
        }
    }

    public void OpenGallery()
    {
        GalleryOverlay.DOFade(1, 0.75f).SetEase(Ease.OutQuad).OnComplete(() => {
            GalleryOverlay.blocksRaycasts = true;
            GalleryOverlay.interactable = true;
        });
        GalleryContent.transform.localPosition = Vector3.right * 512;
        GalleryContent.transform.DOLocalMoveX(0, 0.75f).SetEase(Ease.OutQuad);
    }

    public void CloseGallery()
    {
        GalleryOverlay.DOFade(0, 0.75f).SetEase(Ease.InQuad).OnComplete(() => {
            GalleryOverlay.blocksRaycasts = false;
            GalleryOverlay.interactable = false;
        });
        GalleryContent.transform.DOLocalMoveX(512, 0.75f).SetEase(Ease.InQuad);
    }

    private IEnumerator ResultsRoutine()
    {
        yield return new WaitForSeconds(1);
        EarningsText.SetActive(true);
        var limit = SessionVariables.Savings + SessionVariables.TodaysEarnings;
        while (SessionVariables.Savings < limit)
        {
            yield return null;
            SFX.IncomeRise.Play();
            SessionVariables.Savings += Time.deltaTime * 100;
        }
        SFX.Sell.Play();
        SessionVariables.Savings = limit;
        var taxExpense = SessionVariables.Expenses.Value.Find(e => e.Name == "Tax");
        float taxPercent = 1;
        if (taxExpense != null)
        {
            taxPercent = taxExpense.Value;
            taxExpense.Value = taxExpense.Value * 0.01f * SessionVariables.TodaysEarnings;
        }
        yield return new WaitForSeconds(1);
        ExpensesText.SetActive(true);
        limit = SessionVariables.Savings - SessionVariables.CalculateExpenses();
        while (SessionVariables.Savings > limit)
        {
            yield return null;
            SFX.IncomeLower.Play();
            SessionVariables.Savings -= Time.deltaTime * 100;
        }
        SessionVariables.Savings = limit;

        if (taxExpense != null)
        {
            taxExpense.Value = taxPercent;
        }

        if (SessionVariables.Savings > 0)
        {
            PlayerPerson.MakeJump();
            SuccessText.text = "Well done!";
            SFX.DaySuccess.Play();
            SuccessText.gameObject.SetActive(true);
            SuccessText.transform.localScale = Vector3.zero;
            SuccessText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.75f);
            CoroutineManager.Start(Canvasloop());
            ContinueButton.gameObject.SetActive(true);
            ContinueButton.transform.localScale = Vector3.zero;
            ContinueButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            if (SessionVariables.todaysDrawings.Count > 0)
            {
                OpenGalleryButton.gameObject.SetActive(true);
                OpenGalleryButton.transform.localScale = Vector3.zero;
                OpenGalleryButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        } else
        {
            PlayerPerson.MakeDie();
            SFX.GameOver.Play();
            MetaManager.instance.ClearSaveData();
            SuccessText.text = "Too bad...";
            SuccessText.gameObject.SetActive(true);
            SuccessText.transform.localScale = Vector3.zero;
            SuccessText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.75f);
            CoroutineManager.Start(Canvasloop());
            LoseButton.gameObject.SetActive(true);
            LoseButton.transform.localScale = Vector3.zero;
            LoseButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            if (SessionVariables.todaysDrawings.Count > 0)
            {
                OpenGalleryButton.gameObject.SetActive(true);
                OpenGalleryButton.transform.localScale = Vector3.zero;
                OpenGalleryButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }
    }

    private IEnumerator Canvasloop()
    {
        int idx = 0;
        if (SessionVariables.todaysDrawings.Count > 0)
        {
            yield return new WaitForSeconds(1.75f);
            Results.DOLocalMoveY(-360, 1).SetEase(Ease.InOutQuint);
            while (SessionVariables.todaysDrawings.Count > 0)
            {
                Canvas.transform.localPosition = Vector3.up * 360;
                CanvasContents.sprite = Sprite.Create(SessionVariables.todaysDrawings[idx % SessionVariables.todaysDrawings.Count], new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
                Canvas.DOLocalMoveY(0, 1).SetEase(Ease.OutQuint);
                yield return new WaitForSeconds(2);
                Canvas.DOLocalMoveY(-360, 1).SetEase(Ease.InQuint);
                idx++;
                yield return new WaitForSeconds(1);
            }
        }
    }
}
