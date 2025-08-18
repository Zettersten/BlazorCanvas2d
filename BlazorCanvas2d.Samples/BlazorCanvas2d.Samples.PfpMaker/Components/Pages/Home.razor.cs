using BlazorCanvas2d.Extensions;
using BlazorCanvas2d.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VeeFriends.Characters;
using VeeFriends.Characters.Abstractions;
using VeeFriends.Characters.Models.Tokens;

namespace BlazorCanvas2d.Samples.PfpMaker.Components.Pages;

public partial class Home
{
    private static readonly Dictionary<string, (int w, int h)> presets = new()
    {
        ["PFP 1:1"] = (500, 500),
        ["X 16:9"] = (1600, 900),
        ["IG 4:5"] = (1080, 1350),
        ["Whatnot 11:17"] = (495, 756),
    };

    private CanvasManager? canvasManager;
    private ICanvas? canvas;
    private IRenderContext? ctx;

    private ElementReference? classicPoseImage;
    private ElementReference? competingPoseImage;
    private ElementReference? evolvingPoseImage;
    private ElementReference? manifestingPoseImage;
    private ElementReference? restingPoseImage;
    private ElementReference? strategizingPoseImage;
    private ElementReference? diamondImage;
    private ElementReference? emeraldImage;
    private ElementReference? lavaImage;
    private ElementReference? goldImage;
    private ElementReference? hologramImage;
    private ElementReference? mangaImage;
    private ElementReference? bubbleGumImage;
    private ElementReference? titleImageNoLogo;

    private ElementReference? patternImage1;
    private ElementReference? patternImage2;
    private ElementReference? patternImage3;
    private ElementReference? patternImage4;
    private ElementReference? patternImage5;
    private ElementReference? patternImage6;
    private ElementReference? patternImage7;
    private ElementReference? patternImage8;
    private ElementReference? patternImage9;
    private ElementReference? patternImage10;

