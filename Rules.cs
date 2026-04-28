using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Final_Project;

public class Rules
{
    private SpriteFont _font;
    private int _width;
    private int _height;
    private Rectangle _backButton;
    private Texture2D _pixelTex;

    private string[] _rules = new string[]
    {
        "HOW TO PLAY",
        "",
        "",
        "1) The ball is situated on a ramp",
        "and rolls down automatically.", 
        "It will gradually accelerate",
        "",
        "2) You start off with 200 coins.",
        " You must buy the components to",
        "place on the ramp. You can collect",
        "coins as you go. If you run out",
        "of coins, oh well...",
        "",
        "3) Drag blocks, springs, and ramps",
        "onto the path to redirect the ball",
        "to avoid holes and reach the end.",
        "",
        "4) Reach the finish line to win!",
        "",
        "5) High score is based on time and",
        "money!"
    };

    public Rules(SpriteFont font, int width, int height, Texture2D pixelTex)
    {
        _font = font;
        _width = width;
        _height = height;
        _pixelTex = pixelTex;

        int buttonWidth = 120;
        int buttonHeight = 50;
        _backButton = new Rectangle(_width - buttonWidth, 0, buttonWidth, buttonHeight);
    }

    public bool Update(MouseState mouse, MouseState prevMouse)
    {
        bool released = mouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
        return released && _backButton.Contains(mouse.Position);
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D bgTex)
    {
        spriteBatch.Draw(
            bgTex,
            new Rectangle(0, 0, _width, _height),
            new Rectangle(0, 0, bgTex.Width, bgTex.Height),
            Color.White);

        float lineDimensionsHeight = 30f;
        float lineStartY = 50f;

        for (int i = 0; i < _rules.Length; i++)
        {
            string rule = _rules[i];
            float scale = (i == 0) ? 3f : 1.5f; //Want title to be larger
            Color color = Color.Black;
            Vector2 Size = scale*_font.MeasureString(rule == "" ? " " : rule); //it's like is there even a rule, if so measure length, if not "" 

            float x = (i == 0) ? _width / 2f - Size.X / 2f - 150f: _width / 2f - 250f; //offset size properly based on if title or rules
            
            spriteBatch.DrawString(
                _font,
                rule,
                new Vector2(x, lineStartY + i *lineDimensionsHeight),
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
        
        spriteBatch.Draw(_pixelTex, _backButton, Color.DarkGray);
        string ButtonName = "Back";
        float scaleButton = 1.5f;
        Vector2 scaledSize = _font.MeasureString(ButtonName) *scaleButton;
        Vector2 buttonPos = new Vector2(
            _backButton.X + _backButton.Width / 2f - scaledSize.X / 2f,
            _backButton.Y + _backButton.Height / 2f - scaledSize.Y / 2f);
        spriteBatch.DrawString(
            _font,
            ButtonName,
            buttonPos,
            Color.White,
            0f,
            Vector2.Zero,
            scaleButton,
            SpriteEffects.None,
            0f);

    }
}