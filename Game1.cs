using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Game1 : Game
{
    PathTracker.PathBounds paths = new PathTracker.PathBounds();
    
    public enum GameState{StartScreen, Preview, Playing, GameOver, GameWin}
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BasicEffect _basicEffect;
    
    private Background _bg;
    private Ball _ball;
    private ProgressBar _progressBar;

    private int width = 450;
    private int height = 800;

    private float ScrollRate = 100f;
    private Texture2D obstacleTex;
    private Texture2D _pixelTex;
    private Texture2D _winScreenTex;

    //GUI
    private SpriteFont _font;
    
    //Game State
    private GameState _gameState = GameState.StartScreen;
    private StartScreen _startScreen;
    private PathwayPan _pathwayPan;
    
    //Coins
    private CoinManager _coins;
    private Texture2D _coinTex;

    private MouseState _previousMouseState;

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
        
        // TODO: use this.Content to load your game content here
        
        obstacleTex = Content.Load<Texture2D>("wood-texture-close");
        Texture2D _blackPixel = new Texture2D(GraphicsDevice, 1, 1);
        _blackPixel.SetData(new[] { Color.Black});
        _pixelTex = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTex.SetData(new[] { Color.AliceBlue});
        _font = Content.Load<SpriteFont>("Courier New");
        _coinTex = Content.Load<Texture2D>("coin_gold");
        _winScreenTex = Content.Load<Texture2D>("paper");
        
        _coins = new CoinManager(_coinTex, height);
        
        _bg = new Background(
            Content.Load<Texture2D>("Adobe Express - file"), 
            10,
            width);
        
        _ball = new Ball(
            Content.Load<Texture2D>(
                "team_1ball.png.f7a4f30e7e4a09bd0ecb3135a3fcfc4f"),
            new Vector2((width+200)/2f, height/2f),
            ScrollRate,
            36,
            0.5f);
        _startScreen = new StartScreen(_font, width + 200, height);
        _pathwayPan = new PathwayPan(4100f);
        _progressBar = new ProgressBar(
            400f,
            4200f,
            _ball,
            _font,
            _pixelTex,
            
            (width+200)/2 + 200,
            50);
        
        //Obstacles
        paths.AddPath(new Rectangle(100, 500, 100, 30), PathType.Obstacle, obstacleTex);
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
        paths.AddPath(new Rectangle(450, 1000, 40, 120), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(200, 1300, 100, 50), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(400, 1400, 20, 120), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(200, 1700, 300, 90), PathType.Obstacle, obstacleTex);
        
        //my little peg board
        paths.AddPath(new Rectangle(150, 2500, 50, 50), 
            PathType.Obstacle, 
            obstacleTex);
        paths.AddPath(new Rectangle(450, 2500, 50, 50), 
            PathType.Obstacle, 
            obstacleTex);
        paths.AddPath(new Rectangle(300, 2700, 50, 50), 
            PathType.Obstacle, 
            obstacleTex);
        paths.AddPath(new Rectangle(150, 2900, 50, 50), 
            PathType.Obstacle, 
            obstacleTex);
        paths.AddPath(new Rectangle(450, 2900, 50, 50), 
            PathType.Obstacle, 
            obstacleTex);
        
        paths.AddPath(new Rectangle(430, 3300, 90, 90), 
            PathType.Obstacle, 
            obstacleTex,
            new List<Vector2>
            {
                new Vector2(470, 3300),
                new Vector2(450, 3345),
                new Vector2(470, 3390),
                new Vector2(520, 3390),
                new Vector2(540, 3345),
                new Vector2(520, 3300),
            });
        
        paths.AddPath(new Rectangle(100, 3450, 100, 80),
            PathType.Obstacle,
            obstacleTex,
            new List<Vector2>
            {
                new Vector2(100, 3450),
                new Vector2(100, 3530),
                new Vector2(200, 3530)
            });
        paths.AddPath(new Rectangle(180, 3700, 20, 120), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(450, 3850, 20, 120), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(120, 4100, 80, 80), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(280, 4150, 80, 80), PathType.Obstacle, obstacleTex);
        paths.AddPath(new Rectangle(350, 4250, 100, 100), 
            PathType.Obstacle, 
            obstacleTex,
            new List<Vector2>
            {
                new Vector2(350, 4350),
                new Vector2(450, 4350),
                new Vector2(450, 4250)
            });
        
        //holes
        paths.AddPath(new Rectangle(100, 1200, 200, 80), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(300, 1500, 100, 100), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(150, 1950, 100, 100), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(400, 2150, 100, 100), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(250, 3100, 250, 80), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(250, 3550, 80, 60), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(300, 3800, 60, 200), PathType.Hole, _blackPixel);
        paths.AddPath(new Rectangle(250, 4300, 125, 50), PathType.Hole, _blackPixel);
        
        //path
        paths.AddPath(new Rectangle(100, 0, 450, 4500), PathType.Path);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        MouseState mouse = Mouse.GetState();

        switch (_gameState)
        {
            case GameState.StartScreen:
                if (_startScreen.Clicked(mouse, _previousMouseState))
                    _gameState = GameState.Preview;
                break;
            case GameState.Preview:
                _pathwayPan.Update(gameTime);
                if (mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    _gameState = GameState.Playing;
                else if (_pathwayPan.Done) //cause can't access ._done ugh
                    _gameState = GameState.Playing;
                break;
            case GameState.Playing:
                _ball.Update(gameTime, paths);
                _coins.Update(gameTime, _ball);
                if (_ball.Center().Y >= 4450f)
                    _gameState = GameState.GameWin;
                else if (_ball.FellOff())
                    _gameState = GameState.GameOver;
                break;
        }
        
        _previousMouseState = mouse; 

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        Vector2 cameraPosition = _ball.Position - new Vector2((width+200)/2f, height / 2f);

        switch (_gameState)
        {
            case GameState.StartScreen:
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    _winScreenTex,
                    new Rectangle(0, 0, width +200, height),
                    new Rectangle(0, 0, _winScreenTex.Width, _winScreenTex.Height),
                    Color.White);
                _startScreen.Draw(_spriteBatch, _pixelTex);
                _spriteBatch.End();
                break;
            
            case GameState.Preview:
                _pathwayPan.Draw(_spriteBatch, _bg, paths, _basicEffect, GraphicsDevice);
                break;
            case GameState.Playing: 
                _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
                _bg.Draw(_spriteBatch, cameraPosition);          
                _spriteBatch.End();
                
                _spriteBatch.Begin();  
                paths.BoundsDraw(_spriteBatch, cameraPosition);
                _ball.Draw(_spriteBatch, cameraPosition);
                _progressBar.Draw(_spriteBatch);
                _coins.Draw(_spriteBatch, cameraPosition);
                _spriteBatch.DrawString(_font, $"Coins: {_coins.CoinCounter}", new Vector2(10, 10), Color.White);
                _spriteBatch.Draw(_pixelTex, new Vector2(100 - cameraPosition.X, 4400 - cameraPosition.Y), new Rectangle(100, 400, 450, 10), Color.Navy);
                _spriteBatch.End();
                
                paths.DrawNonSquares(GraphicsDevice, _basicEffect, cameraPosition);
                break;
            case GameState.GameOver:
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
                
                break;
            
            case GameState.GameWin:
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    _winScreenTex,
                    new Rectangle(0, 0, width +200, height),
                    new Rectangle(0, 0, _winScreenTex.Width, _winScreenTex.Height),
                    Color.White);
                string winningMessage = "You made it!";
                Vector2 winningsize = 4*_font.MeasureString(winningMessage);
                Vector2 winningorigin = new Vector2((width + 200) / 2f - winningsize.X / 8f, height / 2f - winningsize.Y/2f);
                _spriteBatch.DrawString(_font, winningMessage, winningorigin, Color.Black);
                _spriteBatch.End();
                break;
                
        }
        
        
        base.Draw(gameTime);
    }
}