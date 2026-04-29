using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class CoinManager
{
    private List<Coins> _coins = new List<Coins>();
    private Texture2D _coinTex;
    public float _spawnTimer = 0f;
    public float _interval = 2f;
    private Random _rand = new Random();
    private int _screenHeight;
    public int CoinCounter = 200;
    public int CoinsCollected = 0;

    public CoinManager(Texture2D coinTex, int screenHeight)
    {
        _coinTex = coinTex;
        _screenHeight = screenHeight;
    }

    public void Update(GameTime gameTime, Ball ball)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float CameraY = ball.Position.Y - _screenHeight /2f;

        // Spawning
        _spawnTimer += dt;
        if (_spawnTimer >= _interval && ball.Position.Y < 4000f)
        {
            _coins.Add(new Coins(
                _coinTex,
                new Vector2(_rand.Next(100, 500), ball.Position.Y + _rand.Next(100, 500)),
                32, 8, 2.0f));
            _spawnTimer = 0f;
        }

        // Update and collect
        foreach (var coin in _coins)
        {
            coin.Update(gameTime, CameraY, _screenHeight);
            bool wasActive = coin.Active;
            coin.Collected(ball);
            if (wasActive && !coin.Active)
            {
                CoinCounter+=50;
                CoinsCollected++;   
            }
        }

        _coins.RemoveAll(coin => !coin.Active);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        foreach (var coin in _coins)
            coin.Draw(spriteBatch, cameraPosition);
    }
}