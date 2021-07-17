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
            RearrangementDatas.rearrangementDatas = SerializationManager.LoadJSON<List<RearrangementData>>("rearrangementData");


            // save
            //SerializationManager.SaveJSON("storyPlayerData", storyPlayerDatas.storyPlayerDatas);
            SerializationManager.Save("storyPlayerData", StoryPlayerDatas.storyPlayerDatas);
            SerializationManager.SaveJSON("rearrangementData", RearrangementDatas.rearrangementDatas);

            // backup
            //SerializationManager.Backup("storyData", storyDatas.storyDatas);
            isLoaded = true;
        }
    }

    // for main game
    public static StoryDataList StoryDatas = new StoryDataList();
    public static StoryPlayerDataList StoryPlayerDatas = new StoryPlayerDataList();
    public static RearrangementDataList RearrangementDatas = new RearrangementDataList();


    // for animated and rearrangement
    public static int[] SelectedStoryIndices;
    public static int SelectedIndex;

    public static Dictionary<int, Vector2> StoryPosition = new Dictionary<int, Vector2>();
}