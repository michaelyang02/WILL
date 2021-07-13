using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextboxController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject placeholderPrefab;
    private Transform canvasTransform;
    private ScrollRect scrollRect;
    private Transform rearrangementPanelTransform;
    private RectTransform rearrangementPanelRectTransform;
    private RectTransform textboxRectTransform;
    private Transform placeholderTransform;
    private Transform backButtonTransform;

    private Transform lastSubpanelTransform;
    private int lastPlaceholderIndex;

    private Transform currentSubpanelTransform;
    private int currentPlaceholderIndex;

    public StoryData.LineFlags textboxFlag { get; set; }

    private Vector2 lastPosition;

    public static Dictionary<Transform, Vector2> SubPanelBoundaries;
    public static float LeftMargin;
    public static float RightMargin;

    private Coroutine scrollCoroutine;


    void Start()
    {
        textboxRectTransform = GetComponent<RectTransform>();
        canvasTransform = GameObject.Find("Canvas").transform;
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        rearrangementPanelTransform = GameObject.Find("Rearrangement Panel").transform;
        rearrangementPanelRectTransform = rearrangementPanelTransform.GetComponent<RectTransform>();
        backButtonTransform = GameObject.Find("Back").transform;

        StartCoroutine(Initialise());
    }

    IEnumerator Initialise()
    {
        yield return null;

        if (IsDraggable())
        {
            placeholderTransform = Instantiate(placeholderPrefab).transform;
            placeholderTransform.GetComponent<RectTransform>().sizeDelta = textboxRectTransform.sizeDelta;
            placeholderTransform.gameObject.SetActive(false);
            placeholderTransform.SetParent(canvasTransform, false);
        }
        yield break;
    }

    bool IsDraggable()
    {
        return (textboxFlag & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            lastPosition = textboxRectTransform.position;
            textboxRectTransform.Rotate(new Vector3(0f, 0f, 0.5f));

            currentSubpanelTransform = transform.parent;
            currentPlaceholderIndex = transform.GetSiblingIndex();
            lastSubpanelTransform = currentSubpanelTransform;
            lastPlaceholderIndex = currentPlaceholderIndex;

            transform.SetParent(canvasTransform, true);
            transform.SetSiblingIndex(backButtonTransform.GetSiblingIndex());

            placeholderTransform.gameObject.SetActive(true);
            placeholderTransform.SetParent(lastSubpanelTransform, false);
            placeholderTransform.SetSiblingIndex(lastPlaceholderIndex);

            scrollCoroutine = StartCoroutine(Scroll());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            textboxRectTransform.position = lastPosition + (eventData.position - eventData.pressPosition);

            bool isSubpanelChanged = false;

            float x = textboxRectTransform.position.x;

            if (x >= SubPanelBoundaries[currentSubpanelTransform].x &&
            x <= SubPanelBoundaries[currentSubpanelTransform].y)
            {
                // still within current transform do nothing
            }
            else
            {
                if (x < LeftMargin) // far left
                {
                    if (!currentSubpanelTransform.Equals(rearrangementPanelTransform.GetChild(0)))
                    {
                        currentSubpanelTransform = rearrangementPanelTransform.GetChild(0);
                        isSubpanelChanged = true;
                    }
                }
                else if (x > RightMargin) // far right
                {
                    if (!currentSubpanelTransform.Equals(rearrangementPanelTransform.GetChild(rearrangementPanelTransform.childCount - 1)))
                    {
                        currentSubpanelTransform = rearrangementPanelTransform.GetChild(rearrangementPanelTransform.childCount - 1);
                        isSubpanelChanged = true;
                    }
                }
                else // within one subpanel
                {
                    foreach (KeyValuePair<Transform, Vector2> kvp in SubPanelBoundaries)
                    {
                        if (x >= kvp.Value.x && x <= kvp.Value.y)
                        {
                            currentSubpanelTransform = kvp.Key;
                        }
                    }
                    isSubpanelChanged = true;
                }
            }

            float y = textboxRectTransform.position.y;

            bool isIndexChanged = false;

            if (isSubpanelChanged == false)
            {
                if (currentPlaceholderIndex > 1 &&
                y > currentSubpanelTransform.GetChild(currentPlaceholderIndex - 1).GetComponent<RectTransform>().position.y)
                { // move index down if above previous one
                    currentPlaceholderIndex--;
                    isIndexChanged = true;
                }
                else if (currentPlaceholderIndex < currentSubpanelTransform.childCount - 2 &&
                y < currentSubpanelTransform.GetChild(currentPlaceholderIndex + 1).GetComponent<RectTransform>().position.y)
                { // move index up if below following one
                    currentPlaceholderIndex++;
                    isIndexChanged = true;
                }
            }
            else
            {
                if (y >= currentSubpanelTransform.GetChild(0).GetComponent<RectTransform>().position.y)
                {   // above top
                    currentPlaceholderIndex = 1;
                }
                else if (y <= currentSubpanelTransform.GetChild(currentSubpanelTransform.childCount - 1).GetComponent<RectTransform>().position.y)
                {   // below bottom
                    currentPlaceholderIndex = currentSubpanelTransform.childCount - 1;
                }
                else
                {   // between two
                    for (int tIndex = 0; tIndex < currentSubpanelTransform.childCount - 2; tIndex++)
                    {
                        if (y < currentSubpanelTransform.GetChild(tIndex).GetComponent<RectTransform>().position.y &&
                        y > currentSubpanelTransform.GetChild(tIndex + 1).GetComponent<RectTransform>().position.y)
                        {
                            currentPlaceholderIndex = tIndex + 1;
                            break;
                        }
                    }
                }
            }

            if (isSubpanelChanged || isIndexChanged)
            {
                placeholderTransform.SetParent(currentSubpanelTransform, false);
                placeholderTransform.SetSiblingIndex(currentPlaceholderIndex);
            }

            // TODO: add logic to check if the position is invalid, e.g. wrong order
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            textboxRectTransform.Rotate(new Vector3(0f, 0f, -0.5f));

            placeholderTransform.gameObject.SetActive(false);
            placeholderTransform.SetParent(canvasTransform, true);

            transform.SetParent(currentSubpanelTransform, false);
            transform.SetSiblingIndex(currentPlaceholderIndex);

            StopCoroutine(scrollCoroutine);
        }
    }

    IEnumerator Scroll()
    {
        while (true)
        {
            if (Input.mousePosition.y > Screen.height * 0.9f)
            { // scroll up
                scrollRect.verticalNormalizedPosition += 0.005f;
            }
            else if (Input.mousePosition.y < Screen.height * 0.1f)
            { // scroll down
                scrollRect.verticalNormalizedPosition -= 0.005f;
            }
            yield return null;
        }
    }
}