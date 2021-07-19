using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionButtonController : MonoBehaviour
{
    public static Color GetColor(OutcomeType outcomeType)
    {
        int color = (int)outcomeType;
        return new Color32((byte)((color >> 16) & 255), (byte)((color >> 8) & 255), (byte)(color & 255), 255);
    }

    public enum OutcomeColor
    {
        SalmonPink = 0xF3919B,
        Jasmine = 0xFCE388,
        Black = 0x000000
    }

    public enum OutcomeType
    {
        Undiscovered = OutcomeColor.SalmonPink,
        Discovered = OutcomeColor.SalmonPink,
        Selected = OutcomeColor.Jasmine,
        Disabled = OutcomeColor.Black
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

        GetComponent<Image>().color = storyData.GetColor();
        transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = storyData.title;
        outcomePanelTransform = transform.GetChild(1);

        for (int i = 0; i < storyData.outcomes.Count; i++)
        {
            Transform outcomeSquareTransform = Instantiate(outcomeSquarePrefab).transform;
            outcomeSquareTransform.SetParent(outcomePanelTransform, false);

            if (storyPlayerData.outcomeDiscovered[i] == true)
            {
                if (storyPlayerData.selectedOutcome == i)
                {
                    outcomeSquareTransform.GetComponent<Image>().sprite = squareFilledSprite;
                    outcomeSquareTransform.GetComponent<Image>().color = GetColor(OutcomeType.Selected);
                }
                else
                {
                    outcomeSquareTransform.GetComponent<Image>().sprite = squareFilledSprite;
                    outcomeSquareTransform.GetComponent<Image>().color = GetColor(OutcomeType.Discovered);
                }
            }
            else
            {
                outcomeSquareTransform.GetComponent<Image>().sprite = squareBorderSprite;
                outcomeSquareTransform.GetComponent<Image>().color = GetColor(OutcomeType.Undiscovered);
            }
        }
    }
}
