using System.Collections.Generic;

[System.Serializable]
public class StoryPlayerData
{
    public int index { get; }
    public bool discovered { get; }
    public List<bool> outcomeDiscovered { get; }
    public int selectedOutcome { get; }

    public StoryPlayerData(int index, bool discovered, List<bool> outcomeDiscovered, int selectedOutcome = 0)
    {
        this.index = index;
        this.discovered = discovered;
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