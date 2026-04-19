using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Game1 : Game
{
    PathTracker.PathBounds paths = new PathTracker.PathBounds();
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BasicEffect _basicEffect;
    
    private Pause _pauseScreen;
    private Vector2 _mousePos;
    private Vector2 _ballPos;
    
    private Background _bg;
    private Ball _ball;

    private int width = 450;
    private int height = 800;

    private float ScrollRate = 100f;
    private Texture2D obstacleTex;

    //GUI
    private SpriteFont _font;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferHeight = height;
        _graphics.PreferredBackBufferWidth = width+200;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _basicEffect = new BasicEffect(GraphicsDevice);
        
        // Pause Screen!
        Texture2D pause = Content.Load<Texture2D>("pauseButton");
        Texture2D pauseScreen = Content.Load<Texture2D>("pause");
        Texture2D exit = Content.Load<Texture2D>("exit");
        Texture2D resume =  Content.Load<Texture2D>("resume");
        Texture2D settings = Content.Load<Texture2D>("settings");
        _pauseScreen = new Pause(pause, pauseScreen, exit, resume, settings, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        
        _bg = new Background(
            Content.Load<Texture2D>("Adobe Express - file"), 
            10,
            width);
        
        Texture2D ball = Content.Load<Texture2D>("ball");
        _ballPos =  new Vector2((_graphics.PreferredBackBufferWidth - (100f))/2f, 0f);
        _ball = new Ball(ball, _ballPos, 0.5f);
        
        /*_ball = new Ball(
            Content.Load<Texture2D>(
                "team_1ball.png.f7a4f30e7e4a09bd0ecb3135a3fcfc4f"),
            new Vector2((width+200)/2f, height/2f),
            ScrollRate,
            36,
            0.5f);*/

        obstacleTex = Content.Load<Texture2D>("wood-texture-close");
        Texture2D _blackPixel = new Texture2D(GraphicsDevice, 1, 1);
        _blackPixel.SetData(new[] { Color.Black});
        
        //Obstacles
        paths.AddPath(
            new Rectangle(100, 500, 100, 50),
            PathType.Obstacle,
            obstacleTex);
        paths.AddPath(
            new Rectangle(300, 800, 100, 50),
            PathType.Obstacle,
            obstacleTex,
            new List<Vector2>
            {
                new Vector2(300, 850),
                new Vector2(400, 850),
                new Vector2(400, 800)}
            );
        paths.AddPath(new Rectangle(450, 1000, 20, 120), PathType.Obstacle, obstacleTex);
        
        //holes
        
        paths.AddPath(new Rectangle( 100, 1200, 400, 80), PathType.Hole, _blackPixel);
        
        //path
        paths.AddPath(new Rectangle(0, 350, 450, 4200), PathType.Path);
       
        _font = Content.Load<SpriteFont>("Courier New");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        MouseState ms = Mouse.GetState();
        _mousePos = new Vector2(ms.X, ms.Y);
        
        if (_ball.FellOff())
            return;
        _ball.Update(gameTime, paths);
        _pauseScreen.Update(gameTime, _mousePos, ms, _ball, this);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        Vector2 cameraPosition = _ball.Position - new Vector2((width+200)/2f, height / 2f);
        
        _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
        _bg.Draw(_spriteBatch, cameraPosition);          
        _spriteBatch.End();
        
        _spriteBatch.Begin();  
        paths.BoundsDraw(_spriteBatch, cameraPosition);
        _ball.Draw(_spriteBatch, cameraPosition);
        
        if (_ball.FellOff())
        {
            string losingMessage = "You Died :( womp womp";
            Vector2 size = 4*_font.MeasureString(losingMessage);
            Vector2 origin = new Vector2((width + 200) / 2f - size.X / 8f, height / 2f - size.Y/2f);
            _spriteBatch.DrawString(_font, losingMessage, origin, Color.White);
        }
        _spriteBatch.End();
        
        paths.DrawNonSquares(GraphicsDevice, _basicEffect, cameraPosition);
        
        base.Draw(gameTime);
    }
}