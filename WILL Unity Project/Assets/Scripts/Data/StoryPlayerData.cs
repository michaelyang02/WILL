using System.Collections.Generic;

[System.Serializable]
public class StoryPlayerData
{
    public int index { get; }
    public bool isDiscovered { get; set; }
    public bool isRead { get; set; }
    public List<bool> outcomeDiscovered { get; set; }
    public int selectedOutcome { get; set; }

    public StoryPlayerData(int index, bool isDiscovered, bool isRead, List<bool> outcomeDiscovered, int selectedOutcome = 0)
    {
        this.index = index;
        this.isDiscovered = isDiscovered;
        this.isRead = isRead;
        this.outcomeDiscovered = outcomeDiscovered;
        this.selectedOutcome = selectedOutcome;
    }

}

[System.Serializable]
public class StoryPlayerDataList
{
    public List<StoryPlayerData> storyPlayerDatas { get; set; }

    public StoryPlayerDataList()
    {
        storyPlayerDatas = new List<StoryPlayerData>();
    }

    public StoryPlayerData this[int storyIndex]
    {
        get
        {
            foreach (StoryPlayerData story in storyPlayerDatas)
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