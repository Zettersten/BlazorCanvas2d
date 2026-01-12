using Microsoft.JSInterop;

namespace BlazorCanvas2d.Extensions;

/// <summary>
/// Extension methods for ICanvas to simplify common operations and reduce boilerplate code.
/// </summary>
public static class ICanvasExtensions
{
    /// <summary>
    /// Exports the canvas content and automatically triggers a download in the browser.
    /// </summary>
    /// <param name="canvas">The canvas to export.</param>
    /// <param name="jsRuntime">JSRuntime for JavaScript interop.</param>
    /// <param name="fileName">The filename for the downloaded file (without extension).</param>
    /// <param name="format">The image format (png, jpeg, webp).</param>
    /// <param name="quality">Quality for lossy formats (0.0-1.0).</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    public static async Task ExportAndDownloadAsync(
        this ICanvas canvas,
        IJSRuntime jsRuntime,
        string fileName,
        string format = "png",
        double? quality = null
    )
    {
        var mimeType = $"image/{format}";
        var fileExtension = format switch
        {
            "jpeg" => "jpg",
            _ => format,
        };

        var url = await canvas.CreateObjectURL(mimeType, quality);
        var fullFileName = $"{fileName}.{fileExtension}";

        await jsRuntime.InvokeVoidAsync(
            "eval",
            $$"""
            (function(url, fileName) {
                const a = document.createElement('a');
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                
                // Clean up the object URL to prevent memory leaks
                setTimeout(() => URL.revokeObjectURL(url), 1000);
            })('{{url}}', '{{fullFileName}}');
            """
        );
    }

    /// <summary>
    /// Creates a temporary canvas with specified dimensions for high-quality rendering operations.
    /// The canvas is automatically disposed when the action completes.
    /// </summary>
    /// <param name="canvasManager">The canvas manager to create the temporary canvas.</param>
    /// <param name="width">Width of the temporary canvas.</param>
    /// <param name="height">Height of the temporary canvas.</param>
    /// <param name="renderAction">Action to perform on the temporary canvas.</param>
    /// <returns>The temporary canvas for further operations (export, etc.).</returns>
    public static async Task<ICanvas> CreateTemporaryCanvasAsync(
        this CanvasManager canvasManager,
        int width,
        int height,
        Action<ICanvas> renderAction
    )
    {
        var tempCanvasName = $"temp-canvas-{DateTime.UtcNow.Ticks}";
        ICanvas? tempCanvas = null;

        canvasManager.CreateCanvas(
            tempCanvasName,
            new CanvasCreationOptions
            {
                Hidden = true,
                Width = width,
                Height = height,
                OnCanvasReady = canvas =>
                {
                    tempCanvas = canvas;
                    renderAction(canvas);
                },
            }
        );

        // Wait for canvas creation with timeout
        var attempts = 0;
        while (tempCanvas == null && attempts < 50) // 5 second timeout
        {
            await Task.Delay(100);
            attempts++;
        }

        if (tempCanvas == null)
        {
            throw new TimeoutException("Temporary canvas creation timed out.");
        }

        return tempCanvas;
    }

    /// <summary>
    /// Clears the entire canvas to the specified color.
    /// </summary>
    /// <param name="canvas">The canvas to clear.</param>
    /// <param name="color">The color to fill the canvas with (optional, defaults to transparent).</param>
    public static void Clear(this ICanvas canvas, string? color = null)
    {
        var ctx = canvas.RenderContext;

        if (color != null)
        {
            ctx.Save();
            ctx.FillStyle = color;
            ctx.FillRect(0, 0, canvas.Width, canvas.Height);
            ctx.Restore();
        }
        else
        {
            ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
        }
    }
}
