using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Final_Project;

public class Game1 : Game
{
    PathTracker.PathBounds paths = new PathTracker.PathBounds();
    
    public enum GameState{StartScreen, Instructions, Preview, Playing, GameOver, GameWin}
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BasicEffect _basicEffect;
    
    private Background _bg;
    private Ball _ball;
    private ProgressBar _progressBar;

    private int width = 450;
    private int height = 800;
    private float ScrollRate = 80f;
    
    private Texture2D obstacleTex;
    private Texture2D _pixelTex;
    private Texture2D _winScreenTex;
    private Texture2D blockTexture;

    //GUI
    private SpriteFont _font;
    
    //SideBar
    private Sidebar _sidebar;
    
    //Game State
    private GameState _gameState = GameState.StartScreen;
    private StartScreen _startScreen;
    private PathwayPan _pathwayPan;
    
    //Coins
    private CoinManager _coins;
    private Texture2D _coinTex;

    private MouseState _previousMouseState;
    private MouseState mouse =  Mouse.GetState();
    
    //interactive components
    private List<Blocks> _blocks = new List<Blocks>();

    //pause Screen
    private Pause _pauseScreen;
    private Vector2 _mousePos;
    
    //restart button
    private Rectangle _restartButton;
    private bool _restartButtonAppears = false;
    //-------------------------------------------------------------------------------------------------/////////////
    //musica
    private Song introMusic;
    private SoundEffectInstance introMusicInstance;
    private Song gameMusic;
    private SoundEffectInstance gameMusicInstance;
    private bool musicPlaying = false;
    
    //instructions
    private Instructions instructions;
    //-------------------------------------------------------------------------------------------------///////////// ^

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
        
        // Pause Screen!
        Texture2D pause = Content.Load<Texture2D>("pauseButton");
        Texture2D pauseScreen = Content.Load<Texture2D>("pause");
        Texture2D exit = Content.Load<Texture2D>("exit");
        Texture2D resume =  Content.Load<Texture2D>("resume");
        Texture2D settings = Content.Load<Texture2D>("settings");
        _pauseScreen = new Pause(pause, pauseScreen, exit, resume, settings, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        
        //restart button 
        int _restartButtonWidth = 150;
        int _restartButtonHeight = 75;
        _restartButton = new Rectangle(
            (width + 200) / 2 - _restartButtonWidth / 2,
            height / 2 + 60,
            _restartButtonWidth,
            _restartButtonHeight);
        
        obstacleTex = Content.Load<Texture2D>("wood-texture-close");
        Texture2D _blackPixel = new Texture2D(GraphicsDevice, 1, 1);
        _blackPixel.SetData(new[] { Color.Black});
        _pixelTex = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTex.SetData(new[] { Color.AliceBlue});
        _font = Content.Load<SpriteFont>("Courier New");
        
        _coinTex = Content.Load<Texture2D>("coin_gold");
        _winScreenTex = Content.Load<Texture2D>("paper");

        blockTexture = Content.Load<Texture2D>("blocksquare");
        _sidebar = new Sidebar(_pixelTex, width + 200, height, _font, blockTexture);
        
        _coins = new CoinManager(_coinTex, height);
        
        _bg = new Background(
            Content.Load<Texture2D>("Adobe Express - file"), 
            10,
            width);
        
        
        //--------------------------------load sound-------------------------------------------------------------/////////
        Ball.ContentManager = Content;
        //--------------------------------load sound end-------------------------------------------------------------/////////

        _ball = new Ball(
            Content.Load<Texture2D>(
                "team_1ball.png.f7a4f30e7e4a09bd0ecb3135a3fcfc4f"),
            new Vector2((width+200)/2f, height/2f),
            ScrollRate,
            36,
            0.5f,
            Content);    //<<<<<<<<<<---------------------------- added Content paraemter----------///////////
        _startScreen = new StartScreen(_font, width + 200, height, _pixelTex);
        _pathwayPan = new PathwayPan(4100f);
        _progressBar = new ProgressBar(
            400f,
            4450f,
            _ball,
            _font,
            _pixelTex,
            
            (width+200)/2 -200,
            50);
        
        ////////////////////////////////////////------------------------st------------------------////////////////////
        // instrccutiosn screen
        instructions = new Instructions(_font, width, height);
        ////////////////////////////////////////----------------------end--------------------------////////////////////
        
        //Obstacles
        //paths.AddPath(new Rectangle(300, 500, 200, 50), PathType.Obstacle, obstacleTex);
         paths.AddPath(new Rectangle(100, 500,  100, 30), PathType.Obstacle, obstacleTex);
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
        
         paths.AddPath(new Rectangle(450, 10002, 100, 50), PathType.Obstacle, obstacleTex);
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
        
         //interactions
        
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
        
        
        //////////////////////////////////////////----------------more sound---------------------------//////////////////////
        introMusic = Content.Load<Song>("mountainkingforintro");
        gameMusic = Content.Load<Song>("mountainkingforgameplay");
        //loop
        MediaPlayer.IsRepeating = true;

        if (!musicPlaying) //start game with intro playing 
        {
            musicPlaying = true;
            MediaPlayer.Play(introMusic); 
        }
        
        /*
        introMusicInstance = introMusic.CreateInstance();
        introMusicInstance.IsLooped = true;
        gameMusicInstance = gameMusic.CreateInstance();
        gameMusicInstance.IsLooped = true;*/
        
        //////////////////////////////////////////////---------------------end---------------------------//////////////////////// ^
    }
    
