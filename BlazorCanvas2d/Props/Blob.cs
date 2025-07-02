using System;
using System.Buffers;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorCanvas2d;

public sealed class Blob : IDisposable, IAsyncDisposable
{
    private readonly IMemoryOwner<byte> _memoryOwner;
    private readonly ReadOnlyMemory<byte> _data;
    private bool _disposed;

    public string Type { get; }

    public string ObjectUrl { get; }

    public long Size => this._data.Length;

    /// <summary>
    /// Creates a Blob from byte data with optional MIME type.
    /// </summary>
    public Blob(
        ReadOnlySpan<byte> data,
        string type = "application/octet-stream",
        string objectUrl = ""
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        this._memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
        data.CopyTo(this._memoryOwner.Memory.Span);
        this._data = this._memoryOwner.Memory[..data.Length];
        this.Type = type;
        this.ObjectUrl = objectUrl;
    }

    /// <summary>
    /// Returns the blob data as a byte array.
    /// </summary>
    public byte[] ToArray()
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);
        return this._data.ToArray();
    }

    public string ToDataUrl()
    {
        // Base64 encode the data
        var base64 = Convert.ToBase64String(this.ToArray());

        // Compose the data URL
        return $"data:{this.Type};base64,{base64}";
    }

    /// <summary>
    /// Returns a stream for reading the blob data.
    /// </summary>
    public Stream Stream()
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);
        return new ReadOnlyMemoryStream(this._data);
    }

    public void Dispose()
    {
        if (!this._disposed)
        {
            this._memoryOwner?.Dispose();
            this._disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!this._disposed)
        {
            if (this._memoryOwner is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                this._memoryOwner?.Dispose();

            this._disposed = true;
        }
    }
}

/// <summary>
/// High-performance read-only memory stream implementation.
/// </summary>
internal sealed class ReadOnlyMemoryStream(ReadOnlyMemory<byte> memory) : Stream
{
    private int _position;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => memory.Length;

    public override long Position
    {
        get => this._position;
        set => this._position = (int)Math.Clamp(value, 0, memory.Length);
    }

    public override int Read(byte[] buffer, int offset, int count) =>
        this.Read(buffer.AsSpan(offset, count));

    public override int Read(Span<byte> buffer)
    {
        var bytesToRead = Math.Min(buffer.Length, memory.Length - this._position);
        if (bytesToRead <= 0)
            return 0;

        memory.Span.Slice(this._position, bytesToRead).CopyTo(buffer);
        this._position += bytesToRead;
        return bytesToRead;
    }

    public override async Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken
    ) => await this.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult(this.Read(buffer.Span));

    public override long Seek(long offset, SeekOrigin origin) =>
        this.Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => this.Position + offset,
            SeekOrigin.End => this.Length + offset,
            _ => throw new ArgumentException("Invalid seek origin", nameof(origin))
        };

    public override void Flush() { }

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();
}

/// <summary>
/// Data transfer object for blob data from JavaScript.
/// </summary>
internal sealed record BlobData
{
    [JsonPropertyName("data")]
    public byte[] Data { get; init; } = [];

    [JsonPropertyName("type")]
    public string Type { get; init; } = "application/octet-stream";

    [JsonPropertyName("objectUrl")]
    public string ObjectUrl { get; init; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Converts to the Blob.
    /// </summary>
    public Blob ToBlob() => new(this.Data, this.Type, this.ObjectUrl);
}
