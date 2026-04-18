using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class Background
{
    private Texture2D _texture;
    private int _Count;
    private int screenWidth;
    
    public Background(Texture2D tex, int repeatCount, int _screenWidth)
    {
        _texture = tex;
        _Count = repeatCount;
        screenWidth = _screenWidth;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        for (int i = 0; i < _Count; i++)
        {
            Vector2 position = new Vector2(screenWidth-350, i * _texture.Height);
            //subtract because compensating for the texture width later

            spriteBatch.Draw(
                _texture,
                position - cameraPosition,
                Color.White
            );
        }
    }
}