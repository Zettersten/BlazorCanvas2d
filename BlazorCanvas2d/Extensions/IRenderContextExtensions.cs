using Microsoft.AspNetCore.Components;

namespace BlazorCanvas2d.Extensions;

/// <summary>
/// Extension methods for IRenderContext to provide higher-level drawing operations and reduce boilerplate.
/// </summary>
public static class IRenderContextExtensions
{
    /// <summary>
    /// Draws an image centered at the specified coordinates with optional scaling while maintaining aspect ratio.
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="imageRef">Reference to the image element.</param>
    /// <param name="centerX">X coordinate of the center point.</param>
    /// <param name="centerY">Y coordinate of the center point.</param>
    /// <param name="maxWidth">Maximum width (maintains aspect ratio).</param>
    /// <param name="maxHeight">Maximum height (maintains aspect ratio).</param>
    /// <param name="rotation">Rotation angle in radians (optional).</param>
    /// <param name="flipHorizontal">Whether to flip the image horizontally.</param>
    public static void DrawImageCentered(
        this IRenderContext ctx,
        ElementReference imageRef,
        float centerX,
        float centerY,
        float maxWidth,
        float maxHeight,
        float rotation = 0f,
        bool flipHorizontal = false
    )
    {
        ctx.Save();

        ctx.Translate(centerX, centerY);

        if (rotation != 0f)
            ctx.Rotate(rotation);

        if (flipHorizontal)
            ctx.Scale(-1f, 1f);

        // Draw image centered at origin
        ctx.DrawImage(imageRef, -maxWidth / 2f, -maxHeight / 2f, maxWidth, maxHeight);

        ctx.Restore();
    }

    /// <summary>
    /// Draws an image with automatic aspect ratio preservation and centering.
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="imageRef">Reference to the image element.</param>
    /// <param name="originalWidth">Original width of the image.</param>
    /// <param name="originalHeight">Original height of the image.</param>
    /// <param name="centerX">X coordinate of the center point.</param>
    /// <param name="centerY">Y coordinate of the center point.</param>
    /// <param name="targetSize">Target size (width or height, whichever is larger).</param>
    /// <param name="rotation">Rotation angle in radians (optional).</param>
    /// <param name="flipHorizontal">Whether to flip the image horizontally.</param>
    public static void DrawImageCenteredWithAspectRatio(
        this IRenderContext ctx,
        ElementReference imageRef,
        float originalWidth,
        float originalHeight,
        float centerX,
        float centerY,
        float targetSize,
        float rotation = 0f,
        bool flipHorizontal = false
    )
    {
        var aspectRatio = originalWidth / originalHeight;
        float scaledWidth,
            scaledHeight;

        if (aspectRatio > 1) // Width is greater than height
        {
            scaledWidth = targetSize;
            scaledHeight = targetSize / aspectRatio;
        }
        else // Height is greater than or equal to width
        {
            scaledHeight = targetSize;
            scaledWidth = targetSize * aspectRatio;
        }

        ctx.DrawImageCentered(
            imageRef,
            centerX,
            centerY,
            scaledWidth,
            scaledHeight,
            rotation,
            flipHorizontal
        );
    }

    /// <summary>
    /// Draws text with optional shadow and multi-line support.
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="text">The text to draw (supports multi-line with \n).</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="font">Font specification (e.g., "24px Arial").</param>
    /// <param name="fillColor">Fill color for the text.</param>
    /// <param name="textAlign">Text alignment.</param>
    /// <param name="textBaseline">Text baseline.</param>
    /// <param name="shadow">Optional shadow configuration.</param>
    /// <param name="lineHeightMultiplier">Line height multiplier for multi-line text.</param>
    public static void DrawStyledText(
        this IRenderContext ctx,
        string text,
        float x,
        float y,
        string font,
        string fillColor,
        TextAlign? textAlign = null,
        TextBaseline? textBaseline = null,
        TextShadow? shadow = null,
        float lineHeightMultiplier = 1.2f
    )
    {
        ctx.Save();

        ctx.Font = font;
        ctx.FillStyle = fillColor;

        if (textAlign != null)
            ctx.TextAlign = textAlign;

        if (textBaseline != null)
            ctx.TextBaseline = textBaseline;

        // Apply shadow if specified
        if (shadow != null)
        {
            ctx.ShadowColor = shadow.Color;
            ctx.ShadowBlur = shadow.Blur;
            ctx.ShadowOffsetX = shadow.OffsetX;
            ctx.ShadowOffsetY = shadow.OffsetY;
        }

        // Handle multi-line text
        var lines = text.Replace("\r", "").Split('\n');
        if (lines.Length == 1)
        {
            ctx.FillText(text, x, y);
        }
        else
        {
            var fontSize = ExtractFontSize(font);
            var lineHeight = fontSize * lineHeightMultiplier;
            var startY = y - (lineHeight * (lines.Length - 1)) / 2f;

            for (int i = 0; i < lines.Length; i++)
            {
                ctx.FillText(lines[i], x, startY + i * lineHeight);
            }
        }

        ctx.Restore();
    }

