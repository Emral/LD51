using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class Tutorial : MonoBehaviour
{
    public GameObject Root;
    public CanvasGroup Overlay;
    public RectTransform BG;
    public Text Title;
    public Text Description;
    public Button Confirm;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private void Start()
    {
        goalPos = BG.transform.localPosition;
        goalRot = BG.transform.localRotation;
        BG.GetComponent<CanvasGroup>().alpha = 1;
        ResetAll();
    }

    private void ResetAll()
    {
        Confirm.gameObject.SetActive(false);
        Confirm.transform.localScale = Vector3.zero;
        Overlay.alpha = 0;
        Overlay.interactable = false;
        Overlay.blocksRaycasts = false;
        BG.transform.localPosition = Vector3.down * 360;
        BG.transform.localRotation = Quaternion.Euler(0, 0, 45);
    }

    public IEnumerator ShowRoutine(TutorialElement elem)
    {
        if (elem != null)
        {
            ShowTutorial(elem);

            while (Overlay.interactable)
            {
                yield return null;
            }
        }

    }

    public async Task Show(TutorialElement elem)
    {
        if (elem == null)
        {
            return;
        }

        ShowTutorial(elem);

        while (Overlay.interactable)
        {
            await Task.Yield();
        }
        
    }

    public void ShowTutorial(TutorialElement elem)
    {
        if (SessionVariables.TutorialsDone.Value.Contains(elem.mechanic) || !MetaManager.TutorialActive)
        {
            return;
        }

        AudioManager.StoredFadeout(0.75f);
        MetaManager.GameCanAdvance = false;
        SessionVariables.TutorialsDone.Add(elem.mechanic);
        ResetAll();
        Overlay.interactable = true;
        Overlay.blocksRaycasts = true;

        Title.text = elem.title;
        Description.text = elem.description;

        Overlay.DOFade(1, 0.75f).SetEase(Ease.OutQuad);
        BG.transform.DOLocalRotateQuaternion(goalRot, 1.25f).SetEase(Ease.OutQuint);
        BG.transform.DOLocalMove(goalPos, 1.25f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            Confirm.gameObject.SetActive(true);
            Confirm.interactable = true;
            Confirm.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        });
    }

    public void Finish()
    {
        SFX.ButtonPress.Play();
        SFX.Pageturn.Play();
        Confirm.interactable = false;
        Confirm.transform.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InBack);
        Overlay.DOFade(0, 0.75f).SetEase(Ease.OutQuad);
        AudioManager.StoredRestore(0.6f);
        BG.transform.DOLocalMove(goalPos - 360f * Vector3.up, 0.6f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Overlay.interactable = false;
            Overlay.blocksRaycasts = false;
            MetaManager.GameCanAdvance = true;
            Confirm.gameObject.SetActive(false);
        });
    }
}
