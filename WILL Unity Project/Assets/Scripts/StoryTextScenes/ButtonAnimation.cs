using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    bool isOut;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOut) 
        {
            LeanTween.moveX(rectTransform, 0f, 0.2f);
            isOut = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOut)
        {
            LeanTween.moveX(rectTransform, 50f, 0.2f);
            isOut = false;
        }
    }
}