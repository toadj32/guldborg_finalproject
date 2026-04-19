using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace guldborg_finalproject;

public class Blocks
{
    public bool isInteracting = false; // if block is being moved
    private Texture2D blockTexture;
    public Vector2 blockPos;
    private float blockSize;
    public float rotation = 0f;
    private Vector2 dragOffset;
    
    //private MouseState mouseState;
    //private MouseState lastMouseState;
    
    public Blocks(Texture2D texture,  Vector2 blockPos,  float blockSize)
    {
        blockTexture = texture;
        this.blockPos = blockPos;
        this.blockSize = blockSize;
    }

    public Rectangle Hitbox
    {
        get
        {
            //bool isVertical = ((int)MathHelper.ToDegrees(rotation) / 90) % 2 != 0; // check orientaiton
            int rotSteps = (int)MathHelper.ToDegrees(rotation) / 90; 
            bool isVertical = (rotSteps % 2) != 0; // check orientaiton
            int w = isVertical ? blockTexture.Height : blockTexture.Width;
            int h = isVertical ? blockTexture.Width : blockTexture.Height;
            
            Rectangle hitbox = new Rectangle((int)blockPos.X/2, (int)blockPos.Y/2, w, h);
            return hitbox;
        }
    }

    
    // mouse/ interaction logic
    public bool MousePokeBlock(MouseState mouse, MouseState prevMouse)
    {
        Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
        //lmb was just pressed/ being held down/ let go
        bool lmbJustDown = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        bool lmbStillDown = mouse.LeftButton == ButtonState.Pressed;
        bool lmbJustReleased = mouse.LeftButton== ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
        //for rotation
        bool rmbJustDown = mouse.RightButton== ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released;
        
        //detect drag
        if (lmbJustDown && Hitbox.Contains(mousePos.ToPoint()))
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


    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 center = new Vector2(blockTexture.Width / 2, blockTexture.Height / 2);
        spriteBatch.Draw(blockTexture,
            blockPos,
            null,
            Color.White, 
            rotation, 
            center, 
            blockSize, 
            SpriteEffects.None, 
            0f);
        
    }
    
    /*
    public List<Rectangle> Collison()
    {
        List<Rectangle> hitboxes = new List<Rectangle>();
        foreach (var block in hitboxes)
        {

        }

    }

    private void ifDragged(MouseState mouseState)
    {
        // change pos according to mouse
        // somehow also allow ball to pass through, maybe if enough of the block overlaps with ball?
    }

    private void Rotate(MouseState mouseState)
    {
        //rotate txture
        // rotate hitbox
    }*/
    
    
    
}