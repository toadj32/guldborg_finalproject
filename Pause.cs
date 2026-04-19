using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Pause
{
    // Return to game
    // Exit game: save high score in a csv file and exit game.
    // Volume settings: a slider mechanism to adjust overall volume.
    private Vector2 _lastAcc;
    private Vector2 _lastSpeed;
    
    private Texture2D _pauseButton;
    private Rectangle _pauseRect;
    private float _pauseButtonScale = 0.1f;

    private float _scale = 0.5f;
    private Texture2D _pauseScreen;
    private Rectangle _pauseScreenRect;
    private Rectangle _pauseSrcRect;

    private Texture2D _exitButton;
    private Rectangle _exitRect;
    private Rectangle _exitSrcRect;

    private Texture2D _resume;
    private Rectangle _resumeRect;
    private Rectangle _resumeSrcRect;

    private Texture2D _settings;
    private Rectangle _settingRect;
    private Rectangle _settingSrcRect;
    
    private MouseState _lastmouseState;
    private bool _paused = false;
    private Matrix world;

    public Pause(Texture2D pause, Texture2D pauseScreen, Texture2D exit, Texture2D resume, Texture2D settings, Vector2 screenSize)
    {
        _pauseButton = pause;
        _pauseRect = new Rectangle((int)(screenSize.X - 100f), 0, (int)(_pauseButton.Width * _pauseButtonScale), (int)(_pauseButton.Height*_pauseButtonScale));
        
        _pauseScreen = pauseScreen;
        _pauseSrcRect = new Rectangle(217, 469, 1203, 1206);
        _pauseScreenRect = new Rectangle(160, 160, (int)(1203 * _scale), (int)(_scale*1206));
        
        _exitButton = exit;
        _exitRect = new Rectangle(600, 605, (int)(_scale*300), (int)(_scale*300));
        _exitSrcRect = new Rectangle(802, 1062, 300, 300);
        
        _resume = resume;
        _resumeRect = new Rectangle(485, 375,  (int)(_scale*313), (int)(_scale*313));
        _resumeSrcRect = new Rectangle(652, 755, 313, 313);
        
        _settings = settings;
        _settingRect = new Rectangle(360, 600,  (int)(_scale*303), (int)(_scale*303));
        _settingSrcRect = new Rectangle(485, 1056, 303, 303);
        world = Matrix.CreateTranslation(new Vector3(50f, 50f, 0f)) * Matrix.CreateScale(_scale);
    }

    public void Update(GameTime gameTime, Vector2 mousePos, MouseState mouseState, Ball ball, Game game)
    {
        if (_pauseRect.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released && !_paused)
        {
            _lastAcc = ball._acc;
            _lastSpeed = ball._velocity;
            ball._acc =  Vector2.Zero;
            ball._velocity = Vector2.Zero;
            _paused = true;
        }

        if (TransformRect(_exitRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released && _paused)
        {
            game.Exit();
        }
        
        if (TransformRect(_resumeRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released && _paused)
        {
            ball._acc = _lastAcc;
            ball._velocity = _lastSpeed;
            _paused = false;
        }
        Console.WriteLine(TransformRect(_exitRect, world));
        Console.WriteLine(TransformRect(_resumeRect, world));

        _lastmouseState = mouseState;
    }

    private Rectangle TransformRect(Rectangle rect, Matrix matrix)
    {
        Vector2 top = Vector2.Transform(new Vector2(rect.X, rect.Y), matrix);
        Vector2 bottom =  Vector2.Transform(new Vector2(rect.Width, rect.Height), matrix);
        return new Rectangle((int)top.X, (int)top.Y, (int)bottom.X, (int)bottom.Y);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_paused)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_pauseButton, new Vector2(_pauseRect.X, _pauseRect.Y), null, Color.White, 0f, Vector2.Zero, _pauseButtonScale, SpriteEffects.None, 0f);
        }
        else
        {
            spriteBatch.Begin(transformMatrix: world);
            spriteBatch.Draw(_pauseScreen, new Vector2(_pauseScreenRect.X, _pauseScreenRect.Y), _pauseSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_exitButton, new Vector2(_exitRect.X, _exitRect.Y), _exitSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_resume, new Vector2(_resumeRect.X, _resumeRect.Y), _resumeSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_settings, new Vector2(_settingRect.X, _settingRect.Y), _settingSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }

        spriteBatch.End();
    }
    
}