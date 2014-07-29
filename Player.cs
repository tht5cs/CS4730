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

		private int speed;
		private int x_accel;
        private int y_accel;
		private double friction;
		public double x_vel;
		public double y_vel;
		public int movedX;
        public int movedY;
		private bool pushing;

        private bool invert = false;
        
        public Player(int x, int y, int width, int height)
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
        }

        public int getX(){
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("prep2.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

		public void Update(Controls controls, GameTime gameTime)
		{
			Move (controls);
			/*
            Jump (controls, gameTime);
            */
		}

        

		public void Move(Controls controls)
		{
            if (controls.onPress(Keys.M, Buttons.LeftShoulder))
            {
                invert = !invert;
                speed = speed * -1;
                
            }

            if (spriteX < 430 || spriteX > 0 || spriteY > 0 || spriteY < 590)
            {
                /*

                // Sideways Acceleration
                if (controls.onPress(Keys.Right, Buttons.DPadRight))
                    x_accel += speed;
                else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                    x_accel = 0;
                if (controls.onPress(Keys.Left, Buttons.DPadLeft))
                    x_accel -= speed;
                else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                    x_accel = 0;

                // Y axis Accelration
                if (controls.onPress(Keys.Up, Buttons.DPadUp))
                    y_accel -= speed;
                else if (controls.onRelease(Keys.Up, Buttons.DPadUp))
                    y_accel = 0;
                if (controls.onPress(Keys.Down, Buttons.DPadDown))
                    y_accel += speed;
                else if (controls.onRelease(Keys.Down, Buttons.DPadDown))
                    y_accel = 0;
                  
                 */

                // Sideways Acceleration
                bool right = controls.isHeld(Keys.Right, Buttons.DPadRight);
                bool left = controls.isHeld(Keys.Left, Buttons.DPadLeft);
                bool up = controls.isHeld(Keys.Up, Buttons.DPadUp);
                bool down = controls.isHeld(Keys.Down, Buttons.DPadDown);

                if (right)
                    x_accel = speed;
                else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                    x_accel = 0;
                if (left)
                    x_accel = -speed;
                else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                    x_accel = 0;
                if (right && left)
                    x_accel = 0;

                // Y axis Accelration
                if (up)
                    y_accel = -speed;
                else if (controls.onRelease(Keys.Up, Buttons.DPadUp))
                    y_accel = 0;
                if (down)
                    y_accel = speed;
                else if (controls.onRelease(Keys.Down, Buttons.DPadDown))
                    y_accel = 0;
                if (up && down)
                    y_accel = 0;
            }
            
			double playerFriction = pushing ? (friction * 3) : friction;
			x_vel = x_vel * (1 - playerFriction) + x_accel * .10;
            y_vel = y_vel * (1 - playerFriction) + y_accel * .10;
             
			movedX = Convert.ToInt32(x_vel);
            movedY = Convert.ToInt32(y_vel);
			spriteX += movedX;
            spriteY += movedY;

            if (spriteX >= 430)
            {
                spriteX = 429;
            }
            if (spriteX <= 0)
            {
                spriteX = 1;
            }
            if (spriteY >= 590)
            {
                spriteY = 589;
            }
            if (spriteY <= 0)
            {
                spriteY = 1;
            }

			// Gravity
            /*
			if (!grounded)
			{
				y_vel += gravity;
				if (y_vel > maxFallSpeed)
					y_vel = maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
			}
			else
			{
				y_vel = 1;
			}

			grounded = false;
            */
			// Check up/down collisions, then left/right
			checkCollisions();

		}

		private void checkCollisions()
		{
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

        /*
		private void Jump(Controls controls, GameTime gameTime)
		{
			// Jump on button press
			if (controls.onPress(Keys.Space, Buttons.A) && grounded)
			{
				y_vel = -11;
				jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
				grounded = false;
			}

			// Cut jump short on button release
			else if (controls.onRelease(Keys.Space, Buttons.A) && y_vel < 0)
			{
				y_vel /= 2;
			}
		}  */
    }
}
