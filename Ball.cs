using System;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Final_Project;

public class Ball
{
    //--------------------------------load sound-------------------------------------------------------------/////////
    public static ContentManager ContentManager { get; set; }
    private SoundEffect ballHitSound;
    //--------------------------------load sound-------------------------------------------------------------///////// ^

    
    //changing the sprite for 
    private const int cols = 36;
    private const int rows = 18;
    private const int frameWidth = 100;
    private const int frameHeight = 100;
    private const float circ = MathF.PI * frameWidth;
    private float _roll;
    private float _rollSpeed = 4f;
    private Rectangle _ballRect;
    private Color _color = Color.White;
    public bool _defaultColor;
    
    private Texture2D _ballTex;
    public Vector2 Position;
    public Vector2 _acc;

    public int _frameNum;
    public int _frameIndex;
    private Rectangle _spriteFrame;
    
    public float Scale;
    public float ScrollSpeed;
    
    //when it encounters obstacles
    public Vector2 Velocity = Vector2.Zero;
    public float DampeningCoeff = 0.7f;
    public float FallSpeed = 0.8f; //when it falls off the path
    public float smallestScale = 0.05f; //smallest it goes before the failure message pops up
    
    public bool _fell;
    private bool _falling = false;
    
    ///////////////////////--------------------------------------animation hierarchy-------------------------------/////////////////////////
    private bool _changeBallLayer = false;
    //---------
    private Texture2D _sparkleTex;
    private int _sparkleFrameWidth = 100;
    private int _sparkleFrameHeight = 100;
    private int _sparkleTotCols = 10;
    private int _sparkleTotRows = 8;
    private int _sparkleTotFrames = 80;
    private int _sparkleFrame = 0;
    private float _sparkleTimer = 0f;
    private float _sparkleFrameDuration = 0.02f;

    //private ProgressBar progress;
    //////////////////////////------------------------------------------hierarchy end------------------------------------///////////

    
    public Ball(
        Texture2D ballTex,
        Vector2 position,
        float scrollSpeed,
        int frameNum,
        float scale,
        ContentManager content, 
        bool defaultColor) /////<<<<<<<<<<---------------------------- added Content paraemter----------///////////
    {
        _ballTex = ballTex;
        Position = position;
        ScrollSpeed = scrollSpeed;
        _frameNum = frameNum;
        _frameIndex = _frameNum;
        Scale = scale;
        _acc = new Vector2(0f, 10f);
        _defaultColor = defaultColor;
        _spriteFrame = new Rectangle(_frameIndex * 100, 0, 100, 100);
        
        /////////--------------------------------load sound-------------------------------------------------------------/////////
        //using var stream = TitleContainer.OpenStream("Content/JDSherbert - Wooden UI SFX Pack - Confirm - 1.wav"); ahwdhdakwhfkwajhf
        //ballHitSound = SoundEffect.FromStream(stream);
        //not using .wav
        ballHitSound = content.Load<SoundEffect>("JDSherbert - Wooden UI SFX Pack - Confirm - 1");
        _sparkleTex = content.Load<Texture2D>("sparkles");
        ///////////--------------------------------load sound-------------------------------------------------------------///////// ^

        
    }

    public void Update(GameTime gameTime, PathTracker.PathBounds paths)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // ball color
        if (_defaultColor)
        {
            _color =  Color.White;
        }
        else
        {
            _color = Color.DarkRed;
        }
        
        _spriteFrame = getSourceRect();
        
