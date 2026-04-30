using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Instructions
{
    private SpriteFont _font;
    private int _width;
    private int _height;
    private Rectangle _okButton;

    public Instructions(SpriteFont font, int width, int height)
    {
        _font = font;
        _width = width;
        _height = height;

        _okButton = new Rectangle(
            width / 2 + 50,
            height / 2 + 200,
            100, 50);
    }

    public bool Clicked(MouseState mouse, MouseState previousMouse)
    {
        if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            if (_okButton.Contains(mouse.Position))
                return true;
        return false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        string Title = "How To Play";
        Vector2 titlesize = _font.MeasureString(Title);
        spriteBatch.DrawString(
            _font,
            Title,
            new Vector2(_width/3f - titlesize.X +70f, _height / 2f -350f),
            Color.Black,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0f);

        string instructions = "LMB + drag to take blocks + springs from the side\n" +
                              "Blocks can guide ball\n" + "Springs can propel ball\n \n" +
                              "Make sure the blocks are on the path\n" +
                              "Collect coins to purchase more blocks\n \n" +
                              "Click left mouse key to skip preview\n" +
                              "Press the top right button to Pause or \n    access Settings\n \n" +
                              "r - restart\n" + "b - return to start screen\n" + "space - pause menu\n" + 
                              "m - mute/unmute\n \n" +
                              "Complete faster + more coins = higher score\n \nKeep the ball on the path!\n" +
                              "There are checkpoints marked by\n" +
                              "the white lines that will trigger\n" +
                              "the ending animation to give you hope\n" +
                              "and excitement!";
        Vector2 textSize = _font.MeasureString(instructions);
        spriteBatch.DrawString(
            _font,
            instructions,
            new Vector2(_width / 2.5f - textSize.X / 2.5f + 30f, _height / 2f - 200),
            Color.Black,
            0f,
            Vector2.Zero,
            0.85f,
            SpriteEffects.None,
            0f);

        spriteBatch.Draw(texture, _okButton, Color.DarkBlue);

        string button = "OK";
        Vector2 buttonSize = 1.5f * _font.MeasureString(button);
        spriteBatch.DrawString(
            _font,
            button,
            new Vector2(
                _okButton.X + _okButton.Width / 2f - buttonSize.X / 2f,
                _okButton.Y + _okButton.Height / 2f - buttonSize.Y / 2f),
            Color.White,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            0f);
        
    }
}