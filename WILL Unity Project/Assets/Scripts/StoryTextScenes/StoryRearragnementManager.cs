using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryRearragnementManager : MonoBehaviour
{

    public GameObject leftPanel;
    public GameObject rightPanel;
    public GameObject middlePanel;

    public GameObject rearrangementPanel;

    public GameObject subPanelPrefab;
    public GameObject textboxPrefab;
    public GameObject backgroundPanelPrefab;

    private List<GameObject> subPanelList;
    private List<List<GameObject>> textboxList;


    void Start()
    {
        subPanelList = new List<GameObject>();
        textboxList = new List<List<GameObject>>();


        LoadStories();
    }

    void LoadStories()
    {
        List<KeyValuePair<int, int>> storyList = new List<KeyValuePair<int, int>>(StaticDataManager.SelectedStoryOutcomes);
        storyList.Sort((p, q) => p.Key.CompareTo(q.Key));

        // change dimensions of panels dynamically based on number of stories

        float size = (5 - storyList.Count) * 100f;

        leftPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 0);
        rightPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 0);

        RectTransform rearrangementRectTransform = rearrangementPanel.GetComponent<RectTransform>();
        RectTransform middleRectTransform = middlePanel.GetComponent<RectTransform>();

        rearrangementRectTransform.offsetMin = new Vector2(size, rearrangementRectTransform.offsetMin.y);
        rearrangementRectTransform.offsetMax = new Vector2(-size, rearrangementRectTransform.offsetMax.y);
        middleRectTransform.offsetMin = new Vector2(size, middleRectTransform.offsetMin.y);
        middleRectTransform.offsetMax = new Vector2(-size, middleRectTransform.offsetMax.y);


        int subPanelIndex = 0;

        foreach (KeyValuePair<int, int> keyValuePair in storyList)
        {
            StoryData storyData = StaticDataManager.StoryDatas[keyValuePair.Key];

            // instantiate side background panel
            if (subPanelIndex == 0)
            {
                leftPanel.GetComponent<Image>().color = storyData.GetColor();
            }
            if (subPanelIndex == storyList.Count - 1)
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
            subPanelList.Add(tempSubPanelGameObject);

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

            string outcomeText = string.Join("\n", storyData.outcomes[keyValuePair.Value].outcomeText).Replace("-", "\n").Replace("\\", "");

            textboxList.Add(new List<GameObject>());

            foreach (TextBlock textBlock in textBlocks)
            {
                GameObject tempTextboxGameObject = Instantiate(textboxPrefab);
                tempTextboxGameObject.GetComponent<TextboxController>().textboxFlag = textBlock.flag;
                tempTextboxGameObject.transform.SetParent(tempSubPanelGameObject.transform, false);

                Image backgroundImage = tempTextboxGameObject.transform.GetChild(1).GetComponent<Image>();
                Image banner = tempTextboxGameObject.transform.GetChild(2).GetComponent<Image>();
                Image type = tempTextboxGameObject.transform.GetChild(3).GetComponent<Image>();
                TMPro.TMP_Text text = tempTextboxGameObject.transform.GetChild(4).GetComponent<TMPro.TMP_Text>();

                // set text
                text.text = textBlock.text;

                if ((textBlock.flag & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable)
                {
                    // set text color
                    text.color = Color.black;

                    // banner image
                    banner.color = storyData.GetColor();
                    tempTextboxGameObject.transform.GetChild(2).gameObject.SetActive(true);

                    // textbox color
                    backgroundImage.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    // Undraggable
                    // textbox color
                    backgroundImage.color = new Color(0.25f, 0.25f, 0.25f, 0.75f);
                }

                // TODO: add logic to detect other flags and change type image
                textboxList[subPanelIndex].Add(tempTextboxGameObject);
            }

            GameObject outcomeTextboxGameObject = Instantiate(textboxPrefab);
            outcomeTextboxGameObject.GetComponent<TextboxController>().textboxFlag = StoryData.LineFlags.None;
            outcomeTextboxGameObject.transform.SetParent(tempSubPanelGameObject.transform, false);

            // set text
            outcomeTextboxGameObject.transform.GetChild(4).GetComponent<TMPro.TMP_Text>().text = outcomeText;

            // textbox color
            outcomeTextboxGameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.25f);

            textboxList[subPanelIndex].Add(outcomeTextboxGameObject);

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
    }

    public void BackToMainGame()
    {
        CameraManager.SetFocusPosition(StaticDataManager.StoryPosition[StaticDataManager.SelectedStoryOutcomes[StaticDataManager.SeletedStoryOutcomeIndex].Key]);
        SceneManager.LoadSceneAsync("MainGameScene");
    }
}

class TextBlock
{
    public string text { get; set; }
    public StoryData.LineFlags flag { get; set; }
}
