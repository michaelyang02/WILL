using System.Collections.Generic;
using System.Linq;

public static class StoryManager
{
    // key: the level at which the root node is introduced
    // value: list of story indices of root nodes
    public static Dictionary<int, List<int>> levelRootDict = new Dictionary<int, List<int>>()
    {
        {0, new List<int> {0}}
    };

    public static Dictionary<int, List<int>> childrenDict = null;

    public static void CheckAnyStoryDiscovered()
    {
        foreach (StoryData storyData in StaticDataManager.StoryDatas)
        {
            if (!StaticDataManager.StoryPlayerDatas[storyData.index].isDiscovered)
            {
                bool isDiscovered = true;
                foreach (StoryData.OutcomeIndices outcomeIndices in storyData.requiredDiscoveredOutcomes)
                {
                    if (!StaticDataManager.StoryPlayerDatas[outcomeIndices.storyIndex].outcomeDiscovered[outcomeIndices.outcomeIndex])
                    {
                        isDiscovered = false;
                        break;
                    }
                }
                StaticDataManager.StoryPlayerDatas[storyData.index].isDiscovered = isDiscovered;
            }
        }
    }

    // rule 1: any node is enabled iff parent node and all required nodes are enabled
    // rule 2: any requirement node must be at a lower level
    // rule 3: any companion node must be at the same level

    public static void CheckAnyEnabled()
    {
        // generate a children dict
        if (childrenDict == null)
        {
            childrenDict = new Dictionary<int, List<int>>();

            foreach (StoryData storyData in StaticDataManager.StoryDatas)
            {
                if (!childrenDict.ContainsKey(storyData.parentIndex))
                {
                    childrenDict.Add(storyData.parentIndex, new List<int>());
                }
                childrenDict[storyData.parentIndex].Add(storyData.index);
            }
        }

        int level = 0;
        Queue<int> queuedIndices = new Queue<int>();

        List<int[]> companionList = new List<int[]>();

        // add level 0 root nodes in
        levelRootDict[0].ToList().ForEach(r => queuedIndices.Enqueue(r));

        // use -1 as marker for end of level
        queuedIndices.Enqueue(-1);

        while (queuedIndices.Any())
        {
            int index = queuedIndices.Dequeue();

            if (index != -1)
            {
                StoryData storyData = StaticDataManager.StoryDatas[index];
                bool isEnabled = true;

                if (storyData.parentIndex >= 0 && !StaticDataManager.StoryPlayerDatas[storyData.parentIndex].isEnabled)
                { // check parent
                    isEnabled = false;
                }
                else if (storyData.requiredEnabledOutcomes.Any() && 
                storyData.requiredEnabledOutcomes.Any(oi => StaticDataManager.StoryPlayerDatas[oi.storyIndex].selectedOutcome != oi.outcomeIndex || 
                !StaticDataManager.StoryPlayerDatas[oi.storyIndex].outcomeEnabled[oi.outcomeIndex]))
                { // check any of the required outcomes is not enabled or not selected
                    isEnabled = false;
                }
                else if (storyData.disablingOutcomes.Any() && storyData.disablingOutcomes.Any(oi => StaticDataManager.StoryPlayerDatas[oi.storyIndex].selectedOutcome == oi.outcomeIndex && StaticDataManager.StoryPlayerDatas[oi.storyIndex].outcomeEnabled[oi.outcomeIndex]))
                { // check any of the disabling outcomes is selected and enabled
                    isEnabled = false;
                }
                
                StaticDataManager.StoryPlayerDatas[index].isEnabled = isEnabled;

                int[] indices = StaticDataManager.RearrangementDatas[index].indices;
                if (indices.Length > 1 && !companionList.Contains(indices))
                { // with companion
                    companionList.Add(indices);
                }
                
                // add children indices
                if (childrenDict.ContainsKey(index))
                {
                    childrenDict[index].ForEach(i => queuedIndices.Enqueue(i));
                }

                // outcomes of this story
                for (int outcomeIndex = 0; outcomeIndex < storyData.outcomes.Count; outcomeIndex++)
                {
                    if (!isEnabled)
                    { // story disabled
                        StaticDataManager.StoryPlayerDatas[index].outcomeEnabled[outcomeIndex] = false;
                    }
                    else
                    { // test all required outcomes enabled, true either not required or all enabled and selected AND either no disabling or all disabled or not selected
                        StaticDataManager.StoryPlayerDatas[index].outcomeEnabled[outcomeIndex] = 
                        (!storyData.outcomes[outcomeIndex].requiredOutcomes.Any() || 
                        storyData.outcomes[outcomeIndex].requiredOutcomes.All(oi => StaticDataManager.StoryPlayerDatas[oi.storyIndex].selectedOutcome == oi.outcomeIndex && StaticDataManager.StoryPlayerDatas[oi.storyIndex].outcomeEnabled[oi.outcomeIndex])) && (!storyData.outcomes[outcomeIndex].disablingOutcomes.Any() || storyData.outcomes[outcomeIndex].disablingOutcomes.All(oi => StaticDataManager.StoryPlayerDatas[oi.storyIndex].selectedOutcome != oi.outcomeIndex || !StaticDataManager.StoryPlayerDatas[oi.storyIndex].outcomeEnabled[oi.outcomeIndex]));
                    }
                }
            }
            else
            {
                // check companion nodes
                foreach (int[] indices in companionList)
                {
                    bool isAnyDisabled = false;
                    foreach (int i in indices)
                    {
                        if (!StaticDataManager.StoryPlayerDatas[i].isEnabled)
                        { // if any is disabled
                            isAnyDisabled = true;
                            break;
                        }
                    }

                    if (isAnyDisabled)
                    { // set all companions and their outcomes to false
                        foreach (int j in indices)
                        {
                            StaticDataManager.StoryPlayerDatas[j].isEnabled = false;
                            for (int o = 0; o < StaticDataManager.StoryPlayerDatas[j].outcomeEnabled.Count; o++)
                            {
                                StaticDataManager.StoryPlayerDatas[j].outcomeEnabled[o] = false;
                            }
                        }
                    }
                }

                companionList.Clear();

                // add next level root nodes
                if (levelRootDict.ContainsKey(++level))
                {
                    levelRootDict[level].ToList().ForEach(r => queuedIndices.Enqueue(r));
                }
                // add next marker
                if (queuedIndices.Any() || level <= levelRootDict.Keys.Max() )
                { // add only if there are any in queue or if there are more root nodes to add
                    queuedIndices.Enqueue(-1);
                }
            }
        }
    }
}