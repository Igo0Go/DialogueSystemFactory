using System.Collections.Generic;

public interface IHavePreviousNodes
{
    List<int> PreviousNodeNumbers { get; set; }
    void RemoveThisNodeFromNext(int nodeForRemoving);
}
