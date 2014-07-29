#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;
#endregion

namespace InfiniTag
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class InfiniTagMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player1;
        List<Mobile> mobList;
        Controls controls;
        ScrollBack background;
        Random rnd;
        Double mobTimer;
        int scrollSpeed;

        SpriteFont Font1;



        //HUD stuff
        Texture2D RuleBar;
        private int score;
        private double initialTime = 3;
        private double time = 3;
        private int meter;
        private int meterRed;
        private int meterGreen;
        private int meterBlue;
        private int gameSounds;
        private int leaderBoard;

        //HUD location information
        private int ruleBarX = 5;
        int barThickness = 35;



        Vector2 scorePos = new Vector2(10, 10);

        private int screenWidth = 480;
        private int screenHeight = 640;

        private bool pause = false;
        private bool gameOver = false;

        public InfiniTagMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            background = new ScrollBack();
            scrollSpeed = 3;

            mobTimer = 0;
            rnd = new Random();
            player1 = new Player(screenWidth/2-25, screenHeight/2-25, 50, 50);
            mobList = new List<Mobile>();
            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player1.LoadContent(this.Content);
            background.Initialize(Content, "bg1.jpg", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -scrollSpeed);

            RuleBar = Content.Load<Texture2D>("Bar.png") as Texture2D;

            Font1 = Content.Load<SpriteFont>("font");

            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void NewMobile()
        {
            int xpos = rnd.Next(1, (screenWidth-51));
            Mobile tempMob = new Mobile(xpos, -50, 50, 50, scrollSpeed, rnd.Next(1, 8));
            tempMob.LoadContent(this.Content);
            mobList.Add(tempMob);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();
            if (controls.onPress(Keys.Enter, Buttons.LeftShoulder))
            {
                pause = !pause;
            }

            if (!(pause || gameOver))
            {
                background.Update(gameTime);

                mobTimer += gameTime.ElapsedGameTime.TotalSeconds;
                time -= gameTime.ElapsedGameTime.TotalSeconds;
                if (time <= 0)
                    gameOver = true;

                if (mobTimer > 0.5)
                {
                    mobTimer = 0;
                    NewMobile();
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                // TODO: Add your update logic here
                //Up, down, left, right affect the coordinates of the sprite

                player1.Update(controls, gameTime);
                borderCheck();


                // Mob update/ collision check loop
                int playerX = player1.getX();
                int playerY = player1.getY();
                for (int i = mobList.Count - 1; i >= 0; i--)
                {
                    mobList[i].Update(controls, gameTime);
                    int mobX = mobList[i].getX();
                    int mobY = mobList[i].getY();

                    //collision detection loop
                    if (Collision(mobX, mobY, playerX, playerY, 37))
                    {
                        // temporary collision code
                        switch(mobList[i].getId())
                        {
                            case 1:
                                mobList.RemoveAt(i);
                                score++;
                                meterRed++;
                                meter++;
                                time = initialTime;
                                break;
                            case 2:
                                mobList.RemoveAt(i);
                                score++;
                                meterGreen++;
                                meter++;
                                time = initialTime;
                                break;
                            case 3:
                                mobList.RemoveAt(i);
                                score++;
                                meterBlue++;
                                meter++;
                                time = initialTime;
                                break;
                            default:
                                gameOver = true;
                                break;
                        }
                    }

                    else if (mobY > screenHeight)
                    {
                        mobList.RemoveAt(i);
                    }
                }
                if (meter == 20)
                    emptyMeters();



                if (gameOver)
                {
                    Reset();
                    gameOver = false;
                }

                base.Update(gameTime);

            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            foreach (Mobile m in mobList)
            {
                m.Draw(spriteBatch);
            }
            player1.Draw(spriteBatch);

            drawRuleBar();
            drawTimeBar();
            drawScore();



            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void drawScore()
        {
            string scoreString = "Score: " + score;
            spriteBatch.DrawString(Font1, scoreString, scorePos, Color.Black);
        }

        public void drawRuleBar()
        {
            //this is the background color of the bar
            spriteBatch.Draw(RuleBar, new Rectangle(ruleBarX, this.Window.ClientBounds.Height - 50, RuleBar.Width / 2, barThickness+5), new Rectangle(0, 45, RuleBar.Width, barThickness), Color.Black);


            //If the above clamp is changed then the (double)RuleCharge/x must also be modified.
            //this is the stuff that will fill up the bar, might need to be modified for color coding (split up the bar itself?)
            spriteBatch.Draw(RuleBar, new Rectangle(ruleBarX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterRed / 20)), barThickness),
                 new Rectangle(0, 45, RuleBar.Width, barThickness), Color.Red);

            int redMeterEndX = ruleBarX + (int)(RuleBar.Width / 2 * ((double)meterRed / 20));

            spriteBatch.Draw(RuleBar, new Rectangle(redMeterEndX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterGreen / 20)), barThickness),
                 new Rectangle(0, 45, RuleBar.Width, barThickness), Color.Green);

            int greenMeterEndX = redMeterEndX + (int)(RuleBar.Width / 2 * ((double)meterGreen / 20));

            spriteBatch.Draw(RuleBar, new Rectangle(greenMeterEndX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterBlue / 20)), barThickness),
                 new Rectangle(0, 45, RuleBar.Width, barThickness), Color.Blue);

            //This draws the border of the bar
            spriteBatch.Draw(RuleBar, new Rectangle(5,
                this.Window.ClientBounds.Height - 50, RuleBar.Width / 2, barThickness+5), new Rectangle(0, 0, RuleBar.Width, barThickness), Color.White);
        }

        public void drawTimeBar()
        {
            spriteBatch.Draw(RuleBar, new Rectangle(ruleBarX + RuleBar.Width / 2, this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * (time / initialTime)), barThickness + 5), new Rectangle(0, 45, RuleBar.Width, barThickness), Color.Yellow);
        }

        // checks if two particles 1 and 2 come within distance d of each other.
        // returns true if they do, else false.
        private bool Collision(int x1, int y1, int x2, int y2, int d)
        {
            if (((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) < d * d)
                return true;
            else
                return false;
        }

        //this method keeps the player in the borders.
        private void borderCheck()
        {
            int x = player1.getX();
            int y = player1.getY();

            if (x >= (screenWidth-50))
            {
                player1.setX(screenWidth-51);
            }
            if (x <= 0)
            {
                player1.setX(1);
            }
            if (y >= (screenHeight-50))
            {
                player1.setY(screenHeight-51);
            }
            if (y <= 0)
            {
                player1.setY(1);
            }
        }

        private void emptyMeters()
        {
            meterRed = 0;
            meterGreen = 0;
            meterBlue = 0;
            meter = 0;
        }

        private void Reset()
        {
            for (int i = mobList.Count - 1; i >= 0; i--)
            {
            mobList.RemoveAt(i);
            }
            player1.setX(screenWidth/2-25);
            player1.setY(screenHeight/2-25);
            score = 0;
            time = 0;
            emptyMeters();
            time = initialTime;
        }


        }

    }