    private void Restart()
    {
        
        _coins = new CoinManager(_coinTex, height);
        _sidebar.Blocks.Clear();
        _ball = new Ball(
            Content.Load<Texture2D>("team_1ball.png.f7a4f30e7e4a09bd0ecb3135a3fcfc4f"),
            new Vector2((width + 200) / 2f, height / 2f),
            ScrollRate,
            36,
            0.5f);
        _progressBar = new ProgressBar(
            400f,
            4450f,
            _ball,
            _font,
            _pixelTex,
            (width + 200) / 2 + 200,
            50);
        
        _restartButtonAppears = false;
        _gameState = GameState.Playing;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        mouse = Mouse.GetState();
        Vector2 cameraPosition = _ball.Position - new Vector2((width+200)/2f, height / 2f);
        _mousePos = new Vector2(mouse.X, mouse.Y);
        Vector2 worldMouse = _mousePos + cameraPosition;

        switch (_gameState)
        {
            case GameState.StartScreen:
        ///////////////////////////////////////-----------------------------instructions & music----------------------------------////////////////
                if (_startScreen.Clicked(mouse, _previousMouseState))
                {
                    _gameState = GameState.Preview;
                    MediaPlayer.Play(gameMusic);
                }
                else if (_startScreen.RulesClicked(mouse, _previousMouseState))
                {
                    _gameState = GameState.Instructions;
                }
                break;
            
            case GameState.Instructions:
                if (mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _gameState = GameState.Preview;
                    // i put this here cus it starts playing after play button click
                    MediaPlayer.Play(gameMusic);
                }

                break;
            ///////////////////////////////////////-----------------------------instructions & musci end ----------------------------------////////
            
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
                _pauseScreen.Update(gameTime, _mousePos, mouse, _ball, this, _coins);
                
                //interactive components
                _sidebar.Update(gameTime, mouse, _previousMouseState, cameraPosition, _coins);
                foreach (var block in _sidebar.Blocks)
                {
                    block.BlockCollision(_ball);
                }
                
                if (_ball.Center().Y >= 4450f)
                    _gameState = GameState.GameWin;
                else if (_ball.FellOff())
                    _gameState = GameState.GameOver;
                break;
            case GameState.GameOver:
                _restartButtonAppears = true;
                if (mouse.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed &&
                    _restartButton.Contains(mouse.Position))
                {
                    Restart();
                }

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
        
////////////////////////////////////////////----------------------camera matrix-------------------------//////////////////////////////////        
        Matrix cameraMatrix = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0);
        
        float progress = _progressBar.GetProgress(); // for sparkles
////////////////////////////////////////////-----------------------end------------------------//////////////////////////////////

        switch (_gameState)
        {
            case GameState.StartScreen:
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    _winScreenTex,
                    new Rectangle(0, 0, width +200, height),
                    new Rectangle(0, 0, _winScreenTex.Width, _winScreenTex.Height),
                    Color.White);
                _startScreen.Draw(_spriteBatch, _winScreenTex);
                _spriteBatch.End();
                break;
            
            case GameState.Instructions:
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    _winScreenTex,
                    new Rectangle(0, 0, width +200, height),
                    new Rectangle(0, 0, _winScreenTex.Width, _winScreenTex.Height),
                    Color.White);
                instructions.Draw(_spriteBatch, _pixelTex);
                _spriteBatch.End();
                break;
            