        Vector2 center = Center();
        //Hear me out, this was so dumb, but the collision just would not work afte rthe first hit
        PathTracker obstacle = paths.getObstacle(center, Radius);
        
        
    /////////////////////////////////////////////------------------------------------sparkle--------------////////////////////    
        _sparkleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_sparkleTimer >= _sparkleFrameDuration)
        {
            _sparkleTimer = 0f;
            _sparkleFrame = (_sparkleFrame + 1) % 80;
            System.Diagnostics.Debug.WriteLine($"Sparkle frame: {_sparkleFrame}"); //debug
        }
    /////////////////////////////////////////////------------------------------------sparkle end--------------////////////////////    
        
    
        if (obstacle != null)
        {
            ObstacleCollision(obstacle);
            if (Velocity.LengthSquared() > 0.01f)
            {
                float horizontalRatio = Math.Abs(Velocity.X) / (Math.Abs(Velocity.Y) + 0.001f);
                if (horizontalRatio < 0.1f)
                {
                    StuckCollision(obstacle);
                    return;
                }
            }
            /////////////////////-------------------------------sound--------------------------------------////////////
            ballHitSound.Play();
            
            ////////////////////-------------------------------sound-------------------------------/////////   ^
        }

        ScrollSpeed += _acc.Y * dt;
        ScrollSpeed = MathHelper.Clamp(ScrollSpeed, 0f, 130f);
        Position += Velocity * dt;
        float speed = Velocity.Length() + ScrollSpeed;
        _roll += speed * dt * _rollSpeed;
        
        float overlap = paths.overlapOnPath(center, Radius);
        PathType centerType = paths.FindPathType(center);
     
        if (centerType == PathType.Hole && overlap < 0.5f)
        {
            _falling = true;
            // More than half the ball is off the path — start falling.
            //_changeBallLayer = true; //////<<<<<<<<<<<<<<<<<<<<<<<<<<<--------experiemnting with layers-------///////////////////////////////////////
            Position.Y += ScrollSpeed * 0.7f * dt;
            Position += Velocity * 0.4f * dt;
            Scale -= 1.2f* FallSpeed * dt;
            if (Scale < smallestScale)
            {
                Scale = smallestScale;
                _fell = true;
            }
            return;
        }

        if (!_falling)
        {
            Position.Y += ScrollSpeed * dt;
        }
        else
        {
            Position.Y += ScrollSpeed * 0.7f * dt;
            Position += Velocity * 0.4f * dt;
            Scale -= 1.2f* FallSpeed * dt;
            if (Scale < smallestScale)
            {
                Scale = smallestScale;
                _fell = true;
            }
        }
    }
    

    public bool FellOff()
    {
        return _fell;
    }
    

    private void ObstacleCollision(PathTracker obstacle)
    {
        //not totally accurate physics but like this is hard man
        //general idea is to get the center of the ball and the normal of the edge the ball touches
        //and then reflect it back
        //psych that didn't work
        Vector2 ballCenter = Center();
        Vector2 obstacleNormal = obstacle.getNormal(ballCenter);
        
        float horizontalRatio = Math.Abs(Velocity.X) / (Math.Abs(Velocity.Y) + 0.001f);
        float obstalceMult = 4f;
        if (horizontalRatio < 0.1f)
        {
            float obstacleMult = 0f;
        }
        Position += obstacleNormal*obstalceMult; //gotta have a little push out
        
        Vector2 incoming;
        if (Velocity.LengthSquared() > 0.01f)
        {
            incoming = Velocity;
        }
        else
        {
            incoming = new Vector2(0, ScrollSpeed);
        }

        float IntoSurface = Vector2.Dot(incoming, obstacleNormal);
        if (IntoSurface >= 0)
            return;
        Vector2 velocityIntoSurface = IntoSurface * obstacleNormal;
        Vector2 parallelVelocity = incoming - velocityIntoSurface; 
        //separate the into surface component from the parallel component. 
        //reflect the perpendicular component for bounces and keep the parallel component for sliding off surfaces
        Velocity = parallelVelocity * 1.05f + (-velocityIntoSurface * (DampeningCoeff + 0.2f));
    }

    private void StuckCollision(PathTracker obstacle)
    {
        Vector2 normal = obstacle.getNormal(Center());
        
        if (Math.Abs(normal.Y) > 0.9f)
        {
            Velocity.X *= 0.6f;
            Velocity.Y -= 1.0f;
            Velocity.Y = Math.Clamp(Velocity.Y, 0f, 0.05f);
            
            if (Math.Abs(Velocity.X) < 0.05f) Velocity.X = 0f;
            if (Math.Abs(Velocity.Y) < 0.05f) Velocity.Y = 0f;
        }
        else
        {
            // fallback (non-horizontal surfaces)
            Velocity *= 0.8f;
        }
        
        ScrollSpeed *= 0.4f;
    }
    
    
    //ugh the annoying error with accessing static fields in non-static contexts
    public float Radius
    {
        get
        {
            return (46f * Scale) / 2f;
        }
    }
    
    public Vector2 Center()
    {
            return new Vector2(Position.X + Radius, Position.Y + Radius);
    }
    
    private Rectangle getSourceRect()
    {
        float angle = 0f;
        if (Velocity.LengthSquared() > 0.001f)
        {
            angle = MathF.Atan2(Velocity.Y, Velocity.X); // -pi to pi
        }
        int row = 0;
        float normAng = (angle + MathF.PI) / (2f * MathF.PI); // 0 - 1
        int col = (int)(normAng * cols ) % cols;
        float rollFract = (_roll % circ) / circ;
        row = (rows - 1 - (int)(rollFract * rows)) % rows;
        return new Rectangle(col*frameWidth, row*frameHeight, frameWidth, frameHeight);
    }
    
