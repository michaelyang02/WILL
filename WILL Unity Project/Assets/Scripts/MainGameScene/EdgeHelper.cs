using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EdgeHelper : MonoBehaviour
{
    public Vector2 size { get; set; }

    public static EdgeHelper Instance;

    public GameObject[] edgeUndottedPrefabs;
    public GameObject[] edgeDottedPrefabs;

    public void Awake()
    {
        Instance = this;
    }

    public CompositeEdge GenerateEdge(EdgeType edgeType, params int[] indices)
    {
        List<Edge> edges = new List<Edge>();
        GameObject[] edgePrefabs;
        Color color;


        if (edgeType == EdgeType.Enable || edgeType == EdgeType.Disable)
        {
            edgePrefabs = edgeDottedPrefabs;
        }
        else
        {
            edgePrefabs = edgeUndottedPrefabs;
        }

        if (edgeType == EdgeType.Children)
        {
            if (!StaticDataManager.StoryPlayerDatas[indices[0]].isEnabled)
            { // if child disabled
                color = ColorManager.GetColor(EdgeType.Disable);
            }
            else
            {
                color = ColorManager.GetColor(StaticDataManager.StoryDatas[indices[0]].character);
            }
        }
        else
        {
            color = ColorManager.GetColor(edgeType);
        }

        for (int i = 0; i < indices.Length - 1; i++)
        {
            GameObject masterGameObject = new GameObject();
            if (edgeType != EdgeType.Children) masterGameObject.SetActive(false);
            Vector2Int startPosition = StaticDataManager.StoryPosition[indices[i]];
            Vector2Int endPosition = StaticDataManager.StoryPosition[indices[i + 1]];

            int horizontal = Math.Abs(endPosition.x - startPosition.x);
            int vertical = Math.Abs(endPosition.y - startPosition.y);
            int right = Math.Sign(endPosition.x - startPosition.x);
            int up = Math.Sign(endPosition.y - startPosition.y);

            for (int h = 1; h < horizontal; h++)
            { // horizontal
                GameObject edgeGO = Instantiate(edgePrefabs[0], MainGameManager.Instance.GetWorldPosition(endPosition - right * h * Vector2Int.right), Quaternion.identity);
                edgeGO.transform.SetParent(masterGameObject.transform, false);
                edgeGO.GetComponent<SpriteRenderer>().color = color;
                edgeGO.GetComponent<SpriteRenderer>().size = size;
            }
            for (int v = 1; v < vertical; v++)
            { // vertical
                GameObject edgeGO = Instantiate(edgePrefabs[1],
                MainGameManager.Instance.GetWorldPosition(startPosition + up * v * Vector2Int.up), Quaternion.identity);
                edgeGO.transform.SetParent(masterGameObject.transform, false);
                edgeGO.GetComponent<SpriteRenderer>().color = color;
                edgeGO.GetComponent<SpriteRenderer>().size = size; ;
            }
            if (right != 0 && up != 0)
            { // corner
                Vector2 cornerPosition = MainGameManager.Instance.GetWorldPosition(new Vector2Int(startPosition.x, endPosition.y));

                GameObject edgeGO = Instantiate(edgePrefabs[2], cornerPosition, Quaternion.identity);
                if (right > 0 && up > 0)
                {
                    edgeGO.transform.Rotate(0f, 0f, 0f);
                }
                else if (right > 0 && up < 0)
                {
                    edgeGO.transform.Rotate(0f, 0f, 90f);
                }
                else if (right < 0 && up > 0)
                {
                    edgeGO.transform.Rotate(0f, 0f, 270f);

                }
                else if (right < 0 && up < 0)
                {
                    edgeGO.transform.Rotate(0f, 0f, 180f);

                }

                edgeGO.transform.SetParent(masterGameObject.transform, false);
                edgeGO.GetComponent<SpriteRenderer>().color = color;
                edgeGO.GetComponent<SpriteRenderer>().size = size;
            }

            Edge tempEdge = new Edge() { startIndex = indices[i], endIndex = indices[i + 1], edgeType = edgeType, edgeGameObject = masterGameObject, character = (edgeType == EdgeType.Children) ? StaticDataManager.StoryDatas[indices[0]].character : StoryData.Character.None };
            edges.Add(tempEdge);
        }

        if (edges.Count == 1)
        {
            return edges[0];
        }
        else
        {
            GameObject gameObject = new GameObject();
            edges.ForEach(e => e.edgeGameObject.transform.SetParent(gameObject.transform));
            return new EdgeCollection() { edges = edges, character = StoryData.Character.None, edgeGameObject = gameObject};
        }
    }
}

public enum EdgeType
{
    Children = -1,
    Companion = ColorManager.EdgeColor.VividTangerine,
    Enable = ColorManager.EdgeColor.MintGreen,
    Disable = ColorManager.EdgeColor.Black
}

public abstract class CompositeEdge
{
    public abstract bool this[int index] { get; }
    public StoryData.Character character { get; set; }
    public GameObject edgeGameObject { get; set; }
    public abstract void SetActive(bool state);
}

public class Edge : CompositeEdge
{
    public int startIndex { get; set; }
    public int endIndex { get; set; }
    public EdgeType edgeType { get; set; }

    public bool isDiscovered() => StaticDataManager.StoryPlayerDatas[startIndex].isDiscovered && StaticDataManager.StoryPlayerDatas[endIndex].isDiscovered;

    public override bool this[int index] => (index == startIndex || index == endIndex) && isDiscovered();
    public override void SetActive(bool state) => edgeGameObject.SetActive(state);
}

public class EdgeCollection : CompositeEdge
{
    public List<Edge> edges { get; set; }

    public override bool this[int index] => edges.Any(e => e[index]);
    public override void SetActive(bool state) => edges.Where(e => e.isDiscovered()).ToList().ForEach(e => e.SetActive(state));
}