            case GameState.Preview:
                _pathwayPan.Draw(_spriteBatch, _bg, paths, _basicEffect, GraphicsDevice);
                break;
            
            case GameState.Playing: 
                _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
                _bg.Draw(_spriteBatch, cameraPosition);          
                _spriteBatch.End();
                
                paths.DrawNonSquares(GraphicsDevice, _basicEffect, cameraPosition);
                
                //interactive components
                _spriteBatch.Begin(transformMatrix: cameraMatrix);
                foreach (var block in _blocks)
                {
                    block.Draw(_spriteBatch, cameraPosition, _mousePos);
                }
                _spriteBatch.End();
                
                _spriteBatch.Begin(); 
                paths.BoundsDraw(_spriteBatch, cameraPosition);
                _spriteBatch.Draw(_pixelTex, new Vector2(100 - cameraPosition.X, 4400 - cameraPosition.Y), new Rectangle(100, 400, 450, 10), Color.Navy);
                _ball.Draw(_spriteBatch, cameraPosition, progress); //////// added parameter /////////////////
                _coins.Draw(_spriteBatch, cameraPosition);
                _spriteBatch.DrawString(_font, $"Coins: {_coins.CoinCounter}", new Vector2(10, 10), Color.White);
                
                //sidebar
                _sidebar.Draw(_spriteBatch, cameraPosition, mouse);
                
                _progressBar.Draw(_spriteBatch);
                _spriteBatch.End();
                
                _pauseScreen.Draw(_spriteBatch); //uses matrix transform in its spritebatch.Begin
                
                break;
                
            case GameState.GameOver:
                _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
                _bg.Draw(_spriteBatch, cameraPosition);          
                _spriteBatch.End();
                
                _spriteBatch.Begin();  
                paths.BoundsDraw(_spriteBatch, cameraPosition);
                _ball.Draw(_spriteBatch, cameraPosition, progress); //////// added parameter /////////////////
        
                if (_ball.FellOff())
                {
                    string losingMessage = "You Died :( womp womp";
                    Vector2 size = 4*_font.MeasureString(losingMessage);
                    Vector2 origin = new Vector2((width + 200) / 2f - size.X / 8f, height / 2f - size.Y/2f);
                    _spriteBatch.DrawString(_font, losingMessage, origin, Color.White);

                    //restart button
                    _spriteBatch.Draw(_pixelTex, _restartButton, Color.DarkRed);
                    string restartText = "RESTART";
                    Vector2 Size = 1.5f*_font.MeasureString(restartText);
                    Vector2 textPos = new Vector2(
                        _restartButton.X + _restartButton.Width / 2f - Size.X / 2f,
                        _restartButton.Y + _restartButton.Height / 2f - Size.Y / 2f);
                    _spriteBatch.DrawString(_font, restartText, textPos, Color.White, 0f, Vector2.Zero, 1.5f,
                        SpriteEffects.None, 0f);

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