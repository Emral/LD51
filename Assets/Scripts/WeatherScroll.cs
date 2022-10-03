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

    // Start is called before the first frame update
    void Start()
    {
        circle.fillAmount = 0;
        for (int i = 0; i < 10; i++)
        {
            var p = Instantiate(prefab, root);
            switch(SessionVariables.UpcomingWeathers[i])
            {
                case Weather.Sunny:
                    p.img.sprite = SunSprite;
                    break;
                case Weather.Cloudy:
                    p.img.sprite = CloudSprite;
                    break;
                case Weather.Overcast:
                    p.img.sprite = OvercastSprite;
                    break;
                case Weather.Rain:
                    p.img.sprite = RainSprite;
                    break;
                case Weather.Thunder:
                    p.img.sprite = ThunderSprite;
                    break;
            }

            var day = i + SessionVariables.Day;
            if (SessionVariables.Day != 1)
            {
                day = day - 1;
            }

            p.txt.text = SessionVariables.GetDayStringShort(day);
        }

        if (SessionVariables.Day != 1)
        {
            SessionVariables.RemoveOldestWeather();
        }

        CoroutineManager.Start(Scroll());
    }

    private IEnumerator Scroll()
    {
        yield return new WaitForSeconds(1);

        if (SessionVariables.Day - SessionVariables.LastDay > 0)
        {
            transform.DOLocalMoveX(transform.localPosition.x - (87.4f + 8) * (SessionVariables.Day - SessionVariables.LastDay), 1.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(2f);
        }

        circle.DOFillAmount(1, 0.4f).SetEase(Ease.OutQuint);
    }
}
