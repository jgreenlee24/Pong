using System;
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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Paddle : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region protected Members
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;
        protected Texture2D paddleSprite;
        protected Vector2 paddlePosition;
        Rectangle bounds;

        public bool wireframe = false;

        protected const float DEFAULT_X_SPEED = 250;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the paddle horizontal speed.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the X position of the paddle.
        /// </summary>
        public float X
        {
            get { return paddlePosition.X; }
            set { paddlePosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the paddle.
        /// </summary>
        public float Y
        {
            get { return paddlePosition.Y; }
            set { paddlePosition.Y = value; }
        }

        public int Width
        {
            get { return paddleSprite.Width; }
        }

        /// <summary>
        /// Gets the height of the paddle's sprite.
        /// </summary>
        public int Height
        {
            get { return paddleSprite.Height; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the paddle.
        /// </summary>
        ///         
        public Rectangle Boundary
        {
            get
            {
                bounds = new Rectangle((int)paddlePosition.X, (int)paddlePosition.Y,
                    paddleSprite.Width, paddleSprite.Height);
                return bounds;
            }
        }


        #endregion
        #region Code
        public Paddle(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            Speed = DEFAULT_X_SPEED;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            paddleSprite = contentManager.Load<Texture2D>(@"Content\Images\hand");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {           
            spriteBatch.Begin();
            spriteBatch.Draw(paddleSprite, paddlePosition, Color.White);

            Texture2D SimpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            SimpleTexture.SetData<Int32>(pixel, 0, SimpleTexture.Width * SimpleTexture.Height);

            if (wireframe)
            {
                Color color = Color.White;
                spriteBatch.Draw(SimpleTexture, new Rectangle(bounds.Left, bounds.Top, bounds.Width, 1), color);
                spriteBatch.Draw(SimpleTexture, new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 1), color);
                spriteBatch.Draw(SimpleTexture, new Rectangle(bounds.Left, bounds.Top, 1, bounds.Height), color);
                spriteBatch.Draw(SimpleTexture, new Rectangle(bounds.Right, bounds.Top, 1, bounds.Height + 1), color);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
