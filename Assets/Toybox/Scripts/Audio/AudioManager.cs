using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public enum SFX
{
    None = 0,
    ButtonHover = 17,
    ButtonUnhover = 1,
    ButtonPress = 2,
    GameStart = 3,
    ToMainGame = 4,
    ToNextDay = 5,
    DayAdvance = 6,
    SelectColor = 7,
    Toggle = 8,
    Sell = 9,
    NewCustomer = 10,
    DayEnd = 11,
    IncomeRise = 12,
    IncomeLower = 13,
    DaySuccess = 14,
    GameOver = 15,
    Pageturn = 16,
    ReadySetGoClick = 18,
    SharpieCircle = 19,

    Customer_Greenguy = 100,
    Customer_Banker = 101,
    Customer_Gilgamesh = 102,
    Customer_Body = 103,
    Customer_Bogos = 104,
    Customer_Stickman = 105,
    Customer_Bucket = 106,
    Customer_Carrod = 107,
    Customer_Steven = 108,
    Customer_Evilsteven = 109,
    Customer_Ghost = 110,
    Customer_Grandma = 111,
    Customer_John = 112,
    Customer_Marlon = 113,
    Customer_Nick = 114,
    Customer_Stan = 115,
    Customer_Tinsuit = 116,
    Customer_Duck = 117,
    Customer_Cat = 118,
    Customer_Mushman = 119,
    Customer_Count = 120,
    Customer_Robot = 121,
    Customer_Ronin = 122,
    Customer_Businessman = 123,
    Customer_Eligor = 124,
    Customer_Elvis = 125,
    Customer_Car = 126,
    Customer_RealCar = 127,
    Customer_Match = 128,
    Customer_Bowling = 129, 
    Customer_Bungus = 130,
    Customer_Hand = 131,
    Customer_Gordon = 132,
    Customer_Bouncy = 133,
    Customer_UFO = 134,
    Customer_Bloo = 135,
    Customer_Pumpkin = 136,
    Customer_Horseman = 137,
    Customer_Crocodile = 138,
    Customer_Oldman = 139,
    Customer_FBI = 140,
    Customer_Snowgirl = 141,
    Customer_Tree = 142,
    Customer_WorriedSkeleton = 143,
    Customer_Nutcracker = 144,
    Customer_Karategirl = 145,
    Customer_Magician = 146,
    Customer_WormGF = 147,
    Customer_Fairy = 148,
    Customer_Nagitrout = 149,
    Customer_Crowd = 150,
    Customer_Girl = 151,
    Customer_Kimonogirl = 152,
    Customer_Polygongirl = 153,
    Customer_Summergirl = 154,
    Customer_Surfmush = 155,
    Customer_Parrot = 156,
    Customer_Lamp = 157,
    Customer_Flurret = 158,
    Customer_Frog = 159,
    Customer_Iceman = 160,
    Customer_Snowman = 161,
    Customer_Toaster = 162,
    Customer_MirrorGoblin = 163,
    Customer_Grrrol = 164,
    Customer_Anubis = 165,
    Customer_Scientist = 166,
    Customer_Traveller = 167,
    Customer_Benfranklin = 168,
    Customer_Hoodie = 169,
    Customer_Luminati = 170,
    Customer_Moray = 171,
    Customer_Pengour = 172,
    Customer_Sleeper = 173,
    Customer_Slug = 174,
    Customer_Zeus = 175
}

public enum BGM
{
    None = -1,
    MainMenu,
    PreGame,
    PostGame,
    GameRegular,
    GameRain,
    GameThunderStorm,
    GameFestival
}


public class AudioManager : MonoBehaviour
{
    public AudioSource SFXSource;
    public AudioSource MusicSource;
    public AudioSource MusicSubTrackSource;
    public AudioSource TutorialSource;

    public AudioDefinition data;

    private static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private float time;
    private BGM targetClip;

    public static async void ChangeMusic(AudioClip newClip, float blendSpeed = 0.5f)
    {
        if (newClip != instance.MusicSource.clip)
        {
            if (instance.MusicSource.clip != null)
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.unscaledDeltaTime * blendSpeed;
                    instance.MusicSource.volume = t;
                    await System.Threading.Tasks.Task.Yield();
                }
            }
            instance.MusicSource.clip = newClip;
            instance.MusicSource.Play();

