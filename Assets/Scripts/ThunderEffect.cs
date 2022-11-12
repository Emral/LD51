using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ThunderEffect : MonoBehaviour
{
    public SFX sound;
    public List<CanvasGroup> thunderEffects;
    public List<Image> blackedOutImages;

    [MinMaxSlider(minValue: 4, maxValue: 100)]
    public Vector2 cooldownRange;

    // Start is called before the first frame update
    void Start()
    {
        var weather = SessionVariables.GetTodaysWeather().weather;
        if (weather == Weather.Thunder || weather == Weather.Snowstorm)
        {
            CoroutineManager.Start(ThunderLoop());
        }
    }

    private IEnumerator ThunderLoop()
    {
        while (true)
        {
            float cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            while (!MetaManager.GameCanAdvance || cooldown > 0)
            {
                yield return null;
                cooldown -= Time.deltaTime;
            }

            sound.Play();

            foreach (var cg in thunderEffects)
            {
                var c = cg;
                cg.DOFade(1, 0.125f).SetEase(Ease.OutQuad).OnComplete(() => c.DOFade(0, 1.5f).SetEase(Ease.InQuad));
            }

            foreach (var im in blackedOutImages)
            {
                var i = im;
                im.DOColor(Color.black + Color.white * 0.1f, 0.125f).SetEase(Ease.OutQuad).OnComplete(() => i.DOColor(Color.white, 1.5f).SetEase(Ease.InQuad));
            }
        }
    }
}
