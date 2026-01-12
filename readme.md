![BlazorCanvas2d Logo](https://raw.githubusercontent.com/Zettersten/BlazorCanvas2d/main/icon.png)

## BlazorCanvas2d

[![NuGet version](https://badge.fury.io/nu/BlazorCanvas2d.svg)](https://badge.fury.io/nu/BlazorCanvas2d)

BlazorCanvas2d is a Blazor component library that wraps the browser Canvas 2D API with a C#-friendly surface and batched JS interop.

## Requirements

- **.NET 10**
- **Blazor WebAssembly** (recommended for best perf)

## Install

```bash
dotnet add package BlazorCanvas2d
```

In your `_Imports.razor`:

```csharp
@using BlazorCanvas2d
```

## Basic usage

### Single canvas

```csharp
@page "/simple-canvas"

<Canvas Width="800" Height="600" OnCanvasReady="OnCanvasReady" />

@code {
    private void OnCanvasReady(ICanvas canvas)
    {
        var ctx = canvas.RenderContext;

        ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
        ctx.FillStyle = "blue";
        ctx.FillRect(50, 50, 200, 100);

        ctx.Font = "24px Arial";
        ctx.FillStyle = "white";
        ctx.FillText("Hello, BlazorCanvas2d!", 60, 100);
    }
}
```

### Multiple canvases (`CanvasManager`)

Best practice: create canvases once, after the component has rendered and the `@ref` is available.

```csharp
@page "/multi-canvas"

<CanvasManager @ref="_manager" />

@code {
    private CanvasManager? _manager;
    private int _count;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender || _manager is null) return;

        _manager.CreateCanvas($"canvas{_count++}", new CanvasCreationOptions
        {
            Width = 400,
            Height = 300,
            OnCanvasReady = canvas =>
            {
                var ctx = canvas.RenderContext;
                ctx.FillStyle = "hsl(200, 70%, 60%)";
                ctx.FillRect(0, 0, canvas.Width, canvas.Height);
            }
        });
    }
}
```

## Canvas Configuration

When creating a canvas, you can configure its behavior using the following options:

```csharp
// Create a canvas with specific configuration 
CanvasManager.CreateCanvas("gameCanvas", new CanvasCreationOptions 
{ 
    // Required dimensions Width = 800, Height = 600,
    // Optional rendering configuration
    Alpha = true,                           // Enable transparency
    ColorSpace = ColorSpace.Srgb,           // Use sRGB color space
    Desynchronized = true,                  // Reduce input latency
    WillReadFrequently = false,             // Optimize for drawing over pixel reading
    Hidden = false,                         // Canvas is visible

    // Event handlers
    OnCanvasReady = HandleCanvasReady,      // Called when canvas is initialized
    OnFrameReady = HandleFrameUpdate,       // Called on each animation frame
    OnKeyDown = HandleKeyDown,              // Keyboard press events
    OnKeyUp = HandleKeyUp,                  // Keyboard release events
    OnMouseMove = HandleMouseMove,          // Mouse movement
    OnMouseDown = HandleMouseDown,          // Mouse button press
    OnMouseUp = HandleMouseUp,              // Mouse button release
    OnMouseWheel = HandleMouseWheel,        // Mouse scroll wheel
    OnResize = HandleCanvasResize           // Window resize events
});
```


## Animation Loop

For animated content or games, use the `OnFrameReady` callback to create an animation loop:

```csharp
private void CreateAnimatedCanvas() 
{ 
    CanvasManager.CreateCanvas("animationDemo", new CanvasCreationOptions 
    { 
        Width = 800, 
        Height = 600, 
        OnCanvasReady = canvas => { 
            // Store canvas reference for use in the frame update 
            _canvas = canvas;
            // Initialize your animation state here
            _angle = 0;
        },
        OnFrameReady = timeStamp =>
        {
            // This is called on each animation frame
            UpdateAnimation(timeStamp);
        }
    });
}

private float _angle; 

private ICanvas _canvas;

private void UpdateAnimation(float timeStamp) 
{ 
    
    // Increment animation state _angle += 0.01f;
    var ctx = _canvas.RenderContext;

    // Clear canvas
    ctx.ClearRect(0, 0, _canvas.Width, _canvas.Height);

    // Save current state
    ctx.Save();

    // Move to center of canvas
    ctx.Translate(_canvas.Width / 2, _canvas.Height / 2);

    // Rotate based on animation state
    ctx.Rotate(_angle);

    // Draw a rectangle
    ctx.FillStyle = "purple";
    ctx.FillRect(-100, -50, 200, 100);

    // Restore original state
    ctx.Restore();
}
```

## Working with Images

Loading and rendering images requires a reference to the image element:

```csharp
@page "/image-demo" 
@inject IJSRuntime JS

<img @ref="ImageRef" src="sample-image.png" style="display: none;" /> 
<Canvas Width="800" Height="600" OnCanvasReady="CanvasReadyHandler" />

@code 
{ 
    private ElementReference ImageRef;

    private async void CanvasReadyHandler(ICanvas canvas)
    {
        var ctx = canvas.RenderContext;
    
        // Clear canvas
        ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
    
        // Draw the image
        ctx.DrawImage(ImageRef, 10, 10);
    
        // Draw a scaled version
        ctx.DrawImage(ImageRef, 300, 10, 200, 150);
    
        // Draw a portion of the image (source x, y, width, height, dest x, y, width, height)
        ctx.DrawImage(ImageRef, 0, 0, 100, 100, 600, 10, 150, 150);
    }
}
```

## Exporting Canvas Content

BlazorCanvas2d provides several methods to export canvas content:

```csharp
private async Task ExportCanvas(ICanvas canvas) 
{ 
    // Create an object URL (fast, browser-managed). Revoke when done.
    string objectUrl = await canvas.CreateObjectURL("image/png");
    
    // Create a data URL (base64). Useful for small images.
    string dataUrl = await canvas.ToDataUrl("image/jpeg", 0.9);

    // Low-level blob export returns a DTO containing the object URL:
    BlobData blob = await canvas.ToBlob("image/webp", 0.8);

    // Trigger a download using the library module (no eval):
    var module = await JS.InvokeAsync<IJSObjectReference>(
        "import",
        "./_content/BlazorCanvas2d/blazorCanvas2d.js"
    );
    await module.InvokeVoidAsync("downloadUrl", objectUrl, "canvas-export.png", true);
}
```


## Advanced Usage

### Path Operations

Create complex shapes using path operations:

```csharp
private void DrawStar(ICanvas canvas, float x, float y, float radius, int points, string fillColor) 
{ 
    var ctx = canvas.RenderContext; 
    
    float step = MathF.PI * 2 / points; 
    float halfStep = step / 2;

    ctx.Save();
    ctx.BeginPath();
    ctx.Translate(x, y);
    ctx.MoveTo(radius, 0);

    for (int i = 1; i < points * 2; i++)
    {
        float currentRadius = i % 2 == 0 ? radius : radius * 0.4f;
        float angle = i * halfStep;
        ctx.LineTo(
            MathF.Cos(angle) * currentRadius,
            MathF.Sin(angle) * currentRadius
        );
    }

    ctx.ClosePath();
    ctx.FillStyle = fillColor;
    ctx.Fill();
    ctx.Restore();
}
```


### Gradients and Patterns

Create gradients and patterns for rich visual effects:

```csharp
private void DrawGradients(ICanvas canvas) 
{ 
    var ctx = canvas.RenderContext;
    
    // Linear gradient
    var linearGradient = ctx.CreateLinearGradient(0, 0, 200, 0);

    linearGradient
        .AddColorStop(0, "red")
        .AddColorStop(0.5f, "green")
        .AddColorStop(1, "blue");
              
    ctx.FillStyle = linearGradient;
    ctx.FillRect(50, 50, 200, 100);

    // Radial gradient
    var radialGradient = ctx.CreateRadialGradient(400, 100, 0, 400, 100, 80);

    radialGradient
        .AddColorStop(0, "white")
        .AddColorStop(1, "purple");
             
    ctx.FillStyle = radialGradient;
    ctx.FillRect(320, 50, 160, 100);

    // Conic gradient (color wheel)
    var conicGradient = ctx.CreateConicGradient(0, 600, 100);
    conicGradient
        .AddColorStop(0, "red")
        .AddColorStop(0.17f, "orange")
        .AddColorStop(0.33f, "yellow")
        .AddColorStop(0.5f, "green")
        .AddColorStop(0.67f, "blue")
        .AddColorStop(0.83f, "indigo")
        .AddColorStop(1, "violet");
            
    ctx.FillStyle = conicGradient;
    ctx.BeginPath();
    ctx.Arc(600, 100, 80, 0, MathF.PI * 2);
    ctx.Fill();
}
```

### Handling Input

Process user input with event handlers:

```csharp
private void SetupInteractiveCanvas() 
{ 
    CanvasManager.CreateCanvas("interactive", new CanvasCreationOptions { 
        Width = 800, 
        Height = 600, 
        OnCanvasReady = canvas => _canvas = canvas, 
        OnMouseMove = HandleMouseMove, 
        OnMouseDown = HandleMouseDown, 
        OnMouseUp = HandleMouseUp, 
        OnKeyDown = HandleKeyDown 
    }); 
}

private void HandleMouseMove(MouseMoveEvent evt) 
{ 
    // Access mouse coordinates 
    float x = (float)evt.ClientX; 
    float y = (float)evt.ClientY;
    // Update UI or game state based on mouse position
}

private void HandleKeyDown(KeyboardPressEvent evt) 
{ 
    // React to keyboard input 
    switch (evt.Key) { 
        case "ArrowUp": // Move character up 
            break; 
        case "Space": // Jump or fire 
            break; 
    }

    // Check for modifier keys
    if (evt.Modifiers.Ctrl)
    {
        // Handle Ctrl+key combinations
    }
}
```

## Performance Considerations

- **Batch operations**: Group similar drawing operations together to reduce JavaScript interop overhead
- **Reuse paths**: For complex shapes that don't change, define paths once and reuse them
- **Limit canvas size**: Larger canvases require more memory and processing power
- **Use appropriate settings**: Disable alpha if transparency isn't needed
- **Image optimization**: Pre-scale images to their final display size when possible
- **Layer multiple canvases**: For complex UIs, use multiple canvases for different layers

## Browser Compatibility

BlazorCanvas2d works in all modern browsers that support HTML5 Canvas and Blazor WebAssembly:

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Android Chrome)

Some advanced features like `display-p3` color space may have limited support in older browsers.

## License

BlazorCanvas2d is licensed under the MIT License. See the LICENSE file for more details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request or open issues on the [GitHub repository](https://github.com/Zettersten/BlazorCanvas2d).
