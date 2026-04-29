using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public class HighScore
{
    public float _highScore;
    public float _currentScore;
    private SpriteFont _font;
    private Texture2D _board;
    public int _prevCoinsCollected = 0;
    private int _currentCoinsCollected = 0;
    public int _mostCoins;

    private const string path = "highscore.json";

    public HighScore(SpriteFont font, Texture2D scoreBoard)
    {
        _font = font;
        _board = scoreBoard;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var data =  JsonSerializer.Deserialize<ScoreData>(json);
            _highScore = data.HighScore;
            _mostCoins = data.MostCoins;
        }
    }

    public void Update(GameTime gameTime, CoinManager coinManager)
    {
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _currentScore += time;
        
        _currentCoinsCollected = coinManager.CoinsCollected;
        if (_currentCoinsCollected > _prevCoinsCollected)
        {
            _currentScore += (_currentCoinsCollected -  _prevCoinsCollected);
            _prevCoinsCollected = _currentCoinsCollected;
        }
        
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
        }

        if (_currentCoinsCollected > _mostCoins)
        {
            _mostCoins = _currentCoinsCollected;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_board, new Rectangle(15, 600, 150, 100), Color.DarkRed);
        spriteBatch.Draw(_board, new Rectangle(20, 605, 140, 90), Color.MidnightBlue);
        spriteBatch.DrawString(_font, $"Best: {_highScore:F2}", new Vector2(30, 615), Color.White);
        spriteBatch.DrawString(_font, $"Score: {_currentScore:F2}", new Vector2(30, 640), Color.White);
        spriteBatch.DrawString(_font, $"Coins: {_currentCoinsCollected}",  new Vector2(30, 665), Color.White);
        spriteBatch.End();
    }

    public void Save()
    {
        var data = new ScoreData
        {
            HighScore = _highScore,
            MostCoins = _mostCoins
        };
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
        
        /*string fullPath = Path.GetFullPath(path);
        Console.WriteLine(fullPath);*/
    }

    public void Reset()
    {
        _currentScore = 0;
    }
}