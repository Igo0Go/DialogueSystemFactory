using System.Collections.Generic;

public interface IHaveNextNodes
{
    List<int> NextNodesNumbers { get; set; }
    void RemoveThisNodeFromPrevious(int nodeForRemoving);
}
