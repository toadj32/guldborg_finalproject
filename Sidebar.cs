using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project;

public class Sidebar
{
    public List<Blocks> Blocks = new List<Blocks>();
    private List<SidebarItem> _items = new List<SidebarItem>();
    public Texture2D _itemTex;
    public int _panelWidth = 150;
    public int _panelPos;
    private int _screenHeight;
    
    private SidebarItem _draggedItem = null;
    private Vector2 _dragScreenPos;  
    private bool _isDragging = false;

    public Sidebar(Texture2D itemTex, int screenWidth, int screenHeight, SpriteFont font, Texture2D blockTex)
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
                    coins.CoinCounter -= (int)_draggedItem.Price;
                    Blocks.Add(new Blocks(_draggedItem.ItemTex, worldPos));
                }

                //when u not dragging anymore
                _isDragging = false;
                _draggedItem = null;
            }
        }
        
        Vector2 worldMouse = mouseScreenPos + cameraPosition;
        foreach (var block in Blocks)
        {
            block.MousePokeBlock(mouse, prevMouse, worldMouse);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, MouseState mouse)
    {
        spriteBatch.Draw(_itemTex,
            new Rectangle(_panelPos, 0, _panelWidth, _screenHeight),
            new Color(30, 30, 30, 180));
        
        foreach (var item in _items)
        {
            item.Draw(spriteBatch, true);
        }
        
        Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
        foreach (var block in Blocks)
        {
            block.Draw(spriteBatch, cameraPosition, mousePos);
        }
        
        if (_isDragging && _draggedItem != null)
        {
            float scale = Math.Min(
                (_draggedItem.ItemSlot.Width - 10f) / _draggedItem.ItemTex.Width,
                (_draggedItem.ItemSlot.Height - 20f) / _draggedItem.ItemTex.Height);

            spriteBatch.Draw(
                _draggedItem.ItemTex,
                _dragScreenPos,
                null,
                Color.White * 0.75f,  
                0f,
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

        public void Draw(SpriteBatch spriteBatch, bool gotMoney)
        {
            Color slotBg = gotMoney ? new Color(40, 40, 40, 200) : new Color(20, 20, 20, 150);
            float scale = Math.Min(
                (ItemSlot.Width - 10f) / ItemTex.Width,
                (ItemSlot.Height - 20f) / ItemTex.Height);
            Vector2 iconPos = new Vector2(
                ItemSlot.X + ItemSlot.Width / 2f,
                ItemSlot.Y + ItemSlot.Height / 2f - 8f);

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
            
            string price = $"{(int)Price} Coins";
            Vector2 textSize = _font.MeasureString(price);
            Vector2 textPos = new Vector2(
                ItemSlot.X + ItemSlot.Width / 2f - textSize.X / 2f,
                ItemSlot.Bottom - textSize.Y - 2f);
            spriteBatch.DrawString(
                _font, price, textPos, //if you poor, it grays out
                gotMoney ? Color.Yellow : Color.Black);
        }
    }
}