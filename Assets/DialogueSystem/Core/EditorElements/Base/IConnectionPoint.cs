using System;

public interface IConnectionPoint : IDrawableElement, IEquatable<IConnectionPoint>
{
    ConnectionPointType PointType { get; set; }
    int NodeIndex { get; }
    Connection CurrentConnection { get; set; }
    Action<int> OnRemoveNext { get; set; }
    Action<int> OnRemovePrevoius { get; set; }
    Action<IConnectionPoint> OnClickConnectionPoint { get; set; }
    void UpdateDelegates(Action<IConnectionPoint> OnClickConnectionPoint);
}
