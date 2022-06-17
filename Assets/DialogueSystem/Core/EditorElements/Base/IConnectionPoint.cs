using System;

public interface IConnectionPoint : IDrawableElement, IEquatable<IConnectionPoint>
{
    int NodeIndex { get; }
    int PointIndex { get; }
    ConnectionPointType PointType { get; set; }
    Connection CurrentConnection { get; set; }

    Action<int> OnRemoveNext { get; set; }
    Action<int> OnRemovePrevoius { get; set; }
    Action<IConnectionPoint> OnClickConnectionPoint { get; set; }

    void UpdateData(Action<IConnectionPoint> OnClickConnectionPoint);

    /// <summary>
    /// Сохраняет в родительском элементе ссылку на узел
    /// </summary>
    /// <param name="saveNodeIndex">Индекс сохраняемого узла</param>
    void SaveReferenceToNode(int saveNodeIndex);

    /// <summary>
    /// Зачищает ссылку в родитеском элементе для указанного узла
    /// </summary>
    /// <param name="nodeReference">индекс узла, ссылку на который нужно зачистить</param>
    void ClearReferenceToNodeByValue(int nodeReference);
}
