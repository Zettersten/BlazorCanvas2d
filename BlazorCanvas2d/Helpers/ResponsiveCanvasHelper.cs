namespace BlazorCanvas2d.Helpers;

/// <summary>
/// Helper class for managing responsive canvas dimensions and scaling.
/// </summary>
public static class ResponsiveCanvasHelper
{
    /// <summary>
    /// Calculates display dimensions that fit within specified constraints while maintaining aspect ratio.
    /// </summary>
    /// <param name="targetWidth">Desired canvas width.</param>
    /// <param name="targetHeight">Desired canvas height.</param>
    /// <param name="maxDisplayWidth">Maximum display width constraint.</param>
    /// <param name="maxDisplayHeight">Maximum display height constraint.</param>
    /// <returns>A tuple containing (displayWidth, displayHeight, scale).</returns>
    public static (int displayWidth, int displayHeight, float scale) CalculateDisplayDimensions(
        int targetWidth,
        int targetHeight,
        int maxDisplayWidth = 640,
        int maxDisplayHeight = 480
    )
    {
        var aspectRatio = (float)targetWidth / targetHeight;

        // If dimensions fit within limits, use them as-is
        if (targetWidth <= maxDisplayWidth && targetHeight <= maxDisplayHeight)
        {
            return (targetWidth, targetHeight, 1.0f);
        }

        int displayWidth,
            displayHeight;

        // Scale down proportionally to fit within limits
        if (aspectRatio > 1) // Width is larger
        {
            displayWidth = Math.Min(targetWidth, maxDisplayWidth);
            displayHeight = (int)(displayWidth / aspectRatio);

            // Ensure height doesn't exceed max
            if (displayHeight > maxDisplayHeight)
            {
                displayHeight = maxDisplayHeight;
                displayWidth = (int)(displayHeight * aspectRatio);
            }
        }
        else // Height is larger or square
        {
            displayHeight = Math.Min(targetHeight, maxDisplayHeight);
            displayWidth = (int)(displayHeight * aspectRatio);

            // Ensure width doesn't exceed max
            if (displayWidth > maxDisplayWidth)
            {
                displayWidth = maxDisplayWidth;
                displayHeight = (int)(displayWidth / aspectRatio);
            }
        }

        var scale = (float)displayWidth / targetWidth;
        return (displayWidth, displayHeight, scale);
    }

    /// <summary>
    /// Configuration for responsive canvas behavior.
    /// </summary>
    /// <param name="ActualWidth">The actual/target width for export.</param>
    /// <param name="ActualHeight">The actual/target height for export.</param>
    /// <param name="DisplayWidth">The width used for display.</param>
    /// <param name="DisplayHeight">The height used for display.</param>
    /// <param name="Scale">The scaling factor applied to display elements.</param>
    /// <param name="IsScaled">Whether the display is scaled down from actual dimensions.</param>
    public record ResponsiveCanvasConfig(
        int ActualWidth,
        int ActualHeight,
        int DisplayWidth,
        int DisplayHeight,
        float Scale,
        bool IsScaled
    )
    {
        /// <summary>
        /// Creates a responsive canvas configuration for the specified target dimensions.
        /// </summary>
        /// <param name="targetWidth">Target canvas width.</param>
        /// <param name="targetHeight">Target canvas height.</param>
        /// <param name="maxDisplayWidth">Maximum display width.</param>
        /// <param name="maxDisplayHeight">Maximum display height.</param>
        /// <returns>A configuration object with calculated dimensions and scaling.</returns>
        public static ResponsiveCanvasConfig Create(
            int targetWidth,
            int targetHeight,
            int maxDisplayWidth = 640,
            int maxDisplayHeight = 480
        )
        {
            var (displayWidth, displayHeight, scale) = CalculateDisplayDimensions(
                targetWidth,
                targetHeight,
                maxDisplayWidth,
                maxDisplayHeight
            );

            var isScaled = displayWidth != targetWidth || displayHeight != targetHeight;

            return new ResponsiveCanvasConfig(
                targetWidth,
                targetHeight,
                displayWidth,
                displayHeight,
                scale,
                isScaled
            );
        }

        /// <summary>
        /// Scales a coordinate value from actual space to display space.
        /// </summary>
        public float ScaleToDisplay(float actualValue) => actualValue * Scale;

        /// <summary>
        /// Scales a coordinate value from display space to actual space.
        /// </summary>
        public float ScaleToActual(float displayValue) => displayValue / Scale;
    }
}

/// <summary>
/// Extension methods for working with responsive canvas configurations.
/// </summary>
public static class ResponsiveCanvasExtensions
{
    /// <summary>
    /// Creates a canvas using responsive dimensions.
    /// </summary>
    /// <param name="canvasManager">The canvas manager.</param>
    /// <param name="name">Canvas name.</param>
    /// <param name="config">Responsive configuration.</param>
    /// <param name="onCanvasReady">Canvas ready callback.</param>
    /// <param name="onFrameReady">Frame ready callback.</param>
    /// <param name="additionalOptions">Additional canvas creation options.</param>
    public static void CreateResponsiveCanvas(
        this CanvasManager canvasManager,
        string name,
        ResponsiveCanvasHelper.ResponsiveCanvasConfig config,
        Action<ICanvas> onCanvasReady,
        Action<float>? onFrameReady = null,
        Action<CanvasCreationOptions>? additionalOptions = null
    )
    {
        var options = new CanvasCreationOptions
        {
            Hidden = false,
            Width = config.DisplayWidth,
            Height = config.DisplayHeight,
            OnCanvasReady = onCanvasReady,
            OnFrameReady = onFrameReady,
        };

        additionalOptions?.Invoke(options);
        canvasManager.CreateCanvas(name, options);
    }
}
