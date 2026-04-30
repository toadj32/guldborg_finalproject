using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Sidebar
{
    //public static ContentManager ContentManager { get; set; }
    //private SoundEffect ballHitSound;
    
    public List<Blocks> Blocks = new List<Blocks>();
    public List<Spring> Springs = new List<Spring>();
    private List<SidebarItem> _items = new List<SidebarItem>();
    public Texture2D _itemTex;
    private float rotation = 0.0f;
    public int _panelWidth = 150;
    public int _panelPos;
    private int _screenHeight;
    private bool vert; // if the spring generated is vertical or horizontal
    
    private SidebarItem _draggedItem = null;
    private Vector2 _dragScreenPos;  
    private bool _isDragging = false;
    private ContentManager _content;

    //private float dragTimer = 0f; // tryign to make block remain existing when let go of but not interactable

    public Sidebar(Texture2D itemTex, int screenWidth, int screenHeight, SpriteFont font, Texture2D blockTex, Texture2D springTex)
    {
        _itemTex = itemTex;
        _panelPos = screenWidth - _panelWidth;
        _screenHeight = screenHeight;

        AddItem(new SidebarItem(
            "Block",
            blockTex,
            20f,
            new Rectangle(_panelPos + 25, 100, 100, 100),
            font));
        
        AddItem(new SidebarItem(
            "Spring",
            springTex,
            30f,
            new Rectangle(_panelPos + 50, 270, (int)(springTex.Width*0.15f), (int)(springTex.Height*0.15f)),
            font));
    }

    public void AddItem(SidebarItem item)
    {
        _items.Add(item);
    }

    public void Update(GameTime gameTime, MouseState mouse, MouseState prevMouse, Vector2 cameraPosition, CoinManager coins)
    {
        Vector2 mouseScreenPos = new Vector2(mouse.X, mouse.Y);

        // --- Pick up: start drag when clicking on a sidebar item ---
        if (!_isDragging && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
        {
            foreach (var item in _items)
            {
                if (item.ItemSlot.Contains(mouse.Position) && coins.CoinCounter >= item.Price)
                {
                    _isDragging = true;
                    _draggedItem = item;
                    if (item.ItemName == "Spring")
                    {
                        vert = new Random().Next(2) == 0;
                        rotation = vert 
                            ? 0f 
                            : (float)(Math.PI / 2f); 
                    }
                    
                    //dragTimer = (float)gameTime.ElapsedGameTime.TotalSeconds; // tryign to make block remain existing when let go of but not interactable
                    
                    break;
                }
            }
        }
        
        if (_isDragging)
        {
            _dragScreenPos = mouseScreenPos;

            //no, do not just spawn at the ball when I click you
            if (mouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed)
            {
                bool droppedOnSidebar = mouse.X >= _panelPos;

                if (!droppedOnSidebar)
                {
                    //ugh include offset
                    Vector2 worldPos = mouseScreenPos + cameraPosition;
                    
                    if (_draggedItem.ItemName == "Block")
                    {
                        coins.CoinCounter -= (int)_draggedItem.Price;
                        var newBlock = new Blocks(_draggedItem.ItemTex, worldPos);
                        newBlock.isPlaced = true;
                        Blocks.Add(newBlock);
                    }
                    else if (_draggedItem.ItemName == "Spring")
                    {
                        var newSpring = new Spring(_draggedItem.ItemTex, vert);
                        newSpring.setPos(worldPos);
                        newSpring.justSpawned = false;
                        newSpring.isPlaced = true;
                        Springs.Add(newSpring);
                    }
                }

                //when u not dragging anymore
                _isDragging = false;
                _draggedItem = null;
            }
        }
        
        Vector2 worldMouse = mouseScreenPos + cameraPosition;
        foreach (var block in Blocks)
        {
            if (!block.isPlaced)
            {
                block.MousePokeBlock(mouse, prevMouse, worldMouse);
            }
            // add spring interaction stuff
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, MouseState mouse)
    {
        spriteBatch.Draw(_itemTex,
            new Rectangle(_panelPos, 0, _panelWidth, _screenHeight),
            new Color(30, 30, 30, 180));
        
        foreach (var item in _items)
        {
            item.Draw(spriteBatch, true, rotation);
        }
        
        if (_isDragging && _draggedItem != null)
        {
            float scale = Math.Min(
                (_draggedItem.ItemSlot.Width - 10f) / _draggedItem.ItemTex.Width,
                (_draggedItem.ItemSlot.Height - 20f) / _draggedItem.ItemTex.Height);
            if (_draggedItem.ItemName == "Spring")
            {
                scale = 0.17f;
            }
            
            spriteBatch.Draw(
                _draggedItem.ItemTex,
                _dragScreenPos,
                null,
                Color.White * 0.75f,  
                rotation,
                new Vector2(_draggedItem.ItemTex.Width / 2f, _draggedItem.ItemTex.Height / 2f),
                scale,
                SpriteEffects.None,
                0f);
        }
    }

    public class SidebarItem
    {
        public string ItemName;
        public Texture2D ItemTex;
        public float Price;
        public Rectangle ItemSlot;
        public SpriteFont _font;

        public SidebarItem(string itemName, Texture2D itemTex, float price, Rectangle itemSlot, SpriteFont font)
        {
            ItemName = itemName;
            ItemTex = itemTex;
            Price = price;
            ItemSlot = itemSlot;
            _font = font;
        }

        public bool IsClicked(MouseState mouse, MouseState prevMouse)
        {
            return ItemSlot.Contains(new Point(mouse.X, mouse.Y))
                   && mouse.LeftButton == ButtonState.Pressed
                   && prevMouse.LeftButton == ButtonState.Released;
        }

        public void Draw(SpriteBatch spriteBatch, bool gotMoney, float springRot)
        {
            Color slotBg = gotMoney ? new Color(40, 40, 40, 200) : new Color(20, 20, 20, 150); //YA GOT MONEY????
            float scale = Math.Min(
                (ItemSlot.Width - 10f) / ItemTex.Width,
                (ItemSlot.Height - 20f) / ItemTex.Height);
            Vector2 iconPos = new Vector2(
                ItemSlot.X + ItemSlot.Width / 2f,
                ItemSlot.Y + ItemSlot.Height / 2f - 8f);
            if (ItemName == "Block")
            {
                spriteBatch.Draw(
                    ItemTex,
                    iconPos,
                    null,
                    gotMoney ? Color.White : Color.Black,
                    0f,
                    new Vector2(ItemTex.Width / 2f, ItemTex.Height / 2f),
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            if (ItemName == "Spring")
            {
                Vector2 position = new Vector2(550, 250);
                if (springRot > 0f)
                {
                    position = new Vector2(629, 250);
                }
                spriteBatch.Draw(
                    ItemTex,
                    position,
                    null,
                    gotMoney ? Color.White : Color.Black,
                    springRot,
                    new Vector2((ItemTex.Width*0.15f) / 2f, (0.15f*ItemTex.Height) / 2f),
                    0.15f,
                    SpriteEffects.None,
                    0f);
            }
            
            string price = $"{(int)Price} Coins";
            Vector2 textSize = _font.MeasureString(price);
            Vector2 textPos = new Vector2(
                ItemSlot.X + ItemSlot.Width / 2f - textSize.X / 2f,
                ItemSlot.Bottom - textSize.Y - 2f);
            spriteBatch.DrawString(
                _font, price, textPos, //if you poor, it grays out   // damn // HAHAH
                gotMoney ? Color.Yellow : Color.Black);
        }
    }
}