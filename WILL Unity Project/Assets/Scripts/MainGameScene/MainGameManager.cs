using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainGameManager : MonoBehaviour
{

    public static MainGameManager Instance;

    public GameObject squarePrefab;
    public float gridSize;

    public event Action<GameObject> onAnyClicked;

    private static bool isLoaded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SquareController.spriteSize = gridSize * Vector2.one;

        InitialiseSquare(0, Vector2Int.zero);
        isLoaded = true;    
    }

    void InitialiseSquare(int storyIndex, Vector2Int position)
    {
        GameObject gameObject = Instantiate(squarePrefab, (Vector2) position * gridSize, Quaternion.identity);
        gameObject.GetComponent<SquareController>().storyIndex = storyIndex;
        
        if (!isLoaded)
        {
            StaticDataManager.StoryPosition.Add(storyIndex, (Vector2) position * gridSize);
        }
    }

    public void SquareClicked(GameObject clickedSquare)
    {
        onAnyClicked?.Invoke(clickedSquare);
    }
}
