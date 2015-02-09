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
        private Ball ball2;
        private PaddleHuman paddle;
        private PaddleComputer comp_paddle;
        private Obstacle brick;
        private Obstacle brick2;
        private Obstacle brick3;
        private Obstacle brick4;
        private Obstacle brick5;
        private SpriteBatch scoreRecord;
        private SpriteBatch credits;
        private bool paused = false;
        private bool pauseKeyDown = false;
        private bool collision = false;

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
            ball2 = new Ball(this);
            paddle = new PaddleHuman(this);
            comp_paddle = new PaddleComputer(this);
            brick = new Obstacle(this);
            brick2 = new Obstacle(this);
            brick3 = new Obstacle(this);

            Components.Add(ball);
            Components.Add(ball2);
            Components.Add(paddle);
            Components.Add(comp_paddle);
            Components.Add(brick);
            Components.Add(brick2);
            Components.Add(brick3);
            comp_paddle.ball = Components[0] as Ball;
            comp_paddle.ball2 = Components[1] as Ball;

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
            ball.Y = GraphicsDevice.Viewport.Height / 2 - ball.Height / 2 + 100;
            ball2.X = GraphicsDevice.Viewport.Width / 2 - ball2.Width / 2;
            ball2.Y = GraphicsDevice.Viewport.Height / 2 - ball2.Height / 2 - 100;

            // Randomly select ball direction and speed

            // Initialize paddle positions
            comp_paddle.Y = 0;
            paddle.Y = GraphicsDevice.Viewport.Height - paddle.Height;
            comp_paddle.X = paddle.X = GraphicsDevice.Viewport.Width / 2 - paddle.Width / 2;
            brick.Y = GraphicsDevice.Viewport.Height / 2;
            brick.X = GraphicsDevice.Viewport.Width / 2 + 150;
            brick2.Y = GraphicsDevice.Viewport.Height / 2;
            brick2.X = GraphicsDevice.Viewport.Width / 2;
            brick3.Y = GraphicsDevice.Viewport.Height / 2;
            brick3.X = GraphicsDevice.Viewport.Width / 2 - 150;
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

            if (myScore >= 20 || computerScore >= 20)
            {
                paused = true;
            }

            if (paused)
            {
                paddle.Speed = 0;
                comp_paddle.Speed = 0;
                ball.SpeedX = 0;
                ball.SpeedY = 0;
            }

            // Fortify Collision Detection
            if (ball.SpeedX == 0 && ball.SpeedY == 0)
            {
                ball.SpeedX = ball.X < ball2.X ? -50 : 50;
                ball.SpeedY = ball.Y < ball2.Y ? -50 : 50;
                ball2.SpeedX = ball.SpeedX * -1;
                ball2.SpeedY = ball.SpeedY * -1;
            }
            if (ball.Boundary.Intersects(ball2.Boundary) && collision == true)
            {                
                // if they're going different directions    
                ball.SpeedX = ball.X < ball2.X ? -50 : 50;
                ball.SpeedY = ball.Y < ball2.Y ? -50 : 50;
                ball2.SpeedX = ball.SpeedX * -1;
                ball2.SpeedY = ball.SpeedY * -1;
                collision = false;
            }
            else if (collision == true) collision = false;

            // Enable wireframing
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                    paddle.wireframe = true;
                    comp_paddle.wireframe = true;
                    ball.wireframe = true;
            }

            // New game - press 'n'
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                paused = false;
                myScore = 0;
                computerScore = 0;
                ball.Reset();
                ball2.Reset();

                // Initialize ball position
                ball.X = GraphicsDevice.Viewport.Width / 2 - ball.Width / 2;
                ball.Y = GraphicsDevice.Viewport.Height / 2 - ball.Height / 2 + 100;
                ball2.X = GraphicsDevice.Viewport.Width / 2 - ball2.Width / 2;
                ball2.Y = GraphicsDevice.Viewport.Height / 2 - ball2.Height / 2 - 100;
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
            if (ball.Y < 0 || ball.Y > maxY || ball2.Y < 0 || ball2.Y > maxY)
            {
                if (!paused)
                {
                    //Increment the score accordingly
                    if (ball.Y < 0 || ball2.Y < 0 && !paused)
                    {
                        myScore++;
                        scoreSound.Play();
                    }
                    else
                    {
                        computerScore++;
                        compScoreSound.Play();
                    }
                }
                if (ball.Y < 0 || ball.Y > maxY)
                {
                    // Score! - reset ball
                    ball.Reset();
                }
                else ball2.Reset();

                // Reset timer and stop ball's Update() from executing
                delayTimer = 0;
                //ball.Enabled = false;
            }

            // Collision with Wall - Ball 1
            if (ball.X < 0 || ball.X > maxX)
            {
                ball.ChangeHorzDirection();
            }

            // Collision with Wall - Ball 2
            if (ball2.X < 0 || ball2.X > maxX)
            {
                ball2.ChangeHorzDirection();
            }

            for (int j = 0; j < 2; j++) {
               Ball temp = j == 0 ? ball : ball2;

                //Collision with obstacle
                Obstacle tempBrick = brick;

                for (int i = 0; i < 3; i++)
                {
                    if (i == 0) tempBrick = brick;
                    else if (i == 1) tempBrick = brick2;
                    else if (i == 2) tempBrick = brick3;

                    if (temp.Boundary.Intersects(tempBrick.Boundary))
                    {
                        //Moving to the top right-hand corner
                        if (temp.SpeedY < 0 && temp.SpeedX > 0)
                        {
                            if (temp.X + temp.Boundary.Width < tempBrick.Boundary.X)
                                temp.ChangeHorzDirection();
                            else
                                temp.ChangeVertDirection();
                        }
                        //Moving to the top left-hand corner
                        else if (temp.SpeedY < 0 && temp.SpeedX < 0)
                        {
                            if (temp.X < (tempBrick.Boundary.X + tempBrick.Boundary.Width - 5))
                                temp.ChangeVertDirection();
                            else
                                temp.ChangeHorzDirection();    
                        }
                        //Moving to the bottom left-hand corner
                        else if (temp.SpeedY > 0 && temp.SpeedX < 0)
                        {
                            if (temp.Boundary.X > (tempBrick.Boundary.X + tempBrick.Boundary.Width - 5))
                                temp.ChangeHorzDirection();
                            else
                                temp.ChangeVertDirection();
                        }
                        //Moving to the bottom right-hand corner
                        else if (temp.SpeedY > 0 && temp.SpeedX > 0)
                        {
                            if (temp.Boundary.X + temp.Boundary.Width < tempBrick.Boundary.X)
                                temp.ChangeHorzDirection();
                            else
                                temp.ChangeVertDirection();
                        }

                        tempBrick.Reset(ref ball, GraphicsDevice);
                    }
                }
            }

            // Collision with Paddle - Ball 1
            if (ball.Boundary.Intersects(paddle.Boundary) && ball.SpeedY > 0 ||
                ball.Boundary.Intersects(comp_paddle.Boundary) && ball.SpeedY < 0)
            {
                if (!paused) pongSound.Play();

                // Redirect ball vertically - collided with paddle
                ball.ChangeVertDirection();
                ball.SpeedUp();
            }

            // Collision with Paddle - Ball 2
            if (ball2.Boundary.Intersects(paddle.Boundary) && ball2.SpeedY > 0 ||
                ball2.Boundary.Intersects(comp_paddle.Boundary) && ball2.SpeedY < 0)
            {
                pongSound.Play();

                // Redirect ball vertically - collided with paddle
                ball2.ChangeVertDirection();
                ball2.SpeedUp();
            }

            // Collision with Ball - loop through Ball Array
            if (ball.Boundary.Intersects(ball2.Boundary))
            {
                // Enable Reflection
                /*
                // Calculate the Tangent
                double radians = 0.0;
                radians = Math.Atan2(ball.Y - ball2.Y, ball.X - ball2.X);
                float tangent = (float)radians * 180 / (float)Math.PI;

                // Calculate the Original Angles
                double ballAngle1, ballAngle2;
                ballAngle1 = Math.Atan2(ball.SpeedX, ball.SpeedX);
                ballAngle2 = Math.Atan2(ball2.SpeedY, ball2.SpeedX);
                float degrees1, degrees2;
                degrees1 = (float)ballAngle1 * 180 / (float)Math.PI;
                degrees2 = (float)ballAngle2 * 180 / (float)Math.PI;

                // Calculate the New Angles
                float botAngle = ball.Y > ball2.Y ? degrees2 : degrees1;
                float topAngle = ball.Y > ball2.Y ? degrees1 : degrees2;
                float bAngle = Math.Abs(tangent - botAngle) * -1;
                float tAngle = Math.Abs(tangent - topAngle) * -1;

                ball.SpeedX *= (float)Math.Cos(bAngle);
                ball.SpeedY *= (float)Math.Sin(bAngle);
                ball2.SpeedX *= (float)Math.Cos(tAngle);
                ball2.SpeedY *= (float)Math.Sin(tAngle);
                */
                collision = true;


                // Change directions
                for (int i = 0; i < 2; i++)
                {
                    Ball temp = i == 0 ? ball : ball2;
                    //Moving to the top right-hand corner
                    if (temp.SpeedY < 0 && temp.SpeedX > 0)
                    {
                        if (temp.X + temp.Boundary.Width < brick.Boundary.X)
                            temp.ChangeVertDirection();
                        else
                            temp.ChangeHorzDirection();
                    }
                    //Moving to the top left-hand corner
                    else if (temp.SpeedY < 0 && temp.SpeedX < 0)
                    {
                        if (temp.X < (brick.Boundary.X + brick.Boundary.Width))
                            temp.ChangeHorzDirection();
                        else
                            temp.ChangeVertDirection();
                    }
                    //Moving to the bottom left-hand corner
                    else if (temp.SpeedY > 0 && temp.SpeedX < 0)
                    {
                        if (temp.Boundary.X > (brick.Boundary.X + brick.Boundary.Width))
                            temp.ChangeHorzDirection();
                        else
                            temp.ChangeVertDirection();
                    }
                    //Moving to the bottom right-hand corner
                    else if (temp.SpeedY > 0 && temp.SpeedX > 0)
                    {
                        if (temp.Boundary.X + temp.Boundary.Width < brick.Boundary.X)
                            temp.ChangeHorzDirection();
                        else
                            temp.ChangeVertDirection();
                    }
                }
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
             