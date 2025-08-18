# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Repository Overview

BlazorCanvas2d is a high-performance HTML5 Canvas API wrapper for Blazor applications. It provides a comprehensive C# interface to the browser's 2D rendering capabilities with TypeScript interop for optimal performance.

## Key Development Commands

### Building and Running
```bash
# Restore all dependencies
dotnet restore

# Build the entire solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Run a specific sample project (from the sample directory)
dotnet run --project BlazorCanvas2d.Samples.PfpMaker

# Run any sample project (change directory first)
cd BlazorCanvas2d.Samples\[SampleName]
dotnet run
```

### Code Formatting and Quality
```bash
# Format code using csharpier (must be in repository root)
dotnet csharpier .

# Install local tools (if not already installed)
dotnet tool restore

# Check and install husky hooks
dotnet husky install
```

### Package Management
```bash
# Pack the library for NuGet distribution
dotnet pack ./BlazorCanvas2d/BlazorCanvas2d.csproj --configuration Release

# Build package with versioning (uses Nerdbank.GitVersioning)
dotnet pack --configuration Release --no-build -o ./nupkg
```

### TypeScript Compilation
The TypeScript files in `BlazorCanvas2d/wwwroot/` are automatically compiled during build. Manual compilation is handled by MSBuild targets.

## Architecture Overview

### Core Components

**Canvas Components**
- `Canvas.razor` - Single canvas component for simple scenarios
- `CanvasManager.razor` - Multi-canvas management component for complex applications
- `CanvasCreationOptions.cs` - Configuration options for canvas creation

**Rendering System**
- `RenderContext2D.cs` - Main rendering context implementation split across multiple files:
  - `RenderContext2D.Interop.cs` - JavaScript interop operations
  - `RenderContext2D.Methods.cs` - Canvas API method implementations
  - `RenderContext2D.Properties.cs` - Canvas property management
- `IRenderContext.cs` - Rendering context abstraction

**JavaScript Interop Layer**
- `blazorCanvas2d.ts` - TypeScript implementation for canvas operations
- `JsOp.cs` - JavaScript operation marshaling
- `MarshalReference.cs` / `MarshalReferencePool.cs` - Efficient object reference management

**Event System**
- Comprehensive event handling for mouse, keyboard, and canvas events
- Custom JSON converters for efficient event serialization
- Event types: `MouseMoveEvent`, `MouseClickEvent`, `MouseScrollEvent`, `KeyboardPressEvent`

**Properties and Configuration**
- Canvas properties (colors, fonts, transformations) in `Props/` directory
- Support for gradients, patterns, filters, and advanced Canvas 2D features
- Configurable rendering options (alpha, color space, performance settings)

### Sample Projects Structure

Each sample in `BlazorCanvas2d.Samples/` demonstrates specific functionality:
- **SimpleCanvas** - Basic canvas usage
- **MouseCoords** - Mouse event handling
- **KeyAndMouse** - Combined input handling
- **PanAndZoom** - Canvas transformation techniques
- **ResizeCanvas** - Responsive canvas handling
- **FallingBlocks** - Game-like animation example
- **DoomFire** - Advanced pixel manipulation
- **PfpMaker** - Complex application using VeeFriends characters

### Development Tools Integration

**Code Quality**
- CSharpier for consistent code formatting (configured in `.editorconfig`)
- Husky git hooks for pre-commit formatting
- Nullability and implicit usings enabled across projects

**Versioning**
- Nerdbank.GitVersioning for semantic versioning from git tags
- Version configuration in `version.json`

**Package Publishing**
- Automated NuGet publishing via GitHub Actions on release
- Package metadata configured in main project file

## Working with the Codebase

### Adding New Canvas Features
1. Implement new methods in appropriate `RenderContext2D` file
2. Add corresponding TypeScript implementations in `blazorCanvas2d.ts`
3. Create or update property classes in `Props/` if needed
4. Add sample usage in relevant sample project

### Creating New Samples
1. Create new project in `BlazorCanvas2d.Samples/` directory
2. Reference the main `BlazorCanvas2d` project
3. Follow existing sample patterns for structure
4. Add project to main solution file

### Event Handling Extensions
1. Define event model in `Events/` directory
2. Create JSON converter in `Serialization/` directory
3. Add TypeScript event binding in main TypeScript file
4. Update canvas options and abstractions as needed

## Framework and Technology Stack

- **.NET 9.0** - Target framework
- **Blazor WebAssembly** - Client-side hosting model
- **TypeScript** - JavaScript interop with type safety
- **Canvas 2D API** - Underlying web technology
- **Nerdbank.GitVersioning** - Version management
- **CSharpier** - Code formatting
- **Husky** - Git hooks for quality gates
