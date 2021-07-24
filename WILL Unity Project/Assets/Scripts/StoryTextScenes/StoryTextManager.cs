using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{

    public GameObject storyTextGameObject;
    public GameObject startGameObject;
    public GameObject nextLetterGameObject;
    public GameObject replayGameObject;
    public GameObject backGameObject;

    private TMPro.TMP_Text storyText;

    void Start()
    {
        storyText = storyTextGameObject.GetComponent<TMPro.TMP_Text>();

        StoryData storyData = StaticDataManager.StoryDatas[StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex]];
        StoryPlayerData storyPlayerData = StaticDataManager.StoryPlayerDatas[StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex]];

        GetComponent<Image>().color = ColorManager.GetColor(storyData.character);

        // add story text
        string initialText = string.Join("\n", StoryAnimatedTextManager.GetRearrangedInitialText(storyData)).Replace("-", "\n").Replace("\\", "");
        string outcomeText = string.Join("\n", storyData.outcomes[StaticDataManager.StoryPlayerDatas[storyData.index].
        selectedOutcome].outcomeText).Replace("-", "\n").Replace("\\", "");

        storyText.text = "\n" + initialText + "\n\n" + outcomeText + "\n ";

        // set story as read
        storyPlayerData.isRead = true;

        // start button
        bool isAllCompanionRead = true;
        foreach (int companionIndex in StaticDataManager.SelectedStoryIndices)
        {
            if (StaticDataManager.StoryPlayerDatas[companionIndex].isRead == false)
            {
                isAllCompanionRead = false;
                break;
            }
        }

        if (isAllCompanionRead == true)
        {
            startGameObject.SetActive(true);
        }

        // next letter button
        if (StaticDataManager.SelectedStoryIndices.Length != 1)
        {
            nextLetterGameObject.SetActive(true);
        }
    }

    public void StartRerrangement()
    {
        SceneManager.LoadSceneAsync("StoryRearrangementScene");
    }

    public void GoToNextLetter()
    {
        int nextIndex = (StaticDataManager.SelectedIndex + 1) % StaticDataManager.SelectedStoryIndices.Length;
        StaticDataManager.SelectedIndex = nextIndex;

        if (StaticDataManager.StoryPlayerDatas[StaticDataManager.SelectedStoryIndices[nextIndex]].isRead == true)
        {
            SceneManager.LoadSceneAsync("StoryTextScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("StoryAnimatedTextScene");
        }
    }

    public void ReplayStory()
    {
        SceneManager.LoadSceneAsync("StoryAnimatedTextScene");
    }

    public void BackToMainGame()
    {
        CameraManager.SetFocusPosition(StaticDataManager.StoryPosition[StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex]]);
        SceneManager.LoadSceneAsync("MainGameScene");
    }
}
