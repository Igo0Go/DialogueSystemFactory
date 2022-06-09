using System.Collections.Generic;

public interface IHavePreviousNodes
{
    List<int> PreviousNodeNumbers { get; set; }
    void RemoveThisNodeFromPrevious(int nodeForRemoving);
    void AddThisNodeInPrevious(int newNode);
}
