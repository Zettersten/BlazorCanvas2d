namespace BlazorCanvas2d.Abstractions;

public abstract class CanvasManagerBase : ComponentBase
{
    protected readonly Dictionary<string, CanvasCreationOptions> _names = [];
    protected readonly Dictionary<string, ICanvas> _canvases = [];

    public void CreateCanvas(string name, CanvasCreationOptions options)
    {
        this._names.Add(name, options);

        this.StateHasChanged();
    }

    public bool RemoveCanvas(string name)
    {
        var removed = this._names.Remove(name);
        this._canvases.Remove(name);

        if (removed)
        {
            this.StateHasChanged();
        }

        return removed;
    }

    internal async ValueTask OnChildCanvasAddedAsync(ICanvas canvas)
    {
        // Capture the canvas so callers can access it by name if needed.
        // This also avoids relying on @ref with dictionary indexers.
        if (!string.IsNullOrWhiteSpace(canvas.Name))
        {
            this._canvases[canvas.Name] = canvas;
        }

        await this.OnCanvasAdded.InvokeAsync(canvas);
    }

    [Parameter]
    public EventCallback<ICanvas> OnCanvasAdded { get; set; }
}
