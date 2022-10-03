using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PostgamePlayerAnimation : MonoBehaviour
{
    public List<Sprite> IdleLoop;
    public Sprite NoticeGood;
    public Sprite PreJump;
    public Sprite Jump;
    public Sprite HappyIdle;
    public Sprite Kneel;
    public Sprite Defeat;

    public Image img;

    private float timer;

    private float state = 0;

    public void MakeJump()
    {
        state = 2;
        timer = 0;
    }

    private void JumpFunc()
    {
        state = 6;
        timer = 0;
        img.transform.DOLocalJump(img.transform.localPosition, 36, 1, 0.6f).SetEase(Ease.Linear).OnComplete(() => state = 3);
    }

    public void MakeDie()
    {
        state = 1;
        timer = 0;
        img.transform.DOShakePosition(1, randomnessMode: ShakeRandomnessMode.Harmonic);
    }

    // Update is called once per frame
    void Update()
    {
        timer += 2 * Time.deltaTime;

        if (state == 0)
        {
            img.sprite = IdleLoop[Mathf.FloorToInt(timer) % IdleLoop.Count];
        } else if (state == 1)
        {
            img.sprite = Kneel;

            if (timer >= 2)
            {
                state = 4;
                img.transform.DOShakePosition(1, randomnessMode: ShakeRandomnessMode.Harmonic);
            }
        } else if (state == 2)
        {
            img.sprite = NoticeGood;

            if (timer >= 1.5f)
            {
                state = 5;
                timer = 0;
                img.transform.DOShakePosition(0.25f, randomnessMode: ShakeRandomnessMode.Harmonic);
            }
        }
        else if (state == 3)
        {
            img.sprite = HappyIdle;
        }
        else if (state == 4)
        {
            img.sprite = Defeat;
        }
        else if (state == 5)
        {
            img.sprite = PreJump;

            if (timer >= 1)
            {
                state = 6;
                timer = 0;
                JumpFunc();
            }
        }
        else if (state == 6)
        {
            img.sprite = Jump;

            if (timer >= 1.2f)
            {
                state = 3;
            }
        }
    }
}
