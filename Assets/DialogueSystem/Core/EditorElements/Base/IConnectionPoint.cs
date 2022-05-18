using System;

public interface IConnectionPoint : IDrawableElement, IEquatable<IConnectionPoint>
{
    ConnectionPointType PointType { get; set; }
    Action<IConnectionPoint> OnClickConnectionPoint { get; set; }
}