            if (newClip != null)
            {
                float t = 0;
                while (t < 1)
                {
                    t += Time.unscaledDeltaTime * blendSpeed;
                    instance.MusicSource.volume = Mathf.Min(t, 1);
                    await System.Threading.Tasks.Task.Yield();
                }
            }
        }
    }

    public static void ChangeMusic(BGM introClip, BGM loopingClip)
    {
        instance.targetClip = loopingClip;
        ChangeMusic(instance.data.GetMusic(introClip), instance.data.GetMusic(loopingClip));
    }

    public static void ChangeMusic(BGM clip)
    {
        instance.targetClip = clip;
        ChangeMusic(null, instance.data.GetMusic(clip));
    }

    private void OnApplicationQuit()
    {
        MusicSource.Stop();
        MusicSubTrackSource.Stop();
        MusicSource.clip = null;
        MusicSubTrackSource.clip = null;
    }

    public static async void FadeOut()
    {
        instance.targetClip = BGM.None;
        if (instance.MusicSource.clip != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 0.5f;
                instance.MusicSource.volume = t;
                instance.MusicSubTrackSource.volume = t;
                await System.Threading.Tasks.Task.Yield();
            }
        }
        instance.MusicSource.Stop();
        instance.MusicSubTrackSource.Stop();
    }

    public static async void ChangeMusic(Music introClip, Music loopingClip)
    {
        if (((introClip != null && introClip.clip != instance.MusicSource.clip) || introClip == null) && (loopingClip != null && loopingClip.clip != instance.MusicSource.clip))
        {
            if (introClip != null)
            {
                if (instance.MusicSource.clip != null)
                {
                    float t = 1;
                    while (t > 0)
                    {
                        t -= Time.unscaledDeltaTime * 0.5f;
                        instance.MusicSource.volume = t;
                        instance.MusicSubTrackSource.volume = t;
                        await System.Threading.Tasks.Task.Yield();
                    }
                }
                instance.MusicSource.clip = introClip.clip;
                instance.MusicSource.Play();
                instance.MusicSource.time = 0;
                instance.MusicSource.volume = introClip.volume;
                instance.MusicSource.loop = false;
                instance.MusicSubTrackSource.clip = introClip.subLoopClip;
                instance.MusicSubTrackSource.Play();
                instance.MusicSubTrackSource.time = 0;
                instance.time = 0;
                instance.MusicSubTrackSource.volume = 0;
                instance.MusicSubTrackSource.loop = false;
                while (instance.MusicSource.isPlaying)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
                if (!Application.isPlaying || instance.MusicSource.clip != introClip.clip)
                {
                    return;
                }
            }

            instance.MusicSource.clip = loopingClip.clip;
            instance.MusicSource.loop = true;
            instance.MusicSource.Play();
            instance.MusicSource.time = 0;
            instance.time = 0;
            instance.MusicSource.volume = loopingClip.volume;
            instance.MusicSubTrackSource.clip = loopingClip.subLoopClip;
            instance.MusicSubTrackSource.loop = true;
            instance.MusicSubTrackSource.Play();
            instance.MusicSubTrackSource.time = 0;
            instance.MusicSubTrackSource.volume = 0;
        }
    }

    public static void StoredFadeout(float duration)
    {
        instance.time = instance.MusicSource.time;
        instance.MusicSource.DOFade(0, duration);
        instance.MusicSubTrackSource.DOFade(0, duration);
        instance.TutorialSource.volume = 0;
        instance.TutorialSource.time = (5 + Random.Range(0, 7) * 10) % instance.TutorialSource.clip.length;
        instance.TutorialSource.Play();
        instance.TutorialSource.DOFade(1, duration);
    }
    public static void StoredRestore(float duration)
    {
        if (instance.targetClip != BGM.None && instance.MusicSubTrackSource.clip != null)
        {
            instance.MusicSource.time = (instance.time - duration);
            instance.MusicSubTrackSource.time = (instance.time - duration);
            instance.MusicSource.DOFade(1, duration);
            instance.MusicSubTrackSource.DOFade(1, duration);
        }
        instance.TutorialSource.DOFade(0, duration).OnComplete(() => instance.TutorialSource.Stop());
    }

    public static void SetSubtrackVolume(float volume)
    {
        instance.MusicSubTrackSource.volume = volume;
    }

    public static void PlaySound(SFX sfx, float volumeModifier = 1, AudioSource sourceToPlayFrom = null)
    {
        if (sourceToPlayFrom == null)
        {
            sourceToPlayFrom = instance.SFXSource;
        }
        Sound sound = instance.data.GetSound(sfx);
        if (sound != null)
        {
            sourceToPlayFrom.PlayOneShot(sound.clip, sound.volume * volumeModifier);
        }
    }

    public static void ToggleMusic()
    {
        instance.MusicSource.mute = !instance.MusicSource.mute;
        instance.MusicSubTrackSource.mute = !instance.MusicSubTrackSource.mute;
        instance.SFXSource.mute = !instance.SFXSource.mute;
    }
}
