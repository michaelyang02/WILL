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


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SquareController.spriteSize = gridSize * Vector2.one;

        InitialiseSquare(StaticDataManager.storyDatas[0], StaticDataManager.storyPlayerDatas[0], Vector2Int.zero);
        InitialiseSquare(StaticDataManager.storyDatas[1], StaticDataManager.storyPlayerDatas[1], Vector2Int.one * 2);
    
    }

    void InitialiseSquare(StoryData story, StoryPlayerData storyPlayer, Vector2Int position)
    {
        GameObject gameObject = Instantiate(squarePrefab, (Vector2) position * gridSize, Quaternion.identity);
        gameObject.GetComponent<SquareController>().storyData = story;
        gameObject.GetComponent<SquareController>().storyPlayerData = storyPlayer;
    }

    public void SquareClicked(GameObject clickedSquare)
    {
        onAnyClicked?.Invoke(clickedSquare);
    }
}
