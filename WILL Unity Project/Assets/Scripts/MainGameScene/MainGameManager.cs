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

    private List<SquareController> squareControllers;
    private List<CompositeEdge> edges;

    private int selectedSquareIndex = -1;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
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
            edges.Where(e => e[selectedSquareIndex]).ToList().ForEach(e => e.SetActive(false));
        }
        if (index != -1)
        {
            squareControllers[index].Select();
            edges.Where(e => e[index]).ToList().ForEach(e => e.SetActive(true));
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
        foreach (StoryData storyData in StaticDataManager.StoryDatas)
        {
            if (storyData.parentIndex >= 0)
            { // parent-child edge
                edges.Add(EdgeHelper.GenerateEdge(EdgeType.Children, storyData.parentIndex, storyData.index));
            }
            if (storyData.requiredEnableddOutcomes.Any())
            { // requirement edge
                foreach (StoryData.OutcomeIndices outcomeIndices in storyData.requiredEnableddOutcomes)
                {
                    if (StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].outcomeEnabled[outcomeIndices.outcomeIndex] && StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].selectedOutcome == outcomeIndices.outcomeIndex)
                    { // required outcome enabled and selected
                        edges.Add(EdgeHelper.GenerateEdge(EdgeType.Enable, storyData.index, outcomeIndices.storyIndex));
                    }
                    else
                    {
                        edges.Add(EdgeHelper.GenerateEdge(EdgeType.Disable, storyData.index, outcomeIndices.storyIndex));
                    }
                }
            }
        }

        // companion indices
        foreach (RearrangementData rd in StaticDataManager.RearrangementDatas.Values.Distinct())
        {
            edges.Add(EdgeHelper.GenerateEdge(EdgeType.Companion, rd.indices));
        }
    }
}

public enum EdgeType
    {
        Children,
        Companion = ColorManager.EdgeColor.VividTangerine,
        Enable = ColorManager.EdgeColor.MintGreen,
        Disable = ColorManager.EdgeColor.Black
    }

public class EdgeHelper : MonoBehaviour
{
    public static CompositeEdge GenerateEdge(EdgeType edgeType, params int[] indices)
    {
        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < indices.Length - 1; i++)
        {
            GameObject masterGameObject = Instantiate(new GameObject());
            Vector2Int startPosition = StaticDataManager.StoryPosition[indices[i]];
            Vector2Int endPosition = StaticDataManager.StoryPosition[indices[i + 1]];

            int horizontal = Math.Abs(endPosition.x - endPosition.x);
            int vertical = Math.Abs(endPosition.y - startPosition.y);
            int right = Math.Sign(endPosition.x - startPosition.x);
            int up = Math.Sign(endPosition.y - startPosition.y);

            for (int v = 1; v < vertical - 1; v++)
            { // vertical
                GameObject edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.Vertical),
                MainGameManager.Instance.GetWorldPosition(startPosition + up * v * Vector2Int.up), Quaternion.identity);
                edgeGO.transform.SetParent(masterGameObject.transform, false);

                if (edgeType == EdgeType.Children)
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(StaticDataManager.StoryDatas[indices[i]].character);
                }
                else
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(edgeType);
                }
                edgeGO.GetComponent<SpriteRenderer>().size = MainGameManager.Instance.gridSize * Vector2.one;
            }
            for (int h = 1; h < horizontal; h++)
            { // horizontal
                GameObject edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.Horizontal), MainGameManager.Instance.GetWorldPosition(endPosition - right * h * Vector2Int.right), Quaternion.identity);
                edgeGO.transform.SetParent(masterGameObject.transform, false);
                if (edgeType == EdgeType.Children)
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(StaticDataManager.StoryDatas[indices[i]].character);
                }
                else
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(edgeType);
                }
                edgeGO.GetComponent<SpriteRenderer>().size = MainGameManager.Instance.gridSize * Vector2.one;
            }
            if (right != 0 && up != 0)
            { // corner
                GameObject edgeGO = null;
                Vector2 cornerPosition = new Vector2(startPosition.x, endPosition.y);

                if (right < 0 && up < 0)
                {
                    edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.LeftUpCorner), cornerPosition, Quaternion.identity);
                }
                else if (right > 0 && up < 0)
                {
                    edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.RightUpCorner), cornerPosition, Quaternion.identity);
                }
                else if (right < 0 && up > 0)
                {
                    edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.LeftDownCorner), cornerPosition, Quaternion.identity);
                }
                else if (right > 0 && up > 0)
                {
                    edgeGO = Instantiate(EdgePrefab.Instance.GetEdgePrefab(EdgePrefab.EdgePrefabType.RightDownCorner), cornerPosition, Quaternion.identity);
                }

                if (edgeType == EdgeType.Children)
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(StaticDataManager.StoryDatas[indices[i]].character);
                }
                else
                {
                    edgeGO.GetComponent<SpriteRenderer>().color = ColorManager.GetColor(edgeType);
                }
                edgeGO.GetComponent<SpriteRenderer>().size = MainGameManager.Instance.gridSize * Vector2.one;
            }

            Edge tempEdge = new Edge() { startIndex = indices[i], endIndex = indices[i + 1], edgeType = edgeType, edgeGameObject = masterGameObject };
            edges.Add(tempEdge);
        }

        if (edges.Count == 1)
        {
            return edges[0];
        }
        else
        {
            return new EdgeCollection() { edges = edges };
        }
    }
}

public abstract class CompositeEdge
{

    public abstract bool this[int index] { get; }

    public abstract void SetActive(bool state);
    

}

public class Edge : CompositeEdge
{
    public int startIndex { get; set; }
    public int endIndex { get; set; }
    public EdgeType edgeType { get; set; }
    public GameObject edgeGameObject { get; set; }

    public override bool this[int index] => (index == startIndex || index == endIndex);
    public override void SetActive(bool state) => edgeGameObject.SetActive(state);
}

public class EdgeCollection : CompositeEdge
{
    public List<Edge> edges { get; set; }

    public override bool this[int index] => edges.Any(e => e[index]);
    public override void SetActive(bool state) => edges.ForEach(e => e.edgeGameObject.SetActive(state));
}

public class EdgePrefab : MonoBehaviour
{
    public static EdgePrefab Instance;

    public enum EdgePrefabType
    {
        Horizontal,
        Vertical,
        LeftUpCorner,
        LeftDownCorner,
        RightUpCorner,
        RightDownCorner
    }

    [EnumNamedArray(typeof(EdgePrefabType))]
    public GameObject[] edgePrefabs = new GameObject[Enum.GetNames(typeof(EdgePrefabType)).Length];

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetEdgePrefab(EdgePrefabType edgeType)
    {
        return edgePrefabs[(int)edgeType];
    }
}