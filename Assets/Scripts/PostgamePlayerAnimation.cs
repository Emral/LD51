using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PostgamePlayerAnimation : MonoBehaviour
{
    public List<Sprite> IdleLoop;
    public List<Sprite> HappyIdleLoop;
    public Sprite Jump;
    public Sprite Defeat;

    public Image img;

    private float timer;

    private float state = 0;

    public void MakeJump()
    {
        state = 2;
        img.transform.DOLocalJump(img.transform.localPosition, 12, 2, 1f).OnComplete(() => state = 3);
    }

    public void MakeDie()
    {
        state = 1;
        img.transform.DOShakePosition(1, randomnessMode: ShakeRandomnessMode.Harmonic);
    }

    // Update is called once per frame
    void Update()
    {
        timer += 4 * Time.deltaTime;

        if (state == 0)
        {
            img.sprite = IdleLoop[Mathf.FloorToInt(timer) % IdleLoop.Count];
        } else if (state == 1)
        {
            img.sprite = Defeat;
        } else if (state == 2)
        {
            img.sprite = Jump;
        } else
        {
            img.sprite = HappyIdleLoop[Mathf.FloorToInt(timer) % IdleLoop.Count];
        }
    }
}
