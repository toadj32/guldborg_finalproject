using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace guldborg_finalproject;

public class Ball
{
    private Texture2D _ballTex;
    public Vector2 Position;
    private const int cols = 36;
    private const int rows = 18;
    private const int frameWidth = 100;
    private const int frameHeight = 100;
    private const float circ = MathF.PI * frameWidth;
    
    public Vector2 _acc;
    private Vector2 _pos;
    public Vector2 _velocity;
    private float _roll;
    private float _rollSpeed = 2f;
    private Vector2 _origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
    private Rectangle _ballRect;
    public float Scale;

    /*public int _frameNum;
    public int _frameIndex = 0;
    private Rectangle _spriteFrame;*/
    
    /*public float timer = 0f;*/
    private float _animationSpeed = 0.03f;
    
    //when it encounters obstacles
    public Vector2 Velocity = Vector2.Zero;
    public float DampeningCoeff = 0.7f;
    public float FallSpeed = 0.8f; //when it falls off the path
    public float smallestScale = 0.05f; //smallest it goes before the failure message pops up
    
    public bool _fell;
    private bool _falling = false;
    public Ball(Texture2D ballTex, Vector2 position, float scale)
    {
        _ballTex = ballTex;
        Position = position;
        _velocity = Vector2.Zero;
        /*_frameNum = frameNum;*/
        Scale = scale;
        _acc = new Vector2(0f, 5f);
        _ballRect = new Rectangle((int)_pos.X, (int)_pos.Y, frameWidth, frameHeight);
        /*_spriteFrame = new Rectangle(_frameIndex * 100, 0, 100, 100);*/
        
    }

    public void Update(GameTime gameTime, PathTracker.PathBounds paths)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        /*timer += dt;
        
        if (timer >= _animationSpeed)
        {
            _frameIndex = (_frameIndex + 1) % _frameNum;
            _spriteFrame = new Rectangle(_frameIndex * 100, 0, 100, 100);
            timer = 0f;
        }*/

        Vector2 center = Center();
        //Hear me out, this was so dumb, but the collision just would not work afte rthe first hit
        PathTracker obstacle = paths.getObstacle(center, Radius);
        if (obstacle != null)
        {
            ObstacleCollision(obstacle);
        }
        
        float overlap = paths.overlapOnPath(center, Radius);
        PathType centerType = paths.FindPathType(center);

        if (centerType == PathType.Hole && overlap < 0.5f)
        {
            // More than half the ball is off the path — start falling.
            Position.Y += _velocity.Y * 0.8f * dt;
            Position += Velocity * dt;
            Scale -= FallSpeed * dt;
            if (Scale < smallestScale)
            {
                Scale = smallestScale;
                _fell = true;
            }
            return;
        }
        if (Velocity.LengthSquared() > 0.01f)
        {
            Velocity *= (1f - 8f *dt);
            if (Velocity.LengthSquared() < 1f)
            {
                Velocity = Vector2.Zero;
                _animationSpeed = 0.03f;
            }
        }
        
        Position += Velocity * dt;
        
        PathTracker obstacle3 = paths.getObstacle(center, Radius);
        if (obstacle3 != null)
        {
            ObstacleCollision(obstacle3);
        }
        Position += _velocity * dt;
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
        
        Position += obstacleNormal*4f; //gotta have a little push out
        
        Vector2 incoming;
        if (Velocity.LengthSquared() > 0.01f)
        {
            incoming = Velocity;
        }
        else
        {
            incoming = new Vector2(0, _velocity.Y);
        }

        float IntoSurface = Vector2.Dot(incoming, obstacleNormal);
        if (IntoSurface >= 0)
            return;
        Vector2 velocityIntoSurface = IntoSurface * obstacleNormal;
        Vector2 parallelVelocity = incoming - velocityIntoSurface; 
        //separate the into surface component from the parallel component. 
        //reflect the perpendicular component for bounces and keep the parallel component for sliding off surfaces
        Velocity = parallelVelocity * 0.9f + (-velocityIntoSurface * 1.5f);
        _animationSpeed *= 0.7f;
    }
    
    
    //ugh the annoying error with accessing static fields in non-static contexts
    public float Radius
    {
        get
        {
            return (60f * Scale) / 2f;
        }
    }
    
    public Vector2 Center()
    {
            return new Vector2(Position.X + Radius, Position.Y + Radius);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        /*spriteBatch.Draw(
            _ballTex,
            Position-cameraPosition,
            _spriteFrame,
            Color.White,
            (MathHelper.Pi)/2, //rotate 90
            new Vector2(50f, 50f),
            Scale,
            SpriteEffects.None,
            0f);*/
    }
}