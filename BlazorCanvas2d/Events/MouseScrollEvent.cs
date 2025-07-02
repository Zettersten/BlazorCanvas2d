using BlazorCanvas2d.Serialization;

namespace BlazorCanvas2d;

[JsonConverter(typeof(MouseScrollEventJsonConverter))]
public readonly struct MouseScrollEvent(
    double clientX,
    double clientY,
    double deltaX,
    double deltaY
)
{
    public readonly double DeltaX = deltaX;

    public readonly double DeltaY = deltaY;

    public readonly double ClientX = clientX;

    public readonly double ClientY = clientY;

    public static readonly MouseScrollEvent Zero = new(0, 0, 0, 0);
}