    [Inject]
    public Manifest Manifest { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    public ITokenEntity? Character { get; set; }

    // ------- Mobile responsive state -------
    private bool showToolsPanel = false;

    // ------- Canvas size/presets -------
    private string presetKey = "PFP 1:1";
    private ResponsiveCanvasHelper.ResponsiveCanvasConfig canvasConfig =
        ResponsiveCanvasHelper.ResponsiveCanvasConfig.Create(500, 500);

    // ------- Background -------
    private string bgHex = "#00ad83";
    private string backgroundType = "solid";

    // ------- Character state -------
    private int poseIndex = 0;
    private int offsetX = 0;
    private int offsetY = 0;
    private int rotationDeg = 0;
    private bool flipped = false;
    private int charSizePx = 480; // dest size used for DrawImage (square)

    // ------- Text state -------
    private string text = "";
    private int textX = 0;
    private int textY = 0;
    private int textSize = 64;
    private string textHex = "#ffffff";
    private string textFontFamily = "Arial";
    private TextAlign textAlign = TextAlign.Center;
    private bool textShadow = true;

    // ------- Export state -------
    private bool isExporting = false;

    protected override void OnInitialized()
    {
        try
        {
            this.Character = this.Manifest.Series2.GetRandomEntity();
            base.OnInitialized();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing component: {ex.Message}");
            throw;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender || this.canvasManager is null)
        {
            return;
        }

        this.canvasManager.CreateResponsiveCanvas(
            "rare-main",
            this.canvasConfig,
            this.OnMainCanvasReady,
            this.OnMainFrameReady
        );

        base.OnAfterRender(firstRender);
    }

    private void OnMainCanvasReady(ICanvas canvas)
    {
        this.canvas = canvas;
        this.ctx = canvas.RenderContext;
    }

    // Mobile tools panel toggle
    private void ToggleToolsPanel()
    {
        this.showToolsPanel = !this.showToolsPanel;
    }

    // Get CSS classes for tools panel based on mobile state
    private string GetToolsPanelClasses()
    {
        var baseClasses = "flex flex-col";

        if (this.showToolsPanel)
        {
            // Mobile overlay panel when shown
            return $"{baseClasses} fixed inset-y-0 right-0 w-80 z-50 bg-slate-900 lg:relative lg:inset-auto lg:w-72 xl:w-80 lg:bg-transparent";
        }
        else
        {
            // Hidden on mobile, visible on desktop
            return $"{baseClasses} hidden lg:flex lg:w-72 xl:w-80";
        }
    }

    private List<ElementReference> ImageRefs
    {
        get
        {
            var list = new List<ElementReference>();

            if (this.classicPoseImage.HasValue)
            {
                list.Add(this.classicPoseImage.Value);
            }
            if (this.competingPoseImage.HasValue)
            {
                list.Add(this.competingPoseImage.Value);
            }
            if (this.evolvingPoseImage.HasValue)
            {
                list.Add(this.evolvingPoseImage.Value);
            }
            if (this.manifestingPoseImage.HasValue)
            {
                list.Add(this.manifestingPoseImage.Value);
            }
            if (this.restingPoseImage.HasValue)
            {
                list.Add(this.restingPoseImage.Value);
            }
            if (this.strategizingPoseImage.HasValue)
            {
                list.Add(this.strategizingPoseImage.Value);
            }
            if (this.diamondImage.HasValue)
            {
                list.Add(this.diamondImage.Value);
            }
            if (this.emeraldImage.HasValue)
            {
                list.Add(this.emeraldImage.Value);
            }
            if (this.lavaImage.HasValue)
            {
                list.Add(this.lavaImage.Value);
            }
            if (this.goldImage.HasValue)
            {
                list.Add(this.goldImage.Value);
            }
            if (this.hologramImage.HasValue)
            {
                list.Add(this.hologramImage.Value);
            }
            if (this.mangaImage.HasValue)
            {
                list.Add(this.mangaImage.Value);
            }
            if (this.bubbleGumImage.HasValue)
            {
                list.Add(this.bubbleGumImage.Value);
            }
            if (this.titleImageNoLogo.HasValue)
            {
                list.Add(this.titleImageNoLogo.Value);
            }

            return list;
        }
    }

    private List<ElementReference> BackgroundRefs
    {
        get
        {
            var list = new List<ElementReference>();

            if (this.patternImage1.HasValue)
            {
                list.Add(this.patternImage1.Value);
            }
            if (this.patternImage2.HasValue)
            {
                list.Add(this.patternImage2.Value);
            }
            if (this.patternImage3.HasValue)
            {
                list.Add(this.patternImage3.Value);
            }
            if (this.patternImage4.HasValue)
            {
                list.Add(this.patternImage4.Value);
            }
            if (this.patternImage5.HasValue)
            {
                list.Add(this.patternImage5.Value);
            }
            if (this.patternImage6.HasValue)
            {
                list.Add(this.patternImage6.Value);
            }
            if (this.patternImage7.HasValue)
            {
                list.Add(this.patternImage7.Value);
            }
            if (this.patternImage8.HasValue)
            {
                list.Add(this.patternImage8.Value);
            }
            if (this.patternImage9.HasValue)
            {
                list.Add(this.patternImage9.Value);
            }
            if (this.patternImage10.HasValue)
            {
                list.Add(this.patternImage10.Value);
            }

            return list;
        }
    }

    private List<(string Name, int Index)> ImagePoses
    {
        get
        {
            var list = new List<(string Name, int Index)>();

            if (this.Character is not Series2Token)
            {
                return list;
            }

            if (this.classicPoseImage.HasValue)
            {
                list.Add(("Classic", 0));
            }

            if (this.competingPoseImage.HasValue)
            {
                list.Add(("Competing", 1));
            }

            if (this.evolvingPoseImage.HasValue)
            {
                list.Add(("Evolving", 2));
            }

            if (this.manifestingPoseImage.HasValue)
            {
                list.Add(("Manifesting", 3));
            }

            if (this.restingPoseImage.HasValue)
            {
                list.Add(("Resting", 4));
            }

            if (this.strategizingPoseImage.HasValue)
            {
                list.Add(("Strategizing", 5));
            }

            if (this.diamondImage.HasValue)
            {
                list.Add(("Diamond", 6));
            }

            if (this.emeraldImage.HasValue)
            {
                list.Add(("Emerald", 7));
            }

            if (this.lavaImage.HasValue)
            {
                list.Add(("Lava", 8));
            }

            if (this.goldImage.HasValue)
            {
                list.Add(("Gold", 9));
            }

            if (this.hologramImage.HasValue)
            {
                list.Add(("Hologram", 10));
            }

            if (this.mangaImage.HasValue)
            {
                list.Add(("Manga", 11));
            }

            if (this.bubbleGumImage.HasValue)
            {
                list.Add(("Bubble Gum", 12));
            }

            if (this.titleImageNoLogo.HasValue)
            {
                list.Add(("Title (No Logo)", 13));
            }

            return list;
        }
    }

    private List<(string Name, int Index, float Height, float Width)> BackgroundPatterns
    {
        get
        {
            var list = new List<(string Name, int Index, float Height, float Width)>();

            if (this.patternImage1.HasValue)
            {
                list.Add(("Pattern 1", 0, 2084, 6250));
            }

            if (this.patternImage2.HasValue)
            {
                list.Add(("Pattern 2", 1, 2084, 6250));
            }

            if (this.patternImage3.HasValue)
            {
                list.Add(("Pattern 3", 2, 6250, 2084));
            }

            if (this.patternImage4.HasValue)
            {
                list.Add(("Pattern 4", 3, 2869, 1320));
            }

            if (this.patternImage5.HasValue)
            {
                list.Add(("Pattern 5", 4, 2869, 1320));
            }

            if (this.patternImage6.HasValue)
            {
                list.Add(("Pattern 6", 5, 2869, 1320));
            }

            if (this.patternImage7.HasValue)
            {
                list.Add(("Pattern 7", 6, 2869, 1320));
            }

            if (this.patternImage8.HasValue)
            {
                list.Add(("Pattern 8", 7, 2869, 1320));
            }

            if (this.patternImage9.HasValue)
            {
                list.Add(("Pattern 9", 8, 2084, 6250));
            }

            if (this.patternImage10.HasValue)
            {
                list.Add(("Pattern 10", 9, 6250, 2084));
            }

            return list;
        }
    }

    private static readonly Dictionary<string, string> availableFonts = new()
    {
        ["Arial"] = "Arial, sans-serif",
        ["Helvetica"] = "Helvetica, Arial, sans-serif",
        ["Times New Roman"] = "\"Times New Roman\", serif",
        ["Georgia"] = "Georgia, serif",
        ["Courier New"] = "\"Courier New\", monospace",
        ["Verdana"] = "Verdana, sans-serif",
        ["Roboto"] = "Roboto, sans-serif",
        ["Obviously Narrow"] = "\"obviously-narrow\", sans-serif",
        ["Obviously Condensed"] = "\"obviously-condensed\", sans-serif",
    };

    private void ResizeCanvas()
    {
        if (this.canvas is null)
        {
            return;
        }

        this.canvas.Resize(this.canvasConfig.DisplayWidth, this.canvasConfig.DisplayHeight);
        this.poseIndex = 0;
        this.offsetX = 0;
        this.offsetY = 0;
        this.rotationDeg = 0;
        this.flipped = false;
        this.charSizePx = 480;
        this.textX = 0;
        this.textY = 0;
    }

    // **** ALL drawing happens here; framework flushes after this callback ****
    private void OnMainFrameReady(float timestamp)
    {
        if (this.ctx is null || this.canvas is null)
        {
            return;
        }

        // Clear canvas with background color
        this.canvas.Clear(this.bgHex);

        // Background pattern using helper
        if (this.backgroundType != "solid")
        {
            var bgRef = this.BackgroundPatterns.FirstOrDefault(x => this.backgroundType == x.Name);
            var bgImageRef = this.BackgroundRefs[bgRef.Index];

            if (bgImageRef.Id != null)
            {
                this.ctx.DrawBackgroundImageCover(
                    bgImageRef,
                    bgRef.Width,
                    bgRef.Height,
                    this.canvasConfig.DisplayWidth,
                    this.canvasConfig.DisplayHeight
                );
            }
        }

        // Character sprite using helper extension
        if (
            this.ImageRefs.Count > 0
            && this.poseIndex >= 0
            && this.poseIndex < this.ImageRefs.Count
        )
        {
            var img = this.ImageRefs[this.poseIndex];

            if (img is { Id: not null })
            {
                var (aspectRatio, scaledWidth, scaledHeight) =
                    this.GetDimensions()
                    ?? (
                        1f,
                        this.charSizePx * this.canvasConfig.Scale,
                        this.charSizePx * this.canvasConfig.Scale
                    );

                var centerX =
                    this.canvasConfig.DisplayWidth / 2f
                    + this.canvasConfig.ScaleToDisplay(this.offsetX);
                var centerY =
                    this.canvasConfig.DisplayHeight / 2f
                    + this.canvasConfig.ScaleToDisplay(this.offsetY);
                var rotation = this.rotationDeg * MathF.PI / 180.0f;

                this.ctx.DrawImageCentered(
                    img,
                    centerX,
                    centerY,
                    scaledWidth,
                    scaledHeight,
                    rotation,
                    this.flipped
                );
            }
        }

        // Text using helper extension
        if (!string.IsNullOrWhiteSpace(this.text))
        {
            var shadow = this.textShadow
                ? TextShadow.DropShadow(4 * this.canvasConfig.Scale, 2 * this.canvasConfig.Scale)
                : null;
            var font =
                $"{this.textSize * this.canvasConfig.Scale}px {availableFonts[this.textFontFamily]}";
            var x =
                this.canvasConfig.DisplayWidth / 2f + this.canvasConfig.ScaleToDisplay(this.textX);
            var y =
                this.canvasConfig.DisplayHeight / 2f + this.canvasConfig.ScaleToDisplay(this.textY);

            this.ctx.DrawStyledText(
                this.text,
                x,
                y,
                font,
                this.textHex,
                this.textAlign,
                TextBaseline.Middle,
                shadow
            );
        }
    }

    // ---------- Control handlers (update state only; Render() runs next frame) ----------

    private void OnPresetChanged(ChangeEventArgs e)
    {
        this.presetKey = (string)e.Value!;
        var (w, h) = presets[this.presetKey];
        this.canvasConfig = ResponsiveCanvasHelper.ResponsiveCanvasConfig.Create(w, h);
        this.ResizeCanvas();
    }

    private void OnBackgroundChanged(ChangeEventArgs e)
    {
        this.backgroundType = (string)e.Value!;
    }

    private void OnBgColorChanged(ChangeEventArgs e) => this.bgHex = (string)e.Value!;

    private void OnPoseChanged(ChangeEventArgs e) => this.poseIndex = int.Parse((string)e.Value!);

    private void OnCharSizeChanged(ChangeEventArgs e) =>
        this.charSizePx = int.Parse((string)e.Value!);

    private void OnRotationChanged(ChangeEventArgs e) =>
        this.rotationDeg = int.Parse((string)e.Value!);

    private void OnXChanged(ChangeEventArgs e) => this.offsetX = int.Parse((string)e.Value!);

    private void OnYChanged(ChangeEventArgs e) => this.offsetY = int.Parse((string)e.Value!);

    private void Flip() => this.flipped = !this.flipped;

    private void OnTextChanged(ChangeEventArgs e) => this.text = (string?)e.Value ?? "";

    private void OnTextSizeChanged(ChangeEventArgs e) =>
        this.textSize = int.Parse((string)e.Value!);

    private void OnTextXChanged(ChangeEventArgs e) => this.textX = int.Parse((string)e.Value!);

    private void OnTextYChanged(ChangeEventArgs e) => this.textY = int.Parse((string)e.Value!);

    private void OnTextAlignChanged(ChangeEventArgs e)
    {
        this.textAlign = ((string)e.Value!) switch
        {
            "left" => TextAlign.Left,
            "right" => TextAlign.Right,
            _ => TextAlign.Center,
        };
    }

    private void OnTextFontChanged(ChangeEventArgs e) => this.textFontFamily = (string)e.Value!;

    private void OnTextColorChanged(ChangeEventArgs e) => this.textHex = (string)e.Value!;

    private void OnTextShadowToggle(ChangeEventArgs e) => this.textShadow = (bool)e.Value!;

    // ---------- Export using extension method ----------
    private async Task ExportAsync()
    {
        if (this.canvas is null || this.isExporting)
            return;

        try
        {
            this.isExporting = true;
            this.StateHasChanged();

            var fileName =
                $"vf_{this.Character?.Slug ?? "character"}_pose{this.poseIndex}_{this.canvasConfig.ActualWidth}x{this.canvasConfig.ActualHeight}_{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            await this.canvasManager!.RenderAndExportAsync(
                this.JSRuntime,
                this.canvasConfig.ActualWidth,
                this.canvasConfig.ActualHeight,
                fileName,
                this.RenderExportCanvas
            );
        }
        finally
        {
            this.isExporting = false;
            this.StateHasChanged();
        }
    }

