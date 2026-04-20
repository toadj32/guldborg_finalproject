using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class Coins
{
    public Texture2D _coinTex;
    public int _frameNum;
    public int _frameIndex = 0;
    public Rectangle _spriteFrame;
    public Vector2 Position;
    public int Dimensions;
    private float _animationSpeed = 0.1f;
    public float Scale;
    
    public bool Active = true;

    public float timer = 0f;
    
    public Coins(
        Texture2D coinTex, 
        Vector2 position, 
        int dimensions,
        int frameNum,
        float scale)
    {
        Position = position;
        _coinTex = coinTex;
        Dimensions = dimensions;
        _spriteFrame = new Rectangle(_frameIndex * dimensions, 0, dimensions, dimensions);
        _frameNum = frameNum;
        Scale = scale;
    }

    public void Update(GameTime gameTime, float cameraPosition, int height)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timer += dt;

        if (timer >= _animationSpeed)
        {
            _frameIndex = (_frameIndex + 1) % _frameNum;
            _spriteFrame = new Rectangle(_frameIndex*Dimensions, 0, Dimensions, Dimensions);
            timer = 0f;
        }
        
        if (Position.Y < cameraPosition)
           Active = false;
    }

    public Rectangle Bounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)(Dimensions*Scale), (int)(Dimensions*Scale));
    }

    public void Collected(Ball ball)
    {
        if (Active && Bounds().Intersects(new Rectangle(
                (int)(ball.Center().X - ball.Radius),
                (int)(ball.Center().Y - ball.Radius),
                (int)(ball.Radius *2f),
                (int)(ball.Radius *2f))))
            Active = false;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        if (!Active)
            return;
        spriteBatch.Draw(
            _coinTex, 
            Position - cameraPosition, 
            _spriteFrame,
            Color.White,
            0f,
            Vector2.Zero,
            Scale,
            SpriteEffects.None,
            0f);
    }
}