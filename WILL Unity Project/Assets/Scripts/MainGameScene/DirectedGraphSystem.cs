using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DirectedGraphSystem<T>
{
    Dictionary<T, HashSet<T>> adjacencyList = new Dictionary<T, HashSet<T>>();

    public bool Add(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList.Add(node, null);
            return true;
        }
        else { return false; }
    }

    public bool Remove(T node)
    {
        if (adjacencyList.Remove(node))
        {
            foreach (HashSet<T> set in adjacencyList.Values) if (set != null)
                {
                    set.Remove(node);
                }
            return true;
        }
        else
        {
            return false;
        }
    }

    public int Count()
    {
        return adjacencyList.Count;
    }

    public bool AddChildNode(T parent, T child)
    {
        if (!adjacencyList.ContainsKey(parent))
        {
            adjacencyList.Add(parent, null);
        }
        if (!adjacencyList.ContainsKey(child))
        {
            adjacencyList.Add(child, null);
        }
        if (adjacencyList[parent] == null)
        {
            adjacencyList[parent] = new HashSet<T>();
        }

        return adjacencyList[parent].Add(child);
    }

    public bool RemoveChildNode(T parent, T child)
    {
        if (!adjacencyList.ContainsKey(parent) || adjacencyList[parent] == null)
        {
            return false;
        }
        else
        {
            return adjacencyList[parent].Remove(child);
        }
    }

    public bool Contains(T node)
    {
        return adjacencyList.ContainsKey(node);        
    }
}
