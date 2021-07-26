using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    #region square
    public GameObject squarePrefab;
    public Sprite squareSelectedSprite;
    public Sprite squareDeselectedSprite;
    #endregion

    public float gridSize;
    public Vector2 gridOffset;

    public static MainGameManager Instance;

    private Dictionary<int, StoryData.Character> indexCharacters;
    private List<SquareController> squareControllers;
    private List<CompositeEdge> edges;

    private int selectedSquareIndex = -1;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        EdgeHelper.Instance.size = MainGameManager.Instance.gridSize * Vector2.one;
        indexCharacters = StaticDataManager.StoryDatas.ToDictionary(d => d.index, d => d.character);
        squareControllers = new List<SquareController>();
        edges = new List<CompositeEdge>();
        
        GenerateSquares();
        GenerateEdges();
    }

    public void SquareClick(int index)
    {
        if (selectedSquareIndex != -1)
        {
            squareControllers[selectedSquareIndex].Deselect();
            // set all edges connected to previous one false
            edges.Where(e => e[selectedSquareIndex]).ToList().ForEach(e => e.SetActive(false));
            // set all children edges to true
            edges.Where(e => e.character != StoryData.Character.None).ToList().ForEach(e => e.SetActive(true));

            // set all squares back to normal
            squareControllers.ForEach(s => s.GreyOut(false));
        }
        if (index != -1)
        {
            List<StoryData.Character> companionCharacters = StaticDataManager.RearrangementDatas[index].indices.Select(i => indexCharacters[i]).ToList();

            squareControllers[index].Select();
            // set all edges unrelated to companions to false
            edges.Where(e => e.character == StoryData.Character.None || !companionCharacters.Contains(e.character)).ToList().ForEach(e => e.SetActive(false));
            // all edges related to this one to true
            edges.Where(e => e[index]).ToList().ForEach(e => e.SetActive(true));

            squareControllers.Where(s => !companionCharacters.Contains(indexCharacters[s.storyIndex])).ToList().ForEach(s => s.GreyOut(true));
        }
        selectedSquareIndex = index;
    }

    public Vector2 GetWorldPosition(Vector2Int gridPosition)
    {
        return (Vector2)gridPosition * gridSize + gridOffset;
    }

    public void GenerateSquares()
    {
        StoryManager.CheckAnyStoryDiscovered();
        StoryManager.CheckAnyEnabled();

        squareControllers.ForEach(sc => Destroy(sc.gameObject));
        squareControllers.Clear();

        for (int index = 0; index < StaticDataManager.StoryPosition.Count; index++)
        {
            if (StaticDataManager.StoryPlayerDatas[index].isDiscovered)
            {
                GameObject squareGO = Instantiate(squarePrefab, GetWorldPosition(StaticDataManager.StoryPosition[index]), Quaternion.identity);
                squareGO.transform.SetParent(null);
                SquareController squareController = squareGO.GetComponent<SquareController>();
                squareControllers.Add(squareController);
                squareController.storyIndex = index;
                
                // set color
                if (StaticDataManager.StoryPlayerDatas[index].isEnabled)
                {
                    squareController.SetColor(ColorManager.GetColor(StaticDataManager.StoryDatas[index].character));
                }
                else
                { // disabled color
                    squareController.SetColor(Color.black);
                }
            }
        }
    }

    public void GenerateEdges()
    {
        edges.ForEach(e => Destroy(e.edgeGameObject));
        edges.Clear();

        foreach (StoryData storyData in StaticDataManager.StoryDatas)
        {
            if (storyData.parentIndex >= 0)
            { // parent-child edge
                edges.Add(EdgeHelper.Instance.GenerateEdge(EdgeType.Children, storyData.index, storyData.parentIndex));
            }
            if (storyData.requiredEnabledOutcomes.Any())
            { // requirement edge
                foreach (StoryData.OutcomeIndices outcomeIndices in storyData.requiredEnabledOutcomes)
                {
                    if (storyData.parentIndex == outcomeIndices.storyIndex)
                    { // skip if the required story is of the parent
                        continue;
                    }

                    if (StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].outcomeEnabled[outcomeIndices.outcomeIndex] && StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].selectedOutcome == outcomeIndices.outcomeIndex)
                    { // required outcome enabled and selected
                        edges.Add(EdgeHelper.Instance.GenerateEdge(EdgeType.Enable, storyData.index, outcomeIndices.storyIndex));
                    }
                    else
                    {
                        edges.Add(EdgeHelper.Instance.GenerateEdge(EdgeType.Disable, storyData.index, outcomeIndices.storyIndex));
                    }
                }
            }
        }

        // companion indices
        foreach (RearrangementData rd in StaticDataManager.RearrangementDatas.Values.Distinct()) if (rd.indices.Length > 1)
        {
            edges.Add(EdgeHelper.Instance.GenerateEdge(EdgeType.Companion, rd.indices));
        }
    }
}