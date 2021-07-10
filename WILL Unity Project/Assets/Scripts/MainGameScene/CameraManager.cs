using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float defaultOrthographicSize;

    public float cameraMovementTime;

    private Vector3 newPosition;

    public float[] cameraZoomLevels; // requires 1f, in descending order
    public float cameraZoomLevel { get; private set; } = 1f;
    private int cameraZoomLevelIndex;

    private Camera orthographicCamera;
    private float newOrthographicSize;

    private static Vector3 focusPosition = new Vector3(0f, 0f, -5f);
    private BackgroundClicker backgroundClicker;

    public event Action<float> onZoomChange; // give zoom level to listener

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        orthographicCamera = GetComponent<Camera>();
        backgroundClicker = transform.GetChild(0).GetComponent<BackgroundClicker>();

        newPosition = focusPosition;
        transform.position = focusPosition;

        // default cameraZoomLevels with smaller number more zoomed out
        // cameraZoomLevels = new float[] {2f, 1f, 0.5f, 0.25f};

        cameraZoomLevelIndex = Array.IndexOf(cameraZoomLevels, 1f);

        newOrthographicSize = defaultOrthographicSize;
        orthographicCamera.orthographicSize = defaultOrthographicSize;
    }

    void Update()
    {
        // press F to focus, scroll to zoom in and out
        if (!PauseScreenManager.GameIsPaused)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                FocusCamera(focusPosition);
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                if (cameraZoomLevelIndex < cameraZoomLevels.Length - 1)
                {
                    cameraZoomLevelIndex += 1;
                    cameraZoomLevel = cameraZoomLevels[cameraZoomLevelIndex];
                    onZoomChange?.Invoke(cameraZoomLevel);
                }
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                if (cameraZoomLevelIndex > 0)
                {
                    cameraZoomLevelIndex -= 1;
                    cameraZoomLevel = cameraZoomLevels[cameraZoomLevelIndex];
                    onZoomChange?.Invoke(cameraZoomLevel);
                }
            }
            newOrthographicSize = defaultOrthographicSize / cameraZoomLevel;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraMovementTime);
            orthographicCamera.orthographicSize = Mathf.Lerp(orthographicCamera.orthographicSize, newOrthographicSize, Time.deltaTime * cameraMovementTime);
        }
    }

    public void FocusCamera(Vector2 focusPosition)
    {
        CameraManager.focusPosition.x = focusPosition.x;
        CameraManager.focusPosition.y = focusPosition.y;

        newOrthographicSize = defaultOrthographicSize;
        cameraZoomLevelIndex = Array.IndexOf(cameraZoomLevels, 1f);
        cameraZoomLevel = 1f;
        onZoomChange?.Invoke(cameraZoomLevel);

        newPosition = CameraManager.focusPosition;
        backgroundClicker.EndInertia();
    }

    public void SetNewPosition(Vector3 newPosition)
    {
        this.newPosition = transform.position + newPosition;
    }

    public static void SetNewFocusPosition()
    {
        // TODO: set focus position to the exitted square from back button.
    }
}
