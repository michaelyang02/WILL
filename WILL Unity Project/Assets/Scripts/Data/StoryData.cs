using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StoryData
{

    public static List<StoryData> stories = new List<StoryData>();

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
        ChinaPink =         0xdb6b98,
        LavenderFlorel =    0xa77acd,
        VioletBlueCrayola = 0x6a70c8,
        FieryRose =         0xeb5c6c,
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
        None =          0,
        Draggable =     1,
        Switching =     1 << 1,
        Unswappable =   1 << 2,
        Numbered1 =     1 << 3,
        Numbered2 =     1 << 4,
        Numbered3 =     1 << 5
    }

    [System.Serializable]
    public enum TextboxColor
    {
        
    }


    [System.Serializable]
    public class Outcome
    {
        //public List<TextBlock> requiredArrangement { get; }
        public List<string> outcomeText { get; }
        public List<int> enabledStories { get; }
        public List<int> disabledStories { get; }

        public Outcome(List<string> outcomeText, List<int> enabledStories, List<int> disabledStories)
        {
            this.outcomeText = outcomeText;
            this.enabledStories = enabledStories;
            this.disabledStories = disabledStories;
        }
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

        public Effect(Effect.EffectType effectType, int effectNumber)
        {
            this.effectType = effectType;
            this.effectNumber = effectNumber;
        }
    }

    public int index { get; }
    public List<int> childrenIndices { get; }
    public List<int> companionIndices { get; }

    public Character character { get; }

    public string title { get; }
    public List<string> initialText { get; }
    public List<Outcome> outcomes { get; }
    public Dictionary<int, LineFlags> lastLineTypes { get; }
    public Dictionary<int, Effect> lineEffects { get; }

    public StoryData(int index, List<int> childrenIndices,
    List<int> companionIndices, Character character, string title,
    List<string> initialText, List<Outcome> outcomes,
    Dictionary<int, LineFlags> lastLineTypes, Dictionary<int, Effect> lineEffects)
    {
        this.index = index;
        this.childrenIndices = childrenIndices;
        this.companionIndices = companionIndices;
        this.character = character;
        this.title = title;
        this.initialText = initialText;
        this.outcomes = outcomes;
        this.lastLineTypes = lastLineTypes;
        this.lineEffects = lineEffects;
    }

    // TODO: generate enable/disable list

    public UnityEngine.Color GetColor()
    {
        int color = (int)character;
        return new Color32((byte)((color >> 16) & 255), (byte)((color >> 8) & 255), (byte)(color & 255), 255);
    }


    /*
    public static List<string> initial = new List<string>()
    {
        "Overcast.\\ Just as gloomy as alway.\n THe light struggled to glow at all",
        "Just as I put down the two grocery bags, he came.\\ And he is now gone.",
        "Just as <color=#78bfe3><u>Stirling</u></color> did."
    };

    public static List<Outcome> outcomes1 = new List<Outcome>()
    {
        new Outcome(initial, new List<int>() {1, 2}, null)
    };

    public static Dictionary<int, ParagraphFlags> last = new Dictionary<int, ParagraphFlags>()
    {
        {-1, ParagraphFlags.Draggable | ParagraphFlags.Unswappable},
        {-2, ParagraphFlags.Exposition}
    };

    public static StoryData story = new StoryData(0, new List<int>() {1, 2}, null, Character.MrsJacobs, "Time is right", initial, outcomes1, last);
    */
}

[System.Serializable]
public class StoryDataList
{
    public List<StoryData> storyDatas { get; set; }

    public StoryDataList()
    {
        storyDatas = new List<StoryData>();
    }

    public StoryData this[int storyIndex]
    {
        get
        {
            foreach (StoryData story in storyDatas)
            {
                if (story.index == storyIndex)
                {
                    return story;
                }
            }
            return null;
        }
    }
}