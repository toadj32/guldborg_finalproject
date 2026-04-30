using System;
using Final_Project;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Pause
{
    // Return to game
    // Exit game: save high score in a csv file and exit game.
    // Volume settings: a slider mechanism to adjust overall volume.
    private Vector2 _lastAcc;
    private Vector2 _lastSpeed;
    private float _lastScrollSpeed;
    
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

    private Texture2D _settingScreen;
    private Rectangle _settingScrnRect;
    private Rectangle _settingsScrnSrcRect;
    
    private Texture2D _vol;
    private Rectangle _volRect;
    private Rectangle _volSrcRect;
    public float volPer;
    private bool _draggingSlider = false;
    
    private Texture2D _ball1;
    private Rectangle _ball1Rect;
    private Rectangle _ball1SrcRect;
    
    private Texture2D _ball2;
    private Rectangle _ball2Rect;
    private Rectangle _ball2SrcRect;
    
    private Texture2D _check;
    private Rectangle _checkRect;

    private Texture2D _back;
    private Rectangle _backRect;
    private Rectangle _backSrcRect;
    
    private MouseState _lastmouseState;
    public bool defaultColor { set; get; }
    public bool _paused = false;
    private bool _sett = false;
    public bool _exit = false;
    private Matrix world;

    // make block bool that let it be used or not.
    
    public Pause(Texture2D pause, Texture2D pauseScreen, Texture2D exit, Texture2D resume, Texture2D settings, Texture2D settingsScreen, Texture2D volumeButton, Texture2D ball1, Texture2D ball2, Texture2D check, Texture2D back, Vector2 screenSize)
    {
        volPer = 0.5f;
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

        _settingScreen = settingsScreen;
        _settingScrnRect = new Rectangle(155, 155, 1203, 1206);
        _settingsScrnSrcRect = new Rectangle(205, 467, (int)(1201), (int)(1203));
        
        _ball1 = ball1;
        _ball1Rect = new Rectangle(410, 500, 218, 206);
        _ball1SrcRect = new Rectangle(559, 866, 218, 206);
        
        _ball2 = ball2;
        _ball2Rect = new Rectangle(660, 500, 218, 206);
        _ball2SrcRect = new Rectangle(868, 866, 218, 206);
        
        _check = check;
        _checkRect = new Rectangle(_ball1Rect.X, _ball1Rect.Y, _ball1Rect.Width/4, _ball1Rect.Height/2);
        
        _back = back;
        _backRect = new Rectangle(915, 240, 111, 111);
        _backSrcRect = new Rectangle(1214, 495, 111, 111);

        // Range = 430 - 735
        _vol = volumeButton;
        _volRect = new Rectangle(430+(int)((735-430)*volPer), 775, 88, 94);
        _volSrcRect = new Rectangle(567, 1298, 88, 94);

        defaultColor = true;
        world = Matrix.CreateTranslation(new Vector3(50f, 50f, 0f)) * Matrix.CreateScale(_scale);
    }

    public void Update(GameTime gameTime, Vector2 mousePos, MouseState mouseState, Ball ball, Game game, CoinManager coins)
    {
        if (_pauseRect.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released && !_paused)
        {
            _lastAcc = ball._acc;
            _lastSpeed = ball.Velocity;
            _lastScrollSpeed = ball.ScrollSpeed;
            ball._acc =  Vector2.Zero;
            ball.Velocity = Vector2.Zero;
            ball.ScrollSpeed = 0f;
            coins._interval = float.MaxValue;
            _paused = true;
        }

        if (_paused)
        {
            
            if (_sett)
            {
                if (TransformRect(_ball1Rect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed &&
                    _lastmouseState.LeftButton == ButtonState.Released && !defaultColor)
                {
                    _checkRect = new Rectangle(_ball1Rect.X, _ball1Rect.Y, _ball1Rect.Width/4, _ball1Rect.Height/2);
                    defaultColor = true;
                }
                if (TransformRect(_ball2Rect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed &&
                    _lastmouseState.LeftButton == ButtonState.Released && defaultColor)
                {
                    _checkRect = new Rectangle(_ball2Rect.X, _ball2Rect.Y, _ball2Rect.Width/4, _ball2Rect.Height/2);
                    defaultColor = false;
                }

                if (TransformRect(_backRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed &&
                    _lastmouseState.LeftButton == ButtonState.Released)
                {
                    _sett = false;
                }

                if (_draggingSlider || (TransformRect(_volRect, world).Contains(mousePos)) &&
                    mouseState.LeftButton == ButtonState.Pressed)
                {
                    _draggingSlider = mouseState.LeftButton == ButtonState.Pressed;

                    int clampX = (int)Math.Clamp(mousePos.X, TransformRect(new Rectangle(430, 0, 0, 0), world).X,
                        TransformRect(new Rectangle(735, 0, 0, 0), world).X);
                    float localX = Vector2.Transform(new Vector2(clampX, 0), Matrix.Invert(world)).X;
                    volPer = (localX - 430) / (735 - 430);

                    _volRect.X = 430 + (int)((735 - 430) * volPer);

                }
            }
            else
            {
                if (TransformRect(_settingRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released)
                {
                    _sett = true;
                }
                
                if (TransformRect(_exitRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released)
                {
                    _exit = true;
                    game.Exit();
                }
        
                if (TransformRect(_resumeRect, world).Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && _lastmouseState.LeftButton == ButtonState.Released)
                {
                    ball._acc = _lastAcc;
                    ball.Velocity = _lastSpeed;
                    ball.ScrollSpeed = _lastScrollSpeed;
                    coins._interval = 2f;
                    _paused = false;
                }
            }
            
        }

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
            if (!_sett)
            {
                spriteBatch.Begin(transformMatrix: world);
                spriteBatch.Draw(_pauseScreen, new Vector2(_pauseScreenRect.X, _pauseScreenRect.Y), _pauseSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_exitButton, new Vector2(_exitRect.X, _exitRect.Y), _exitSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_resume, new Vector2(_resumeRect.X, _resumeRect.Y), _resumeSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_settings, new Vector2(_settingRect.X, _settingRect.Y), _settingSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Begin(transformMatrix: world);
                spriteBatch.Draw(_settingScreen, new Vector2(_settingScrnRect.X, _settingScrnRect.Y), _settingsScrnSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_ball1, new Vector2(_ball1Rect.X, _ball1Rect.Y), _ball1SrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_ball2, new Vector2(_ball2Rect.X, _ball2Rect.Y), _ball2SrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_check, new Vector2(_checkRect.X, _checkRect.Y), null, Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_back, new Vector2(_backRect.X, _backRect.Y), _backSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(_vol, new Vector2(_volRect.X, _volRect.Y), _volSrcRect, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    public void SyncSlider()
    {
        _volRect.X = 430 + (int)((735 - 430) * volPer);
    }
    
}