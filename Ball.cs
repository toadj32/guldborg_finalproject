using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace guldborg_finalproject;

public class Ball
{
    private Texture2D _ball;
    private float _ballMass = 0.5f;

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

    public Ball(Texture2D ball, Vector2 position)
    {
        _ball = ball;
        _pos = position;
        _velocity = Vector2.Zero;
        _acc = new Vector2(0f, 5f);
        _ballRect = new Rectangle((int)_pos.X, (int)_pos.Y, frameWidth, frameHeight);
    }

    public void Update(GameTime gameTime)
    {
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _velocity += _acc * time;
        _velocity.Y = MathHelper.Clamp(_velocity.Y, -130f, 130f);
        _velocity.X = MathHelper.Clamp(_velocity.X, -130f, 130f);
        _pos += _velocity * time;
        _roll += _velocity.Length() * time * _rollSpeed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle src = getSourceRect();
        _ballRect =  new Rectangle((int)_pos.X, (int)_pos.Y, frameWidth, frameHeight);
        spriteBatch.Draw(_ball, _ballRect, src, Color.White);
    }

    private Rectangle getSourceRect()
    {
        float angle = 0f;
        if (_velocity.LengthSquared() > 0.001f)
        {
            angle = MathF.Atan2(_velocity.Y, _velocity.X); // -pi to pi
        }
        int row = 0;
        float normAng = (angle + MathF.PI) / (2f * MathF.PI); // 0 - 1
        int col = (int)((normAng * cols ) - 27 + cols) % cols;
        float rollFract = (_roll % circ) / circ;
        row = (int)(rollFract * rows) % rows;
        return new Rectangle(col*frameWidth, row*frameHeight, frameWidth, frameHeight);
    }

    public void Collide(Rectangle[] relevObjs)
    {
        if (relevObjs.Length > 0)
        {
            foreach (Rectangle r in relevObjs)
            {
                if (_ballRect.Intersects(r))
                {
                    return;
                    // Checks what kind of object it interacts with first
                }
            }
        }
    }
}