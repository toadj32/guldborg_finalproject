using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class ProgressBar
{
    private float _pathStart;   
    private float _pathEnd;      
    private Ball _ball;
    private SpriteFont _font;
    private Texture2D _pixel;
    
    private int _barXPos;
    private int _barYPos;
    private int _barWidth;
    private int _barHeight;

    public ProgressBar(float pathStart, 
        float pathEnd, 
        Ball ball, 
        SpriteFont font, 
        Texture2D pixel,
        int barXPos, 
        int barYPos, 
        int barWidth = 20, 
        int barHeight = 200)
    {
        _pathStart  = pathStart;
        _pathEnd    = pathEnd;
        _ball       = ball;
        _font       = font;
        _pixel      = pixel;
        _barXPos       = barXPos;
        _barYPos       = barYPos;
        _barWidth   = barWidth;
        _barHeight  = barHeight;
    }

    private float GetProgress()
    {
        float distance = (_pathEnd - _pathStart);
        float progress = (_ball.Center().Y - _pathStart) / (distance);
        return MathHelper.Clamp(progress, 0f, 1f);
        //return as decimal
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float progress = GetProgress();
        int coloredAmount = (int)(_barHeight * progress); //int because I want it as a percentage (no decimals)
        
        spriteBatch.Draw(_pixel, //can use the one from Game made for holes
            new Rectangle(_barXPos, _barYPos, _barWidth, _barHeight),
            Color.DarkGray);
        
        spriteBatch.Draw(_pixel,
            new Rectangle(_barXPos, _barYPos, _barWidth, coloredAmount),
            Color.LightGreen);
        
        string percentage = $"{(int)(progress * 100)}%";
        Vector2 Size = _font.MeasureString(percentage);
        spriteBatch.DrawString(
            _font, 
            percentage,
            new Vector2(_barXPos + _barWidth / 2f - Size.X / 2f, _barYPos + _barHeight + 8),
            Color.White);
    }
}