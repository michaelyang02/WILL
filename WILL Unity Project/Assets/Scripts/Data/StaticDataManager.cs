using System.Collections.Generic;
using UnityEngine;

public class StaticDataManager : MonoBehaviour
{

    static bool isLoaded = false;

    void Awake()
    {
        if (!isLoaded)
        {
            // load
            StoryDatas.storyDatas = SerializationManager.LoadJSON<List<StoryData>>("storyData");
            StoryPlayerDatas.storyPlayerDatas = SerializationManager.LoadJSON<List<StoryPlayerData>>("storyPlayerData");


            // save
            //SerializationManager.SaveJSON("storyPlayerData", storyPlayerDatas.storyPlayerDatas);
            SerializationManager.Save("storyPlayerData", StoryPlayerDatas.storyPlayerDatas);


            // backup
            //SerializationManager.Backup("storyData", storyDatas.storyDatas);
            isLoaded = true;
        }
    }

    // for main game
    public static StoryDataList StoryDatas = new StoryDataList();
    public static StoryPlayerDataList StoryPlayerDatas = new StoryPlayerDataList();


    // for animated and rearrangement
    public static List<KeyValuePair<int, int>> SelectedStoryOutcomes = new List<KeyValuePair<int, int>>();
    public static int SeletedStoryOutcomeIndex = 0;

    public static Dictionary<int, Vector2Int> StoryPosition = new Dictionary<int, Vector2Int>();
}