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
    public class Obstacle : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region protected Members
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;
        protected Texture2D obstacleSprite;
        protected Vector2 obstaclePosition;
        Rectangle bounds;

        public bool wireframe = false;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the X position of the paddle.
        /// </summary>
        public float X
        {
            get { return obstaclePosition.X; }
            set { obstaclePosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the paddle.
        /// </summary>
        public float Y
        {
            get { return obstaclePosition.Y; }
            set { obstaclePosition.Y = value; }
        }

        public int Width
        {
            get { return obstacleSprite.Width; }
        }

        /// <summary>
        /// Gets the height of the paddle's sprite.
        /// </summary>
        public int Height
        {
            get { return obstacleSprite.Height; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the paddle.
        /// </summary>
        ///         
        public Rectangle Boundary
        {
            get
            {
                bounds = new Rectangle((int)obstaclePosition.X, (int)obstaclePosition.Y,
                    obstacleSprite.Width, obstacleSprite.Height - 10);
                return bounds;
            }
        }

        #endregion
        #region Code
        public Obstacle(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
        }
        public void Reset()
        {
            //Randomly choose a new location
            Random rand = new Random();
            //float 

            obstaclePosition.Y = GraphicsDevice.Viewport.Height + 50;
            obstaclePosition.X = GraphicsDevice.Viewport.Width + 50;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            obstacleSprite = contentManager.Load<Texture2D>(@"Content\Images\brick");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(obstacleSprite, obstaclePosition, Color.White);

            Texture2D SimpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            SimpleTexture.SetData<Int32>(pixel, 0, SimpleTexture.Width * SimpleTexture.Height);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
