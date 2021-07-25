using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextboxController : BoxController, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject placeholderPrefab;
    public static Transform CanvasTransform;
    public static ScrollRect ScrollRect;
    public static Transform RearrangementPanelTransform;
    public static RectTransform RearrangementPanelRectTransform;
    public static Transform BackButtonTransform;
    private RectTransform textboxRectTransform;
    private Transform placeholderTransform;
    private Image placeholderImage;

    private Transform lastSubpanelTransform;
    private int lastPlaceholderIndex;

    private Transform currentSubpanelTransform;
    private int currentPlaceholderIndex;

    private Vector2 lastPosition;
    private static bool isValidPosition;

    public static Dictionary<Transform, Vector2> SubPanelBoundaries;
    public static float LeftMargin;
    public static float RightMargin;

    private static Coroutine ScrollCoroutine;
    private static float ScrollSpeed = 0.0075f;

    private static Color ValidColor = new Color(0f, 0f, 0f, 0.5f);
    private static Color InvalidColor = new Color(1f, 0f, 0f, 0.75f);

    private static Dictionary<Transform, List<Transform>> lastTransforms = new Dictionary<Transform, List<Transform>>();

    private Transform lastSwitchingSubpanelTransform;
    private int lastSwitchingIndex;
    private static bool isSwitched;

    void Start()
    {
        textboxRectTransform = GetComponent<RectTransform>();

        StartCoroutine(Initialise());
    }

    IEnumerator Initialise()
    {
        yield return null;

        if (IsDraggable())
        {
            placeholderTransform = Instantiate(placeholderPrefab).transform;
            placeholderTransform.GetComponent<RectTransform>().sizeDelta = textboxRectTransform.sizeDelta;
            placeholderTransform.GetComponent<BoxController>().storyIndex = storyIndex;
            placeholderTransform.GetComponent<BoxController>().boxFlag = boxFlag;

            placeholderTransform.gameObject.SetActive(false);
            placeholderTransform.SetParent(CanvasTransform, false);

            placeholderImage = placeholderTransform.GetComponent<Image>();
        }
        yield break;
    }

    bool IsDraggable()
    {
        return (boxFlag & StoryData.LineFlags.Draggable) == StoryData.LineFlags.Draggable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            isValidPosition = true;
            isSwitched = false;
            placeholderImage.color = ValidColor;

            lastPosition = textboxRectTransform.position;
            textboxRectTransform.Rotate(new Vector3(0f, 0f, 0.5f));

            lastTransforms.Clear();

            foreach (Transform transform in RearrangementPanelTransform)
            {
                lastTransforms.Add(transform, new List<Transform>());
                foreach (Transform textboxTransform in transform)
                {
                    lastTransforms[transform].Add(textboxTransform);
                }
            }

            currentSubpanelTransform = transform.parent;
            currentPlaceholderIndex = transform.GetSiblingIndex();
            lastSubpanelTransform = transform.parent;
            lastPlaceholderIndex = transform.GetSiblingIndex();

            transform.SetParent(CanvasTransform, true);
            transform.SetSiblingIndex(BackButtonTransform.GetSiblingIndex());

            placeholderTransform.gameObject.SetActive(true);
            placeholderTransform.SetParent(lastSubpanelTransform, false);
            placeholderTransform.SetSiblingIndex(lastPlaceholderIndex);

            ScrollCoroutine = StartCoroutine(Scroll());
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
                    isSubpanelChanged = (currentSubpanelTransform != RearrangementPanelTransform.GetChild(0));
                    if (isSubpanelChanged) currentSubpanelTransform = RearrangementPanelTransform.GetChild(0);
                }
                else if (x > RightMargin) // far right
                {
                    isSubpanelChanged = (currentSubpanelTransform != RearrangementPanelTransform.GetChild(RearrangementPanelTransform.childCount - 1));
                    if (isSubpanelChanged) currentSubpanelTransform = RearrangementPanelTransform.GetChild(RearrangementPanelTransform.childCount - 1);
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

                    if ((currentSubpanelTransform.GetChild(currentPlaceholderIndex).GetComponent<TextboxController>().boxFlag & StoryData.LineFlags.Pinned) == StoryData.LineFlags.Pinned)
                    {
                        currentPlaceholderIndex--;
                        currentSubpanelTransform.GetChild(currentPlaceholderIndex).SetSiblingIndex(currentPlaceholderIndex + 2);
                    }

                    isIndexChanged = true;
                }
                else if (currentPlaceholderIndex < currentSubpanelTransform.childCount - 2 &&
                y < currentSubpanelTransform.GetChild(currentPlaceholderIndex + 1).GetComponent<RectTransform>().position.y)
                { // move index up if below following one
                    currentPlaceholderIndex++;

                    if ((currentSubpanelTransform.GetChild(currentPlaceholderIndex).GetComponent<TextboxController>().boxFlag & StoryData.LineFlags.Pinned) == StoryData.LineFlags.Pinned)
                    {
                        currentPlaceholderIndex++;
                        currentSubpanelTransform.GetChild(currentPlaceholderIndex).SetSiblingIndex(currentPlaceholderIndex - 2);
                    }

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

            if ((isSubpanelChanged && (boxFlag & StoryData.LineFlags.Switching) == 0) || isIndexChanged)
            { // either subpanel changed but not switching or index changed
                placeholderTransform.SetParent(currentSubpanelTransform, false);
                placeholderTransform.SetSiblingIndex(currentPlaceholderIndex);
            }
            else if (isSubpanelChanged)
            { // if subpanel change and switching
                if (currentSubpanelTransform != lastSubpanelTransform)
                { // if not in last transform
                    placeholderTransform.SetParent(currentSubpanelTransform, false);

                    if (isSwitched)
                    {
                        Transform oldSwitchedTransform = lastSubpanelTransform.GetChild(lastPlaceholderIndex);
                        oldSwitchedTransform.SetParent(lastSwitchingSubpanelTransform);
                        oldSwitchedTransform.SetSiblingIndex(lastSwitchingIndex);
                    }

                    Transform switchingTransform = null;

                    foreach (Transform child in currentSubpanelTransform)
                    {
                        if ((child.GetComponent<BoxController>().boxFlag & StoryData.LineFlags.Switching) == StoryData.LineFlags.Switching)
                        { // check if new subpanel has switching textbox too
                            switchingTransform = child;
                            lastSwitchingSubpanelTransform = currentSubpanelTransform;
                            lastSwitchingIndex = child.GetSiblingIndex();
                            break;
                        }
                    }

                    if (lastSwitchingIndex < currentPlaceholderIndex) currentPlaceholderIndex--;
                    placeholderTransform.SetSiblingIndex(currentPlaceholderIndex);

                    if (switchingTransform != null)
                    {
                        switchingTransform.SetParent(lastSubpanelTransform, false);
                        switchingTransform.SetSiblingIndex(lastPlaceholderIndex);
                    }
                    isSwitched = (switchingTransform != null);
                }
                else
                { // returned to the last transform

                    placeholderTransform.SetParent(currentSubpanelTransform, false);

                    if (isSwitched)
                    {
                        Transform oldSwitchedTransform = lastSubpanelTransform.GetChild(lastPlaceholderIndex);
                        if (lastPlaceholderIndex < currentPlaceholderIndex) currentPlaceholderIndex--;
                        oldSwitchedTransform.SetParent(lastSwitchingSubpanelTransform);
                        oldSwitchedTransform.SetSiblingIndex(lastSwitchingIndex);
                    }

                    placeholderTransform.SetSiblingIndex(currentPlaceholderIndex);

                    isSwitched = false;
                }
            }

            if (isSubpanelChanged || isIndexChanged)
            {
                CheckValidity();

                // the arrangement is invalid if either
                // 1) invalid position
                // 2) switching, not switched and current subpanel is not the last one
                placeholderImage.color = (!isValidPosition || ((boxFlag & StoryData.LineFlags.Switching) ==
                StoryData.LineFlags.Switching && !isSwitched && currentSubpanelTransform != lastSubpanelTransform)) ? InvalidColor : ValidColor;
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            textboxRectTransform.Rotate(new Vector3(0f, 0f, -0.5f));

            placeholderTransform.gameObject.SetActive(false);
            placeholderTransform.SetParent(CanvasTransform, true);

            if (!isValidPosition || ((boxFlag & StoryData.LineFlags.Switching) ==
            StoryData.LineFlags.Switching && !isSwitched && currentSubpanelTransform !=
            lastSubpanelTransform))
            {
                foreach (List<Transform> transforms in lastTransforms.Values)
                {
                    transforms.ForEach(t => t.SetParent(null, false));
                }

                foreach (var kvp in lastTransforms)
                {
                    kvp.Value.ForEach(t => t.SetParent(kvp.Key, false));
                }
            }
            else
            {
                transform.SetParent(currentSubpanelTransform, false);
                transform.SetSiblingIndex(currentPlaceholderIndex);
            }

            StopCoroutine(ScrollCoroutine);
        }
    }

    static void CheckValidity()
    {
        foreach (Transform subpanelTransform in RearrangementPanelTransform)
        { // check every subpanel and every textbox
            int storyIndex = subpanelTransform.GetComponent<SubpanelController>().storyIndex;
            byte highestNumber = 0;

            foreach (Transform textboxTransform in subpanelTransform)
            {
                BoxController boxController = textboxTransform.GetComponent<BoxController>();

                if ((boxController.boxFlag & StoryData.LineFlags.Unswappable) == StoryData.LineFlags.Unswappable &&
                boxController.storyIndex != storyIndex)
                { // if unswappable and swapped
                    isValidPosition = false;
                    return;
                }

                byte number = (byte)(boxController.boxFlag & (StoryData.LineFlags.Numbered1 | StoryData.LineFlags.Numbered2 | StoryData.LineFlags.Numbered3));
                if (number != 0 && number < highestNumber)
                { // if number lower than previous ones
                    isValidPosition = false;
                    return;
                }
                if (number > highestNumber) highestNumber = number;
            }
        }
        isValidPosition = true;
    }

    IEnumerator Scroll()
    {
        while (true)
        {
            if (Input.mousePosition.y > Screen.height * 0.95f)
            { // scroll up
                ScrollRect.verticalNormalizedPosition += ScrollSpeed;
            }
            else if (Input.mousePosition.y < Screen.height * 0.05f)
            { // scroll down
                ScrollRect.verticalNormalizedPosition -= ScrollSpeed;
            }
            yield return null;
        }
    }
}