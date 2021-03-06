﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    class PaddleHuman : Paddle
    {

        public PaddleHuman(Game game)
            : base(game)
        {
        }
        #region Private Variables
        private bool mouseControl = false;
        private MouseState ms = Mouse.GetState();
        #endregion

        #region Methods
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            paddleSprite = contentManager.Load<Texture2D>(@"Content\Images\player_paddle");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Scale the movement based on time
            float moveDistance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Press M to toggle mouse-control / keyboard-control
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                mouseControl = true;
            }

            // mouse-control
            if (mouseControl == true)
            {
                ms = Mouse.GetState();
                if (X > ms.X)
                {
                    X -= moveDistance;
                }
                else if (X < ms.X)
                {
                    X += moveDistance;
                }
            }

            // keyboard-control
            KeyboardState newKeyState = Keyboard.GetState();
            if ((newKeyState.IsKeyDown(Keys.Left) || newKeyState.IsKeyDown(Keys.A)) &&
                X - moveDistance >= 0 )
            {
                X -= moveDistance;
            }
            else if ((newKeyState.IsKeyDown(Keys.Right) || newKeyState.IsKeyDown(Keys.D)) &&
                X + paddleSprite.Width + moveDistance <= GraphicsDevice.Viewport.Width)
            {
                X += moveDistance;
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
