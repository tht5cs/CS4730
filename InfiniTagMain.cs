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



        //HUD stuff
        Texture2D RuleBar;
        private int score;
        private int time;
        private int meter;
        private int meterRed;
        private int meterGreen;
        private int meterBlue;
        private int gameSounds;
        private int leaderBoard;

        //HUD location information
        private int ruleBarX = 5;

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
                                break;
                            case 2:
                                mobList.RemoveAt(i);
                                score++;
                                meterGreen++;
                                meter++;
                                break;
                            case 3:
                                mobList.RemoveAt(i);
                                score++;
                                meterBlue++;
                                meter++;
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





            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void drawRuleBar()
        {
            //this is the background color of the bar
            spriteBatch.Draw(RuleBar, new Rectangle(5, this.Window.ClientBounds.Height - 50, RuleBar.Width / 2, 44), new Rectangle(0, 45, RuleBar.Width, 44), Color.Black);


            //If the above clamp is changed then the (double)RuleCharge/x must also be modified.
            //this is the stuff that will fill up the bar, might need to be modified for color coding (split up the bar itself?)
            spriteBatch.Draw(RuleBar, new Rectangle(ruleBarX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterRed / 20)), 44),
                 new Rectangle(0, 45, RuleBar.Width, 44), Color.Red);

            int redMeterEndX = ruleBarX + (int)(RuleBar.Width / 2 * ((double)meterRed / 20));

            spriteBatch.Draw(RuleBar, new Rectangle(redMeterEndX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterGreen / 20)), 44),
                 new Rectangle(0, 45, RuleBar.Width, 44), Color.Green);

            int greenMeterEndX = redMeterEndX + (int)(RuleBar.Width / 2 * ((double)meterGreen / 20));

            spriteBatch.Draw(RuleBar, new Rectangle(greenMeterEndX,
                 this.Window.ClientBounds.Height - 50, (int)(RuleBar.Width / 2 * ((double)meterBlue / 20)), 44),
                 new Rectangle(0, 45, RuleBar.Width, 44), Color.Blue);

            //This draws the border of the bar
            spriteBatch.Draw(RuleBar, new Rectangle(5,
                this.Window.ClientBounds.Height - 50, RuleBar.Width / 2, 44), new Rectangle(0, 0, RuleBar.Width, 44), Color.White);
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

        public void emptyMeters()
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
        }


        }

    }




