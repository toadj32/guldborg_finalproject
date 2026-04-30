using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class finalBoss
{
    private Texture2D _cat;
    private Vector2 _pos;
    public bool done = false;
    private float _scale = 1.0f;
    public bool slidein = false; // if you haven't fully slid in
    private float _startX = -300f; 
    private float _targetX = 20f; 
    private float _slideSpeed = 400f;

    public finalBoss(Texture2D cat)
    {
        _cat = cat;
        _pos = new Vector2(50f, 600f);
    }

    public void Update(GameTime gameTime, Ball ball)
    {
        float time = (float)gameTime.TotalGameTime.TotalSeconds;
        float dt =  (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (done) // end game!
        {
            _scale -= time * 0.3f;
            _scale = Math.Max(_scale, 0f);
            if (_scale == 0f)
            {
                done = false;
                _startX = -300f;
            }
        }
        if (!slidein) // sliding into screen
        {
            _startX += _slideSpeed * dt;
            _pos.X = _startX;
            _pos.Y = 600f;
            if (_startX >= _targetX)
            {
                _startX = _targetX;
                slidein = true;
            }
        }
        else
        {
            Collide(ball);
        }
        _pos.X = 0f + 100f * (float)Math.Sin(1.2f * time);
        _pos.Y = 600f + 80f * (float)Math.Cos(0.8f * time);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_cat, _pos, null,
            Color.White * _scale, 0f,
            new Vector2(_cat.Width / 2f, _cat.Height / 2f),
            1.0f,
            SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
            0f);
        spriteBatch.End();
    }

    public void Collide(Ball ball)
    {
        Rectangle[] paws = HitBox();
        Rectangle ballRect = new Rectangle(
            (int)(ball.Center().X - ball.Radius),
            (int)(ball.Center().Y - ball.Radius),
            (int)(ball.Radius * 2),
            (int)(ball.Radius * 2));

        foreach (Rectangle paw in paws)
        {
            if (!paw.Intersects(ballRect))
            {
                Vector2 pawCenter = new Vector2(paw.X + paw.Width / 2f, paw.Y + paw.Height / 2f);
                Vector2 normal = ball.Center() - pawCenter;
                if (normal != Vector2.Zero) 
                    normal.Normalize();

                ball.Position += normal * 3f;
                float intoSurface = Vector2.Dot(ball.Velocity, normal);
                if (intoSurface < 0)
                {
                    ball.Velocity -= 2 * intoSurface * normal;
                    ball.Velocity *= 0.8f;
                }
                ball.ScrollSpeed *= 0.5f;
                break;
            }
        }
    }

    public Rectangle[] HitBox()
    {
        Rectangle top = new Rectangle((int)(_pos.X + 30) , (int)(_pos.Y - 80), 100, 110);
        Rectangle bottom = new Rectangle((int)(_pos.X + 110) , (int)(_pos.Y + 110), 130, 100);
        return [top,bottom];
    }
    
    // TROUBLESHOOT WHERE PAW RECTS ARE
    public void DrawHitbox(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Begin();
        foreach (Rectangle paw in HitBox())
            spriteBatch.Draw(pixel, paw, Color.Red * 0.5f);
        spriteBatch.End();
    }
    
    public void Reset()
    {
        done = false;
        slidein = false;
        _startX = -300f;
        _scale = 1f;
        _pos = new Vector2(-300f, 600f);
    }

}