﻿using System;
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
        private int speed;
        public double x_vel;
        public double y_vel;
        public int movedX;
        public int movedY;
        int identity; //controls color/obstacle identity

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

        public int getX()
        {
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
            }
            else
            {
                img = "alien.png";
            }

            image = content.Load<Texture2D>(img);
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
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }
    }
}
