using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class StartScreen
{
    private SpriteFont _font;
    private int _width;
    private int _height;
    private Rectangle _startButton;
    private Rectangle _rulesButton;
    private Texture2D _pixelTex;

    private Rules _rulesScreen;
    private bool _rulesUp = false;

    public StartScreen(SpriteFont font, int width, int height, Texture2D pixelTex)
    {
        _font = font;
        _width = width;
        _height = height;
        _pixelTex = pixelTex;

        _startButton = new Rectangle(
            width / 2 + 50,
            height / 2 + 50,
            100, 50);
        _rulesButton = new Rectangle(
            width/2 - 150,
            height /2 + 50,
            100, 50);
        _rulesScreen = new Rules(font, width, height, pixelTex);
    }

    public bool Clicked(MouseState mouse, MouseState previousMouse)
    {

        bool released = (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released);
        if (_rulesUp)
        {
            if (_rulesScreen.Update(mouse, previousMouse))
                _rulesUp = false;
            return false;
        }
        if (!released)
            return false;
        if (_rulesButton.Contains(mouse.Position))
        {
            _rulesUp = true;
            return false;
        }
        if (_startButton.Contains(mouse.Position))
                return true;

        /*if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            if (_startButton.Contains(mouse.Position))
                return true; */

        return false;
    }
    
    
    ///////////////////////////////////////-------------------------for instructions screen--------------------------------------///////////
    public bool RulesClicked(MouseState mouse, MouseState previousMouse)
    {
        if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            if (_rulesButton.Contains(mouse.Position))
                return true; 
        return false;
    }
    ///////////////////////////////////////---------------------------------------------------------------/////////// 

    
    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        if (_rulesUp)
        {
            _rulesScreen.Draw(spriteBatch, texture);
            return;
        }
        string Title = "GuldBorg";
        Vector2 titlesize = _font.MeasureString(Title);
        spriteBatch.DrawString(
            _font,
            Title,
            new Vector2(_width/2f - 5f*titlesize.X/2f, _height/2f - 100),
            Color.Black,
            0f, 
            Vector2.Zero,
            5f,
            SpriteEffects.None,
            0f);
        
        spriteBatch.Draw(texture, _startButton, Color.DarkBlue);
        spriteBatch.Draw(texture, _rulesButton, Color.Navy);

        string button = "PLAY";
        Vector2 buttonSize = 1.5f* _font.MeasureString(button);
        spriteBatch.DrawString(
            _font,
            button,
            new Vector2(
                _startButton.X + _startButton.Width / 2f - buttonSize.X / 2f,
                _startButton.Y + _startButton.Height / 2f - buttonSize.Y / 2f),
            Color.White,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            0f);
        
        string rulesButton = "RULES";
        Vector2 rulesbuttonSize = 1.5f*_font.MeasureString(rulesButton);
        spriteBatch.DrawString(
            _font,
            rulesButton,
            new Vector2(
                _rulesButton.X + _rulesButton.Width / 2f - rulesbuttonSize.X / 2f,
                _rulesButton.Y + _rulesButton.Height / 2f - rulesbuttonSize.Y / 2f),
            Color.White,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            0f);
    }
}