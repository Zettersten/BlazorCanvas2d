namespace BlazorCanvas2d.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    private object? _fillStyle;

    public object? FillStyle
    {
        get => this._fillStyle;
        set
        {
            this._fillStyle = value;
            this.SetProperty("fillStyle", value);
        }
    }

    private string? _strokeStyle;

    public string? StrokeStyle
    {
        get => this._strokeStyle;
        set
        {
            this._strokeStyle = value;
            this.SetProperty("strokeStyle", value);
        }
    }

    private int _lineWidth;

    public int LineWidth
    {
        get => this._lineWidth;
        set
        {
            this._lineWidth = value;
            this.SetProperty("lineWidth", value);
        }
    }

    private string? _font;

    public string? Font
    {
        get => this._font;
        set
        {
            this._font = value;
            this.SetProperty("font", value);
        }
    }

    private TextAlign? _textAlign;

    public TextAlign? TextAlign
    {
        get => this._textAlign;
        set
        {
            this._textAlign = value;
            this.SetProperty("textAlign", value?.Value);
        }
    }

    private TextBaseline? _textBaseline;

    public TextBaseline? TextBaseline
    {
        get => this._textBaseline;
        set
        {
            this._textBaseline = value;
            this.SetProperty("textBaseline", value?.Value);
        }
    }

    private TextRendering? _textRendering;

    public TextRendering? TextRendering
    {
        get => this._textRendering;
        set
        {
            this._textRendering = value;
            this.SetProperty("textRendering", value?.Value);
        }
    }

    private Filter? _filter;

    public Filter? Filter
    {
        get => this._filter;
        set
        {
            this._filter = value;
            this.SetProperty("filter", value?.Value);
        }
    }

    private float _globalAlpha;

    public float GlobalAlpha
    {
        get => this._globalAlpha;
        set
        {
            this._globalAlpha = value;
            this.SetProperty("globalAlpha", value);
        }
    }

    private GlobalCompositeOperation? _globalCompositeOperation;

    public GlobalCompositeOperation? GlobalCompositeOperation
    {
        get => this._globalCompositeOperation;
        set
        {
            this._globalCompositeOperation = value;
            this.SetProperty("globalCompositeOperation", value?.Value);
        }
    }

    private ImageSmoothingQuality? _imageSmoothingQuality;

    public ImageSmoothingQuality? ImageSmoothingQuality
    {
        get => this._imageSmoothingQuality;
        set
        {
            this._imageSmoothingQuality = value;
            this.SetProperty("imageSmoothingQuality", value?.Value);
        }
    }

    private bool _imageSmoothingEnabled;

    public bool ImageSmoothingEnabled
    {
        get => this._imageSmoothingEnabled;
        set
        {
            this._imageSmoothingEnabled = value;
            this.SetProperty("imageSmoothingEnabled", value);
        }
    }

    private string? _shadowColor;

    public string? ShadowColor
    {
        get => this._shadowColor;
        set
        {
            this._shadowColor = value;
            this.SetProperty("shadowColor", value);
        }
    }

    private float _shadowOffsetX;

    public float ShadowOffsetX
    {
        get => this._shadowOffsetX;
        set
        {
            this._shadowOffsetX = value;
            this.SetProperty("shadowOffsetX", value);
        }
    }

    private float _shadowOffsetY;

    public float ShadowOffsetY
    {
        get => this._shadowOffsetY;
        set
        {
            this._shadowOffsetY = value;
            this.SetProperty("shadowOffsetY", value);
        }
    }

    private float _shadowBlur;

    public float ShadowBlur
    {
        get => this._shadowBlur;
        set
        {
            this._shadowBlur = value;
            this.SetProperty("shadowBlur", value);
        }
    }
}
