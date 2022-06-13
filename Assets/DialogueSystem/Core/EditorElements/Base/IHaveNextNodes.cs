using System.Collections.Generic;

public interface IHaveNextNodes
{
    void RemoveThisNodeFromNext(int nodeForRemoving);
    void AddThisNodeInNext(int newNode, int outPoinIndex);
}
