using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SquareController : MonoBehaviour
{
    #region sprite
    private SpriteRenderer spriteRenderer;
    public Sprite squareBorderSprite;
    public Sprite squareFilledSprite;

    private Color squareColor;
    private Color squareGreyedColor;
    private static float GreyedOutOpacity = 0.2f;
    #endregion

    #region button
    public GameObject descriptionButtonPrefab;
    private GameObject descriptionButton;

    private Vector3 YExtent = Vector3.zero;
    private static float YDirectionButtonFactor = 1.1f;
    #endregion

    public static Vector2 spriteSize;

    private bool isClicked = false;
    private GameObject canvas;

    private static float CameraZoomLevelThreshold = 1f;
    private static MainGameManager mainGameManager;
    private static CameraManager cameraManager;

    public StoryData storyData { get; set; }
    public StoryPlayerData storyPlayerData { get; set; }

    static SquareController()
    {
        mainGameManager = MainGameManager.Instance;
        cameraManager = CameraManager.Instance;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        canvas = GameObject.Find("Canvas");

        mainGameManager = MainGameManager.Instance;
        cameraManager = CameraManager.Instance;

        squareColor = storyData.GetColor();
        squareGreyedColor = squareColor;
        squareGreyedColor.a = GreyedOutOpacity;

        spriteRenderer.sprite = squareBorderSprite;
        spriteRenderer.color = squareColor;

        spriteRenderer.size = spriteSize;
        GetComponent<BoxCollider2D>().size = spriteSize;

        YExtent.y = spriteRenderer.bounds.extents.y;

        mainGameManager.onAnyClicked += OnMouseClick;
    }

    void Update()
    {
        ButtonFollow();
    }

    void OnDestroy()
    {
        // remove the event listeners
        mainGameManager.onAnyClicked -= OnMouseClick;
        cameraManager.onZoomChange -= OnZoomChange;
    }

    void OnMouseDown()
    {
        // notify controller this square has been pressed
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mainGameManager.SquareClicked(gameObject);
        }
    }

    void Select()
    {
        // add selected storyData to StaticDataManager
        StaticDataManager.selectedStoryOutcomes.Clear();
        StaticDataManager.selectedStoryOutcomes.Add(new KeyValuePair<int, int>(storyData.index, storyPlayerData.selectedOutcome));

        foreach (int companionIndex in storyData.companionIndices)
        {
            StaticDataManager.selectedStoryOutcomes.Add(new KeyValuePair<int, int>(companionIndex, StaticDataManager.storyPlayerDatas[companionIndex].selectedOutcome));
        }

        // change sprite and show the button
        spriteRenderer.sprite = squareFilledSprite;
        isClicked = true;
        DisplayButton();

        // subscribe to any change to zoom level
        OnZoomChange(cameraManager.cameraZoomLevel);
        cameraManager.onZoomChange += OnZoomChange;
        cameraManager.FocusCamera(transform.position);
    }

    void Deselect()
    {
        // change back and unsubscribe
        spriteRenderer.sprite = squareBorderSprite;
        isClicked = false;
        cameraManager.onZoomChange -= OnZoomChange;
        Destroy(descriptionButton);
    }

    void OnMouseClick(GameObject clickedGameObject)
    {
        // if the clicked one is the background, deselected it (but normal color)
        // if the clicked one is this one, change it to selected
        // if the clicked one is another square, deselect it

        if (clickedGameObject == null)
        {
            spriteRenderer.color = squareColor;
            if (isClicked)
            {
                Deselect();
            }
        }
        else if (clickedGameObject == gameObject)
        {
            spriteRenderer.color = squareColor;
            if (!isClicked)
            {
                Select();
            }
        }
        else
        {
            spriteRenderer.color = squareGreyedColor;
            Deselect();
        }
    }

    void DisplayButton()
    {
        // add button 
        descriptionButton = Instantiate(descriptionButtonPrefab);
        descriptionButton.transform.SetParent(canvas.transform, false);
        descriptionButton.transform.SetSiblingIndex(0);
        descriptionButton.GetComponent<Button>().image.color = spriteRenderer.color;
        descriptionButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = storyData.title;
    }

    void ButtonFollow()
    {
        // button follows mouse it is clicked
        if (isClicked)
        {
            Vector3 buttonPos = Camera.main.WorldToScreenPoint(transform.position - YDirectionButtonFactor * YExtent);
            descriptionButton.transform.position = buttonPos;
        }
    }

    void OnZoomChange(float cameraZoomLevel)
    {
        // if the zoom is changed, check if the zoom level is big enough
        // for the button to be displayed
        if (cameraZoomLevel < CameraZoomLevelThreshold)
        {
            descriptionButton.SetActive(false);

        }
        else
        {
            descriptionButton.SetActive(true);
        }
    }
}
