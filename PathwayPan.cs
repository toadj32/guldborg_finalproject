using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class PathwayPan
{
    private float _panSpeed = 200f;
    private float _currentPos = 0f;
    private float _endPos;
    private bool _done = false;
    public bool Done  => _done;

    public PathwayPan(float endPos)
    {
        _endPos = endPos;
        _currentPos = 0f;
    }

    public float Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_currentPos < _endPos)
        {
            _currentPos += _panSpeed * dt;
            if (_currentPos >= _endPos)
                _currentPos = _endPos;
        }
        else
        {
            _done = true;
        }
        return _currentPos;
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Background bg,
        PathTracker.PathBounds paths,
        BasicEffect basicEffect,
        GraphicsDevice gd)
    {
        Vector2 cameraPosition = new Vector2(0, _currentPos);
        spriteBatch.Begin(samplerState: SamplerState.PointWrap);
        bg.Draw(spriteBatch, cameraPosition);
        spriteBatch.End();
        
        spriteBatch.Begin();
        paths.BoundsDraw(spriteBatch, cameraPosition);
        spriteBatch.End();
        
        paths.DrawNonSquares(gd, basicEffect, cameraPosition);
    }
    
}