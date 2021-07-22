using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SquareController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public GameObject descriptionButtonPrefab;
    private GameObject descriptionButton;

    private Vector3 YExtent = Vector3.zero;
    private static float YDirectionButtonFactor = 1.1f;

    private bool isClicked = false;
    private GameObject canvas;

    public int storyIndex { get; set; }

    private static float CameraZoomLevelThreshold = 1f;
    private static MainGameManager mainGameManager;
    private static CameraManager cameraManager;

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

        spriteRenderer.sprite = MainGameManager.Instance.squareDeselectedSprite;
        spriteRenderer.size = MainGameManager.Instance.gridSize * Vector2.one;
        GetComponent<BoxCollider2D>().size = MainGameManager.Instance.gridSize * Vector2.one;

        YExtent.y = spriteRenderer.bounds.extents.y;
    }

    void Update()
    {
        ButtonFollow();
    }

    void OnDestroy()
    {
        // remove the event listener
        cameraManager.onZoomChange -= OnZoomChange;
    }

    void OnMouseDown()
    {
        // notify controller this square has been pressed
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            MainGameManager.Instance.SquareClick(storyIndex);
        }
    }

    public void Select()
    {
        // change sprite and show the button
        spriteRenderer.sprite = MainGameManager.Instance.squareSelectedSprite;
        isClicked = true;
        DisplayButton();

        // subscribe to any change to zoom level
        OnZoomChange(cameraManager.cameraZoomLevel);
        cameraManager.onZoomChange += OnZoomChange;
        cameraManager.FocusCamera(transform.position);
    }

    public void Deselect()
    {
        // change back and unsubscribe
        spriteRenderer.sprite = MainGameManager.Instance.squareDeselectedSprite;
        isClicked = false;
        cameraManager.onZoomChange -= OnZoomChange;
        Destroy(descriptionButton);
    }

    void DisplayButton()
    {
        // add button 
        descriptionButton = Instantiate(descriptionButtonPrefab);
        descriptionButton.transform.SetParent(canvas.transform, false);
        descriptionButton.transform.SetSiblingIndex(0);
        descriptionButton.GetComponent<DescriptionButtonController>().storyIndex = storyIndex;
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

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
