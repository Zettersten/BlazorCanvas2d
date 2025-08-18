using Microsoft.JSInterop;

namespace BlazorCanvas2d.Extensions;

/// <summary>
/// Extension methods for CanvasManager to simplify common canvas management operations.
/// </summary>
public static class CanvasManagerExtensions
{
    /// <summary>
    /// Creates a temporary canvas, performs rendering operations, exports the result, and cleans up automatically.
    /// </summary>
    /// <param name="canvasManager">The canvas manager.</param>
    /// <param name="jsRuntime">JSRuntime for download operations.</param>
    /// <param name="width">Width of the export canvas.</param>
    /// <param name="height">Height of the export canvas.</param>
    /// <param name="fileName">Name of the downloaded file (without extension).</param>
    /// <param name="renderAction">Action to perform the rendering operations.</param>
    /// <param name="format">Image format (png, jpeg, webp).</param>
    /// <param name="quality">Quality for lossy formats (0.0-1.0).</param>
    /// <returns>A task representing the export operation.</returns>
    public static async Task RenderAndExportAsync(
        this CanvasManager canvasManager,
        IJSRuntime jsRuntime,
        int width,
        int height,
        string fileName,
        Action<ICanvas> renderAction,
        string format = "png",
        double? quality = null)
    {
        ICanvas? exportCanvas = null;
        
        try
        {
            exportCanvas = await canvasManager.CreateTemporaryCanvasAsync(width, height, renderAction);
            await exportCanvas.ExportAndDownloadAsync(jsRuntime, fileName, format, quality);
        }
        finally
        {
            if (exportCanvas != null)
            {
                await exportCanvas.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Creates a scaled version of the main canvas content and exports it at a different resolution.
    /// </summary>
    /// <param name="canvasManager">The canvas manager.</param>
    /// <param name="jsRuntime">JSRuntime for download operations.</param>
    /// <param name="sourceCanvas">The source canvas to scale from.</param>
    /// <param name="exportWidth">Width of the export canvas.</param>
    /// <param name="exportHeight">Height of the export canvas.</param>
    /// <param name="fileName">Name of the downloaded file.</param>
    /// <param name="additionalRenderAction">Additional rendering to perform on the export canvas.</param>
    /// <param name="format">Image format.</param>
    /// <param name="quality">Quality for lossy formats.</param>
    /// <returns>A task representing the export operation.</returns>
    public static async Task ExportScaledAsync(
        this CanvasManager canvasManager,
        IJSRuntime jsRuntime,
        ICanvas sourceCanvas,
        int exportWidth,
        int exportHeight,
        string fileName,
        Action<ICanvas, float>? additionalRenderAction = null,
        string format = "png",
        double? quality = null)
    {
        var scaleX = (float)exportWidth / sourceCanvas.Width;
        var scaleY = (float)exportHeight / sourceCanvas.Height;

        await canvasManager.RenderAndExportAsync(
            jsRuntime,
            exportWidth,
            exportHeight,
            fileName,
            exportCanvas =>
            {
                var ctx = exportCanvas.RenderContext;
                
                // Copy the source canvas content scaled to the export dimensions
                ctx.Save();
                ctx.Scale(scaleX, scaleY);
                
                // Note: This would need additional implementation to copy from source canvas
                // For now, we'll call the additional render action if provided
                additionalRenderAction?.Invoke(exportCanvas, Math.Min(scaleX, scaleY));
                
                ctx.Restore();
            },
            format,
            quality);
    }

    /// <summary>
    /// Creates multiple export canvases with different sizes from the same rendering operations.
    /// Useful for generating multiple resolution versions of the same content.
    /// </summary>
    /// <param name="canvasManager">The canvas manager.</param>
    /// <param name="jsRuntime">JSRuntime for download operations.</param>
    /// <param name="sizes">Array of (width, height, suffix) tuples for different export sizes.</param>
    /// <param name="baseFileName">Base filename (size suffix will be appended).</param>
    /// <param name="renderAction">Action to perform the rendering operations.</param>
    /// <param name="format">Image format.</param>
    /// <param name="quality">Quality for lossy formats.</param>
    /// <returns>A task representing all export operations.</returns>
    public static async Task ExportMultipleSizesAsync(
        this CanvasManager canvasManager,
        IJSRuntime jsRuntime,
        (int width, int height, string suffix)[] sizes,
        string baseFileName,
        Action<ICanvas, float> renderAction,
        string format = "png",
        double? quality = null)
    {
        var tasks = sizes.Select(async size =>
        {
            var scale = Math.Min((float)size.width / 500f, (float)size.height / 500f); // Assume 500x500 base
            var fileName = $"{baseFileName}_{size.suffix}_{size.width}x{size.height}";
            
            await canvasManager.RenderAndExportAsync(
                jsRuntime,
                size.width,
                size.height,
                fileName,
                canvas => renderAction(canvas, scale),
                format,
                quality);
        });

        await Task.WhenAll(tasks);
    }
}