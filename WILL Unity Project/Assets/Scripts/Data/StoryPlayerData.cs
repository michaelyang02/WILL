using System.Collections.Generic;

[System.Serializable]
public class StoryPlayerData
{
    public int index { get; set;}
    public bool isDiscovered { get; set; }
    public bool isRead { get; set; }
    public List<bool> outcomeDiscovered { get; set; }
    public int selectedOutcome { get; set; }

}

[System.Serializable]
public class StoryPlayerDataList : List<StoryPlayerData>
{
    new public StoryPlayerData this[int index]
    {
        get
        {
            foreach (StoryPlayerData storyPlayerData in this)
            {
                if (storyPlayerData.index == index)
                {
                    return storyPlayerData;
                }
            }
            return null;
        }
    }
}