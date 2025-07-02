namespace BlazorCanvas2d.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    internal async ValueTask<T> DirectCall<T>(string method, params object[] args) =>
        await this._blazorexAPI.InvokeAsync<T>("directCall", this._id, method, args);

    async ValueTask IRenderContext.ProcessBatchAsync()
    {
        await this._blazorexAPI.InvokeVoidAsync("processBatch", this._id, this._jsOps);
        this._jsOps.Clear();
    }
}
