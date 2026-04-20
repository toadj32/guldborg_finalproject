using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class ProgressBar
{
    private float _pathStart;    // Y where the path begins
    private float _pathEnd;      // Y where the path ends
    private Ball _ball;
    private SpriteFont _font;
    private Texture2D _pixel;
    
    // Bar dimensions and position on screen
    private int _barX;
    private int _barY;
    private int _barWidth;
    private int _barHeight;

    public ProgressBar(float pathStart, float pathEnd, Ball ball, SpriteFont font, Texture2D pixel,
        int barX, int barY, int barWidth = 20, int barHeight = 200)
    {
        _pathStart  = pathStart;
        _pathEnd    = pathEnd;
        _ball       = ball;
        _font       = font;
        _pixel      = pixel;
        _barX       = barX;
        _barY       = barY;
        _barWidth   = barWidth;
        _barHeight  = barHeight;
    }

    private float GetProgress()
    {
        float progress = (_ball.Center().Y - _pathStart) / (_pathEnd - _pathStart);
        return MathHelper.Clamp(progress, 0f, 1f);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float progress = GetProgress();
        int filledHeight = (int)(_barHeight * progress);

        // Background (empty bar)
        spriteBatch.Draw(_pixel,
            new Rectangle(_barX, _barY, _barWidth, _barHeight),
            Color.DarkGray);

        // Filled portion — grows downward
        spriteBatch.Draw(_pixel,
            new Rectangle(_barX, _barY, _barWidth, filledHeight),
            Color.LimeGreen);

        // Percentage text below the bar
        string pct = $"{(int)(progress * 100)}%";
        Vector2 textSize = _font.MeasureString(pct);
        spriteBatch.DrawString(
            _font, pct,
            new Vector2(_barX + _barWidth / 2f - textSize.X / 2f, _barY + _barHeight + 8),
            Color.White);
    }
}