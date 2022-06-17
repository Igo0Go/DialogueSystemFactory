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
    /// ��������� � ������������ �������� ������ �� ����
    /// </summary>
    /// <param name="saveNodeIndex">������ ������������ ����</param>
    void SaveReferenceToNode(int saveNodeIndex);

    /// <summary>
    /// �������� ������ � ���������� �������� ��� ���������� ����
    /// </summary>
    /// <param name="nodeReference">������ ����, ������ �� ������� ����� ���������</param>
    void ClearReferenceToNodeByValue(int nodeReference);
}
