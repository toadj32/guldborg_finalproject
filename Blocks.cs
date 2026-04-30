﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Final_Project;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public class Blocks
{
    //public static ContentManager ContentManager { get; set; }
    //private SoundEffect ballHitSound;
    
    public bool isInteracting = false; // if block is being moved
    public Texture2D blockTexture;
    public Vector2 blockPos;
    public bool isPlaced { get; set; } = false;
    public float rotation = 0f;
    private Vector2 dragOffset;
    
    public Blocks(Texture2D texture,  Vector2 blockPos)
    {
        blockTexture = texture;
        this.blockPos = blockPos;
        
        //ballHitSound = content.Load<SoundEffect>("JDSherbert - Wooden UI SFX Pack - Confirm - 1");
    }

    public Rectangle Hitbox()
    {
            //bool isVertical = ((int)MathHelper.ToDegrees(rotation) / 90) % 2 != 0; // check orientaiton
            int rotSteps = (int)MathHelper.ToDegrees(rotation) / 90; 
            bool isVertical = (rotSteps % 2) != 0; // check orientaiton
            float w = isVertical ? blockTexture.Height*0.2f : blockTexture.Width *0.2f;
            float h = isVertical ? blockTexture.Width*0.2f : blockTexture.Height *0.2f;
            
            Rectangle hitbox = new Rectangle((int)(blockPos.X - w/2), (int)(blockPos.Y - h/2), (int)(w*1.25), (int)(h*1.25));
            return hitbox;
    }

    
    // mouse/ interaction logic
    public bool MousePokeBlock(MouseState mouse, MouseState prevMouse, Vector2 mousePos)
    {
        // if block is placed, you can't use it again har har
        if (isPlaced)
        {
            isInteracting = false;
            return isInteracting;
        }
        
        //lmb was just pressed/ being held down/ let go
        bool lmbJustDown = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        bool lmbStillDown = mouse.LeftButton == ButtonState.Pressed;
        bool lmbJustReleased = mouse.LeftButton== ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
        //for rotation
        
        //detect drag
        if (lmbJustDown && Hitbox().Contains(mousePos.ToPoint()) && !isPlaced)
        {
            isInteracting = true;
            dragOffset = mousePos - blockPos;
            //return true;
        }
        if (lmbStillDown && isInteracting && !isPlaced) // beign dragged
        {
            blockPos = mousePos-dragOffset;
            return true;
        }

        if (lmbJustReleased && isInteracting)
        {
            isInteracting = false;
            isPlaced = true;
            return false;
            // add decrease size to make it look like its falling
            //return false;
        }

        return isInteracting;

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Vector2 mousePos)
    {   Vector2 center = new Vector2(blockTexture.Width / 2, blockTexture.Height / 2);
        spriteBatch.Draw(blockTexture,
            blockPos, // - cameraPosition,
            null,
            Color.White, 
            rotation, 
            center, 
            0.2f, 
            SpriteEffects.None, 
            0f);
    }

    public bool BlockCollision(Ball ball) //c ahnged this to bool to implemment sound upon hit
    {
        if (!Hitbox().Intersects(new Rectangle(
                (int)(ball.Center().X - ball.Radius),
                (int)(ball.Center().Y - ball.Radius),
                (int)(ball.Radius * 2),
                (int)(ball.Radius * 2))))
        {
            //ballHitSound.Play();
            return false;
        }

        Vector2 normal = ball.Center() - new Vector2(
            Hitbox().X + Hitbox().Width / 2f,
            Hitbox().Y + Hitbox().Height / 2f);
        normal.Normalize();

        ball.Position += normal * 2f;
        ball.Velocity = normal * 80f;

        return true;
    }
}