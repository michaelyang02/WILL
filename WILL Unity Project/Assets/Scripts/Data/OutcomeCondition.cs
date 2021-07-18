using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 5 types of conditions
// 1. strict sequence (seq 0.1, 0.2) 0.1 must be immediately before 0.2, etc.
// 1*. loose sequence (bef 0.1, 0.2) 0.1 is before but not necessarily immediately before 0.2 (may add later)
// 2. include (inc 0.1)
// 3. exclude (exc 0.4)
// 4. others (eta) et alii; outcome with condition must be last

public abstract class OutcomeCondition
{
    public abstract bool IsConditionMet(List<RearrangementData.TextboxIndices> textboxIndices);

    public static OutcomeCondition FromString(string conditionString)
    {
        List<RearrangementData.TextboxIndices> textboxIndices = new List<RearrangementData.TextboxIndices>();

        if (conditionString.Length > 4)
        {
            string[] conditions = conditionString.Substring(4).Split(new string[] { ", " }, System.StringSplitOptions.None);
            textboxIndices = conditions.Select(s =>
            {
                string storyString = s.Substring(0, s.IndexOf('.'));
                string textboxString = s.Substring(s.IndexOf('.') + 1);
                return new RearrangementData.TextboxIndices() { storyIndex = int.Parse(storyString), textboxIndex = int.Parse(textboxString) };
            }).ToList();
        }

        switch (conditionString.Substring(0, 3))
        {
            case "eta":
                return new OthersCondition();
            case "seq":
                return new SequenceCondition { matchTextboxIndices = textboxIndices };
            case "inc":
                return new IncludeCondition { matchTextboxIndices = textboxIndices };
            case "exc":
                return new ExcludeCondition { matchTextboxIndices = textboxIndices };
            default:
                return null;
        }
    }
}

public class SequenceCondition : OutcomeCondition
{
    public List<RearrangementData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementData.TextboxIndices> textboxIndices)
    {
        int matchListCount = matchTextboxIndices.Count;

        for (int i = 0; i < textboxIndices.Count - matchTextboxIndices.Count + 1; i++)
        {
            bool isMatch = true;
            for (int x = 0; x < matchTextboxIndices.Count; x++)
            {
                if (!textboxIndices[i + x].Equals(matchTextboxIndices[x]))
                {
                    isMatch = false;
                    break;
                }
            }
            if (isMatch) return true;
        }
        return false;
    }

    public override string ToString()
    {
        return "seq " + string.Join(", ", matchTextboxIndices.Select(m => m.storyIndex.ToString() + "." + m.textboxIndex).ToList());
    }
}

public class IncludeCondition : OutcomeCondition
{
    public List<RearrangementData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementData.TextboxIndices> textboxIndices)
    {
        return !textboxIndices.Except(matchTextboxIndices).Any();
    }

    public override string ToString()
    {
        return "inc " + string.Join(", ", matchTextboxIndices.Select(m => m.storyIndex.ToString() + "." + m.textboxIndex).ToList());
    }
}

public class ExcludeCondition : OutcomeCondition
{
    public List<RearrangementData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementData.TextboxIndices> textboxIndices)
    {
        return !textboxIndices.Intersect(matchTextboxIndices).Any();
    }

    public override string ToString()
    {
        return "exc " + string.Join(", ", matchTextboxIndices.Select(m => m.storyIndex.ToString() + "." + m.textboxIndex).ToList());
    }
}

public class OthersCondition : OutcomeCondition
{
    public override bool IsConditionMet(List<RearrangementData.TextboxIndices> textboxIndices)
    {
        return true;
    }

    public override string ToString()
    {
        return "eta";
    }
}

public static class OutcomeConditionList
{
    public static bool IsConditionMet(this List<OutcomeCondition> outcomeConditions, List<RearrangementData.TextboxIndices> textboxIndices)
    {
        return outcomeConditions.Aggregate(true, (satisfied, next) =>
        satisfied && next.IsConditionMet(textboxIndices));
    }
}