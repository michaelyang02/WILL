using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DescriptionButtonController : MonoBehaviour
{

    public enum OutcomeType
    {
        Undiscovered = ColorManager.OutcomeColor.SalmonPink,
        Discovered = ColorManager.OutcomeColor.SalmonPink,
        Selected = ColorManager.OutcomeColor.Jasmine,
        Disabled = ColorManager.OutcomeColor.Black
    }

    public int storyIndex { get; set; }
    public GameObject outcomeSquarePrefab;
    public Sprite squareFilledSprite;
    public Sprite squareBorderSprite;

    private Transform outcomePanelTransform;

    void Start()
    {
        StoryData storyData = StaticDataManager.StoryDatas[storyIndex];
        StoryPlayerData storyPlayerData = StaticDataManager.StoryPlayerDatas[storyIndex];

        // do not need to have disabled description button
        // as it will not be allowed to be opened in the first place
        GetComponent<Image>().color = ColorManager.GetColor(storyData.character);
        transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = storyData.title;
        outcomePanelTransform = transform.GetChild(1);

        if (storyPlayerData.isRead)
        {
            outcomePanelTransform.gameObject.SetActive(true);

            for (int i = 0; i < storyData.outcomes.Count; i++)
            {
                Transform outcomeSquareTransform = Instantiate(outcomeSquarePrefab).transform;
                outcomeSquareTransform.SetParent(outcomePanelTransform, false);

                if (storyPlayerData.outcomeDiscovered[i] == true)
                {
                    if (storyPlayerData.selectedOutcome == i)
                    {
                        outcomeSquareTransform.GetComponent<Image>().sprite = squareFilledSprite;
                        outcomeSquareTransform.GetComponent<Image>().color = ColorManager.GetColor(OutcomeType.Selected);
                    }
                    else
                    {
                        outcomeSquareTransform.GetComponent<Image>().sprite = squareFilledSprite;
                        outcomeSquareTransform.GetComponent<Image>().color = ColorManager.GetColor(OutcomeType.Discovered);
                    }
                }
                else
                {
                    outcomeSquareTransform.GetComponent<Image>().sprite = squareBorderSprite;
                    outcomeSquareTransform.GetComponent<Image>().color = ColorManager.GetColor(OutcomeType.Undiscovered);
                }
            }
        }
        else
        {
            outcomePanelTransform.gameObject.SetActive(false);
        }
    }

    public void LoadScene()
    {
        StaticDataManager.SelectedStoryIndices = StaticDataManager.RearrangementDatas[storyIndex].indices;
        StaticDataManager.SelectedIndex = Array.IndexOf(StaticDataManager.SelectedStoryIndices, storyIndex);

        if (StaticDataManager.StoryPlayerDatas[storyIndex].isRead)
        {
            SceneManager.LoadSceneAsync("StoryTextScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("StoryAnimatedTextScene");
        }
    }
}
