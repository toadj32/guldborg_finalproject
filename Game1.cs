using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Rectangle[] _inViewObj;
    private Ball _ball;
    private Vector2 _ballPos;
    private float _scale = 0.5f;

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
        _ballPos =  new Vector2((_graphics.PreferredBackBufferWidth - (ball.Width*_scale)), 0f);
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

        Matrix world = Matrix.CreateScale(_scale);
        _spriteBatch.Begin(transformMatrix: world);
        _ball.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}