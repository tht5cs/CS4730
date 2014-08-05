﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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
        int scrollSpeed = 4;
        double spawnRate = 5;

        



        //HUD stuff
        Texture2D RuleBar;
        SpriteFont Font1;
        private int score;
        private double scoreMultiplier = 1;
        private double initialTime = 3.3;
        private double time;
        private int maxMeter = 15;
        private int meter;
        private int meterRed;
        private int meterGreen;
        private int meterBlue;
        private int gameSounds;
        private int leaderBoard;

        //Change warning stuff
        private double changeDelay = 4;
        private double delayTimer = -2;
        Vector2 warningPos;

        //HUD location information
        private int borderSpacing = 5;
        private int barY = 0;

        private int timeBarX = 0;

        int barLength;
        int barThickness = 20;



        Vector2 scorePos;

        private int screenWidth = 480;
        private int screenHeight = 640;

        private bool pause = true;
        private bool gameOver = false;

        //rule meters and things
        private int ruleColorIndex = 0;
        private int ruleIndex = 0;

        //warning  text parameters
        private Color warningColor = Color.White;
        private String warningText = "";

        bool[] rulesRed = new bool[5];
        /*
         * 0: WASD remapping
         * 1: left-right inversion
         * 2: up-down inversion
         * 3: lethal borders
         * 4: hyper speed
         */
        string[] rulesRedText = 
        {
            "WASD controls",
            "left-right inversion",
            "up-down inversion",
            "lethal borders",
            "Hyper Speed"
        };
        bool[] rulesGreen = new bool[4];
        /*
         * 0:erases rule bar
         * 1:erases time bar
         * 2:erases score bar
         * 3:no change warnings
         */
        string[] rulesGreenText = 
        {
            "Rule Bar",
            "Time Bar",
            "Score Count",
            "Change Warnings"
        };

        bool[]rulesBlue = new bool[3];
        string[] rulesBlueText = 
        {
            "Theme for Harold var 3",
            "George Street Shuffle",
            "Fun in a Bottle"
        };

        //sound
        private Song currSong;
        private Song themeForHarold;
        private Song georgeStreetShuffle;
        private Song funInABottle;
        private Song gameOverSong;

        private SoundEffect tagPing;

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

            
            rnd = new Random();
            player1 = new Player(screenWidth/2-25, screenHeight/2-25, 50, 50);
            mobList = new List<Mobile>();
            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();

            mobTimer = 0;
            time = initialTime;

            adjustBars();
            scorePos = new Vector2(borderSpacing, borderSpacing);
            warningPos = new Vector2(borderSpacing, screenHeight - borderSpacing - barThickness - 30);

            MediaPlayer.Play(currSong);
            MediaPlayer.Pause();

        }

        //loads all of the songs
        protected void LoadSound()
        {
            themeForHarold = Content.Load<Song>("songs/Theme for Harold var 3.wav");
            georgeStreetShuffle = Content.Load<Song>("songs/George Street Shuffle.wav");
            funInABottle = Content.Load<Song>("songs/Fun in a Bottle.wav");
            gameOverSong = Content.Load<Song>("songs/Cryptic Sorrow.wav");

            //sfx
            tagPing = Content.Load<SoundEffect>("sfx/tag ping.wav");
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

            Font1 = Content.Load<SpriteFont>("JSM");

            LoadSound();
            currSong = themeForHarold;

            
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
            Mobile tempMob = new Mobile(xpos, -50, 50, 50, scrollSpeed, rnd.Next(1, 7));
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

            processUtilInput(controls);

            if (!(pause || gameOver))
            {
                double timePassed = gameTime.ElapsedGameTime.TotalSeconds;

                background.Update(gameTime);
                if (rulesRed[4])
                    background.Update(gameTime);
                score += (int) (100*timePassed*scoreMultiplier);
                if (delayTimer > 0)
                    delayTimer -= timePassed;
                mobTimer += timePassed;
                time -= timePassed;
                if (time <= 0)
                    endGame();

                if (mobTimer > (1/spawnRate))
                {
                    mobTimer = 0;
                    NewMobile();
                }

                

                // TODO: Add your update logic here
                //Up, down, left, right affect the coordinates of the sprite

                processMovementInput(controls);
                if (rulesRed[4])
                    player1.Update(controls, gameTime);
                player1.Update(controls, gameTime);
                borderCheck();


                // Mob update/ collision check loop
                int playerX = player1.getX();
                int playerY = player1.getY();
                for (int i = mobList.Count - 1; i >= 0; i--)
                {
                    if (rulesRed[4])
                        mobList[i].Update(controls, gameTime);
                    mobList[i].Update(controls, gameTime);
                    int mobX = mobList[i].getX();
                    int mobY = mobList[i].getY();

                    //collision detection loop
                    if (Collision(mobX, mobY, playerX, playerY, 32))
                    {
                        // temporary collision code
                        switch(mobList[i].getId())
                        {
                            case 1:
                                tagPing.Play(0.5f,0,0);
                                mobList.RemoveAt(i);
                                score += (int)(500 * scoreMultiplier);
                                meterRed++;
                                meter++;
                                time = initialTime;
                                break;
                            case 2:
                                tagPing.Play(0.5f, 0, 0);
                                mobList.RemoveAt(i);
                                score += (int)(350 * scoreMultiplier);
                                meterGreen++;
                                meter++;
                                time = initialTime;
                                break;
                            case 3:
                                tagPing.Play(0.5f, 0, 0);
                                mobList.RemoveAt(i);
                                score += (int)(200 * scoreMultiplier);
                                meterBlue++;
                                meter++;
                                time = initialTime;
                                break;
                            default:
                                endGame();
                                break;
                        }
                    }

                    else if (mobY > screenHeight)
                    {
                        mobList.RemoveAt(i);
                    }
                }
                if (meter >= maxMeter)
                {                    
                    //changeRule(chooseRuleColor());
                    ruleColorIndex = chooseRuleColor();
                    ruleIndex = pickRule(ruleColorIndex);
                    delayTimer = changeDelay;
                    setWarningText(ruleColorIndex, ruleIndex);
                    emptyMeters();
                }
                if (delayTimer <= 0 && delayTimer > -1)
                {
                    changeRule(ruleColorIndex, ruleIndex);
                    delayTimer = -2;
                }



                base.Update(gameTime);

            }
        }

        public void endGame()
        {
            gameOver = true;
            rulesGreen[2] = false;
            changeSong(gameOverSong);
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

            if (rulesRed[3])
                drawMovementBorder();

            foreach (Mobile m in mobList)
            {
                m.Draw(spriteBatch);
            }
            player1.Draw(spriteBatch);

            if (!rulesGreen[0])
                drawRuleBar();
            if (!rulesGreen[1])
                drawTimeBar();
            if (!rulesGreen[2])
                drawScore();
            if (delayTimer > 0 && !rulesGreen[3])
                spriteBatch.DrawString(Font1, warningText + " in: " + (int)delayTimer, warningPos, warningColor);
            

            if (pause)
                drawPause();

            if (gameOver)
                drawGameOver();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void adjustBars()
        {
            barY = screenHeight - barThickness - borderSpacing;
            barLength = (screenWidth - 2 * borderSpacing) / 2;
            timeBarX = screenWidth - barLength - borderSpacing;
        }

        public void drawScore()
        {
            string scoreString = "Score: " + score;
            spriteBatch.DrawString(Font1, scoreString, scorePos, Color.Black);
        }

        public void drawBar(int xPos, int yPos, int length, int thickness, Color color)
        {
            spriteBatch.Draw(RuleBar, new Rectangle(xPos, yPos, length, thickness), new Rectangle(0, 45, RuleBar.Width, RuleBar.Height), color);
        }

        public void drawBorder(int xPos, int yPos, int length, int thickness, Color color)
        {
            spriteBatch.Draw(RuleBar, new Rectangle(xPos, yPos, length, thickness), new Rectangle(0, 0, RuleBar.Width, RuleBar.Height/2), color);
        }

        public void drawRuleBar()
        {
            drawBar(borderSpacing, barY, (int)(barLength * ((double)meterRed / maxMeter)), barThickness, Color.Red);

            int redMeterEndX = borderSpacing + (int)(barLength * ((double)meterRed / maxMeter));

            drawBar(redMeterEndX, barY, (int)(barLength * ((double)meterGreen / maxMeter)), barThickness, Color.Green);

            int greenMeterEndX = redMeterEndX + (int)(barLength* ((double)meterGreen / maxMeter));

            drawBar(greenMeterEndX, barY, (int)(barLength * ((double)meterBlue / maxMeter)), barThickness, Color.Blue);

            drawBorder(borderSpacing, barY, barLength, barThickness, Color.Black);
        }

        public void drawTimeBar()
        {
            int gre = 0;
            int red = 0;

            
            if (time > initialTime/2)
            {
                gre = 255;
                red = 510 - (int)(time * 255 / (initialTime/2));
            }
            else if (time < initialTime/2)
            {
                gre = (int)(time * 255 / (initialTime / 2));
                red = 255;
            }

            Color dyn = new Color(red, gre, 0, 255);

            drawBar(timeBarX, barY, (int)(barLength * (time/initialTime)), barThickness, dyn);
            drawBorder(timeBarX, barY, barLength, barThickness, Color.Black);
        }

        public void drawMovementBorder()
        {
            drawBar(0, 0, screenWidth, borderSpacing, Color.Red);
            drawBar(0, screenHeight-borderSpacing-barThickness-5, screenWidth, borderSpacing, Color.Red);

            drawBar(0, -50, borderSpacing, screenHeight - borderSpacing - barThickness - 5+50, Color.Red);
            drawBar(screenWidth-borderSpacing, -50, borderSpacing, screenHeight - borderSpacing - barThickness - 5+50, Color.Red);

        }

        public void drawPause()
        {
            Vector2 pos = new Vector2(screenWidth/2-65, screenHeight/2-65);
            spriteBatch.DrawString(Font1, "GAME PAUSED", pos, Color.Black);
        }
        public void drawGameOver()
        {
            Vector2 pos = new Vector2(screenWidth / 2 - 65, screenHeight / 2 - 65);
            spriteBatch.DrawString(Font1, "GAME OVER", pos, Color.Black);
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
            int xSize = player1.getSpriteWidth();
            int y = player1.getY();
            int bottom = screenHeight - borderSpacing - barThickness - player1.getSpriteHeight();

            if (x >= (screenWidth-xSize))
            {
                if (rulesRed[3])
                    endGame();
                player1.setX(screenWidth - xSize - 1);
                
            }
            if (x <= 0)
            {
                if (rulesRed[3])
                    endGame();
                player1.setX(1);
                
            }
            if (y >= bottom)
            {
                if (rulesRed[3])
                    endGame();
                player1.setY(bottom-1);
                
            }
            if (y <= 0)
            {
                if (rulesRed[3])
                    endGame();
                player1.setY(1);
                
            }
        }

        //sets all change meters to 0. Used in reset and new change.
        private void emptyMeters()
        {
            meterRed = 0;
            meterGreen = 0;
            meterBlue = 0;
            meter = 0;
        }

        //Puts the game at a blank slate for a new game
        private void Reset()
        {
            for (int i = mobList.Count - 1; i >= 0; i--)
            {
            mobList.RemoveAt(i);
            }
            player1.setX(screenWidth/2-25);
            player1.setY(screenHeight/2-25);
            score = 0;
            delayTimer = -2;
            emptyMeters();
            time = initialTime;

            player1.stop();

            for (int i = 0; i < rulesRed.GetLength(0); i++)
            {
                rulesRed[i] = false;
            }
            for (int i = 0; i < rulesGreen.GetLength(0); i++)
            {
                rulesGreen[i] = false;
            }
            currSong = themeForHarold;
        }

        //returns 0 for red, 1 for  green, 2 for blue
        private int chooseRuleColor()
        {
            int r = rnd.Next(1, maxMeter+1);
            if (r <= meterRed)
                return 0;
            if (r <= (meterRed+meterGreen))
                return 1;
            return 2;
        }

        private int pickRule(int color)
        {
            switch(color)
            {
                case 0:
                    return rnd.Next(0, rulesRed.GetLength(0));
                case 1:
                    return rnd.Next(0, rulesGreen.GetLength(0));
                case 2:
                    int r = rnd.Next(0, rulesBlueText.GetLength(0));
                    switch (r)
                    {
                        case 0:
                            if (currSong != themeForHarold)
                                return r;
                            else
                                return r + 1;
                        case 1:
                            if (currSong != georgeStreetShuffle)
                                return r;
                            else
                                return r + 1;
                        case 2:
                            if (currSong != funInABottle)
                                return r;
                            else
                                return 0;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        private void changeRule(int color, int i)
        {
            switch(color)
            {
                case 0:
                    changeRuleRed(i);
                    break;
                case 1:
                    changeRuleGreen(i);
                    break;
                default:
                    changeRuleBlue(i);
                    break;
            }
        }

        private void changeRuleRed(int i)
        {
            rulesRed[i] = !rulesRed[i];            
        }
        private void changeRuleGreen(int i)
        {
            rulesGreen[i] = !rulesGreen[i];
        }

        private void changeRuleBlue(int i)
        {
            MediaPlayer.Pause();
            switch (i)
            {
                case 0:
                    changeSong(themeForHarold);
                    break;
                case 1:
                    changeSong(georgeStreetShuffle);
                    break;
                case 2:
                    changeSong(funInABottle);
                    break;
                default:
                    break;
            }
            MediaPlayer.Play(currSong);
        }

        private void setWarningText(int color, int rule)
        {
            string onOrOff = "on";
            switch (color)
            {
                case 0:
                    warningColor = Color.Red;
                    if (rulesRed[rule] == true)
                        onOrOff = "off";
                    warningText = rulesRedText[rule] + " " + onOrOff;
                    break;
                case 1:
                    warningColor = Color.Green;
                    if (rulesGreen[rule] == false)
                        onOrOff = "off";
                    warningText = rulesGreenText[rule] +" "+ onOrOff;
                    break;
                case 2:
                    warningColor = Color.Blue;
                    warningText = rulesBlueText[rule];
                    break;
                default:
                    break;
            }
        }

        private void changeSong(Song s)
        {
            MediaPlayer.Pause();
            currSong = s;
            MediaPlayer.Play(currSong);
        }

        private void processMovementInput(Controls controls)
        {
            // the player's x and y position
            int pX = player1.getX();
            int xS = player1.getSpriteWidth();
            int pY = player1.getY();
            int yS = player1.getSpriteHeight();
            // hte player's x and Y speed
            int pXS = player1.getSpeed();
            int pYS = player1.getSpeed();

            if (rulesRed[1] == true)
            {
                pXS = -pXS;
            }
            if (rulesRed[2] == true)
            {
                pYS = -pYS;
            }
            if (pX < (screenWidth-xS) || pX > 0 || pX > 0 || pX < (screenHeight-yS))
            {

                // Sideways Acceleration
                bool right = controls.isHeld(Keys.Right, Buttons.DPadRight);
                bool left = controls.isHeld(Keys.Left, Buttons.DPadLeft);
                bool up = controls.isHeld(Keys.Up, Buttons.DPadUp);
                bool down = controls.isHeld(Keys.Down, Buttons.DPadDown);

                if (rulesRed[0] == true)
                {
                    right = controls.isHeld(Keys.D, Buttons.DPadRight);
                    left = controls.isHeld(Keys.A, Buttons.DPadLeft);
                    up = controls.isHeld(Keys.W, Buttons.DPadUp);
                    down = controls.isHeld(Keys.S, Buttons.DPadDown);
                }

                if (right)
                    player1.setXAccel(pXS);
                else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                    player1.setXAccel(0);
                if (left)
                    player1.setXAccel(-pXS);
                else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                    player1.setXAccel(0);
                if (right && left)
                    player1.setXAccel(0);
                if (!(right || left))
                    player1.setXAccel(0);

                // Y axis Accelration
                if (up)
                    player1.setYAccel(-pYS);
                else if (controls.onRelease(Keys.Up, Buttons.DPadUp))
                    player1.setYAccel(0);
                if (down)
                    player1.setYAccel(pYS);
                else if (controls.onRelease(Keys.Down, Buttons.DPadDown))
                    player1.setYAccel(0);
                if (up && down)
                    player1.setYAccel(0);
                if (!(up || down))
                    player1.setYAccel(0);
            }
        }

        private void processUtilInput(Controls controls)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (controls.onPress(Keys.Enter, Buttons.LeftShoulder))
            {
                if (pause == true)
                    MediaPlayer.Resume();
                else
                    MediaPlayer.Pause();
                pause = !pause;
                if (gameOver)
                {
                    Reset();
                    gameOver = false;
                    pause = false;
                    MediaPlayer.Play(currSong);
                }
            }
        }

        }

    }




