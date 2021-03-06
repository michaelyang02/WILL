using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class StoryAnimatedTextManager : MonoBehaviour
{

    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text storyText;

    public GameObject autoText;
    public static float WritingTime = 0.025f; // 0.025f

    private StoryData storyData;
    private bool isAUTO;

    void Start()
    {
        storyData = StaticDataManager.StoryDatas[StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex]];
        List<string> outcomeText = storyData.outcomes[StaticDataManager.StoryPlayerDatas[storyData.index].selectedOutcome].outcomeText;

        titleText.text = storyData.title;
        storyText.text = "";

        GetComponent<Image>().color = ColorManager.GetColor(storyData.character);

        StartCoroutine(AnimateWriting(GetRearrangedInitialText(storyData), outcomeText));
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

    public static List<string> GetRearrangedInitialText(StoryData storyData)
    {
        List<string> rearrangementText = new List<string>(storyData.initialText);
        RearrangementPlayerData rpd = StaticDataManager.RearrangementPlayerDatas[storyData.index];

        foreach (KeyValuePair<int, StoryData.LineFlags> kvp in storyData.lastLineTypes)
        {
            rearrangementText[rearrangementText.Count + kvp.Key] = "";
        }
        rearrangementText.RemoveAll(s => s == "");

        Dictionary<RearrangementPlayerData.TextboxIndices, List<string>> textboxStrings = new Dictionary<RearrangementPlayerData.TextboxIndices, List<string>>();

        foreach (int index in StaticDataManager.RearrangementDatas[storyData.index].indices)
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

        foreach (RearrangementPlayerData.TextboxIndices tbi in rpd.rearrangementTextboxIndices[storyData.index])
        {
            rearrangementText.AddRange(textboxStrings[tbi]);
        }

        return rearrangementText;
    }

    IEnumerator AnimateWriting(params List<string>[] writingList)
    {
        //string signature = ("\n\n<align=\"right\">" + storyData.GetCharacter());

        foreach (List<string> writing in writingList)
        {
            yield return StartCoroutine(NarrateLines(writing));
        }

        yield return StartCoroutine(WaitProceeding(20f, false));
        SceneTransition.Instance("StoryAnimatedTextScene").FadeOut("StoryTextScene", false);
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
                yield return new WaitForSeconds(waitingFactor * WritingTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(waitingFactor * WritingTime);
        }
        yield break;
    }

    public void BackToPreviousScene()
    {
        if (StaticDataManager.StoryPlayerDatas[storyData.index].isRead)
        {
            SceneTransition.Instance("StoryAnimatedTextScene").FadeOut("StoryTextScene", false);
        }
        else
        {
            CameraManager.SetFocusPosition(StaticDataManager.StoryPosition[storyData.index]);
            SceneTransition.Instance("StoryAnimatedTextScene").FadeOut("MainGameScene", false);
        }
    }
}
