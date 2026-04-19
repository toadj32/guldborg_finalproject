using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace guldborg_finalproject;

public class Spring
{
    private Texture2D _spring;
    private float _k;
    private float _springMass = 0.5f;
    private Texture2D _block;
    private float _fb; 
    private float _b = 0.97f; // damping
    private float _eqL; // resting length of each segment
    private Vector2 _blockPosition;
    private int _segNum = 6;
    private Rectangle _sourceRect;
    private Vector2[] _scale;
    private Vector2[] _nodes;
    private float[] _vel;
    public float maxY; 

    public Spring(Texture2D spr, Texture2D blk, float springConstant, float blockMass)
    {
        _spring = spr;
        _block = blk;
        _k = springConstant;
        _fb = blockMass * 9.81f;
        _nodes = new Vector2[_segNum + 1];
        _vel = new float[_segNum + 1];
        _scale = new Vector2[_segNum];
        _eqL = spr.Height / (float)_segNum * 0.7f;
        
        for (int i = 0; i < _nodes.Length; i++)
        {
            _vel[i] = 0f;
            if (i != _nodes.Length - 1)
            {
                _scale[i] = new Vector2(0.7f, _eqL * 0.3f);
                _nodes[i] = new Vector2(200f, i * _eqL * 0.3f);
            }
        }
        _blockPosition = new Vector2(_nodes[_segNum].X + (_block.Width), _nodes[_segNum].Y + (_block.Height * 0.55f));
    }

    public void update(GameTime gameTime, float ground, bool drop)
    {
        if (drop == true)
        {
            float dt =  (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
            
            for (int i = 1; i < _nodes.Length; i++)
            {
                float distBelow = 0f;
                float distAbove = _nodes[i].Y - _nodes[i - 1].Y;
                if (i < _nodes.Length - 1)
                {
                    distBelow = _nodes[i+1].X - _nodes[i].X;
                }
                float force = -_k * (distAbove - _eqL) + _fb + _k * (distBelow - _eqL);
                float acc = force / (_springMass / _segNum);
                _vel[i] += acc * dt;
                _vel[i] *= _b;
                _nodes[i].Y += _vel[i] * dt;
                if (i == 1)
                {
                    maxY = _nodes[i].Y;
                }
                if (_nodes[i].Y < _nodes[i - 1].Y + 2.0f)
                { // No overlapping spring segments
                    _nodes[i].Y = _nodes[i - 1].Y + 2.0f;

                    if (_vel[i] < 0f)
                    {
                        _vel[i] = 0f;
                    }
                }
                _nodes[i].Y = MathHelper.Clamp(_nodes[i].Y, _nodes[i - 1].Y, ground - _block.Height);
            }
            _blockPosition = new Vector2(_nodes[_segNum].X + (_block.Width), 
                _nodes[_segNum].Y - 15f);
            _blockPosition.Y = MathHelper.Clamp(_blockPosition.Y, 0f, ground - _block.Height);
        }
    }

    public void draw(SpriteBatch spriteBatch)
    {
        int sliceHeight = (_spring.Height) / _segNum;
        
        spriteBatch.Begin();
        for (int i = 0; i < _segNum; i++)
        {
            Rectangle sourceRect = new Rectangle(0, i * sliceHeight, (_spring.Width), sliceHeight);
            _scale[i].Y = (_nodes[i+1].Y - _nodes[i].Y) / sliceHeight;

            spriteBatch.Draw(_spring, _nodes[i], sourceRect, 
                Color.White, 0.0f, Vector2.Zero, 
                _scale[i], SpriteEffects.None, 0.0f);
        }
        spriteBatch.Draw(_block, _blockPosition, null, Color.White, 0.0f, Vector2.Zero, 
            0.8f, SpriteEffects.None, 0.0f);
        spriteBatch.End();
    }
    
}