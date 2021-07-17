using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RearrangementData
{
    [System.Serializable]
    public struct TextboxIndices
    {
        public int storyIndex { get; set; }
        public int textboxIndex { get; set; }
    }

    public int[] indices { get; set; }

    public Dictionary<int, List<TextboxIndices>> rearrangementTextboxIndices { get; set; }
}

[System.Serializable]
public class RearrangementDataList
{
    public List<RearrangementData> rearrangementDatas { get; set; }

    public RearrangementDataList()
    {
        rearrangementDatas = new List<RearrangementData>();
    }

    public RearrangementData this[int index]
    {
        get
        {
            foreach (RearrangementData rearrangement in rearrangementDatas)
            {
                if (rearrangement.indices.Contains(index))
                {
                    return rearrangement;
                }
            }
            return null;
        }
    }
}