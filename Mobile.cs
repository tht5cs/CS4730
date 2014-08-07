using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace InfiniTag
{
    class Mobile : Sprite
    {
        protected Texture2D image2;
        private int speed;
        public double x_vel;
        public double y_vel;
        public int movedX;
        public int movedY;
        int identity; //controls color/obstacle identity
        Color spriteColor = Color.White;

        public Mobile(int x, int y, int width, int height, int inSpeed, int id)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.identity = id;

			// Movement
			speed = inSpeed;
			x_vel = 0;
			y_vel = 0;
			movedX = 0;
            movedY = 0;
        }

        public int getId()
        {
            return identity;
        }

        public void LoadContent(ContentManager content)
        {
            string img = "";
            if (identity == 1)
            {
                img = "prep2.png";
                spriteColor = Color.Red;
            }
            else if (identity == 2)
            {
                img = "prep2.png";
                spriteColor = Color.Lime;
            }
            else if (identity == 3)
            {
                img = "prep2.png";
                spriteColor = Color.Blue;
            }
            else
            {
                img = "box_small.png";
            }

            image = content.Load<Texture2D>(img);
        }
        public void setImages(Texture2D img1, Texture2D img2)
        {
            if (identity == 1)
            {
                spriteColor = Color.Red;
            }
            else if (identity == 2)
            {
                spriteColor = Color.Lime;
            }
            else if (identity == 3)
            {
                spriteColor = Color.Blue;
            }
            image = img1;
            image2 = img2;
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            spriteY += speed;
            //Move(controls);
            /*
            Jump (controls, gameTime);
            */
        }


        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), spriteColor);
        }
        public void Draw2(SpriteBatch sb)
        {
            sb.Draw(image2, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), spriteColor);
        }
    }
}
