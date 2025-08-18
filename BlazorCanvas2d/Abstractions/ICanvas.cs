namespace BlazorCanvas2d;

/// <summary>
/// Canvas abstraction for Blazor C#.
/// Provides comprehensive 2D rendering capabilities with optimized browser integration.
/// </summary>
public interface ICanvas : IAsyncDisposable
{
    /// <summary>
    /// Gets the canvas width in pixels.
    /// </summary>
    /// <value>The horizontal dimension of the canvas rendering area.</value>
    int Width { get; }

    /// <summary>
    /// Gets the canvas height in pixels.
    /// </summary>
    /// <value>The vertical dimension of the canvas rendering area.</value>
    int Height { get; }

    /// <summary>
    /// Gets the unique identifier for this canvas instance.
    /// </summary>
    /// <value>A string used for canvas identification and DOM element retrieval.</value>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the canvas supports alpha transparency.
    /// </summary>
    /// <value>
    /// <c>true</c> if the canvas includes an alpha channel for transparency;
    /// <c>false</c> to optimize opaque rendering performance by eliminating alpha blending overhead.
    /// </value>
    /// <remarks>
    /// Setting to <c>false</c> enables browser optimizations for opaque content rendering,
    /// improving performance when transparency is not required.
    /// </remarks>
    bool Alpha { get; }

    /// <summary>
    /// Gets the color space configuration for rendering operations.
    /// </summary>
    /// <value>
    /// The color space used for all rendering contexts. Supports sRGB (default) and Display P3
    /// for wide-gamut color reproduction on compatible displays.
    /// </value>
    /// <remarks>
    /// Display P3 provides enhanced color accuracy on modern displays but may impact performance
    /// on older hardware. sRGB ensures maximum compatibility across all devices.
    /// </remarks>
    ColorSpace ColorSpace { get; }

    /// <summary>
    /// Gets a value indicating whether canvas rendering is desynchronized from the main event loop.
    /// </summary>
    /// <value>
    /// <c>true</c> to enable low-latency rendering by decoupling canvas updates from browser paint cycles;
    /// <c>false</c> for standard synchronized rendering behavior.
    /// </value>
    /// <remarks>
    /// Desynchronization reduces input lag for real-time applications but may cause visual artifacts
    /// in some scenarios. Optimal for games and interactive graphics applications.
    /// </remarks>
    bool Desynchronized { get; }

    /// <summary>
    /// Gets a value indicating whether frequent pixel data read operations are expected.
    /// </summary>
    /// <value>
    /// <c>true</c> to optimize for frequent <c>GetImageData()</c> calls by using software rendering;
    /// <c>false</c> to prioritize hardware acceleration for drawing operations.
    /// </value>
    /// <remarks>
    /// Software rendering reduces memory allocation overhead for applications that frequently
    /// read pixel data but sacrifices GPU acceleration benefits for drawing operations.
    /// </remarks>
    bool WillReadFrequently { get; }

    /// <summary>
    /// Gets the 2D rendering context for drawing operations.
    /// </summary>
    /// <value>The context interface providing access to all canvas drawing methods and properties.</value>
    IRenderContext RenderContext { get; }

    /// <summary>
    /// Gets the underlying DOM element reference for direct browser API access.
    /// </summary>
    /// <value>The Blazor element reference enabling JavaScript interop operations.</value>
    ElementReference ElementReference { get; }

    /// <summary>
    /// Resizes the canvas to the specified dimensions and resets the rendering context.
    /// </summary>
    /// <param name="width">The new canvas width in pixels. Must be positive.</param>
    /// <param name="height">The new canvas height in pixels. Must be positive.</param>
    /// <remarks>
    /// This operation clears all canvas content and resets drawing state including transforms,
    /// styles, and clipping regions. Buffer any required content before resizing.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="width"/> or <paramref name="height"/> is less than or equal to zero.
    /// </exception>
    void Resize(int width, int height);

    /// <summary>
    /// Asynchronously converts the canvas content to a binary blob.
    /// </summary>
    /// <param name="type">
    /// The MIME type for the output format. Defaults to "image/png".
    /// Supported formats include "image/png", "image/jpeg", and "image/webp".
    /// </param>
    /// <param name="quality">
    /// Optional quality parameter for lossy formats (0.0 to 1.0).
    /// Only applies to JPEG and WebP formats. Higher values produce better quality and larger file sizes.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Blob}"/> representing the asynchronous operation.
    /// The result is a <see cref="Blob"/> containing the encoded image data.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported MIME type is specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="quality"/> is not between 0.0 and 1.0.
    /// </exception>
    ValueTask<BlobData> ToBlob(string type = "image/png", double? quality = null);

    /// <summary>
    /// Asynchronously converts the canvas content to a base64-encoded data URL.
    /// </summary>
    /// <param name="type">
    /// The MIME type for the output format. Defaults to "image/png".
    /// Supported formats include "image/png", "image/jpeg", and "image/webp".
    /// </param>
    /// <param name="quality">
    /// Optional quality parameter for lossy formats (0.0 to 1.0).
    /// Only applies to JPEG and WebP formats. Higher values produce better quality and larger file sizes.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{String}"/> representing the asynchronous operation.
    /// The result is a data URL string suitable for direct use in HTML img src attributes.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported MIME type is specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="quality"/> is not between 0.0 and 1.0.
    /// </exception>
    /// <remarks>
    /// Data URLs embed the entire image as base64 text, making them suitable for small images
    /// but inefficient for large graphics due to size overhead.
    /// </remarks>
    ValueTask<string> ToDataUrl(string type = "image/png", double? quality = null);

    /// <summary>
    /// Asynchronously creates an object URL for the canvas content, enabling efficient browser resource management.
    /// </summary>
    /// <param name="type">
    /// The MIME type for the output format. Defaults to "image/png".
    /// Supported formats include "image/png", "image/jpeg", and "image/webp".
    /// </param>
    /// <param name="quality">
    /// Optional quality parameter for lossy formats (0.0 to 1.0).
    /// Only applies to JPEG and WebP formats. Higher values produce better quality and larger file sizes.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{String}"/> representing the asynchronous operation.
    /// The result is a blob URL string that can be used directly in browser APIs.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported MIME type is specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="quality"/> is not between 0.0 and 1.0.
    /// </exception>
    /// <remarks>
    /// Object URLs provide memory-efficient access to large image data by creating temporary
    /// browser-managed references. Remember to revoke URLs when no longer needed to prevent memory leaks.
    /// </remarks>
    ValueTask<string> CreateObjectURL(string type = "image/png", double? quality = null);
}
