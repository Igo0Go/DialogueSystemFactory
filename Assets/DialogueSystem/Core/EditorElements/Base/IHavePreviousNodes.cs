public interface IHavePreviousNodes
{
    void RemoveThisNodeFromPrevious(int nodeForRemoving);
    void AddThisNodeInPrevious(int newNode);
}
