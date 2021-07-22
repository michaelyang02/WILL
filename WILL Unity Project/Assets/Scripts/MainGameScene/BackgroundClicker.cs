using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class BackgroundClicker : MonoBehaviour
{
    private MainGameManager mainGameController;
    private CameraManager cameraController;
    private BoxCollider2D boxCollider2D;
    private Vector2 defaultSize;

    private float orthographicHeight;
    private float aspectRatio;
    private Camera orthographicCamera;

    private Vector3 dragOrigin;

    private Queue<float> lastDragSpeeds;

    private Coroutine recordSpeed;
    private Coroutine addSpeed;

    void Start()
    {
        cameraController = CameraManager.Instance;
        mainGameController = MainGameManager.Instance;
        boxCollider2D = GetComponent<BoxCollider2D>();
        lastDragSpeeds = new Queue<float>();

        orthographicCamera = Camera.main;
        orthographicHeight = 2f * orthographicCamera.orthographicSize;
        aspectRatio = Camera.main.aspect;

        defaultSize = new Vector2(orthographicHeight * aspectRatio, orthographicHeight);
        boxCollider2D.size = defaultSize;
        cameraController.onZoomChange += OnZoomChange;
    }

    void OnDestroy()
    {
        cameraController.onZoomChange -= OnZoomChange;
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            MainGameManager.Instance.SquareClick(-1);

            dragOrigin = orthographicCamera.ScreenToWorldPoint(Input.mousePosition);

            if (addSpeed != null)
            {
                StopCoroutine(addSpeed);
            }
            recordSpeed = StartCoroutine(RecordLastDragSpeed());
        }
    }

    void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            cameraController.SetNewPosition(dragOrigin - orthographicCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (recordSpeed != null)
            {
                StopCoroutine(recordSpeed);
            }
            addSpeed = StartCoroutine(AddInertialDragVelocity(Vector3.Normalize(dragOrigin - orthographicCamera.ScreenToWorldPoint(Input.mousePosition))));
        }
    }

    IEnumerator RecordLastDragSpeed()
    {
        int numOfRecords = 0;
        lastDragSpeeds.Clear();

        while (numOfRecords < 400)
        {
            if (numOfRecords > 3)
            {
                lastDragSpeeds.Dequeue();
            }
            lastDragSpeeds.Enqueue(Vector3.Distance(dragOrigin, orthographicCamera.ScreenToWorldPoint(Input.mousePosition)));
            numOfRecords += 1;

            yield return new WaitForSeconds(0.05f);
        }
        yield break;
    }

    IEnumerator AddInertialDragVelocity(Vector3 dragDirection)
    {
        float averageSpeed = 0f;

        if (lastDragSpeeds.Count != 0)
        {
            averageSpeed += 0.5f * lastDragSpeeds.Average();
        }

        averageSpeed += 0.5f * Vector3.Distance(dragOrigin, orthographicCamera.ScreenToWorldPoint(Input.mousePosition));

        for (float f = 1f; f > 0.05f; f -= 0.01f)
        {
            cameraController.SetNewPosition(dragDirection * averageSpeed * Mathf.Pow(f, 2f));
            yield return null;
        }
        yield break;
    }

    public void EndInertia()
    {
        if (addSpeed != null)
        {
            StopCoroutine(addSpeed);
        }
    }

    void OnZoomChange(float cameraZoomLevel)
    {
        boxCollider2D.size = defaultSize / cameraZoomLevel;
    }
}
