using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace InfiniTag
{
    class Player : Sprite
    {

        public int speed;
        public int x_accel;
        public int y_accel;
        public double friction;
        public double x_vel;
        public double y_vel;
        public int movedX;
        public int movedY;
        public bool pushing;

        /*************************/
        public Texture2D Sheet { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int currentFrame;
        public int totalFrames;
        int elapsedTime;
        /***********************/

        public Player(int x, int y, int width, int height, Texture2D sheet, int rows, int columns)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            pushing = false;

            // Movement
            speed = 8;
            friction = .17;
            x_accel = 0;
            y_accel = 0;
            x_vel = 0;
            y_vel = 0;
            movedX = 0;
            movedY = 0;

            /****************/
            Sheet = sheet;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
            elapsedTime = 0;
            /**************/

        }

        public Player()
        {
            // TODO: Complete member initialization
        }

        public int getSpeed()
        {
            return speed;
        }

        public void setXAccel(int x)
        {
            x_accel = x;
        }
        public void setYAccel(int y)
        {
            y_accel = y;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("prep2.png");

        }

        public void Draw(SpriteBatch sb)
        {
            /***************************/
            int width = Sheet.Width / Columns;
            int height = Sheet.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle(spriteX, spriteY, width, height);


            sb.Draw(Sheet, destinationRectangle, sourceRectangle, Color.White);
            /*********************************/



            //sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            /****************/
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= 150)
            {
                currentFrame++;
                elapsedTime = 0;
                if (currentFrame == totalFrames)
                    currentFrame = 0;
            }
            /*************/
            Move(controls);
        }



        public void Move(Controls controls)
        {
            double playerFriction = pushing ? (friction * 3) : friction;
            x_vel = x_vel * (1 - playerFriction) + x_accel * .10;
            y_vel = y_vel * (1 - playerFriction) + y_accel * .10;

            movedX = Convert.ToInt32(x_vel);
            movedY = Convert.ToInt32(y_vel);
            spriteX += movedX;
            spriteY += movedY;

        }

        //this method zeroes out all accelerations and velocities
        public void stop()
        {
            x_accel = 0;
            y_accel = 0;
            x_vel = 0;
            y_vel = 0;
        }


        public void meterIsFull()
        {
            throw new System.NotImplementedException();
        }

        public void getScore()
        {
            throw new System.NotImplementedException();
        }

        public void getTime()
        {
            throw new System.NotImplementedException();
        }

    }
}