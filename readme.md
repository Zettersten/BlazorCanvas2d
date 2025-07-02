![BlazorCanvas2d Logo](https://raw.githubusercontent.com/Zettersten/BlazorCanvas2d/main/icon.png)

# BlazorCanvas2d 🎮🎨

[![NuGet version](https://badge.fury.io/nu/BlazorCanvas2d.svg)](https://badge.fury.io/nu/BlazorCanvas2d)

## Overview

BlazorCanvas2d is a high-performance HTML5 Canvas API wrapper for Blazor applications. It provides a comprehensive C# interface to the browser's 2D rendering capabilities, enabling developers to create rich graphics, interactive visualizations, and games directly within Blazor components.

The library is designed with performance in mind, featuring batch rendering operations and efficient JavaScript interop to minimize overhead.

## Features

- **Complete Canvas 2D API implementation** - Full access to shapes, paths, transforms, text, images, and more
- **Event handling** - Support for mouse, touch, keyboard, and window resize events
- **Multiple canvas management** - Create and control multiple canvases with the CanvasManager
- **Performance optimizations** - Batched rendering operations and efficient memory usage
- **Image export options** - Convert canvas content to blobs, data URLs, or object URLs
- **Rendering configuration** - Fine-tune rendering behavior with options for alpha channels, color spaces, and more

## Installation

Add the BlazorCanvas2d NuGet package to your Blazor project:

```bash
dotnet add package BlazorCanvas2d
```

Add the following using statement to your `_Imports.razor` file:


```csharp
@using BlazorCanvas2d
```

## Basic Usage

### Single Canvas Component

The simplest way to use BlazorCanvas2d is to include a Canvas component in your Razor page:


```csharp
@page "/simple-canvas"

<h1>Simple Canvas Example</h1>
<Canvas Width="800" Height="600" OnCanvasReady="CanvasReadyHandler" />

@code 
{     
    private async void CanvasReadyHandler(ICanvas canvas) 
    { 
        var ctx = canvas.RenderContext;
        
        // Clear the canvas
        ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
    
        // Draw a filled rectangle
        ctx.FillStyle = "blue";
        ctx.FillRect(50, 50, 200, 100);
    
        // Draw some text
        ctx.Font = "24px Arial";
        ctx.FillStyle = "white";
        ctx.FillText("Hello, BlazorCanvas2d!", 60, 100);
    }
}
```

### Using CanvasManager for Multiple Canvases

For more complex applications, you can use the `CanvasManager` component to create and manage multiple canvases:

```csharp
@page "/multi-canvas"

<h1>Multiple Canvas Example</h1>
<CanvasManager @ref="CanvasManager" />
<button @onclick="AddNewCanvas">Add New Canvas</button>

@code 
{ 
    private CanvasManager CanvasManager { get; set; } = default!; 
    private int canvasCount = 0;

    protected override void OnInitialized()
    {
        // Create an initial canvas
        CreateNewCanvas();
    }

    private void AddNewCanvas()
    {
        CreateNewCanvas();
    }

    private void CreateNewCanvas()
    {
        string canvasName = $"canvas{canvasCount++}";
    
        CanvasManager.CreateCanvas(canvasName, new CanvasCreationOptions
        {
            Width = 400,
            Height = 300,
            OnCanvasReady = canvas =>
            {
                var ctx = canvas.RenderContext;
            
                // Draw something unique on each canvas
                ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
                ctx.FillStyle = $"hsl({canvasCount * 30}, 70%, 60%)";
                ctx.FillRect(10, 10, canvas.Width - 20, canvas.Height - 20);
            
                ctx.Font = "18px Arial";
                ctx.FillStyle = "white";
                ctx.FillText($"Canvas #{canvasCount}", 20, 40);
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
    // Get as a binary Blob (for file saving) 
    Blob pngBlob = await canvas.ToBlob("image/png");
    
    // Get as a data URL (for embedding in img tags)
    string dataUrl = await canvas.ToDataUrl("image/jpeg", 0.9);

    // Get as an object URL (for temporary references)
    string objectUrl = await canvas.CreateObjectURL("image/webp", 0.8);

    // Use JS interop to trigger a file download
    await JS.InvokeVoidAsync("downloadFile", objectUrl, "canvas-export.png");
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
