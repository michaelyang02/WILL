using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RearrangementData
{
    [System.Serializable]
    public struct TextboxIndices : IEquatable<TextboxIndices>
    {
        public int storyIndex { get; set; }
        public int textboxIndex { get; set; }

        public bool Equals(TextboxIndices other)
        => other.storyIndex == storyIndex && other.textboxIndex == textboxIndex;
    }

    public int[] indices { get; set; }

    public Dictionary<int, List<TextboxIndices>> rearrangementTextboxIndices { get; set; }
}

[System.Serializable]
public class RearrangementDataList : List<RearrangementData>
{
    new public RearrangementData this[int index]
    {
        get
        {
            foreach (RearrangementData rearrangementData in this)
            {
                if (rearrangementData.indices.Contains(index))
                {
                    return rearrangementData;
                }
            }
            return null;
        }
    }
}