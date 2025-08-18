using BlazorCanvas2d.Renderer;

namespace BlazorCanvas2d;

internal sealed record CanvasGradient : CanvasProperty<int>, ICanvasGradient
{
    private readonly RenderContext2D _context;
    private readonly MarshalReference _marshalReference;

    internal CanvasGradient(RenderContext2D context, MarshalReference marshalReference)
        : base(int.Parse(marshalReference.Id))
    {
        this._context = context;
        this._marshalReference = marshalReference;
    }

    public ICanvasGradient AddColorStop(float offset, string color)
    {
        this._context.Call("addColorStop", [this._marshalReference, offset, color]);

        return this;
    }
}
