using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 5 types of conditions
// 1. strict sequence (seq 0.1, 0.2) 0.1 must be immediately before 0.2, etc.
// 1*. loose sequence (bef 0.1, 0.2) 0.1 is before but not necessarily immediately before 0.2 (may add later)
// 2. include (inc 0.1)

public abstract class OutcomeCondition
{
    public abstract bool IsConditionMet(List<RearrangementPlayerData.TextboxIndices> textboxIndices);

    public static List<OutcomeCondition> FromString(string conditionsString)
    {
        string[] conditionStrings = conditionsString.Split(new string[] { "; "}, System.StringSplitOptions.None);
        List<OutcomeCondition> outcomeConditions = new List<OutcomeCondition>();

        foreach (string cs in conditionStrings)
        {
            List<RearrangementPlayerData.TextboxIndices> textboxIndices = new List<RearrangementPlayerData.TextboxIndices>();

            string[] conditions = cs.Substring(4).Split(new string[] { ", " }, System.StringSplitOptions.None);
            textboxIndices = conditions.Select(s =>
            {
                string storyString = s.Substring(0, s.IndexOf('.'));
                string textboxString = s.Substring(s.IndexOf('.') + 1);
                return new RearrangementPlayerData.TextboxIndices() { storyIndex = int.Parse(storyString), textboxIndex = int.Parse(textboxString) };
            }).ToList();

            switch (cs.Substring(0, 3))
            {
                case "seq":
                    outcomeConditions.Add(new SequenceCondition { matchTextboxIndices = textboxIndices });
                    break;
                case "bef":
                    outcomeConditions.Add(new BeforeCondition { matchTextboxIndices = textboxIndices });
                    break;
                case "inc":
                    outcomeConditions.Add(new IncludeCondition { matchTextboxIndices = textboxIndices });
                    break;
                default:
                    break;
            }
        }
        return outcomeConditions;
    }
}

public class SequenceCondition : OutcomeCondition
{
    public List<RearrangementPlayerData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementPlayerData.TextboxIndices> textboxIndices)
    {
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

public class BeforeCondition : OutcomeCondition
{
    public List<RearrangementPlayerData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementPlayerData.TextboxIndices> textboxIndices)
    {
        for (int i = 0; i < matchTextboxIndices.Count - 1; i++)
        {
            if (textboxIndices.IndexOf(matchTextboxIndices[i]) == -1 || textboxIndices.IndexOf(matchTextboxIndices[i]) > textboxIndices.IndexOf(matchTextboxIndices[i + 1]))
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        return "bef " + string.Join(", ", matchTextboxIndices.Select(m => m.storyIndex.ToString() + "." + m.textboxIndex).ToList());
    }
}

public class IncludeCondition : OutcomeCondition
{
    public List<RearrangementPlayerData.TextboxIndices> matchTextboxIndices { get; set; }

    public override bool IsConditionMet(List<RearrangementPlayerData.TextboxIndices> textboxIndices)
    {
        return !textboxIndices.Except(matchTextboxIndices).Any();
    }

    public override string ToString()
    {
        return "inc " + string.Join(", ", matchTextboxIndices.Select(m => m.storyIndex.ToString() + "." + m.textboxIndex).ToList());
    }
}

public static class OutcomeConditionList
{
    public static bool IsConditionMet(this List<List<OutcomeCondition>> outcomeConditions, List<RearrangementPlayerData.TextboxIndices> textboxIndices)
    {
        return outcomeConditions.Aggregate(false, (satisfied, next) =>
        satisfied || next.Aggregate(true, (s, n) => s && n.IsConditionMet(textboxIndices)));
    }

    public static string OutcomeConditionListString(this List<OutcomeCondition> outcomeConditions)
    {
        return string.Join("; ", outcomeConditions.Select(oc => oc.ToString()));
    }
}