using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

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

    internal async ValueTask OnChildCanvasAddedAsync(ICanvas canvas)
    {
        await this.OnCanvasAdded.InvokeAsync(canvas);
    }

    [Parameter]
    public EventCallback<ICanvas> OnCanvasAdded { get; set; }
}
