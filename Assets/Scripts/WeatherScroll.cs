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

            var d = i;
            if (SessionVariables.Day != SessionVariables.LastDay)
            {
                d = d - 1;
                scroll = true;
            }

            var p = Instantiate(prefab, root);

            var day = d + SessionVariables.Day.Value;
            switch (SessionVariables.GetSeason(day))
            {
                case Season.Spring:
                    p.img.sprite = SessionVariables.GetUpcomingWeather(i).weatherIconSpring;
                    break;
                case Season.Summer:
                    p.img.sprite = SessionVariables.GetUpcomingWeather(i).weatherIconSummer;
                    break;
                case Season.Autumn:
                    p.img.sprite = SessionVariables.GetUpcomingWeather(i).weatherIconAutumn;
                    break;
                case Season.Winter:
                    p.img.sprite = SessionVariables.GetUpcomingWeather(i).weatherIconWinter;
                    break;
            }
            p.txt.text = SessionVariables.GetUpcomingWeather(i).name;

            if (d == 0)
            {
                p.txt.gameObject.SetActive(true);
            }
        }

        if (SessionVariables.Day != SessionVariables.LastDay)
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
