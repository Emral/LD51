using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Guy", menuName = "Data/Guy")]
public class Guy : SerializedScriptableObject
{
    [System.Serializable]
    [InlineProperty]
    public class Chance
    {
        [Range(0f, 1f)]
        public float Multiplier = 1;
    }
    [System.Serializable]
    [InlineProperty]
    public class Weight
    {
        [Range(0, 10)]
        public int WeightValue = 10;
    }

    [System.Serializable]
    public class ThanksOverride
    {
        [Header("Adding elements to any of these will overwrite the default selection of a quality tier.")]
        [Header("<1 dollar")]
        public List<string> worst;
        [Header("<0.2 * maximum")]
        public List<string> bad;
        [Header("<0.5 * maximum")]
        public List<string> decent;
        [Header("<0.75 * maximum")]
        public List<string> good;
        [Header("<0.9 * maximum")]
        public List<string> great;
        [Header("else (full marks")]
        public List<string> exceptional;
    }

    [PreviewField(ObjectFieldAlignment.Left)]
    public Sprite idleSprite;
    public Dictionary<Tag, Sprite> tagAlterEgo = new Dictionary<Tag, Sprite>();
    public bool leavesRightSide = false;

    [Range(-2, 2)]
    public float bias = 0;
    public Dictionary<Weather, Weight> appearanceWeights = new Dictionary<Weather, Weight>();
    public Dictionary<Season, Chance> seasonAppearanceMultipliers = new Dictionary<Season, Chance>();

    public SFX ArrivalSound = SFX.None;

    [Range(0, 1)]
    public float DayTagSelectionChance = 0f;
    public bool OnlyAppearIfDayPrefersTag;
    [Range(0, 1)]
    public float PreferredTagSelectionChance = 0.5f;

    public List<Tag> preferredTags = new List<Tag>();

    public ThanksOverride thanksMessages = new ThanksOverride();
}
