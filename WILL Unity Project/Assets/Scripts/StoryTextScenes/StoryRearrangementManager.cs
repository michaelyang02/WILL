using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryRearrangementManager : MonoBehaviour
{
    public GameObject leftPanel;
    public GameObject rightPanel;
    public GameObject middlePanel;

    public GameObject rearrangementPanel;

    public Transform tempTextboxParentTransform;

    public GameObject subPanelPrefab;
    public GameObject textboxPrefab;
    public GameObject backgroundPanelPrefab;
    public GameObject typeImagePrefab;

    public Sprite switchingSprite;
    public Sprite unswappableSprite;
    public Sprite numbered1Sprite;
    public Sprite numbered2Sprite;
    public Sprite numbered3Sprite;

    private Color typeColor = new Color(1f, 0.2f, 0.2f, 0.5f);

    private Dictionary<int, int> indexToSubpanelIndexDict;
    private List<Transform> subPanelTransforms;
    private List<List<Transform>> textboxTransforms;
    private List<Transform> outcomeTransforms;


    void Start()
    {
        indexToSubpanelIndexDict = new Dictionary<int, int>();
        subPanelTransforms = new List<Transform>();
        textboxTransforms = new List<List<Transform>>();
        outcomeTransforms = new List<Transform>();

        LoadStories();
        Rearrange();
    }

    void LoadStories()
    {

        // change dimensions of panels dynamically based on number of stories

        float size = (5 - StaticDataManager.SelectedStoryIndices.Length) * 100f;

        leftPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 0);
        rightPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 0);

        RectTransform rearrangementRectTransform = rearrangementPanel.GetComponent<RectTransform>();
        RectTransform middleRectTransform = middlePanel.GetComponent<RectTransform>();

        rearrangementRectTransform.offsetMin = new Vector2(size, rearrangementRectTransform.offsetMin.y);
        rearrangementRectTransform.offsetMax = new Vector2(-size, rearrangementRectTransform.offsetMax.y);
        middleRectTransform.offsetMin = new Vector2(size, middleRectTransform.offsetMin.y);
        middleRectTransform.offsetMax = new Vector2(-size, middleRectTransform.offsetMax.y);


        int subPanelIndex = 0;

        foreach (int index in StaticDataManager.SelectedStoryIndices)
        {
            indexToSubpanelIndexDict.Add(index, subPanelIndex);

            StoryData storyData = StaticDataManager.StoryDatas[index];

            // instantiate side background panel
            if (subPanelIndex == 0)
            {
                leftPanel.GetComponent<Image>().color = storyData.GetColor();
            }
            if (subPanelIndex == StaticDataManager.SelectedStoryIndices.Length - 1)
            {
                rightPanel.GetComponent<Image>().color = storyData.GetColor();
            }

            // instantiate background panel
            GameObject tempBackgroundPanel = Instantiate(backgroundPanelPrefab);
            tempBackgroundPanel.transform.SetParent(middlePanel.transform, false);
            tempBackgroundPanel.GetComponent<Image>().color = storyData.GetColor();

            // instantiate subpanel
            GameObject tempSubPanelGameObject = Instantiate(subPanelPrefab);
            tempSubPanelGameObject.transform.SetParent(rearrangementPanel.transform, false);
            tempSubPanelGameObject.GetComponent<SubpanelController>().storyIndex = storyData.index;
            subPanelTransforms.Add(tempSubPanelGameObject.transform);

            // initialise all texboxes

            // sort the lines
            List<KeyValuePair<int, StoryData.LineFlags>> lastLineTypes = storyData.lastLineTypes.ToList();
            lastLineTypes.Sort((p, q) => p.Key.CompareTo(q.Key));

            List<TextBlock> textBlocks = new List<TextBlock>();

            for (int kvpIndex = 0; kvpIndex < lastLineTypes.Count; kvpIndex++)
            {
                string text = storyData.initialText[storyData.initialText.Count + lastLineTypes[kvpIndex].Key].Replace("\\", "");

                // combine lines if 1) they are of the same type and 2) they are not draggable
                if (kvpIndex - 1 >= 0 &&
                (lastLineTypes[kvpIndex].Value & StoryData.LineFlags.Draggable) == 0 &&
                lastLineTypes[kvpIndex - 1].Value == lastLineTypes[kvpIndex].Value)
                {
                    textBlocks.Last().text = textBlocks.Last().text + "\n" + text;
                }
                else
                {
                    textBlocks.Add(new TextBlock { text = text, flag = lastLineTypes[kvpIndex].Value });
                }
            }

            string outcomeText = string.Join("\n", storyData.outcomes[StaticDataManager.StoryPlayerDatas[index].selectedOutcome].outcomeText).Replace("-", "\n").Replace("\\", "");

            textboxTransforms.Add(new List<Transform>());

            foreach (TextBlock textBlock in textBlocks)
            {
                GameObject tempTextboxGameObject = Instantiate(textboxPrefab);
                tempTextboxGameObject.GetComponent<TextboxController>().boxFlag = textBlock.flag;
                tempTextboxGameObject.GetComponent<TextboxController>().storyIndex = storyData.index;
                tempTextboxGameObject.transform.SetParent(tempTextboxParentTransform, false);

                Transform backgroundTransform = tempTextboxGameObject.transform.GetChild(1);
                Image banner = backgroundTransform.GetChild(0).GetComponent<Image>();
                Transform typeTransform = backgroundTransform .GetChild(1);
                TMPro.TMP_Text text = tempTextboxGameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>();

                // set text
                text.text = textBlock.text;

                if ((textBlock.flag & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable)
                {
                    // set text color
                    text.color = Color.black;

                    // banner image
                    banner.color = storyData.GetColor();
                    banner.gameObject.SetActive(true);

                    // textbox color
                    backgroundTransform.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    // Undraggable
                    // textbox color
                    backgroundTransform.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                }


                if ((textBlock.flag & StoryData.LineFlags.Unswappable) == StoryData.LineFlags.Unswappable)
                {
                    Color tempColor = storyData.GetColor();
                    tempColor.a = 0.5f;

                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = unswappableSprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = tempColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }
                if ((textBlock.flag & StoryData.LineFlags.Switching) == StoryData.LineFlags.Switching)
                {
                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = switchingSprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = typeColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }
                if ((textBlock.flag & StoryData.LineFlags.Numbered1) == StoryData.LineFlags.Numbered1)
                {
                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = numbered1Sprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = typeColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }
                if ((textBlock.flag & StoryData.LineFlags.Numbered2) == StoryData.LineFlags.Numbered2)
                {
                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = numbered2Sprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = typeColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }
                if ((textBlock.flag & StoryData.LineFlags.Numbered3) == StoryData.LineFlags.Numbered3)
                {
                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = numbered3Sprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = typeColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }

                textboxTransforms[textboxTransforms.Count - 1].Add(tempTextboxGameObject.transform);
            }

            GameObject outcomeTextboxGameObject = Instantiate(textboxPrefab);
            outcomeTextboxGameObject.GetComponent<TextboxController>().boxFlag = StoryData.LineFlags.None;
            outcomeTextboxGameObject.GetComponent<TextboxController>().storyIndex = storyData.index;
            outcomeTextboxGameObject.transform.SetParent(tempTextboxParentTransform, false);

            // set text
            outcomeTextboxGameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = outcomeText;

            // textbox color
            outcomeTextboxGameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.25f);

            outcomeTransforms.Add(outcomeTextboxGameObject.transform);

            subPanelIndex++;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rearrangementPanel.GetComponent<RectTransform>());


        // initialise sizes and boundaries

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        TextboxController.LeftMargin = rearrangementRectTransform.offsetMin.x * canvas.scaleFactor;
        TextboxController.RightMargin = (canvas.GetComponent<RectTransform>().rect.width + rearrangementRectTransform.offsetMax.x) * canvas.scaleFactor;

        TextboxController.SubPanelBoundaries = new Dictionary<Transform, Vector2>();

        // add boundaries to dictionary
        foreach (Transform child in rearrangementPanel.transform)
        {
            float leftBoundary = child.GetComponent<RectTransform>().anchoredPosition.x * canvas.scaleFactor + TextboxController.LeftMargin;
            float rightBoundary = leftBoundary + child.GetComponent<RectTransform>().sizeDelta.x * canvas.scaleFactor;

            TextboxController.SubPanelBoundaries.Add(child, new Vector2(leftBoundary, rightBoundary));
        }

        // initialise textbox controller static data

        TextboxController.CanvasTransform = GameObject.Find("Canvas").transform;
        TextboxController.ScrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        TextboxController.RearrangementPanelTransform = GameObject.Find("Rearrangement Panel").transform;
        TextboxController.RearrangementPanelRectTransform = TextboxController.RearrangementPanelTransform.GetComponent<RectTransform>();
        TextboxController.BackButtonTransform = GameObject.Find("Back").transform;
    }

    void Rearrange()
    {
        RearrangementData rearrangementData = StaticDataManager.RearrangementDatas[StaticDataManager.SelectedStoryIndices[0]];

        foreach (List<Transform> subPanelTextboxTransforms in textboxTransforms)
        {
            foreach (Transform textboxTransform in subPanelTextboxTransforms)
            {
                textboxTransform.SetParent(tempTextboxParentTransform, false);
            }
        }

        foreach (Transform outcomeTransform in outcomeTransforms)
        {
            outcomeTransform.SetParent(tempTextboxParentTransform, false);
        }

        foreach (KeyValuePair<int, List<RearrangementData.TextboxIndices>> kvp in rearrangementData.rearrangementTextboxIndices)
        {
            foreach (RearrangementData.TextboxIndices textboxIndices in kvp.Value)
            {
                textboxTransforms[indexToSubpanelIndexDict[textboxIndices.storyIndex]]
                [textboxIndices.textboxIndex].SetParent(subPanelTransforms[indexToSubpanelIndexDict[kvp.Key]], false);
            }
        }

        for (int i = 0; i < outcomeTransforms.Count; i++)
        {
            outcomeTransforms[i].SetParent(subPanelTransforms[i], false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rearrangementPanel.GetComponent<RectTransform>());
    }


    public void BackToMainGame()
    {
        CameraManager.SetFocusPosition(StaticDataManager.StoryPosition[StaticDataManager.SelectedStoryIndices[StaticDataManager.SelectedIndex]]);
        SceneManager.LoadSceneAsync("MainGameScene");
    }
}

class TextBlock
{
    public string text { get; set; }
    public StoryData.LineFlags flag { get; set; }
}