    private void RenderExportCanvas(ICanvas exportCanvas)
    {
        var ctx = exportCanvas.RenderContext;

        // Clear with background color using extension
        exportCanvas.Clear(this.bgHex);

        // Background using helper extension
        if (this.backgroundType != "solid")
        {
            var bgRef = this.BackgroundPatterns.FirstOrDefault(x => this.backgroundType == x.Name);
            var bgImageRef = this.BackgroundRefs[bgRef.Index];

            if (bgImageRef.Id != null)
            {
                ctx.DrawBackgroundImageCover(
                    bgImageRef,
                    bgRef.Width,
                    bgRef.Height,
                    this.canvasConfig.ActualWidth,
                    this.canvasConfig.ActualHeight
                );
            }
        }

        // Character sprite using helper extension
        if (
            this.ImageRefs.Count > 0
            && this.poseIndex >= 0
            && this.poseIndex < this.ImageRefs.Count
        )
        {
            var img = this.ImageRefs[this.poseIndex];

            if (img is { Id: not null })
            {
                var (aspectRatio, scaledWidth, scaledHeight) =
                    this.GetExportDimensions() ?? (1f, this.charSizePx, this.charSizePx);

                var centerX = this.canvasConfig.ActualWidth / 2f + this.offsetX;
                var centerY = this.canvasConfig.ActualHeight / 2f + this.offsetY;
                var rotation = this.rotationDeg * MathF.PI / 180.0f;

                ctx.DrawImageCentered(
                    img,
                    centerX,
                    centerY,
                    scaledWidth,
                    scaledHeight,
                    rotation,
                    this.flipped
                );
            }
        }

        // Text using helper extension
        if (!string.IsNullOrWhiteSpace(this.text))
        {
            var shadow = this.textShadow ? TextShadow.DropShadow() : null;
            var font = $"{this.textSize}px {availableFonts[this.textFontFamily]}";
            var x = this.canvasConfig.ActualWidth / 2f + this.textX;
            var y = this.canvasConfig.ActualHeight / 2f + this.textY;

            ctx.DrawStyledText(
                this.text,
                x,
                y,
                font,
                this.textHex,
                this.textAlign,
                TextBaseline.Middle,
                shadow
            );
        }
    }

