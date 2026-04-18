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
        
        Texture2D ball = Content.Load<Texture2D>("ball");
        _ballPos =  new Vector2((_graphics.PreferredBackBufferWidth - (100f))/2f, 0f);
        _ball = new Ball(ball, _ballPos);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _ball.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _ball.Draw(_spriteBatch);
        _ball.Collide(_inViewObjs);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void RelevantObj()
    {
        // Checks for objects in view and updates _inViewObjs
        // Should sort relevant objs in terms of closeness to ball
    }
}