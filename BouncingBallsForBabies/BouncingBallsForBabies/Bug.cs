using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BouncingBallsForBabies
{
    public class Bug
    {
        public Vector2 Position { get; set; }
        public int Speed { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D SquishedTexture { get; set; }
        public Color Colour { get; set; }
        public bool Squished { get; set; }
        public TimeSpan ExpireTime { get; set; }

        public Vector2 MidPoint
        {
            get
            {
                return new Vector2(Position.X + Texture.Width / 2, Position.Y + Texture.Height / 2);
            }
        }

        public Bug(Vector2 position, Texture2D texture, Texture2D squishedTexture, Color colour)
        {
            Position = position;
            Texture = texture;
            SquishedTexture = squishedTexture;
            Colour = colour;
            Squished = false;
            ExpireTime = TimeSpan.MinValue;
        }

        public Ball CheckForSquish(IEnumerable<Ball> aBalls)
        {
            return aBalls.FirstOrDefault(ball => Vector2.Distance(MidPoint, ball.MidPoint) < ball.Texture.Width / 1.5);
        }
    }
}