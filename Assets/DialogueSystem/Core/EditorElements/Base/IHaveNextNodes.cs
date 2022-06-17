using System.Collections.Generic;

public interface IHaveNextNodes
{
    void RemoveThisNodeFromNext(int nodeForRemoving);
    void ClearNextByIndex(int indexOfNextConnectionPoint);
    void AddThisNodeInNext(int newNode, int outPoinIndex);
}
