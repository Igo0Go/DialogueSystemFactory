public interface IHaveNextNodes
{
    /// <summary>
    /// Удалить ссылку на указанный узел из списка следующих
    /// </summary>
    /// <param name="indexOfNodeForRemoving">индекс удаляемого узла</param>
    void RemoveThisNodeFromNext(int indexOfNodeForRemoving);

    /// <summary>
    /// Зачистить выход к следующему узлу (значение = -1) по индексу выхода
    /// </summary>
    /// <param name="indexOfNextConnectionPoint">индекс выхода к следующему узлу, который должен
    /// стать выходом без связи (= -1)</param>
    void ClearNextByIndex(int indexOfNextConnectionPoint);

    /// <summary>
    /// Добавить ссылку на следующий узел в выход с указанным номером
    /// </summary>
    /// <param name="newNodeIndex">Индекс добавляемого узла</param>
    /// <param name="outPoinIndex">Индекс выхода</param>
    void AddThisNodeInNext(int newNodeIndex, int outPoinIndex);
}
