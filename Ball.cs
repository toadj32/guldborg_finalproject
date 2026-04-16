using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace guldborg_finalproject;

public class Ball
{
    private Texture2D _ball;
    private float _ballMass = 0.5f;
    private Rectangle[] _ballFrames;
    private Vector2 _acc;
    private Vector2 _pos;
    private Vector2 _velocity;
    private float _rot;
    private Vector2 _origin;

    public Ball(Texture2D ball, Vector2 position)
    {
        _ball = ball;
        _pos = position;
        _velocity = Vector2.Zero;
        _acc = new Vector2(0f, 0.05f);
        
        _origin = new Vector2(_ball.Width / 2f, _ball.Height / 2f);
    }

    public void Update(GameTime gameTime)
    {
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _velocity += _acc;
        _pos.Y += _velocity.Y * time;
        _rot += MathF.Sqrt(_velocity.X * _velocity.X + _velocity.Y * _velocity.Y) / (_ball.Width*2f);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_ball, _pos, null, Color.White, _rot, _origin, 1.0f, SpriteEffects.None, 0f);
    }
}