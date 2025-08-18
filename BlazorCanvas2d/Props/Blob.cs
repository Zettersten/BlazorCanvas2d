namespace BlazorCanvas2d;

/// <summary>
/// Data transfer object for blob data from JavaScript.
/// </summary>
public sealed record BlobData
{
    [JsonPropertyName("objectUrl")]
    public string ObjectUrl { get; init; } = string.Empty;
}
