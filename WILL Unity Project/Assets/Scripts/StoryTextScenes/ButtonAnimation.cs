using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.moveX(rectTransform, 0f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.moveX(rectTransform, 25f, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.moveX(rectTransform, 25f, 0.2f);
    }
}