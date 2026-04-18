using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Pause
{
    // Return to game
    // Exit game: save high score in a csv file and exit game.
    // Volume settings: a slider mechanism to adjust overall volume.
    
    private Texture2D _pauseButton;
    private Rectangle _pauseRect;

    private Texture2D _pauseScreen;
    private Rectangle _pauseScreenRect;

    private Texture2D _exitButton;
    private Rectangle _exitRect;
    private MouseState _lastmouseState;
    private bool _paused = false;

    public Pause(Texture2D pause, Texture2D pauseScreen, Texture2D exit, Vector2 screenSize)
    {
        _pauseButton = pause;
        _pauseRect = new Rectangle((int)(screenSize.X - 100f), 50, 70, 25);
        _pauseScreen = pauseScreen;
        _pauseScreenRect = new Rectangle(100, 100, (int)(screenSize.X - 200), 900);
        _exitButton = exit;
        _exitRect = new Rectangle((int)(screenSize.X - 100f), (int)(screenSize.Y - 200f), 100, 100);
    }

    public void Update(GameTime gameTime, Vector2 mousePos, MouseState mouseState, Ball ball)
    {
        if (_pauseRect.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released)
        {
            ball._acc =  Vector2.Zero;
            ball._velocity = Vector2.Zero;
            _paused = true;
            _lastmouseState = mouseState;
        }

        if (_exitRect.Contains(mousePos))
        {
            _paused = false;
        }
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_paused)
        {
            spriteBatch.Draw(_pauseButton, _pauseRect, Color.Red);
        }
        else
        {
            spriteBatch.Draw(_pauseScreen, _pauseScreenRect, Color.Cornsilk);
        }
    }
    
}