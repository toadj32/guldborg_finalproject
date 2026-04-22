using System;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Final_Project;

public class Ball
{
    //changing the sprite for 
    private const int cols = 36;
    private const int rows = 18;
    private const int frameWidth = 100;
    private const int frameHeight = 100;
    private const float circ = MathF.PI * frameWidth;
    private float _roll;
    private float _rollSpeed = 4f;
    private Rectangle _ballRect;
    
    private Texture2D _ballTex;
    public Vector2 Position;

    public int _frameNum;
    public int _frameIndex;
    private Rectangle _spriteFrame;
    
    public float Scale;
    public float ScrollSpeed;
    public float timer = 0f;
    private float _animationSpeed = 0.01f;
    
    //when it encounters obstacles
    public Vector2 Velocity = Vector2.Zero;
    public float DampeningCoeff = 0.7f;
    public float FallSpeed = 0.8f; //when it falls off the path
    public float smallestScale = 0.05f; //smallest it goes before the failure message pops up
    
    public bool _fell;
    private bool _falling = false;
    public Ball(
        Texture2D ballTex,
        Vector2 position,
        float scrollSpeed,
        int frameNum,
        float scale)
    {
        _ballTex = ballTex;
        Position = position;
        ScrollSpeed = scrollSpeed;
        _frameNum = frameNum;
        _frameIndex = _frameNum;
        Scale = scale;

        _spriteFrame = new Rectangle(_frameIndex * 100, 0, 100, 100);
        
    }

    public void Update(GameTime gameTime, PathTracker.PathBounds paths)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timer += dt;
        
        _spriteFrame = getSourceRect();
        
        Vector2 center = Center();
        //Hear me out, this was so dumb, but the collision just would not work afte rthe first hit
        PathTracker obstacle = paths.getObstacle(center, Radius);
        
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

                if (horizontalRatio > 0.1f)
                {
                    _animationSpeed = 0.03f;
                    ScrollSpeed = 100f;
                }
            }
        }
        Position += Velocity * dt;
        float speed = Velocity.Length() + ScrollSpeed;
        _roll += speed * dt * _rollSpeed;
        
        float overlap = paths.overlapOnPath(center, Radius);
        PathType centerType = paths.FindPathType(center);
     
        if (centerType == PathType.Hole && overlap < 0.5f)
        {
            // More than half the ball is off the path — start falling.
            Position.Y += ScrollSpeed * 0.3f * dt;
            Position += Velocity * 0.6f * dt;
            Scale -= FallSpeed * dt;
            if (Scale < smallestScale)
            {
                Scale = smallestScale;
                _fell = true;
            }
            return;
        }
        Position.Y += ScrollSpeed * dt;
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
        if (horizontalRatio < 0.1f)
        {
            float obstacleMult = 0f;
        }
        float obstalceMult = 4f;
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
        Velocity -= new Vector2(2, 2);
        Velocity.X = Math.Clamp(Velocity.X, 0, 1);
        Velocity.Y = Math.Clamp(Velocity.Y, 0, 1);
        _animationSpeed *= 2f;
        ScrollSpeed = 50f;
    }
    
    
    //ugh the annoying error with accessing static fields in non-static contexts
    public float Radius
    {
        get
        {
            return (48f * Scale) / 2f;
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

    
    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        float rotation = MathF.Atan2(Velocity.Y, Velocity.X);
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
         }
}