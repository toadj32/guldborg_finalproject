using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Spring
{
    private Texture2D _spring;
    public Vector2 _pos;
    public bool isPlaced { get; set; } = false;
    public bool justSpawned = true;
    private bool _isDragging = false;
    
    private float _k = 5f;
    public bool vertical; // randomly generated in sidebar. false = horizontal spring.
    private float _fb; // force from ball
    private float _b = 0.90f; // damping
    private float _eqL; // resting length of each segment
    private float _overallScale = 0.14f;
    private bool _activated = false;
    private float _scaler = 1.0f;
    private float _compression = 1f;
    private float _compVel = 0f;
    private float _restoreSpeed = 10f;
    
    public Spring(Texture2D spr, bool vert)
    {
        _spring = spr;
        vertical = vert;
        _eqL = (_overallScale* spr.Height);
    }

    private void RestoreSpring(float dt)
    {
        float force = (1f - _compression) * _restoreSpeed;
        _compVel += force * dt;
        _compVel *= _b; // dampen

        if (Math.Abs(_compVel) < 0.001f)
        {
            _compVel = 0f;
        }
        _compression += _compVel * dt;
        _compression = Math.Clamp(_compression, 0.3f, 1.5f);
    }

    public Rectangle[] TipHitbox()
    { // the hitboxes at the top (for vertical) or the sides (for horizontal) should trigger oscillations and compressions but the others don't
        if (vertical)
        {
            // return top and bottom box
            Rectangle topBox = new Rectangle(
                (int)_pos.X,
                (int)_pos.Y,
                (int)(_spring.Width * _overallScale),
                (int)(_spring.Height*0.25f * _overallScale));
            Rectangle bottomBox = new Rectangle(
                (int)_pos.X,
                (int)(_pos.Y + (_spring.Height*0.5f * _overallScale)),
                (int)(_spring.Width * _overallScale),
                (int)(_spring.Height*0.25f * _overallScale));
            return [topBox, bottomBox];
        }
        else
        { // return left and right box
            Rectangle leftBox = new Rectangle(
                (int)_pos.X,
                (int)_pos.Y,
                (int)(_spring.Width * _overallScale * 0.25f),
                (int)(_spring.Height * _overallScale));
            Rectangle rightBox = new Rectangle(
                (int)(_pos.X + (_spring.Width*0.5f * _overallScale)),
                (int)_pos.Y,
                (int)(_spring.Width * _overallScale * 0.25f),
                (int)(_spring.Height * _overallScale));
            return [leftBox, rightBox];
        }
    }
    
    public Rectangle BodyHitbox()
    { // the hitboxes on the side act as a rigid body. return middle box
        if (vertical)
        {
            Rectangle box = new Rectangle(
                (int)(_pos.X),
                (int)(_pos.Y + (_spring.Height*0.25f * _overallScale)),
                (int)(_spring.Width * _overallScale),
                (int)(_spring.Height*0.5f * _overallScale));
            return box;
        }
        else
        { 
            Rectangle box = new Rectangle(
                (int)(_pos.X + (_spring.Width*0.25f * _overallScale)),
                (int)(_pos.Y),
                (int)(_spring.Width * _overallScale * 0.5f),
                (int)(_spring.Height * _overallScale));
            return box;
        }
    }

    public void Update(GameTime gameTime, MouseState mouse, MouseState lastMouse, Ball ball, Vector2 worldMouse)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Dragging
        if (justSpawned)
        {
            justSpawned = false;
            return;
        }

        if (!isPlaced)
        {
            Vector2 offset = Vector2.Zero;
            bool justDown = mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released;
            bool stillDown =  mouse.RightButton == ButtonState.Pressed;
            bool justReleased = mouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed;

            if (justDown && Hitbox().Contains(mouse.Position))
            {
                _isDragging = true;
                offset = worldMouse - _pos;
            }

            if (stillDown && _isDragging)
            {
                _pos = worldMouse - offset;
            }

            if (justReleased && _isDragging)
            {
                _isDragging = false;
                isPlaced = true;
            }
        }
        Collision(ball, dt);
        if (_activated)
        {
            RestoreSpring(dt);
            if (Math.Abs(_compVel) == 0f&& Math.Abs(_compression - 1f) < 0.001f)
            {
                _compression = 1f;
                _activated = false;
            }
        }
    }

    private void Collision(Ball ball, float dt)
    { // Ball collision
        Rectangle ballRect = new Rectangle(
            (int)(ball.Center().X - ball.Radius),
            (int)(ball.Center().Y - ball.Radius),
            (int)(ball.Radius * 2),
            (int)(ball.Radius * 2));
        
        bool tipHit = false;
        foreach (Rectangle tipBox in TipHitbox())
        {
            if (tipBox.Intersects(ballRect))
            {
                tipHit = true;
                _activated = true;
                float compression = ball.Radius + 5f;
                _compVel -= compression * 0.5f;
                
                if(vertical)
                {
                    if (tipBox.Y == (int)_pos.Y) // if top hit box
                    {
                        ball.Position.Y = tipBox.Y - ball.Radius * 2f - 1f;
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y) - compression * 2f;
                        ball.Velocity.X += 10f;
                        ball.ScrollSpeed = 0f;
                    }
                    else // if bottom hit box, push ball downward
                    {
                        ball.Velocity.Y = (float)(Math.Abs(ball.Velocity.Y) + compression * 1.5f);
                        ball.ScrollSpeed *= 0.7f;
                    }
                }
                else
                { // horizontal springgg
                    if (tipBox.X == (int)_pos.X)
                    { // left box
                        ball.Position.X = tipBox.X - ball.Radius * 1.5f;
                        ball.Velocity.X = -Math.Abs(ball.Velocity.X) - compression * 1.5f;
                    }
                    else
                    { // right boz
                        ball.Velocity.X = (float)(Math.Abs(ball.Velocity.X) + compression * 1.5f);
                    }
                }
                break;
            }
        }if (!tipHit && BodyHitbox().Intersects(ballRect))
        {
            Rectangle bodyRect = BodyHitbox();
            Vector2 springCenter = new Vector2(bodyRect.X + (bodyRect.Width / 2f),
                bodyRect.Y + (bodyRect.Height / 2f));
            Vector2 normal = ball.Center() - springCenter;
            if (normal != Vector2.Zero)
            {
                normal.Normalize();
            }

            ball.Position += normal * 1.5f;

            float inSurface = Vector2.Dot(ball.Velocity, normal);
            if (inSurface < 0)
            {
                ball.Velocity -= inSurface * normal * 1.5f;
                ball.Velocity *= 0.7f;
            }
        }

        ball.Velocity += ball._acc * dt * dt * 10f;
        
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 scale = vertical
            ? new Vector2(_overallScale, _overallScale * _compression) 
            : new Vector2(_overallScale * _compression, _overallScale);
            float rotation = vertical
                ? 0f 
                : MathHelper.PiOver2;
            Vector2 origin = new Vector2(_spring.Width / 2f, 0f);
            
            // arghhh work your magic spring
            /*Console.WriteLine($"node0: {_nodes[0]}, node1: {_nodes[1]}, eqL: {_eqL}, scale: {_overallScale}");*/
            spriteBatch.Draw(
                _spring,
                _pos,
                null,
                Color.White,
                rotation,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        
    }

    public void setPos(Vector2 posss)
    {
        _pos = posss; 
    }

    public void DrawHitBox(SpriteBatch spriteBatch, Texture2D pixel)
    {
        foreach (Rectangle tipBox in TipHitbox())
            spriteBatch.Draw(pixel, tipBox, Color.Red * 0.5f);
        spriteBatch.Draw(pixel, BodyHitbox(), Color.Blue * 0.5f);
    }

    Rectangle Hitbox()
    {
        return new Rectangle((int)_pos.X, (int)_pos.Y, (int)(_spring.Width * _overallScale), (int)(_spring.Height * _overallScale));
    }
}

