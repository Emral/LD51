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
    public PostgamePlayerAnimation PlayerPerson;

    public RectTransform Results;
    public RectTransform Canvas;
    public Image CanvasContents;

    public Button LoseButton;
    public Button ContinueButton;
    public Text SuccessText;

    // Start is called before the first frame update
    void Start()
    {
        CoroutineManager.Start(ResultsRoutine());
    }

    private IEnumerator ResultsRoutine()
    {
        yield return new WaitForSeconds(1);
        EarningsText.SetActive(true);
        var limit = SessionVariables.Savings + SessionVariables.TodaysEarnings;
        while (SessionVariables.Savings < limit)
        {
            yield return null;
            SessionVariables.Savings += Time.deltaTime * 100;
        }
        SessionVariables.Savings = limit;
        yield return new WaitForSeconds(1);
        ExpensesText.SetActive(true);
        limit = SessionVariables.Savings - SessionVariables.CalculateExpenses();
        while (SessionVariables.Savings > limit)
        {
            yield return null;
            SessionVariables.Savings -= Time.deltaTime * 100;
        }
        SessionVariables.Savings = limit;

        if (SessionVariables.Savings > 0)
        {
            PlayerPerson.MakeJump();
            SuccessText.text = "Well done!";
            SuccessText.gameObject.SetActive(true);
            SuccessText.transform.localScale = Vector3.zero;
            SuccessText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.75f);
            CoroutineManager.Start(Canvasloop());
            ContinueButton.gameObject.SetActive(true);
            ContinueButton.transform.localScale = Vector3.zero;
            ContinueButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        } else
        {
            PlayerPerson.MakeDie();
            SuccessText.text = "Too bad...";
            SuccessText.gameObject.SetActive(true);
            SuccessText.transform.localScale = Vector3.zero;
            SuccessText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.75f);
            CoroutineManager.Start(Canvasloop());
            LoseButton.gameObject.SetActive(true);
            LoseButton.transform.localScale = Vector3.zero;
            LoseButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private IEnumerator Canvasloop()
    {
        if (SessionVariables.todaysDrawings.Count > 0)
        {
            yield return new WaitForSeconds(1.75f);
            Results.DOMoveY(-180, 1).SetEase(Ease.InOutQuint);
            while (true)
            {
                Canvas.transform.localPosition = Vector3.up * 360;
                CanvasContents.sprite = Sprite.Create(SessionVariables.todaysDrawings[Random.Range(0, SessionVariables.todaysDrawings.Count)], new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
                Canvas.DOMoveY(180, 1).SetEase(Ease.InOutQuint);
                yield return new WaitForSeconds(2);
                Canvas.DOMoveY(-180, 1).SetEase(Ease.InOutQuint);
                yield return new WaitForSeconds(1);
            }
        }
    }
}