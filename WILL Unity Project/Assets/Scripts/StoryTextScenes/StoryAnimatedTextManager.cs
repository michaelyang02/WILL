using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class StoryAnimatedTextManager : MonoBehaviour
{

    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text storyText;

    public GameObject autoText;
    public GameObject nextButton;

    public static float writingTime = 0.001f; // 0.025f

    private StoryData storyData;
    private bool isAUTO;

    void Start()
    {
        storyData = StaticDataManager.storyDatas[StaticDataManager.selectedStoryOutcomes[StaticDataManager.seletedStoryOutcomeIndex].Key];
        List<string> outComeText = storyData.outcomes[StaticDataManager.selectedStoryOutcomes[StaticDataManager.seletedStoryOutcomeIndex].Value].outcomeText;


        titleText.text = storyData.title;
        storyText.text = "";
        nextButton.SetActive(false);
        GetComponent<Image>().color = storyData.GetColor();

        string buttonText;
        UnityAction OnClickAction;

        if (StaticDataManager.seletedStoryOutcomeIndex == StaticDataManager.selectedStoryOutcomes.Count - 1)
        {
            buttonText = "Begin!";
            OnClickAction = GoToRearrangmentScene;
            StaticDataManager.seletedStoryOutcomeIndex = 0;
        }
        else
        {
            buttonText = "Next Letter";
            OnClickAction = GoToNextAnimationScene;
            StaticDataManager.seletedStoryOutcomeIndex++;
        }
        
        StartCoroutine(AnimateWriting(buttonText, OnClickAction, storyData.initialText, outComeText));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isAUTO)
            {
                isAUTO = true;
                autoText.SetActive(true);
            }
            else
            {
                isAUTO = false;
                autoText.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && isAUTO)
        {
            isAUTO = false;
            autoText.SetActive(false);
        }
    }

    IEnumerator AnimateWriting(string endText, UnityAction OnClickAction, params List<string>[] writingList)
    {
        nextButton.GetComponentInChildren<TMPro.TMP_Text>().text = endText;
        nextButton.GetComponent<Button>().onClick.AddListener(OnClickAction);

        foreach (List<string> writing in writingList)
        {
            yield return StartCoroutine(NarrateLines(writing));
        }
        yield return StartCoroutine(WaitProceeding(20f, false));
        nextButton.SetActive(true);
        yield break;
    }

    IEnumerator NarrateLines(List<string> lines)
    {
        int lastIndex = 0;

        for (int l = 0; l <= lines.Count; l++)
        {
            if (l == lines.Count || lines[l] == "-")
            {
                storyText.text = "";

                // construct string of this page

                string pageString = string.Join("\r", lines.GetRange(lastIndex, l - lastIndex));
                string cleanedString = pageString.Replace("\\", "").Replace("\r", "\n");

                lastIndex = l + 1;

                int cleanedIndex = 0;
                bool isInTag = false;

                for (int tIndex = 0; tIndex < pageString.Length; tIndex++)
                {
                    char thisCharacter = pageString[tIndex];

                    if (thisCharacter == '<')
                    {
                        isInTag = true;
                    }

                    storyText.text = cleanedString.Substring(0, cleanedIndex + 1) + "</u></color><color=#00000000>"
                     + Regex.Replace(cleanedString.Substring(cleanedIndex + 1), @"<color=[^>]*>|</color>", "") + "</color>";

                    if (!isInTag)
                    {
                        yield return StartCoroutine(WaitWriting(thisCharacter));
                    }
                    if (thisCharacter == '>')
                    {
                        isInTag = false;
                    }
                    if (thisCharacter != '\\')
                    {
                        cleanedIndex++;
                    }
                }
                    yield return StartCoroutine(WaitProceeding(20f, true));
            }
        }
        yield break;
    }

    IEnumerator WaitWriting(char character)
    {
        switch (character)
        {
            case ' ':
                break;
            case '\\': // in-line break
                yield return StartCoroutine(WaitProceeding(10f, true));
                break;
            case '\r': // line break
                yield return StartCoroutine(WaitProceeding(15f, true));
                break;
            case '\n': // additional paragraph break
                yield return StartCoroutine(WaitProceeding(5f, false));
                break;
            case ',':
            case ';':
            case ':':
                yield return StartCoroutine(WaitProceeding(5f, false));
                break;
            case '.':
            case '!':
            case '?':
                yield return StartCoroutine(WaitProceeding(5f, false));
                break;
            default:
                yield return StartCoroutine(WaitProceeding(1f, false));
                break;
        }
        yield break;
    }

    IEnumerator WaitProceeding(float waitingFactor, bool isInputRequired)
    {
        if (isInputRequired)
        {
            // wait for set of time before proceeding with AUTO or with click
            while (!Input.GetMouseButtonDown(0) && !isAUTO)
            {
                yield return null;
            }
            if (isAUTO)
            {
                yield return new WaitForSeconds(waitingFactor * writingTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(waitingFactor * writingTime);
        }
        yield break;
    }


    public void GoToMainGameScene()
    {
        SceneManager.LoadSceneAsync("MainGameScene");
    }

    public void GoToNextAnimationScene()
    {
        SceneManager.LoadSceneAsync("StoryAnimatedTextScene");
    }

    public void GoToRearrangmentScene()
    {
        SceneManager.LoadSceneAsync("StoryRearrangementScene");
    }
}
