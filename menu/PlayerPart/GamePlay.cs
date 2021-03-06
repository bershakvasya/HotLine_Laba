using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using menu.PlayerPart.Connectors;

namespace menu.PlayerPart
{
    class GamePlay
    {
        static Random r = new Random();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player player;
        private Map map;
        private GraphicsDevice g;
        private List<Enemy> e = new List<Enemy>();
        private Target playerT = new Target();
        private Vector2 position;
        private MouseState lastMouseState;
        private ContentManager Content;
        public GamePlay(ContentManager Content, GraphicsDeviceManager g)
        {
            _graphics = g;
            //_graphics = new GraphicsDeviceManager(game);
            //Content.RootDirectory = "Content";
            //game.IsMouseVisible = true;
            this.Content = Content;

        }
        public void Initialize()
        {
            player = new Player(new Vector2((_graphics.PreferredBackBufferWidth - 40) / 2, (_graphics.PreferredBackBufferHeight - 26) / 2));
            //loader = new MapLoader("die1", new Vector2(100, 100), new Vector2(0, 0));
            
            e.Add(new Enemy(new Vector2(r.Next(100, 600), r.Next(100, 600))));
        }
        public void LoadContent(ContentManager manager ,SpriteBatch _spriteBatch, GraphicsDevice gd)
        {
            
            map = new Map(Content.Load<Texture2D>("unknown"), Vector2.Zero);
            map.LoadContent(manager, gd);
            this._spriteBatch = _spriteBatch;
            player.LoadContent(Content);
            foreach (var enemy in e)
            {
                enemy.LoadContent(Content);
            }
        }
        public void Update()
        {
            
            // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            // Exit();

            // TODO: Add your update logic here
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.X != lastMouseState.X ||
                currentMouseState.Y != lastMouseState.Y)
                position = new Vector2(currentMouseState.X, currentMouseState.Y);
            lastMouseState = currentMouseState;
            Vector2 pos = player.Update();
            playerT.Update(player.Position, player.SourceRectangle);
            foreach (var enemy in e)
            {
                enemy.Update(pos, playerT.Position, playerT.Bound);
            }

            //Map m=loader.Update(pos, playerT.Bound);
            //if (m.OpenNow)
            //{
            //    map = m;
            //}
            map.Update(pos);
            for (int i = 0; i < player.bullets.Count; i++)
            {
                for (int j = 0; j < e.Count; j++)
                {
                    if (e[j].status != PlayerStatus.die)
                    {
                        if (player.bullets[i].bound.Intersects(e[j].boundBox))
                        {
                            e[j].status = PlayerStatus.die;
                            player.bullets.RemoveAt(i);
                            i--;
                            Enemy en = new Enemy(new Vector2(r.Next(100, 600), r.Next(100, 600)));
                            en.LoadContent(Content);
                            e.Add(en);
                            break;
                        }
                    }

                }

            }


            for (int j = 0; j < e.Count; j++)
            {
                if (e[j].status != PlayerStatus.die)
                {
                    if (player.Bound.Intersects(e[j].boundBox) && player.status == PlayerStatus.hit && player.currentFrame == 3)
                    {
                        e[j].status = PlayerStatus.die;
                        Enemy en = new Enemy(new Vector2(r.Next(100, 600), r.Next(100, 600)));
                        en.LoadContent(Content);
                        e.Add(en);
                        break;
                    }
                    else if (player.Bound.Intersects(e[j].boundBox))
                    {

                        if (e[j].interval <= 0)
                        {
                            e[j].status = PlayerStatus.hit;
                            e[j].interval = 50;
                            player.health--;
                            if (player.health <= 0)
                            {
                                player.status = PlayerStatus.die;
                            }
                        }
                        e[j].interval--;
                    }
                }

            }
        }
        public void Draw()
        {
            //g.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            map.Draw(_spriteBatch);
            foreach (var enemy in e)
            {
                enemy.Draw(_spriteBatch);
            }

            player.Draw(_spriteBatch);
            //loader.Draw(_spriteBatch);
        }
    }
}

