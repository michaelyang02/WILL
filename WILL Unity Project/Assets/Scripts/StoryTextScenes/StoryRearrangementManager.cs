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
    public GameObject foregroundPanel;

    public GameObject rearrangementPanel;

    public Transform tempTextboxParentTransform;

    public GameObject subPanelPrefab;
    public GameObject textboxPrefab;
    public GameObject backgroundPanelPrefab;
    public GameObject foregroundPanelPrefab;
    public GameObject typeImagePrefab;

    public Sprite switchingSprite;
    public Sprite unswappableSprite;
    public Sprite numbered1Sprite;
    public Sprite numbered2Sprite;
    public Sprite numbered3Sprite;
    public Sprite pinnedSprite;

    private Color typeColor = new Color(1f, 0.2f, 0.2f, 0.5f);

    private Dictionary<int, Transform> subpanelTransforms;
    private Dictionary<int, Transform> foregroundSubpanelTransforms;
    private Dictionary<RearrangementPlayerData.TextboxIndices, Transform> textboxTransforms;
    private Dictionary<Transform, RearrangementPlayerData.TextboxIndices> textboxTextboxIndices;
    private Dictionary<int, Transform> outcomeTransforms;

    void Start()
    {
        subpanelTransforms = new Dictionary<int, Transform>();
        foregroundSubpanelTransforms = new Dictionary<int, Transform>();
        textboxTransforms = new Dictionary<RearrangementPlayerData.TextboxIndices, Transform>();
        textboxTextboxIndices = new Dictionary<Transform, RearrangementPlayerData.TextboxIndices>();
        outcomeTransforms = new Dictionary<int, Transform>();

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

            StoryData storyData = StaticDataManager.StoryDatas[index];

            // instantiate side background panel
            if (subPanelIndex == 0)
            {
                leftPanel.GetComponent<Image>().color = ColorManager.GetColor(storyData.character);
            }
            if (subPanelIndex == StaticDataManager.SelectedStoryIndices.Length - 1)
            {
                rightPanel.GetComponent<Image>().color = ColorManager.GetColor(storyData.character);
            }

            // instantiate background panel
            GameObject tempBackgroundPanel = Instantiate(backgroundPanelPrefab);
            tempBackgroundPanel.transform.SetParent(middlePanel.transform, false);
            tempBackgroundPanel.GetComponent<Image>().color = ColorManager.GetColor(storyData.character);

            // instantiate subpanel
            GameObject tempSubPanelGameObject = Instantiate(subPanelPrefab);
            tempSubPanelGameObject.transform.SetParent(rearrangementPanel.transform, false);
            tempSubPanelGameObject.GetComponent<SubpanelController>().storyIndex = storyData.index;
            subpanelTransforms.Add(index, tempSubPanelGameObject.transform);

            // instantiate foreground panel
            GameObject tempForegroundPanel = Instantiate(foregroundPanelPrefab);
            tempForegroundPanel.transform.SetParent(foregroundPanel.transform, false);
            tempForegroundPanel.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Outcome " + StaticDataManager.StoryPlayerDatas[index].selectedOutcome.ToString();
            foregroundSubpanelTransforms.Add(index, tempForegroundPanel.transform);

            // initialise all texboxes

            // sort the lines
            List<KeyValuePair<int, StoryData.LineFlags>> lastLineTypes = storyData.lastLineTypes.OrderBy(k => k.Key).ToList();

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

            string outcomeText = string.Join("\n", storyData.outcomes[StaticDataManager.StoryPlayerDatas[index].selectedOutcome].outcomeText).Replace("-", "").Replace("\\", "");

            int textBlockIndex = 0;

            foreach (TextBlock textBlock in textBlocks)
            {
                GameObject tempTextboxGameObject = Instantiate(textboxPrefab);
                tempTextboxGameObject.GetComponent<TextboxController>().boxFlag = textBlock.flag;
                tempTextboxGameObject.GetComponent<TextboxController>().storyIndex = storyData.index;
                tempTextboxGameObject.transform.SetParent(tempTextboxParentTransform, false);

                Transform backgroundTransform = tempTextboxGameObject.transform.GetChild(1);
                Image banner = backgroundTransform.GetChild(0).GetComponent<Image>();
                Transform typeTransform = backgroundTransform.GetChild(1);
                TMPro.TMP_Text text = tempTextboxGameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>();

                // set text
                text.text = textBlock.text;

                if ((textBlock.flag & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable)
                {
                    // set text color
                    text.color = Color.black;

                    // banner image
                    banner.color = ColorManager.GetColor(storyData.character);
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
                    Color tempColor = ColorManager.GetColor(storyData.character);
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
                if ((textBlock.flag & StoryData.LineFlags.Pinned) == StoryData.LineFlags.Pinned)
                {
                    GameObject tempTypeImageGameObject = Instantiate(typeImagePrefab);
                    tempTypeImageGameObject.GetComponent<Image>().sprite = pinnedSprite;
                    tempTypeImageGameObject.GetComponent<Image>().color = typeColor;
                    tempTypeImageGameObject.transform.SetParent(typeTransform, false);
                }

                textboxTransforms.Add(new RearrangementPlayerData.TextboxIndices() { storyIndex = index, textboxIndex = textBlockIndex }, tempTextboxGameObject.transform);
                textBlockIndex++;
            }

            GameObject outcomeTextboxGameObject = Instantiate(textboxPrefab);
            outcomeTextboxGameObject.GetComponent<TextboxController>().boxFlag = StoryData.LineFlags.None;
            outcomeTextboxGameObject.GetComponent<TextboxController>().storyIndex = storyData.index;
            outcomeTextboxGameObject.transform.SetParent(tempTextboxParentTransform, false);

            // set text
            outcomeTextboxGameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = outcomeText;

            // textbox color
            outcomeTextboxGameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.25f);

            outcomeTransforms.Add(index, outcomeTextboxGameObject.transform);

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

        textboxTextboxIndices = textboxTransforms.ToDictionary(i => i.Value, i => i.Key);

        // initialise textbox controller static data

        TextboxController.CanvasTransform = GameObject.Find("Canvas").transform;
        TextboxController.ScrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        TextboxController.RearrangementPanelTransform = GameObject.Find("Rearrangement Panel").transform;
        TextboxController.RearrangementPanelRectTransform = TextboxController.RearrangementPanelTransform.GetComponent<RectTransform>();
        TextboxController.BackButtonTransform = GameObject.Find("Back").transform;
    }

    void Rearrange()
    {
        RearrangementPlayerData rearrangementData = StaticDataManager.RearrangementPlayerDatas[StaticDataManager.SelectedStoryIndices[0]];

        foreach (Transform textboxTransform in textboxTransforms.Values)
        {
            textboxTransform.SetParent(tempTextboxParentTransform);
        }

        foreach (Transform outcomeTransform in outcomeTransforms.Values)
        {
            outcomeTransform.SetParent(tempTextboxParentTransform, false);
        }

        foreach (KeyValuePair<int, List<RearrangementPlayerData.TextboxIndices>> kvp in rearrangementData.rearrangementTextboxIndices)
        {
            foreach (RearrangementPlayerData.TextboxIndices textboxIndices in kvp.Value)
            {
                textboxTransforms[textboxIndices].SetParent(subpanelTransforms[kvp.Key], false);
            }
        }

        foreach (KeyValuePair<int, Transform> kvp1 in outcomeTransforms)
        {
            kvp1.Value.SetParent(subpanelTransforms[kvp1.Key], false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rearrangementPanel.GetComponent<RectTransform>());
    }

    public void DetermineOutcome()
    {
        RearrangementPlayerData rearrangementPlayerData = StaticDataManager.RearrangementPlayerDatas[StaticDataManager.SelectedStoryIndices[0]];

        StaticDataManager.AnimatedOutcomes.Clear();

        foreach (int index in StaticDataManager.SelectedStoryIndices)
        {
            List<RearrangementPlayerData.TextboxIndices> textboxIndices = new List<RearrangementPlayerData.TextboxIndices>();

            foreach (Transform transform in subpanelTransforms[index])
            {
                if (textboxTextboxIndices.ContainsKey(transform))
                {
                    textboxIndices.Add(textboxTextboxIndices[transform]);
                }
            }

            rearrangementPlayerData.rearrangementTextboxIndices[index] = textboxIndices;

            StoryData storyData = StaticDataManager.StoryDatas[index];
            RearrangementData rearrangementData = StaticDataManager.RearrangementDatas.Find(rd => rd.ContainsKey(index));

            for (int i = 0; i < storyData.outcomes.Count; i++)
            {
                if (rearrangementData[index][i].IsConditionMet(textboxIndices))
                { // arrangement meets this outcome's conditions (first)
                    if (!StaticDataManager.StoryPlayerDatas[index].outcomeDiscovered[i])
                    {
                        StaticDataManager.AnimatedOutcomes.Add(new StoryData.OutcomeIndices { storyIndex = index, outcomeIndex = i });
                        StaticDataManager.StoryPlayerDatas[index].outcomeDiscovered[i] = true;
                    }
                    outcomeTransforms[index].GetChild(2).GetComponent<TMPro.TMP_Text>().text = string.Join("\n", storyData.outcomes[i].outcomeText).Replace("-", "").Replace("\\", "");
                    LayoutRebuilder.ForceRebuildLayoutImmediate(outcomeTransforms[index].GetComponent<RectTransform>());

                    foregroundSubpanelTransforms[index].GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Outcome " + i.ToString();
                    StaticDataManager.StoryPlayerDatas[index].selectedOutcome = i;

                    LayoutRebuilder.ForceRebuildLayoutImmediate(subpanelTransforms[index].GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rearrangementPanel.GetComponent<RectTransform>());
                    break;
                }
            }
        }

        if (StaticDataManager.AnimatedOutcomes.Any())
        { // without async to avoid the slight pause before loading thus revealing outcome
            SceneManager.LoadScene("OutcomeAnimatedTextScene", LoadSceneMode.Additive);
        }
    }

    public void Reset()
    {
        foreach (Transform textboxTransform in textboxTransforms.Values)
        {
            textboxTransform.SetParent(tempTextboxParentTransform, false);
        }

        foreach (Transform outcomeTransform in outcomeTransforms.Values)
        {
            outcomeTransform.SetParent(tempTextboxParentTransform, false);
        }

        textboxTransforms.OrderBy(kvp => kvp.Key.storyIndex).ThenBy(kvp => kvp.Key.textboxIndex).ToList().ForEach(t => t.Value.SetParent(subpanelTransforms[t.Key.storyIndex], false));
        outcomeTransforms.ToList().ForEach(o => o.Value.SetParent(subpanelTransforms[o.Key], false));
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
