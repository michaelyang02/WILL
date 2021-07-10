using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadStoryAnimatedTextScene()
    {
        if (StaticDataManager.StoryPlayerDatas[StaticDataManager.SelectedStoryOutcomes[0].Key].isRead == true)
        {
            SceneManager.LoadSceneAsync("StoryTextScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("StoryAnimatedTextScene");
        }
    }
}
