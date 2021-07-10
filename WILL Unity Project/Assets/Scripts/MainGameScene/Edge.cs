using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Edge : MonoBehaviour
{
    public static Edge Instance;

    public enum EdgeType
    {
        Horizontal,
        Vertical,
        LeftUpCorner,
        LeftDownCorner,
        RightUpCorner,
        RightDownCorner
    }

    [EnumNamedArray(typeof(EdgeType))]
    public GameObject[] edgePrefabs = new GameObject[Enum.GetNames(typeof(EdgeType)).Length];

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetEdgePrefab(EdgeType edgeType)
    {
        return edgePrefabs[(int) edgeType];
    }
}
