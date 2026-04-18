using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Rectangle[] _inViewObjs;
    private Ball _ball;
    private Vector2 _ballPos;
    private Pause _pauseScreen;
    private Vector2 _mousePos;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _graphics.PreferredBackBufferHeight = 1500;
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.ApplyChanges();
        
        // Ball!
        Texture2D ball = Content.Load<Texture2D>("ball");
        _ballPos =  new Vector2((_graphics.PreferredBackBufferWidth - (100f))/2f, 0f);
        _ball = new Ball(ball, _ballPos);
        
        // Pause Screen!
        Texture2D pause = Content.Load<Texture2D>("pauseButton");
        Texture2D pauseScreen = Content.Load<Texture2D>("pause");
        Texture2D exit = Content.Load<Texture2D>("exit");
        Texture2D resume =  Content.Load<Texture2D>("resume");
        Texture2D settings = Content.Load<Texture2D>("settings");
        _pauseScreen = new Pause(pause, pauseScreen, exit, resume, settings, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        MouseState ms = Mouse.GetState();
        _mousePos = new Vector2(ms.X, ms.Y);
        
        _ball.Update(gameTime);
        RelevantObj();
        _pauseScreen.Update(gameTime, _mousePos, ms, _ball, this);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _ball.Draw(_spriteBatch);
        _ball.Collide(_inViewObjs);
        _pauseScreen.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void RelevantObj()
    {
        _inViewObjs = new Rectangle[0];
        // Checks for objects in view and updates _inViewObjs
        // Should sort relevant objs in terms of closeness to ball
    }
}