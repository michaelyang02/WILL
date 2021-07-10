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

        InitialiseSquare(0, Vector2Int.zero);
        InitialiseSquare(1, Vector2Int.one * 2);
    
    }

    void InitialiseSquare(int storyIndex, Vector2Int position)
    {
        GameObject gameObject = Instantiate(squarePrefab, (Vector2) position * gridSize, Quaternion.identity);
        gameObject.GetComponent<SquareController>().storyIndex = storyIndex;
    }

    public void SquareClicked(GameObject clickedSquare)
    {
        onAnyClicked?.Invoke(clickedSquare);
    }
}
