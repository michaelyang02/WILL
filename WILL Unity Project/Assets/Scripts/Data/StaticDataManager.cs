using System.Collections.Generic;
using UnityEngine;

public class StaticDataManager : MonoBehaviour
{

    void Awake()
    {
        // load
        storyDatas.storyDatas = SerializationManager.LoadJSON<List<StoryData>>("storyData");
        storyPlayerDatas.storyPlayerDatas = SerializationManager.LoadJSON<List<StoryPlayerData>>("storyPlayerData");
        
        
        // save
        //SerializationManager.SaveJSON("storyPlayerData", storyPlayerDatas.storyPlayerDatas);
        SerializationManager.Save("storyPlayerData", storyPlayerDatas.storyPlayerDatas);


        // backup
        //SerializationManager.Backup("storyData", storyDatas.storyDatas);
    }



    // for main game
    public static StoryDataList storyDatas = new StoryDataList();
    public static StoryPlayerDataList storyPlayerDatas = new StoryPlayerDataList();


    // for animated and rearrangement
    public static List<KeyValuePair<int, int>> selectedStoryOutcomes = new List<KeyValuePair<int, int>>();
    public static int seletedStoryOutcomeIndex = 0;
}