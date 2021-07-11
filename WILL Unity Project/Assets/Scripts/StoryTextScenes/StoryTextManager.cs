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

        StoryData storyData = StaticDataManager.StoryDatas[StaticDataManager.SelectedStoryOutcomes[StaticDataManager.SeletedStoryOutcomeIndex].Key];
        StoryPlayerData storyPlayerData = StaticDataManager.StoryPlayerDatas[StaticDataManager.SelectedStoryOutcomes[StaticDataManager.SeletedStoryOutcomeIndex].Key];
        
        GetComponent<Image>().color = storyData.GetColor();

        // add story text
        string initialText = string.Join("\n", storyData.initialText).Replace("-", "\n").Replace("\\", "");
        string outcomeText = string.Join("\n", storyData.outcomes[StaticDataManager.
        SelectedStoryOutcomes[StaticDataManager.SeletedStoryOutcomeIndex].Value].outcomeText).Replace("-", "\n").Replace("\\", "");
    
        storyText.text = "\n" + initialText + "\n\n" + outcomeText + "\n ";

        // set story as read
        storyPlayerData.isRead = true;

        // start button
        bool isAllCompanionRead = true;
        foreach (int companionIndex in storyData.companionIndices)
        {
            if (StaticDataManager.StoryPlayerDatas[companionIndex].isRead == false)
            {
                isAllCompanionRead = false;
                break;
            }
        }

        if (isAllCompanionRead == true || StaticDataManager.SeletedStoryOutcomeIndex == StaticDataManager.SelectedStoryOutcomes.Count - 1)
        {
            startGameObject.SetActive(true);
        }

        // next letter button
        if (StaticDataManager.SelectedStoryOutcomes.Count != 1)
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
        int nextStoryOutcomeIndex = (StaticDataManager.SeletedStoryOutcomeIndex + 1) % StaticDataManager.SelectedStoryOutcomes.Count;
        StaticDataManager.SeletedStoryOutcomeIndex = nextStoryOutcomeIndex;

        if (StaticDataManager.StoryPlayerDatas[StaticDataManager.SelectedStoryOutcomes[nextStoryOutcomeIndex].Key].isRead == true)
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
        CameraManager.SetFocusPosition(StaticDataManager.StoryPosition[StaticDataManager.SelectedStoryOutcomes[StaticDataManager.SeletedStoryOutcomeIndex].Key]);
        SceneManager.LoadSceneAsync("MainGameScene");
    }
}
