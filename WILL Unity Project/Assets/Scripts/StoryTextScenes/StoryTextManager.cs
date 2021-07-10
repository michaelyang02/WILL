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
    }
}
