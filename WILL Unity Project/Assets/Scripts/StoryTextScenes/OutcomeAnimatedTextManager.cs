using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class OutcomeAnimatedTextManager : MonoBehaviour
{
    public TMPro.TMP_Text outcomeText;

    public GameObject autoText;

    private static bool IsAUTO;

    void Start()
    {
        StoryData.Outcome.OutcomeIndices outcomeIndices = StaticDataManager.AnimatedOutcomes[0];
        StoryData storyData = StaticDataManager.StoryDatas[outcomeIndices.storyIndex];
        outcomeText.text = "";
        GetComponent<Image>().color = storyData.GetColor();
        StartCoroutine(AnimateWriting(storyData.outcomes[outcomeIndices.outcomeIndex].outcomeText));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!IsAUTO)
            {
                IsAUTO = true;
                autoText.SetActive(true);
            }
            else
            {
                IsAUTO = false;
                autoText.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && IsAUTO)
        {
            IsAUTO = false;
            autoText.SetActive(false);
        }
    }

    IEnumerator AnimateWriting(List<string> outcomeText)
    {
        //string signature = ("\n\n<align=\"right\">" + storyData.GetCharacter());

        yield return StartCoroutine(NarrateLines(outcomeText));
        StaticDataManager.AnimatedOutcomes.RemoveAt(0);

        if (StaticDataManager.AnimatedOutcomes.Count == 0)
        {
            IsAUTO = false;
            SceneManager.UnloadSceneAsync("OutcomeAnimatedTextScene");
        }
        else
        {
            SceneManager.UnloadSceneAsync("OutcomeAnimatedTextScene");
            SceneManager.LoadSceneAsync("OutcomeAnimatedTextScene", LoadSceneMode.Additive);
        }
        yield break;
    }

    IEnumerator NarrateLines(List<string> lines)
    {
        int lastIndex = 0;

        for (int l = 0; l <= lines.Count; l++)
        {
            if (l == lines.Count || lines[l] == "-")
            {
                outcomeText.text = "";

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

                    outcomeText.text = cleanedString.Substring(0, cleanedIndex + 1) + "</u></color><color=#00000000>"
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
            while (!Input.GetMouseButtonDown(0) && !IsAUTO)
            {
                yield return null;
            }
            if (IsAUTO)
            {
                yield return new WaitForSeconds(waitingFactor * StoryAnimatedTextManager.writingTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(waitingFactor * StoryAnimatedTextManager.writingTime);
        }
        yield break;
    }
}
