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
            StoryDatas = SerializationManager.LoadJSON<StoryDataList>("storyData");
            StoryPlayerDatas = SerializationManager.LoadJSON<StoryPlayerDataList>("storyPlayerData");
            RearrangementDatas = SerializationManager.LoadJSON<RearrangementDataList>("rearrangementData");

            /*
            StoryDatas.Add(new StoryData
            {
                index = 0,
                childrenIndices = new List<int>(),
                character = StoryData.Character.MrsJacobs,

                title = "Time is right",
                initialText = new List<string> ()
                {
                    "Cold night.",
                    "Desolate."
                },
                outcomes = new StoryData.OutcomeList()
                {
                    new StoryData.Outcome()
                    {
                        outcomeIndex = 0,
                        outcomeConditions = new List<OutcomeCondition>() {OutcomeCondition.FromString("seq 0.1, 0.2, 0.3")},
                        outcomeText = new List<string>()
                        {
                            "Fire.",
                            "Rain."
                        },
                        enabledStories = new List<int>(),
                        enabledOutcomes = new List<StoryData.Outcome.OutcomeIndices>()
                    }
                },
                lastLineTypes = new Dictionary<int, StoryData.LineFlags>()
                {
                    {-1, StoryData.LineFlags.Draggable},
                    {-2, StoryData.LineFlags.None}
                },
                lineEffects = new Dictionary<int, StoryData.Effect>()
            });

            StoryPlayerDatas.Add(new StoryPlayerData()
            {
                index = 0,
                isDiscovered = true,
                isRead = false,
                outcomeDiscovered = new List<bool>()
                {
                    true, false, false
                },
                selectedOutcome = 0
            });

            RearrangementDatas.Add(new RearrangementData()
            {
                indices = new int[] {0},
                rearrangementTextboxIndices = new Dictionary<int, List<RearrangementData.TextboxIndices>>()
                {
                    {0, new List<RearrangementData.TextboxIndices>()
                    {
                        new RearrangementData.TextboxIndices() {storyIndex = 0, textboxIndex = 0},
                        new RearrangementData.TextboxIndices() {storyIndex = 0, textboxIndex = 1}
                    }}
                }
            });
            */

            // save
            SerializationManager.SaveJSON("storyData", StoryDatas);
            SerializationManager.SaveJSON("storyPlayerData", StoryPlayerDatas);
            SerializationManager.SaveJSON("rearrangementData", RearrangementDatas);

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

    public static List<StoryData.Outcome.OutcomeIndices> AnimatedOutcomes = new List<StoryData.Outcome.OutcomeIndices>();
}