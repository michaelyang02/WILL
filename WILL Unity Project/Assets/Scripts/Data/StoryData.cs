using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StoryData
{

    public string GetCharacter()
    {
        switch (character)
        {
            case Character.MrsJacobs:
                return "Mrs Jacobs";
            case Character.DetectiveJohnson:
                return "Detective Johnson";
            default:
                return "Invalid character";
        }
    }

    [System.Serializable]
    public enum HighlightColor
    {
        Blue = 0x78bfe3,
        Green = 0x00a6a6,
        Yellow = 0xefca08,
        Orange = 0xf49f0a,
        Red = 0xd63230
    }

    [System.Serializable]
    public enum HighlightType
    {
        Information = HighlightColor.Blue
    }

    [System.Serializable]
    public enum Color
    {
        ChinaPink = 0xdb6b98,
        LavenderFlorel = 0xa77acd,
        VioletBlueCrayola = 0x6a70c8,
        FieryRose = 0xeb5c6c,
    }

    [System.Serializable]
    public enum Character
    {
        MrsJacobs = Color.LavenderFlorel,
        DetectiveJohnson = Color.VioletBlueCrayola
    }

    [Flags]
    [System.Serializable]
    public enum LineFlags : byte
    {
        None = 0,
        Draggable = 1,
        Switching = 1 << 1,
        Unswappable = 1 << 2,
        Numbered1 = 1 << 3,
        Numbered2 = 1 << 4,
        Numbered3 = 1 << 5
    }

    [System.Serializable]
    public class Outcome
    {
        public struct OutcomeIndices : IEquatable<OutcomeIndices>
        {
            public int storyIndex { get; set; }
            public int outcomeIndex { get; set; }

            public bool Equals(OutcomeIndices other)
            {
                return other.storyIndex == storyIndex && other.outcomeIndex == outcomeIndex;
            }
        }
        public List<OutcomeCondition> outcomeConditions { get; set; }
        public List<string> outcomeText { get; set; }
        public List<int> enabledStories { get; set; }
        public List<OutcomeIndices> enabledOutcomes { get; set; }
    }

    [System.Serializable]
    public struct Effect
    {
        [System.Serializable]
        public enum EffectType
        {
            Animation,
            BackgroundChange
        }

        public EffectType effectType { get; }
        public int effectNumber { get; }
    }

    public int index { get; set; }
    public List<int> childrenIndices { get; set; }

    public Character character { get; set; }

    public string title { get; set; }
    public List<string> initialText { get; set; }
    public List<Outcome> outcomes { get; set; }
    public Dictionary<int, LineFlags> lastLineTypes { get; set; }
    public Dictionary<int, Effect> lineEffects { get; set; }

    // TODO: generate enable/disable list

    public UnityEngine.Color GetColor()
    {
        int color = (int)character;
        return new Color32((byte)((color >> 16) & 255), (byte)((color >> 8) & 255), (byte)(color & 255), 255);
    }
}