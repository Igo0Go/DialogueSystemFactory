using System.Collections.Generic;

public interface IHaveNextNodes
{
    List<int> NextNodesNumbers { get; set; }
    void RemoveThisNodeFromNext(int nodeForRemoving);
    void AddThisNodeInNext(int newNode, int outPoinIndex);
}
