using BlazorCanvas2d.Helpers;

namespace BlazorCanvas2d.Tests;

public sealed class ResponsiveCanvasHelperTests
{
    [Fact]
    public void CalculateDisplayDimensions_WhenWithinLimits_ReturnsOriginalAndScale1()
    {
        var (w, h, scale) = ResponsiveCanvasHelper.CalculateDisplayDimensions(
            targetWidth: 320,
            targetHeight: 240,
            maxDisplayWidth: 640,
            maxDisplayHeight: 480
        );

        Assert.Equal(320, w);
        Assert.Equal(240, h);
        Assert.Equal(1.0f, scale);
    }

    [Fact]
    public void CalculateDisplayDimensions_WhenTooLarge_ScalesDownMaintainingAspectRatio()
    {
        var (w, h, scale) = ResponsiveCanvasHelper.CalculateDisplayDimensions(
            targetWidth: 1920,
            targetHeight: 1080,
            maxDisplayWidth: 640,
            maxDisplayHeight: 480
        );

        Assert.True(w <= 640);
        Assert.True(h <= 480);
        Assert.True(scale > 0f);

        // aspect ratio should be preserved within rounding
        var targetAspect = 1920f / 1080f;
        var displayAspect = w / (float)h;
        Assert.InRange(displayAspect, targetAspect - 0.02f, targetAspect + 0.02f);
    }

    [Fact]
    public void ResponsiveCanvasConfig_Create_SetsIsScaledCorrectly()
    {
        var cfg1 = ResponsiveCanvasHelper.ResponsiveCanvasConfig.Create(320, 240, 640, 480);
        Assert.False(cfg1.IsScaled);

        var cfg2 = ResponsiveCanvasHelper.ResponsiveCanvasConfig.Create(1920, 1080, 640, 480);
        Assert.True(cfg2.IsScaled);
        Assert.True(cfg2.Scale is > 0f and < 1f);
    }
}

