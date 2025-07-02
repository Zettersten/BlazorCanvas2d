using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace BlazorCanvas2d.Interop;

internal sealed class MarshalReferencePool
{
    private readonly ConcurrentDictionary<int, MarshalReference> _cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MarshalReference Next(ElementReference elementReference) =>
        elementReference.Id switch
        {
            null
                => throw new ArgumentException(
                    "ElementReference.Id cannot be null",
                    nameof(elementReference)
                ),
            var id when int.TryParse(id, out var hash) => this.GetOrCreateReference(hash, true),
            var id => this.GetOrCreateReference(id.GetHashCode(), true)
        };

    public MarshalReference Next(params object[] methodParams) =>
        methodParams.Length switch
        {
            0 => this.GetOrCreateReference(0, false),
            1 => this.GetOrCreateReference(methodParams[0]?.GetHashCode() ?? 0, false),
            2
                => this.GetOrCreateReference(
                    HashCode.Combine(methodParams[0], methodParams[1]),
                    false
                ),
            3
                => this.GetOrCreateReference(
                    HashCode.Combine(methodParams[0], methodParams[1], methodParams[2]),
                    false
                ),
            4
                => this.GetOrCreateReference(
                    HashCode.Combine(
                        methodParams[0],
                        methodParams[1],
                        methodParams[2],
                        methodParams[3]
                    ),
                    false
                ),
            _ => this.GetOrCreateReference(CreateKeyFromLargeParams(methodParams), false)
        };

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CreateKeyFromLargeParams(ReadOnlySpan<object> key)
    {
        var hash = new HashCode();
        foreach (var item in key)
            hash.Add(item);
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarshalReference GetOrCreateReference(int keyHash, bool isElementReference) =>
        this._cache.GetOrAdd(
            keyHash,
            static (hash, isElement) => new MarshalReference(hash, isElement),
            isElementReference
        );
}