    private (float aspectRation, float width, float height)? GetDimensions()
    {
        if (this.Character is not Series2Token character)
        {
            return null;
        }

        var imageData = this.poseIndex switch
        {
            0 => character.ClassicPoseImage,
            1 => character.CompetingPoseImage,
            2 => character.EvolvingPoseImage,
            3 => character.ManifestingPoseImage,
            4 => character.RestingPoseImage,
            5 => character.StrategizingPoseImage,
            6 => character.DiamondImage,
            7 => character.EmeraldImage,
            8 => character.LavaImage,
            9 => character.GoldImage,
            10 => character.HologramImage,
            11 => character.MangaImage,
            12 => character.BubbleGumImage,
            13 => character.TitleImageNoLogo,
            _ => null,
        };

        if (imageData is null)
        {
            return null;
        }

        // Calculate scaled dimensions maintaining aspect ratio
        var originalWidth = (float)imageData.Width;
        var originalHeight = (float)imageData.Height;
        var aspectRatio = originalWidth / originalHeight;

        float scaledWidth,
            scaledHeight;

        if (aspectRatio > 1) // Width is greater than height
        {
            scaledWidth = this.charSizePx * this.canvasConfig.Scale;
            scaledHeight = (this.charSizePx * this.canvasConfig.Scale) / aspectRatio;
        }
        else // Height is greater than or equal to width
        {
            scaledHeight = this.charSizePx * this.canvasConfig.Scale;
            scaledWidth = (this.charSizePx * this.canvasConfig.Scale) * aspectRatio;
        }

        return (aspectRatio, scaledWidth, scaledHeight);
    }

