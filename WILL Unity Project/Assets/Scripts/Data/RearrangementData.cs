using System.Collections.Generic;

[System.Serializable]
public class RearrangementData
{
    public int[] indices { get; set; }
    public List<MasterOutcome> masterOutcomes { get; set; }
}

[System.Serializable]
public class MasterOutcome
{
    public Dictionary<int, List<List<OutcomeCondition>>> requiredOutcomeConditions { get; set; }
    public List<StoryData.OutcomeIndices> outcomes { get; set; }
}