using System.Runtime.CompilerServices;

namespace BlazorCanvas2d.Interop;

[StructLayout(LayoutKind.Auto)]
internal readonly record struct JsOp
{
    [JsonInclude]
    public readonly bool IsProperty;

    [JsonInclude]
    public readonly string MethodName;

    [JsonInclude]
    public readonly object? Args;

    private JsOp(string methodName, object? args, bool isProperty) =>
        (this.MethodName, this.Args, this.IsProperty) = (methodName, args, isProperty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsOp FunctionCall(string methodName, object? args) =>
        new(methodName, args, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsOp PropertyCall(string propertyName, object? args) =>
        new(propertyName, args, true);
}
