# BlazorCanvas2d PfpMaker Sample

A sample application demonstrating the BlazorCanvas2d library with VeeFriends characters integration and modern styling using Tailwind CSS.

## Features

- ✨ Create custom profile pictures
- 🎨 Use VeeFriends characters
- ⚡ High-performance canvas rendering
- 🎯 Modern UI with Tailwind CSS
- 📱 Responsive design

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Node.js (for Tailwind CSS build process)

### Running the Application

1. **Install dependencies:**
   ```bash
   npm install
   ```

2. **Build Tailwind CSS:**
   ```bash
   npm run build-css-prod
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

### Development

For development with automatic CSS rebuilding:

```bash
npm run build-css
```

This will watch for changes and rebuild the CSS automatically.

## Project Structure

- `Pages/` - Blazor pages and components
- `Layout/` - Layout components
- `wwwroot/` - Static assets including CSS and generated Tailwind styles
- `tailwind.config.js` - Tailwind CSS configuration
- `package.json` - NPM dependencies and scripts

## Technologies Used

- **Blazor WebAssembly** - Client-side framework
- **BlazorCanvas2d** - High-performance canvas wrapper
- **VeeFriends.Characters** - Character assets
- **Tailwind CSS** - Utility-first CSS framework
- **.NET 9.0** - Runtime platform

## Building for Production

To build optimized CSS for production:

```bash
npm run build-css-prod
dotnet publish -c Release
```
