using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeatherScroll : MonoBehaviour
{
    public Sprite SunSprite;
    public Sprite CloudSprite;
    public Sprite OvercastSprite;
    public Sprite RainSprite;
    public Sprite ThunderSprite;

    public WeatherElement prefab;
    public Transform root;
    public Image circle;
    bool scroll = false;

    // Start is called before the first frame update
    void Start()
    {
        circle.fillAmount = 0;
        for (int i = 0; i < 5; i++)
        {
            var p = Instantiate(prefab, root);
            p.img.sprite = SessionVariables.GetUpcomingWeather(i).weatherIcon;

            var day = i + SessionVariables.Day;
            var d = i + 1;
            if (SessionVariables.Day != 1)
            {
                d = d - 1;
                scroll = true;
            }

            p.txt.text = SessionVariables.GetUpcomingWeather(i).name;

            if (d == 1)
            {
                p.txt.gameObject.SetActive(true);
            }
        }

        if (SessionVariables.Day != 1)
        {
            SessionVariables.RemoveOldestWeather();
        }

        CoroutineManager.Start(Scroll());
    }

    private IEnumerator Scroll()
    {
        yield return new WaitForSeconds(0.75f);

        if (scroll)
        {
            transform.DOLocalMoveX(transform.localPosition.x - (87.4f + 8), 1f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
        }

        SFX.SharpieCircle.Play();
        circle.DOFillAmount(1, 0.4f).SetEase(Ease.OutQuint);
    }
}
