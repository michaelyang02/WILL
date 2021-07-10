using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridSystem<T>
{
    float gridSize;

    Dictionary<T, Vector2Int> elementCoordinates;

    public GridSystem(float gridSize)
    {
        elementCoordinates = new Dictionary<T, Vector2Int>();
        this.gridSize = gridSize;
    }

    public bool Add(T element, Vector2Int coordinate)
    {
        if (elementCoordinates.ContainsKey(element) || elementCoordinates.ContainsValue(coordinate))
        {
            return false;
        } else
        {
            elementCoordinates.Add(element, coordinate);
            return true;
        }
    }

    public bool Remove(T element)
    {
        return elementCoordinates.Remove(element);
    }

    public bool Remove(Vector2Int coordinate)
    {
        foreach (KeyValuePair<T, Vector2Int> keyValuePair in elementCoordinates)
        {
            if (keyValuePair.Value == coordinate)
            {
                return elementCoordinates.Remove(keyValuePair.Key);
            }
        }
        return false;
    }

    public Vector2 getWorldCoordinate(T element)
    {
        return (Vector2)elementCoordinates[element] * gridSize;
        // Throw exception if element does not exist
        // Does not check if element is in the dictionary!
    }

    public T getElement(Vector2Int coordinate)
    {
        foreach (KeyValuePair<T, Vector2Int> keyValuePair in elementCoordinates)
        {
            if (keyValuePair.Value == coordinate)
            {
                return keyValuePair.Key;
            }
        }
        return default(T);
    }

    public T getElement(Vector2 worldCoordinate)
    {
        int xOffset = (int)Math.Round(worldCoordinate.x / gridSize, 0);
        int yOffset = (int)Math.Round(worldCoordinate.y / gridSize, 0);

        return getElement(new Vector2Int(xOffset, yOffset));
    }
    
}
