using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RearrangementPlayerData
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