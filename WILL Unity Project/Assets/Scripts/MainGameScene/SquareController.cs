using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SquareController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    public GameObject descriptionButtonPrefab;
    private GameObject descriptionButton;

    private Vector3 YExtent = Vector3.zero;
    private static float YDirectionButtonFactor = 1.2f;

    private bool isClicked;
    private bool isScaled;
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
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        canvas = GameObject.Find("Canvas");

        mainGameManager = MainGameManager.Instance;
        cameraManager = CameraManager.Instance;

        spriteRenderer.sprite = MainGameManager.Instance.squareDeselectedSprite;
        spriteRenderer.size = MainGameManager.Instance.gridSize * Vector2.one;
        boxCollider2D.size = MainGameManager.Instance.gridSize * Vector2.one;

        YExtent.y = spriteRenderer.bounds.extents.y;
    }

    void Update()
    {
        // button follows mouse it is clicked
        if (isClicked)
        {
            descriptionButton.transform.position = Camera.main.WorldToScreenPoint(transform.position - YDirectionButtonFactor * YExtent);
        }
    }

    void OnDestroy()
    {
        // remove the event listener
        cameraManager.onZoomChange -= OnZoomChange;
    }

    void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!isScaled)
            {
                LeanTween.value(gameObject, MainGameManager.Instance.gridSize, MainGameManager.Instance.gridSize * 1.05f, 0.25f).setEaseOutBack().setOnUpdate((float v) =>
                {
                    spriteRenderer.size = v * Vector2.one;
                    boxCollider2D.size = v * Vector2.one;
                });
                isScaled = true;
            }
        }
    }

    void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (isScaled)
            {
                LeanTween.value(gameObject, spriteRenderer.size.x, MainGameManager.Instance.gridSize, 0.1f).setEaseLinear().setOnUpdate((float v) =>
                {
                    spriteRenderer.size = v * Vector2.one;
                    boxCollider2D.size = v * Vector2.one;
                });
                isScaled = false;
            }
        }
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
        cameraManager.FocusCamera(transform.position);

        DisplayButton();

        // subscribe to any change to zoom level
        OnZoomChange(cameraManager.cameraZoomLevel);
        cameraManager.onZoomChange += OnZoomChange;
    }

    public void Deselect()
    {
        // change back and unsubscribe
        spriteRenderer.sprite = MainGameManager.Instance.squareDeselectedSprite;
        isClicked = false;
        cameraManager.onZoomChange -= OnZoomChange;
        Destroy(descriptionButton);
    }

    public void GreyOut(bool state)
    {
        Color color = spriteRenderer.color;
        color.a = state ? 0.5f : 1f;
        spriteRenderer.color = color;
    }

    void DisplayButton()
    {
        // add button 
        descriptionButton = Instantiate(descriptionButtonPrefab);
        descriptionButton.transform.SetParent(canvas.transform, false);
        descriptionButton.transform.SetSiblingIndex(0);
        descriptionButton.GetComponent<DescriptionButtonController>().storyIndex = storyIndex;
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
