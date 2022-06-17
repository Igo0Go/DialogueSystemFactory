public interface IHavePreviousNodes
{
    /// <summary>
    /// Удалить ссылку на указанный узел из числа предыдущих
    /// </summary>
    /// <param name="indexOfNodeForRemoving">индекс удаляемого узла</param>
    void RemoveThisNodeFromPrevious(int indexOfNodeForRemoving);

    /// <summary>
    /// Добавить ссылку на указанный узел в список предыдущих
    /// </summary>
    /// <param name="newNodeIndex">Индекс нового узла</param>
    void AddThisNodeInPrevious(int newNodeIndex);
}
