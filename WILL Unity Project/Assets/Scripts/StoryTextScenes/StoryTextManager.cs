using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTextManager : MonoBehaviour
{
    
    public GameObject storyGameObject;
    private TMPro.TMP_Text storyText;
    private RectTransform storyRectTransform;
    private List<string> lines = new List<string>
    {
        "Hello, my name is Detective Johnson.", "We would like to ask you a few questions regarding your involvement in the Moray case.",
        "\"What's there to say?\" I asked. There is nothing to say. At least not to you.", "Really?", @"Yes, really, I know who you are, Detective Johnson: the most corrupt cop in the land.", "Well, then. If you don't mind being taken into custody.",
        "In handcuffs.", "I'd like to see you try."
    };

    void Start()
    {
        storyText = storyGameObject.GetComponent<TMPro.TMP_Text>();
        storyRectTransform = storyGameObject.GetComponent<RectTransform>();

        storyText.text = "";
        for (int i = 0; i < lines.Count; i++)
        {
            storyText.text += (lines[i] + '\n');
        }
        StartCoroutine(RunOnceSetTextTransform());
    }

    IEnumerator RunOnceSetTextTransform()
    {
        // this is run after first frame to decrement the bottom
        // until all texts fit inside the rect.

        yield return null;

        while(storyText.isTextOverflowing)
        {
            storyRectTransform.offsetMin -= new Vector2(0f, 10f);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
