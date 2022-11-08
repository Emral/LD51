using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public SFX type;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
}

[System.Serializable]
public class Music
{
    public BGM type;
    public AudioClip clip;
    public AudioClip subLoopClip;
    [Range(0, 1)] public float volume = 1;
}
[CreateAssetMenu(menuName ="Data/Audio", fileName ="Audio")]
public class AudioDefinition : ScriptableObject
{

    public List<Sound> Sounds;
    public List<Music> Music;

    public Sound GetSound(SFX type)
    {
        List<Sound> candidates = Sounds.FindAll(s => s.type == type);
        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public Music GetMusic(BGM type)
    {
        List<Music> candidates = Music.FindAll(s => s.type == type);
        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}

public static class SFXExtensions
{
    public static void Play(this SFX sfx, float volumeModifier = 1, AudioSource sourceToPlayFrom = null)
    {
        AudioManager.PlaySound(sfx, volumeModifier, sourceToPlayFrom);
    }
}