    /// <summary>
    /// Draws a background image that covers the entire canvas area (like CSS background-size: cover).
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="imageRef">Reference to the background image.</param>
    /// <param name="imageWidth">Original width of the background image.</param>
    /// <param name="imageHeight">Original height of the background image.</param>
    /// <param name="canvasWidth">Width of the canvas.</param>
    /// <param name="canvasHeight">Height of the canvas.</param>
    public static void DrawBackgroundImageCover(
        this IRenderContext ctx,
        ElementReference imageRef,
        float imageWidth,
        float imageHeight,
        float canvasWidth,
        float canvasHeight
    )
    {
        var canvasAspectRatio = canvasWidth / canvasHeight;
        var imageAspectRatio = imageWidth / imageHeight;

        float scaledWidth,
            scaledHeight;
        float offsetX = 0,
            offsetY = 0;

        // Scale to cover the canvas (like CSS background-size: cover)
        if (imageAspectRatio > canvasAspectRatio)
        {
            // Image is wider than canvas ratio - scale to fit height, crop width
            scaledHeight = canvasHeight;
            scaledWidth = scaledHeight * imageAspectRatio;
            offsetX = (canvasWidth - scaledWidth) / 2f; // Center horizontally
        }
        else
        {
            // Image is taller than canvas ratio - scale to fit width, crop height
            scaledWidth = canvasWidth;
            scaledHeight = scaledWidth / imageAspectRatio;
            offsetY = (canvasHeight - scaledHeight) / 2f; // Center vertically
        }

        ctx.DrawImage(imageRef, offsetX, offsetY, scaledWidth, scaledHeight);
    }

    /// <summary>
    /// Applies a scaling transformation to all subsequent drawing operations.
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="scale">The scaling factor to apply.</param>
    /// <param name="action">The drawing operations to perform with scaling applied.</param>
    public static void WithScale(this IRenderContext ctx, float scale, Action action)
    {
        ctx.Save();
        ctx.Scale(scale, scale);
        action();
        ctx.Restore();
    }

    /// <summary>
    /// Executes drawing operations with a temporary transformation applied.
    /// </summary>
    /// <param name="ctx">The rendering context.</param>
    /// <param name="transform">The transformation to apply.</param>
    /// <param name="action">The drawing operations to perform.</param>
    public static void WithTransform(
        this IRenderContext ctx,
        Action<IRenderContext> transform,
        Action action
    )
    {
        ctx.Save();
        transform(ctx);
        action();
        ctx.Restore();
    }

    private static float ExtractFontSize(string font)
    {
        // Simple font size extraction - looks for pattern like "24px" or "1.5em"
        var parts = font.Split(' ');
        foreach (var part in parts)
        {
            if (part.EndsWith("px") && float.TryParse(part[..^2], out var size))
                return size;
        }
        return 16f; // Default font size
    }
}

/// <summary>
/// Configuration for text shadow effects.
/// </summary>
/// <param name="Color">Shadow color (e.g., "rgba(0,0,0,0.7)").</param>
/// <param name="Blur">Shadow blur radius.</param>
/// <param name="OffsetX">Horizontal shadow offset.</param>
/// <param name="OffsetY">Vertical shadow offset.</param>
public record TextShadow(string Color, float Blur, float OffsetX, float OffsetY)
{
    /// <summary>
    /// Creates a standard drop shadow.
    /// </summary>
    public static TextShadow DropShadow(float blur = 4f, float offsetY = 2f) =>
        new("rgba(0,0,0,0.7)", blur, 0f, offsetY);
}
