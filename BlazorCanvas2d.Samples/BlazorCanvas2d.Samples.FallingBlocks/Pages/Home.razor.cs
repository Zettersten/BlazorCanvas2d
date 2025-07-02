using Microsoft.AspNetCore.Components;
using Game = BlazorCanvas2d.Samples.FallingBlocks.FallingBlocksGame;

namespace BlazorCanvas2d.Samples.FallingBlocks.Pages;

public partial class Home
{
    private CanvasManager? canvasManager;
    private IRenderContext? context;
    private Func<ValueTask>? focusAction;
    private float lastRenderTime = 0;
    private float lastTickTime = 0;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        this.canvasManager?.CreateCanvas(
            "main",
            new CanvasCreationOptions()
            {
                Hidden = false,
                Width = Game.W,
                Height = Game.H,
                OnCanvasReady = this.OnMainCanvasReady,
                OnFrameReady = this.OnMainFrameReady,
                OnKeyUp = (key) =>
                {
                    if (this.context == null || Game.Lose)
                    {
                        return;
                    }

                    Game.KeyUp(key.KeyCode);
                },
            }
        );
    }

    private void OnMainCanvasReady(ICanvas canvas)
    {
        this.context = canvas.RenderContext;
        this.focusAction = () => canvas.ElementReference.FocusAsync(true);

        Game.Lose = true;
    }

    private void OnMainFrameReady(float timestamp)
    {
        if (this.context == null || Game.Lose)
        {
            return;
        }

        // Render every 30ms
        if (timestamp - this.lastRenderTime >= 30f)
        {
            Game.Render(this.context);
            this.lastRenderTime = timestamp;
        }

        // Tick every 400ms
        if (timestamp - this.lastTickTime >= 400f)
        {
            Game.Tick();
            this.lastTickTime = timestamp;
        }
    }

    private async Task OnNewGame()
    {
        if (this.context == null)
        {
            return;
        }

        this.lastRenderTime = 0;
        this.lastTickTime = 0;

        Game.Init();
        Game.NewShape();
        Game.Lose = false;

        if (this.focusAction is not null)
        {
            await this.focusAction.Invoke();
        }
    }
}
