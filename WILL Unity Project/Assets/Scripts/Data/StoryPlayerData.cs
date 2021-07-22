using System.Collections.Generic;

[System.Serializable]
public class StoryPlayerData
{
    public int index { get; set; }
    public bool isDiscovered { get; set; }
    public bool isEnabled { get; set; }
    public bool isRead { get; set; }
    public List<bool> outcomeDiscovered { get; set; }
    public List<bool> outcomeEnabled { get; set; }
    public int selectedOutcome { get; set; }
}