    private (float aspectRation, float width, float height)? GetExportDimensions()
    {
        if (this.Character is not Series2Token character)
        {
            return null;
        }

        var imageData = this.poseIndex switch
        {
            0 => character.ClassicPoseImage,
            1 => character.CompetingPoseImage,
            2 => character.EvolvingPoseImage,
            3 => character.ManifestingPoseImage,
            4 => character.RestingPoseImage,
            5 => character.StrategizingPoseImage,
            6 => character.DiamondImage,
            7 => character.EmeraldImage,
            8 => character.LavaImage,
            9 => character.GoldImage,
            10 => character.HologramImage,
            11 => character.MangaImage,
            12 => character.BubbleGumImage,
            13 => character.TitleImageNoLogo,
            _ => null,
        };

        if (imageData is null)
        {
            return null;
        }

        // Calculate dimensions at full resolution for export
        var originalWidth = (float)imageData.Width;
        var originalHeight = (float)imageData.Height;
        var aspectRatio = originalWidth / originalHeight;

        float scaledWidth,
            scaledHeight;

        if (aspectRatio > 1) // Width is greater than height
        {
            scaledWidth = this.charSizePx;
            scaledHeight = this.charSizePx / aspectRatio;
        }
        else // Height is greater than or equal to width
        {
            scaledHeight = this.charSizePx;
            scaledWidth = this.charSizePx * aspectRatio;
        }

        return (aspectRatio, scaledWidth, scaledHeight);
    }
}
