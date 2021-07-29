using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryHistoryManager : MonoBehaviour
{
    public static StoryHistoryManager Instance;

    public TMPro.TMP_Text storyText;
    public GameObject nextLetterGO;

    public Transform outcomePanelTransform;
    public GameObject outcomePrefab;

    public Sprite outcomeDiscoveredSprite;
    public Sprite outcomeNotDiscoveredSprite;

    private Dictionary<StoryData.OutcomeIndices, GameObject> outcomeGOs;
    private StoryData.OutcomeIndices previousOutcomeIndices;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int storyIndex = StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex];
        previousOutcomeIndices = new StoryData.OutcomeIndices() { storyIndex = storyIndex, outcomeIndex = 0 };

        if (StaticDataManager.SelectedStoryIndices.Length != 1)
        {
            nextLetterGO.SetActive(true);
        }
        
        transform.GetChild(0).GetComponent<Image>().color = ColorManager.GetColor(StaticDataManager.StoryDatas[storyIndex].character);

        outcomeGOs = new Dictionary<StoryData.OutcomeIndices, GameObject>();

        for (int i = 0; i < StaticDataManager.StoryDatas[storyIndex].outcomes.Count; i++)
        {
            GameObject outcomeGO = Instantiate(outcomePrefab);
            outcomeGO.transform.SetParent(outcomePanelTransform, false);

            StoryData.OutcomeIndices outcomeIndices = new StoryData.OutcomeIndices() { storyIndex = storyIndex, outcomeIndex = i };

            outcomeGOs.Add(outcomeIndices, outcomeGO);
            Image image = outcomeGO.GetComponent<Image>();
            outcomeGO.GetComponent<OutcomeSquareController>().outcomeIndices = outcomeIndices;

            if (StaticDataManager.StoryPlayerDatas[storyIndex].outcomeDiscovered[i])
            {
                image.sprite = outcomeDiscoveredSprite;
            }
            else image.sprite = outcomeNotDiscoveredSprite;

            if (StaticDataManager.StoryPlayerDatas[storyIndex].outcomeEnabled[i])
            {
                if (StaticDataManager.StoryPlayerDatas[storyIndex].selectedOutcome == i)
                {
                    image.color = ColorManager.GetColor(DescriptionButtonController.OutcomeType.Selected);
                }
                else
                {
                    image.color = ColorManager.GetColor(DescriptionButtonController.OutcomeType.Discovered);
                }
            }
            else
            {
                image.color = ColorManager.GetColor(DescriptionButtonController.OutcomeType.Disabled);
            }
        }

        DisplayOutcomeText(new StoryData.OutcomeIndices() { storyIndex = storyIndex, outcomeIndex = StaticDataManager.StoryPlayerDatas[storyIndex].selectedOutcome });
    }

    public static List<string> GetRearrangedOutcomeText(StoryData.OutcomeIndices outcomeIndices)
    {
        List<string> rearrangementText = new List<string>();
        RearrangementPlayerData rpd = StaticDataManager.RearrangementPlayerDatas[outcomeIndices.storyIndex];

        Dictionary<RearrangementPlayerData.TextboxIndices, List<string>> textboxStrings = new Dictionary<RearrangementPlayerData.TextboxIndices, List<string>>();

        foreach (int index in StaticDataManager.RearrangementDatas[outcomeIndices.storyIndex].indices)
        {
            var orderedLineTypes = StaticDataManager.StoryDatas[index].lastLineTypes.OrderBy(k => k.Key).ToList();
            List<string> initialText = StaticDataManager.StoryDatas[index].initialText;

            int textboxIndex = -1;

            for (int i = 0; i < orderedLineTypes.Count; i++)
            {
                if (i > 0 && (orderedLineTypes[i].Value & StoryData.LineFlags.Draggable) == 0 && orderedLineTypes[i - 1].Value == orderedLineTypes[i].Value)
                {
                    textboxStrings[new RearrangementPlayerData.TextboxIndices() { storyIndex = index, textboxIndex = textboxIndex }].Add(initialText[initialText.Count + orderedLineTypes[i].Key]);
                }
                else
                {
                    textboxIndex++;
                    textboxStrings.Add(new RearrangementPlayerData.TextboxIndices() { storyIndex = index, textboxIndex = textboxIndex }, new List<string>() { initialText[initialText.Count + orderedLineTypes[i].Key] });
                }
            }
        }

        foreach (RearrangementPlayerData.TextboxIndices tbi in rpd.outcomeTextboxIndices[outcomeIndices].GetRange(1, rpd.outcomeTextboxIndices[outcomeIndices].Count - 1))
        {
            rearrangementText.AddRange(textboxStrings[tbi]);
        }

        return rearrangementText;
    }

    public void DisplayOutcomeText(StoryData.OutcomeIndices outcomeIndices)
    {
        if (StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].outcomeDiscovered[outcomeIndices.outcomeIndex])
        {
            string initialText = "‣ " + string.Join("\n‣ ", GetRearrangedOutcomeText(outcomeIndices)).Replace("-", "\n").Replace("\\", "");
            string outcomeText = string.Join("\n", StaticDataManager.StoryDatas[outcomeIndices.storyIndex].outcomes[outcomeIndices.outcomeIndex].outcomeText).Replace("-", "\n").Replace("\\", "");

            storyText.text = "\n" + initialText + "\n\n" + outcomeText + "\n ";

            outcomeGOs[previousOutcomeIndices].transform.GetChild(0).gameObject.SetActive(false);
            previousOutcomeIndices = outcomeIndices;
            outcomeGOs[outcomeIndices].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void NextLetter()
    {
        StaticDataManager.SelectedIndex = (StaticDataManager.SelectedIndex + 1) % StaticDataManager.SelectedStoryIndices.Length;
        SceneTransition.Instance("StoryHistoryScene").FadeOut("StoryHistoryScene", true);
    }

    public void Back()
    {
        SceneTransition.Instance("StoryHistoryScene").FadeOut("", false);
    }

}
