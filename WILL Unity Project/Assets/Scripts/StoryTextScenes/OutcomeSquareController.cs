using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutcomeSquareController : MonoBehaviour
{
    public StoryData.OutcomeIndices outcomeIndices { get; set; }

    public void ClickedOutcome()
    {
        StoryHistoryManager.Instance.DisplayOutcomeText(outcomeIndices);
    }
}