////////////////////////////////////-----------------------------animation hierarhcy------------------------------//////////////////////    
    private void BlingBling(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        int col = _sparkleFrame % _sparkleTotCols;
        int row = _sparkleFrame / _sparkleTotCols;

        Rectangle sourceRect = new Rectangle(
            col * _sparkleFrameWidth,
            row * _sparkleFrameHeight,
            _sparkleFrameWidth,
            _sparkleFrameHeight
        );

        Vector2 origin = new Vector2(_sparkleFrameWidth / 2f, _sparkleFrameHeight / 2f);
        Vector2 drawPosition = Position - cameraPosition;

        spriteBatch.Draw(_sparkleTex, drawPosition, sourceRect, Color.White, 0f, origin, 2f, SpriteEffects.None, 0f);
    }
    
    /*private void DrawSparkle(SpriteBatch spriteBatch) --------------matrix version
    {
        int col = _sparkleFrame % _sparkleTotCols;
        int row = _sparkleFrame / _sparkleTotCols;

        Rectangle sourceRect = new Rectangle(
            col * _sparkleFrameWidth,
            row * _sparkleFrameHeight,
            _sparkleFrameWidth,
            _sparkleFrameHeight
        );
        Vector2 origin = new Vector2(_sparkleFrameWidth / 2f, _sparkleFrameHeight / 2f);
        spriteBatch.Draw(_sparkleTex, Position, sourceRect, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    
    }*/
///////////////////////////////////-----------------------------animation hierarhcy end------------------------------//////////////////    


    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, float progress)  //////// added parameter /////////////////
    {
        float rotation = MathF.Atan2(Velocity.Y, Velocity.X);
        spriteBatch.Draw(
            _ballTex,
            Position - cameraPosition,
            _spriteFrame,
            _color,
            rotation,
            new Vector2(50f, 50f),
            Scale,
            SpriteEffects.None,
            0f);
        
     /* if (_changeBallLayer) ------------------------------attempting layer hcange when ball falls
        {
            spriteBatch.Draw(
                _ballTex,
                Position - cameraPosition,
                _spriteFrame,
                Color.White,
                rotation, 
                new Vector2(50f, 50f),
                Scale,
                SpriteEffects.None,
                -1f);
        }
        else
        {
            spriteBatch.Draw(
                _ballTex,
                Position - cameraPosition,
                _spriteFrame,
                Color.White,
                rotation,
                new Vector2(50f, 50f),
                Scale,
                SpriteEffects.None,
                0f);
        }*/

        //////////////////////////////////-----------------------------animation hierarhcy------------------------------///////////////
        //float progressss = progress.GetProgress();
        if (progress > 0.9f)
        {
            BlingBling(spriteBatch, cameraPosition);
        }
        if (progress > 0.5f && progress < 0.6f)
        {
            BlingBling(spriteBatch, cameraPosition);
        }
        if (progress > 0.2f && progress < 0.25f)
        {
            BlingBling(spriteBatch, cameraPosition);
        }

        //////////////////////////////////-----------------------------animation hierarhcy end------------------------------///////////////
        
    }
}