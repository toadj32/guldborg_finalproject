using System;
using System.Collections.Generic;
using System.Numerics;
using Final_Project;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public class Blocks
{
    public bool isInteracting = false; // if block is being moved
    private Texture2D blockTexture;
    public Vector2 blockPos;
    public float rotation = 0f;
    private Vector2 dragOffset;
    
    public Blocks(Texture2D texture,  Vector2 blockPos)
    {
        blockTexture = texture;
        this.blockPos = blockPos;
    }

    public Rectangle Hitbox()
    {
            //bool isVertical = ((int)MathHelper.ToDegrees(rotation) / 90) % 2 != 0; // check orientaiton
            int rotSteps = (int)MathHelper.ToDegrees(rotation) / 90; 
            bool isVertical = (rotSteps % 2) != 0; // check orientaiton
            float w = isVertical ? blockTexture.Height : blockTexture.Width *0.2f;
            float h = isVertical ? blockTexture.Width : blockTexture.Height *0.2f;
            
            Rectangle hitbox = new Rectangle((int)(blockPos.X - w/2), (int)(blockPos.Y - h/2), (int)w, (int)h);
            return hitbox;
    }

    
    // mouse/ interaction logic
    public bool MousePokeBlock(MouseState mouse, MouseState prevMouse, Vector2 cameraPosition)
    {
        Vector2 mousePos = new Vector2(mouse.X, mouse.Y) + cameraPosition;
        //lmb was just pressed/ being held down/ let go
        bool lmbJustDown = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        bool lmbStillDown = mouse.LeftButton == ButtonState.Pressed;
        bool lmbJustReleased = mouse.LeftButton== ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
        //for rotation
        bool rmbJustDown = mouse.RightButton== ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released;
        
        //detect drag
        if (lmbJustDown && Hitbox().Contains(mousePos.ToPoint()))
        {
            isInteracting = true;
            dragOffset = mousePos - blockPos;
            //return true;
        }
        if (lmbStillDown && isInteracting) // beign dragged
        {
            blockPos = mousePos-dragOffset;
            if (rmbJustDown) //allow rotate while dragged
            {
                rotation = (rotation + 0.5f * MathHelper.Pi) % (2 * MathHelper.Pi);
            }
            return true;
        }

        if (lmbJustReleased && isInteracting)
        {
            isInteracting = false;
            // add decrease size to make it look like its falling
            //return false;
        }

        return isInteracting;

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        Vector2 center = new Vector2(blockTexture.Width / 2, blockTexture.Height / 2);
        spriteBatch.Draw(blockTexture,
            blockPos - cameraPosition,
            null,
            Color.White, 
            rotation, 
            center, 
            0.2f, 
            SpriteEffects.None, 
            0f);
        
    }

    public void BlockCollision(Ball ball)
    {
        if (!Hitbox().Intersects(new Rectangle(
                (int)(ball.Center().X - ball.Radius),
                (int)(ball.Center().Y - ball.Radius),
                (int)(ball.Radius * 2),
                (int)(ball.Radius * 2))))
            return;
        Vector2 normal = ball.Center() - new Vector2(
            Hitbox().X + Hitbox().Width / 2f,
            Hitbox().Y + Hitbox().Height / 2f);
        normal.Normalize();

        ball.Position += normal * 2f;
        ball.Velocity = normal * 80f;
    }
}