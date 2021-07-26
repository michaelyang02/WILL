using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public enum HighlightType
    {
        Information = ColorManager.HighlightColor.Blue
    }

    [System.Serializable]
    public enum Character
    {
        None,
        MrsJacobs = ColorManager.CharacterColor.LavenderFlorel,
        DetectiveJohnson = ColorManager.CharacterColor.VioletBlueCrayola
    }

    [Flags]
    [System.Serializable]
    public enum LineFlags : byte
    {
        None = 0,
        Exposition = 1,
        Draggable = 1 << 1,
        Switching = 1 << 2,
        Unswappable = 1 << 3,
        Numbered1 = 1 << 4,
        Numbered2 = 1 << 5,
        Numbered3 = 1 << 6,
        Pinned = 1 << 7
    }

    [System.Serializable]
    public struct OutcomeIndices : IEquatable<OutcomeIndices>
    {
        public int storyIndex { get; set; }
        public int outcomeIndex { get; set; }

        public bool Equals(OutcomeIndices other)
        {
            return other.storyIndex == storyIndex && other.outcomeIndex == outcomeIndex;
        }
    }

    [System.Serializable]
    public class Outcome
    {
        public List<OutcomeIndices> requiredOutcomes { get; set; }
        public List<OutcomeIndices> disablingOutcomes { get; set; }

        public List<string> outcomeText { get; set; }
        public Dictionary<int, StoryData.LineFlags> firstLineTypes { get; set; }
        public Dictionary<int, StoryData.Effect> lineEffects { get; set; }
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
    // -1 for root node
    public int parentIndex { get; set; }

    public Character character { get; set; }
    public string title { get; set; }

    // empty means none required
    public List<OutcomeIndices> requiredDiscoveredOutcomes { get; set; }
    public List<OutcomeIndices> requiredEnabledOutcomes { get; set; }
    public List<OutcomeIndices> disablingOutcomes { get; set; }

    public List<string> initialText { get; set; }
    public Dictionary<int, LineFlags> lastLineTypes { get; set; }
    public Dictionary<int, Effect> lineEffects { get; set; }
    public List<Outcome> outcomes { get; set; }

    public TextData ToTextData()
    {
        return new TextData { character = character, title = title, initialText = initialText, lastLineTypes = lastLineTypes, lineEffects = lineEffects, outcomeTexts = outcomes.Select(o => new TextData.OutcomeText() { outcomeText = o.outcomeText, firstLineTypes = o.firstLineTypes, lineEffects = o.lineEffects }).ToList() };
    }

    public IndexData ToIndexData()
    {
        return new IndexData() { index = index, parentIndex = parentIndex, requiredDiscoveredOutcomes = requiredDiscoveredOutcomes, requiredEnabledOutcomes = requiredEnabledOutcomes, disablingOutcomes = disablingOutcomes, requiredOutcomeIndices = outcomes.Select(o => o.requiredOutcomes).ToList(), disablingOutcomeIndices = outcomes.Select(o => o.disablingOutcomes).ToList() };
    }

    public static StoryData FromDatas(TextData textData, IndexData indexData)
    {
        return new StoryData()
        {
            index = indexData.index,
            parentIndex = indexData.parentIndex,
            character = textData.character,
            title = textData.title,
            requiredDiscoveredOutcomes = indexData.requiredDiscoveredOutcomes,
            requiredEnabledOutcomes = indexData.requiredEnabledOutcomes,
            disablingOutcomes = indexData.disablingOutcomes,
            initialText = textData.initialText,
            lastLineTypes = textData.lastLineTypes,
            lineEffects = textData.lineEffects,
            outcomes = textData.outcomeTexts.Zip(indexData.requiredOutcomeIndices, (o, i) => new {o, i}).Zip(indexData.disablingOutcomeIndices, (x, d) => new Outcome {requiredOutcomes = x.i, disablingOutcomes = d, outcomeText = x.o.outcomeText, firstLineTypes = x.o.firstLineTypes, lineEffects = x.o.lineEffects}).ToList()
        };
    }
}

[System.Serializable]
public class TextData
{
    [System.Serializable]
    public class OutcomeText
    {
        public List<string> outcomeText { get; set; }
        public Dictionary<int, StoryData.LineFlags> firstLineTypes { get; set; }
        public Dictionary<int, StoryData.Effect> lineEffects { get; set; }
    }
    public StoryData.Character character { get; set; }
    public string title { get; set; }
    public List<string> initialText { get; set; }
    public Dictionary<int, StoryData.LineFlags> lastLineTypes { get; set; }
    public Dictionary<int, StoryData.Effect> lineEffects { get; set; }
    public List<OutcomeText> outcomeTexts { get; set; }
}

[System.Serializable]
public class IndexData
{
    public int index { get; set; }
    public int parentIndex { get; set; }
    public List<StoryData.OutcomeIndices> requiredDiscoveredOutcomes { get; set; }
    public List<StoryData.OutcomeIndices> requiredEnabledOutcomes { get; set; }
    public List<StoryData.OutcomeIndices> disablingOutcomes { get; set; }
    public List<List<StoryData.OutcomeIndices>> requiredOutcomeIndices { get; set; }
    public List<List<StoryData.OutcomeIndices>> disablingOutcomeIndices { get; set; }
}