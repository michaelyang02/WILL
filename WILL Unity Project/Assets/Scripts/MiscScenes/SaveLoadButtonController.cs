using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class SaveLoadButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int sibilingIndex;
    private CanvasGroup canvasGroup;

    void Start()
    {
        sibilingIndex = transform.GetSiblingIndex();
        canvasGroup = GetComponent<CanvasGroup>();

        transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = SaveLoadManager.Instance.saveDatas[sibilingIndex].dateTime;
        transform.GetChild(3).GetComponent<TMPro.TMP_Text>().text = SaveLoadManager.Instance.saveDatas[sibilingIndex].discoveredOutcomes.ToString() + "/" + SaveLoadManager.Instance.saveDatas[sibilingIndex].totalOutcomes.ToString();
        //transform.GetChild(4).GetComponent<Image>().sprite = SaveSprite;

        canvasGroup.alpha = 0f;
        LeanTween.value(0f, 1f, 2f).setOnUpdate(f => canvasGroup.alpha = f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SaveLoad()
    {
        if (SaveLoadManager.isSaving)
        {
            if (sibilingIndex == 8)
            {
                StartCoroutine(IllegalOperation());
            }
            else
            {
                SerializationManager.Save("save" + sibilingIndex.ToString(), new PlayerDatas { storyPlayerDatas = StaticDataManager.StoryPlayerDatas, rearrangementPlayerDatas = StaticDataManager.RearrangementPlayerDatas.Values.Distinct().ToList() });

                SaveLoadManager.Instance.saveDatas[sibilingIndex].isSaved = true;
                SaveLoadManager.Instance.saveDatas[sibilingIndex].dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                SaveLoadManager.Instance.saveDatas[sibilingIndex].discoveredOutcomes = StaticDataManager.StoryPlayerDatas.Aggregate(0, (total, spd) => total + ((spd.isRead) ? spd.outcomeDiscovered.Count(o => o) : 0));
                SaveLoadManager.Instance.saveDatas[sibilingIndex].totalOutcomes = StaticDataManager.StoryPlayerDatas.Aggregate(0, (total, spd) => total + ((spd.isRead) ? spd.outcomeDiscovered.Count() : 0));

                SerializationManager.Save("saveData", SaveLoadManager.Instance.saveDatas);

                SaveLoadManager.Instance.UnloadSavedLoaded();
            }
        }
        else
        {
            if (SaveLoadManager.Instance.saveDatas[sibilingIndex].isSaved)
            {
                PlayerDatas playerDatas = SerializationManager.Load<PlayerDatas>("save" + transform.GetSiblingIndex().ToString());
                StaticDataManager.StoryPlayerDatas = playerDatas.storyPlayerDatas;
                StaticDataManager.RearrangementPlayerDatas = playerDatas.rearrangementPlayerDatas.SelectMany(rd => rd.indices, (rd, rdIndex) => new { rdIndex, rd }).ToDictionary(rd => rd.rdIndex, rd => rd.rd);

                SaveLoadManager.Instance.UnloadSavedLoaded();
            }
            else
            {
                StartCoroutine(IllegalOperation());
            }
        }
    }

    IEnumerator IllegalOperation()
    {
        GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1f);
        GetComponent<Image>().color = Color.white;
        yield break;
    }
}
