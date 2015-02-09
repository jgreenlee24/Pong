/*
 * Basketball Pong
 * by Frank McCown, Harding University
 * Spring 2012
 * 
 * Sounds: Creative Commons Sampling Plus 1.0 License.
 * http://www.freesound.org/samplesViewSingle.php?id=34201
 * http://www.freesound.org/samplesViewSingle.php?id=12658
 */

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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        private Ball ball;
        private PaddleHuman paddle;
        private PaddleComputer comp_paddle;
        private Obstacle brick;
        private SpriteBatch scoreRecord;
        private SpriteBatch credits;
        private bool paused = false;
        private bool pauseKeyDown = false;

        //Used to keep track when paused
        private float paddleSpeed;
        private float compSpeed;
        private float ballSpeedX;
        private float ballSpeedY;

        //Keep track of the player and computer scores
        private int myScore;
        private int computerScore;
        private SpriteFont font;

        private SoundEffect pongSound;
        private SoundEffect scoreSound;
        private SoundEffect compScoreSound;
        
        // Used to delay between rounds 
        private float delayTimer = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ball = new Ball(this);
            paddle = new PaddleHuman(this);
            comp_paddle = new PaddleComputer(this);
            brick = new Obstacle(this);

            Components.Add(ball);
            Components.Add(paddle);
            Components.Add(comp_paddle);
            Components.Add(brick);
            comp_paddle.ball = Components[0] as Ball;

            // Call Window_ClientSizeChanged when screen size is changed
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Move paddle back onto screen if it's off
            paddle.Y = GraphicsDevice.Viewport.Height - paddle.Height;
            if (paddle.X + paddle.Width > GraphicsDevice.Viewport.Width)
                paddle.X = GraphicsDevice.Viewport.Width - paddle.Width;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize local variables
            IsMouseVisible = true;
            myScore = 0;
            computerScore = 0;

            // Set the window's title bar
            Window.Title = "Pong!";

            graphics.ApplyChanges();

            // Initialize all components and load content
            base.Initialize();

            // Initialize ball position
            ball.X = GraphicsDevice.Viewport.Width / 2 - ball.Width / 2;
            ball.Y = GraphicsDevice.Viewport.Height / 2 - ball.Height / 2;

            // Randomly select ball direction and speed

            // Initialize paddle positions
            comp_paddle.Y = 0;
            paddle.Y = GraphicsDevice.Viewport.Height - paddle.Height;
            comp_paddle.X = paddle.X = GraphicsDevice.Viewport.Width / 2 - paddle.Width / 2;
            brick.Y = GraphicsDevice.Viewport.Height / 2 + 50;
            brick.X = GraphicsDevice.Viewport.Width / 2;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            scoreRecord = new SpriteBatch(GraphicsDevice);
            credits = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("scoreFont");
            pongSound = Content.Load<SoundEffect>(@"Audio\pong");
            scoreSound = Content.Load<SoundEffect>(@"Audio\playerScore");
            compScoreSound = Content.Load<SoundEffect>(@"Audio\score");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Pause game and display credits
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (pauseKeyDown && paused)
                {
                    pauseKeyDown = false;
                    paused = false;

                    paddle.Speed = paddleSpeed;
                    comp_paddle.Speed = compSpeed;
                    ball.SpeedX = ballSpeedX;
                    ball.SpeedY = ballSpeedY;
                }
                else
                {
                    paused = true;
                    pauseKeyDown = true;

                    paddleSpeed = paddle.Speed;
                    compSpeed = comp_paddle.Speed;
                    ballSpeedX = ball.SpeedX;
                    ballSpeedY = ball.SpeedY;
                }
                    
                // Display credits
            }

            if (paused)
            {
                paddle.Speed = 0;
                comp_paddle.Speed = 0;
                ball.SpeedX = 0;
                ball.SpeedY = 0;
            }

            // Enable wireframing
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                    paddle.wireframe = true;
                    comp_paddle.wireframe = true;
                    ball.wireframe = true;
            }

            // Disable mouse visibility - enable mouse-control
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                if (IsMouseVisible) IsMouseVisible = false;
                else IsMouseVisible = true;
            }

            // Press F to toggle full-screen mode
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Wait until a second has passed before animating ball 
            delayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delayTimer > 1)
                ball.Enabled = true;

            int maxX = GraphicsDevice.Viewport.Width - ball.Width;
            int maxY = GraphicsDevice.Viewport.Height - ball.Height;

            // Score! Reset Ball and Timer
            if (ball.Y < 0 || ball.Y > maxY)
            {
                //Increment the score accordingly
                if (ball.Y < 0)
                {
                    myScore++;
                    scoreSound.Play();
                }
                else
                {
                    computerScore++;
                    compScoreSound.Play();
                }
                // Score! - reset ball
                ball.Reset();

                // Reset timer and stop ball's Update() from executing
                delayTimer = 0;
                ball.Enabled = false;
            }

            // Collision with Wall
            if (ball.X < 0 || ball.X > maxX)
            {
                ball.ChangeHorzDirection();
            }

            //Collision with obstacle
            if (ball.Boundary.Intersects(brick.Boundary))
            {
                //Moving to the top right-hand corner
                if (ball.SpeedY < 0 && ball.SpeedX > 0)
                {
                    if (ball.X + ball.Boundary.Width < brick.Boundary.X)
                        ball.ChangeVertDirection();
                    else
                        ball.ChangeHorzDirection();
                }
                //Moving to the top left-hand corner
                else if (ball.SpeedY < 0 && ball.SpeedX < 0)
                {
                    if (ball.X < (brick.Boundary.X + brick.Boundary.Width))
                        ball.ChangeHorzDirection();
                    else
                        ball.ChangeVertDirection();
                }
                //Moving to the bottom left-hand corner
                else if (ball.SpeedY > 0 && ball.SpeedX < 0)
                {
                    if (ball.Boundary.X > (brick.Boundary.X + brick.Boundary.Width))
                        ball.ChangeHorzDirection();
                    else
                        ball.ChangeVertDirection();
                }
                //Moving to the bottom right-hand corner
                else if (ball.SpeedY > 0 && ball.SpeedX > 0)
                {
                    if (ball.Boundary.X + ball.Boundary.Width < brick.Boundary.X)
                        ball.ChangeHorzDirection();
                    else
                        ball.ChangeVertDirection();
                }

                brick.Reset(ref ball, GraphicsDevice);
            }


            // Collision with Paddle
            if (ball.Boundary.Intersects(paddle.Boundary) && ball.SpeedY > 0 ||
                ball.Boundary.Intersects(comp_paddle.Boundary) && ball.SpeedY < 0)
            {
                pongSound.Play();

                // Enable Reflection
                float ballMiddle = (ball.X + ball.Width) / 2;
                float paddleMiddle = (paddle.X + paddle.Width) / 2;
                float compMiddle = (comp_paddle.X + comp_paddle.Y) / 2;
                if ((ballMiddle < paddleMiddle && ball.SpeedX > 0) ||
                    (ballMiddle > paddleMiddle && ball.SpeedX < 0) ||
                    (ballMiddle < compMiddle && ball.SpeedX > 0) ||
                    (ballMiddle > compMiddle && ball.SpeedX < 0))
                {
                    //ball.ChangeHorzDirection();
                    //ball.SpeedY *= -1;
                    ball.SpeedX *= 1;
                }
                
                // Redirect ball vertically - collided with paddle
                ball.ChangeVertDirection();
                ball.SpeedUp();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the player/computer score to the window
        /// </summary>
        private void DrawText()
        {   
            scoreRecord.DrawString(font, "Player: " + myScore.ToString() + "\nComputer: " + computerScore.ToString(), new Vector2(10, 10), Color.White);
        }

        /// <summary>
        /// Draws the player/computer score to the window
        /// </summary>
        private void DrawCredits()
        {
            credits.DrawString(font, "   Created By:\n   Tyler Apgar\n           &\nJustin Greenlee", new Vector2((GraphicsDevice.Viewport.Width / 2) - 40, (GraphicsDevice.Viewport.Height / 2) - 40), Color.White);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            scoreRecord.Begin();
            GraphicsDevice.Clear(Color.Black);
            DrawText();
            scoreRecord.End();

            if (paused)
            {
                credits.Begin();
                DrawCredits();
                credits.End();
            }
            
            base.Draw(gameTime);
        }
    }
}
