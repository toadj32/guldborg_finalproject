using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Spring
{
    /*public enum SpringState{NoCollision, Colliding, PostCollision}

    private SpringState _state;*/
    private Texture2D _spring;
    private Vector2 _pos;
    public bool isPlaced { get; set; } = false;
    public bool justSpawned = true;
    private bool _isDragging = false;
    
    private float _k = 5f;
    public bool vertical; // randomly generated in sidebar. false = horizontal spring.
    private float _fb; // force from ball
    private float _b = 0.90f; // damping
    private float _eqL; // resting length of each segment
    private int _segNum = 6;
    private Rectangle _sourceRect;
    private Vector2[] _scale;
    private Vector2[] _nodes;
    private float[] _vel;
    private float _overallScale = 0.14f;
    private bool _activated = false;
    
    public Spring(Texture2D spr, bool vert)
    {
        _spring = spr;
        vertical = vert;
        _nodes = new Vector2[_segNum + 1];
        _vel = new float[_segNum + 1];
        _scale = new Vector2[_segNum];
        _eqL = (_overallScale* spr.Height) / (_segNum);
        initializeNodes();
    }

    private void initializeNodes()
    {
        for (int i = 0; i < _nodes.Length; i++)
        {
            _nodes[i] = vertical
                ? new Vector2(_pos.X,  _pos.Y + i * _eqL)
                : new Vector2(_pos.X + i * _eqL, _pos.Y);
            _vel[i] = 0f;
        }
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
                initializeNodes();
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
            NodePhysics(dt);
            bool settled = true;
            for (int i = 1; i <= _segNum; i++)
            {
                if (_vel[i] != 0f)
                {
                    settled = false;
                    break;
                }
            }

            if (settled)
                _activated = false;
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
                _activated = true;
                tipHit = true;
                Vector2 tip = _nodes[_segNum];
                float compression = ball.Radius + 5f;
                
                if(vertical)
                {
                    if (tipBox.Y == (int)_pos.Y) // if top hit box
                    {
                        ball.Position.Y = tipBox.Y - ball.Radius * 2f - 1f;
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y) - compression * 3f;
                        ball.ScrollSpeed = 0f;
                        _vel[0] += compression * 2f;
                    }
                    else // if bottom hit box, push ball downward
                    {
                        Vector2 pushDir = ball.Center() - _nodes[0];
                        if (pushDir != Vector2.Zero) 
                            pushDir.Normalize();
                        ball.Position += pushDir * compression;
                        ball.Velocity += pushDir * compression * 3f;
                        ball.ScrollSpeed *= 0.7f;
                        _vel[_segNum] -= compression * 2f;
                        
                    }
                }
                else
                { // horizontal springgg
                    if (tipBox.X == (int)_pos.X)
                    { // left box
                        ball.Position.X = tipBox.X - ball.Radius * 2f - 1f;
                        ball.Velocity.X = -Math.Abs(ball.Velocity.X) - compression * 3f;
                        _vel[1] += compression * 2f;
                    }
                    else
                    { // right boz
                        Vector2 pushDir = ball.Center() - _nodes[0];
                        if (pushDir != Vector2.Zero) 
                            pushDir.Normalize();
                        ball.Position += pushDir * compression;
                        ball.Velocity += pushDir * compression * 3f;
                        _vel[_segNum] -= compression * 2f;
                    }
                }
                
                break;
            }

            if (!tipHit && BodyHitbox().Intersects(ballRect))
            {
                Rectangle bodyRect = BodyHitbox();
                Vector2 springCenter = new Vector2(bodyRect.X + (bodyRect.Width / 2f),
                    bodyRect.Y + (bodyRect.Height / 2f));
                Vector2 normal = ball.Center() - springCenter;
                if (normal != Vector2.Zero)
                {
                    normal.Normalize();
                }

                ball.Position += normal * 2f;

                float inSurface = Vector2.Dot(ball.Velocity, normal);
                if (inSurface < 0)
                {
                    ball.Velocity -= inSurface * normal * 2f;
                    ball.Velocity *= 0.7f;
                }
                
            }
        }
    }

    private void NodePhysics(float dt)
    {
        for (int i = 1; i <= _segNum; i++)
        {
            float distAbove = vertical
                ? _nodes[i].Y - _nodes[i - 1].Y
                : _nodes[i].X - _nodes[i - 1].X;

            float distBelow = i < _segNum
                ? (vertical ? _nodes[i + 1].Y - _nodes[i].Y : _nodes[i + 1].X - _nodes[i].X)
                : 0f;

            float force = -_k * (distAbove - _eqL) + _k * (distBelow - _eqL);
            float nodeMass = 0.5f / _segNum;
            _vel[i] += (force / (nodeMass)) * dt;
            _vel[i] *= _b;
            if (Math.Abs(_vel[i]) < 0.01f) 
                _vel[i] = 0f;

            if (vertical) _nodes[i].Y += _vel[i] * dt;
            else _nodes[i].X += _vel[i] * dt;

            // prevent segments overlapping
            float minPos = vertical 
                ? _nodes[i - 1].Y + 0.5f 
                : _nodes[i - 1].X + 0.5f;
            if (vertical && _nodes[i].Y < minPos)
            {
                _nodes[i].Y = minPos; if (_vel[i] < 0) _vel[i] = 0;
            }

            if (!vertical && _nodes[i].X < minPos)
            {
                _nodes[i].X = minPos; if (_vel[i] < 0) _vel[i] = 0;
            }
        }
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        int sliceSize = _spring.Height / _segNum;

        for (int i = 0; i < _segNum; i++)
        {
            Rectangle sourceRect = new Rectangle(0, i * sliceSize, _spring.Width, sliceSize);

            float segLen = vertical
                ? _nodes[i + 1].Y - _nodes[i].Y
                : _nodes[i + 1].X - _nodes[i].X;

            Vector2 scale = new Vector2(_overallScale, segLen / sliceSize);

            float rotation = vertical ? 0f : MathHelper.PiOver2;

            // when rotated 90deg, the draw position needs to shift over by the segment width
            Vector2 drawPos = _nodes[i];
            if (!vertical)
                drawPos.X += segLen * _overallScale; // compensate for rotation pivot
            // arghhh work your magic spring
            /*Console.WriteLine($"node0: {_nodes[0]}, node1: {_nodes[1]}, eqL: {_eqL}, scale: {_overallScale}");*/
            spriteBatch.Draw(
                _spring,
                drawPos,
                sourceRect,
                Color.White,
                rotation,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
        
        /*// Test with whole spring not moving
        spriteBatch.Draw(_spring,
            _pos,
            null,
            Color.White,
            0.0f,
            Vector2.Zero,
            _overallScale,
            SpriteEffects.None,
            0f);*/
    }

    public void setPos(Vector2 posss)
    {
        _pos = posss; 
        initializeNodes();
    }

    public void DrawHitBox(SpriteBatch spriteBatch, Texture2D pixel)
    {
        foreach (Rectangle tipBox in TipHitbox())
            spriteBatch.Draw(pixel, tipBox, Color.Red * 0.5f);
        spriteBatch.Draw(pixel, BodyHitbox(), Color.Blue * 0.5f);
    }
    
    public Rectangle Hitbox()
    {
        return new Rectangle((int)_pos.X,
            (int)_pos.Y,
            (int)(_spring.Width * _overallScale),
            (int)(_spring.Height * _overallScale));
    }
}
