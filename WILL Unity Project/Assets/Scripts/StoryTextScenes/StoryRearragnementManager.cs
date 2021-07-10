using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryRearragnementManager : MonoBehaviour
{

    public GameObject leftPanel;
    public GameObject rightPanel;

    public GameObject rearrangementPanel;

    public GameObject subPanelPrefab;
    public GameObject textboxPrefab;

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

        int subPanelIndex = 0;

        foreach (KeyValuePair<int, int> keyValuePair in storyList)
        {
            StoryData storyData = StaticDataManager.StoryDatas[keyValuePair.Key];
            
            // instantiate subpanel
            GameObject tempSubPanelGameObject = Instantiate(subPanelPrefab);
            tempSubPanelGameObject.transform.SetParent(rearrangementPanel.transform, false);
            tempSubPanelGameObject.GetComponent<Image>().color = storyData.GetColor();
            subPanelList.Add(tempSubPanelGameObject);

            // initialise all texboxes
            List<KeyValuePair<int, StoryData.LineFlags>> lastLineTypes = storyData.lastLineTypes.ToList();
            lastLineTypes.Sort((p, q) => p.Key.CompareTo(q.Key));

            string outcomeText = string.Join("\n", storyData.outcomes[keyValuePair.Value].outcomeText).Replace("-", "\n").Replace("\\", "");

            textboxList.Add(new List<GameObject>());

            foreach (KeyValuePair<int, StoryData.LineFlags> kvp in lastLineTypes)
            {
                GameObject tempTextboxGameObject = Instantiate(textboxPrefab);
                tempTextboxGameObject.transform.SetParent(tempSubPanelGameObject.transform, false);

                Image backgroundImage = tempTextboxGameObject.transform.GetChild(1).GetComponent<Image>();
                Image banner = tempTextboxGameObject.transform.GetChild(2).GetComponent<Image>();
                Image type = tempTextboxGameObject.transform.GetChild(3).GetComponent<Image>();
                TMPro.TMP_Text text = tempTextboxGameObject.transform.GetChild(4).GetComponent<TMPro.TMP_Text>();

                // set text
                text.text = storyData.initialText[storyData.initialText.Count + kvp.Key];

                if ((kvp.Value & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable)
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
            outcomeTextboxGameObject.transform.SetParent(tempSubPanelGameObject.transform, false);

            // set text
            outcomeTextboxGameObject.transform.GetChild(4).GetComponent<TMPro.TMP_Text>().text = outcomeText;

            // textbox color
            outcomeTextboxGameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.25f);

            textboxList[subPanelIndex].Add(outcomeTextboxGameObject);
            
            
            subPanelIndex++;
        }

        leftPanel.GetComponent<Image>().color = subPanelList[0].GetComponent<Image>().color;
        rightPanel.GetComponent<Image>().color = subPanelList[subPanelList.Count - 1].GetComponent<Image>().color;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rearrangementPanel.GetComponent<RectTransform>());
    }

}
