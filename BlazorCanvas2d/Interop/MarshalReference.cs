﻿namespace BlazorCanvas2d.Interop;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal readonly record struct MarshalReference
{
    [JsonInclude]
    public int Id { get; init; }

    [JsonInclude]
    public bool IsElementRef { get; init; }

    [
        JsonInclude,
        JsonIgnore(
            Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull
        )
    ]
    public string? ClassInitializer { get; init; }

    /// <summary>Primary constructor with validation</summary>
    public MarshalReference(int id, bool isElementRef = false, string? classInitializer = null)
    {
        (this.Id, this.IsElementRef, this.ClassInitializer) = (id, isElementRef, classInitializer);
    }
}
