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

namespace InfiniTag
{
    class tempText
    {
        private Vector2 pos;
        private string text;
        private double time;
        private SpriteFont font;
        private Color color;

        public tempText(Vector2 p, string s, double t, SpriteFont f, Color c)
        {
            this.pos = p;
            this.text = s;
            this.time = t;
            this.font = f;
            this.color = c;
        }
        public double getTime()
        {
            return time;
        }
        //asdasffadsas

        public void timeDown(double t)
        {
            time -= t;
        }

        public void Update(GameTime gameTime)
        {
            timeDown(gameTime.ElapsedGameTime.TotalSeconds);
        }


        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(font, text, pos, color);
        }
    }
}
