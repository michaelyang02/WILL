using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerDatas
{
    public List<StoryPlayerData> storyPlayerDatas { get; set; }
    public List<RearrangementPlayerData> rearrangementPlayerDatas { get; set; }
}

[System.Serializable]
public class SaveDatas
{
    public bool isSaved { get; set; }
    public string dateTime { get; set; }
    public int discoveredOutcomes { get; set; }
    public int totalOutcomes { get; set; }
}