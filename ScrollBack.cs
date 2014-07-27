using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;

namespace InfiniTag
{
    class ScrollBack
    {
        Texture2D texture;
        int bgHeight, bgWidth, speed;
        Vector2[] positions;
        public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
        {
            bgHeight = screenHeight;
            bgWidth = screenWidth;
            texture = content.Load<Texture2D>(texturePath);
            this.speed = speed;

            // Determine how many tiles of background image is needed to cover the screen
            positions = new Vector2[screenHeight / texture.Height + 1];

            // Set the initial positions of each background image
            for (int i = 0; i < positions.Length; i++)
            {                
                positions[i] = new Vector2(0, i * texture.Height);
            }
        }

        public void Update(GameTime gametime)
        {
            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].Y -= speed;

                // moving the background up
                if (speed > 0)
                {
                    // Check the texture is out of view then put that texture at the end of the screen
                    if (positions[i].Y <= -texture.Height)
                    {
                        positions[i].Y = texture.Height * (positions.Length - 1);
                    }
                }

                // moving the background down
                else
                {                    
                    if (positions[i].Y >= texture.Height * (positions.Length - 1))
                    {
                        positions[i].Y = -texture.Height;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Rectangle rectBg = new Rectangle((int)positions[i].X, (int)positions[i].Y, bgWidth, bgHeight);
                spriteBatch.Draw(texture, rectBg, Color.White);
            }
        }
    }
}
