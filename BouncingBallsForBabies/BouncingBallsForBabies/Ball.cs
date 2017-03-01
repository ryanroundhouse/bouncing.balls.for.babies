using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BouncingBallsForBabies
{
    public class Ball
    {
        public Vector2 Position { get; set; }
        public Vector2 Vector { get; set; }
        public int Speed { get; set; }
        public Texture2D Texture { get; set; }
        public int Score { get; set; }
        public Color Colour { get; set; }

        public Ball(Vector2 position, Vector2 vector, int speed, Texture2D texture, Color colour)
        {
            Position = position;
            Vector = vector;
            Vector.Normalize();
            Speed = speed;
            Texture = texture;
            Colour = colour;
            Score = 0;
        }

        public Vector2 MidPoint
        {
            get
            {
                return new Vector2(Position.X + Texture.Width/2, Position.Y + Texture.Height/2 );
            }
        }

        public bool UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics, float maxX, float maxY, List<Ball> otherBalls)
        {
            var newPosition = Position + (Vector * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            var borderBounce = CheckForBorderBounce(newPosition, graphics, maxX, maxY);
            var ballBounce = CheckForBallBounce(otherBalls);

            return borderBounce || ballBounce;
        }

        private bool CheckForBallBounce(IEnumerable<Ball> otherBalls)
        {
            var collision = false;
            foreach (var ball in otherBalls)
            {
                if (Vector2.Distance(MidPoint, ball.MidPoint) < ball.Texture.Width)
                {
                    var vectorBetweenBalls = MidPoint - ball.MidPoint;
                    vectorBetweenBalls.Normalize();
                    var firstComponent = Vector2.Dot(Vector, vectorBetweenBalls);
                    var secondComponent = Vector2.Dot(ball.Vector, vectorBetweenBalls);
                    var newFirstComponent = Vector += (firstComponent - secondComponent)*vectorBetweenBalls;
                    var newSecondComponent = ball.Vector += (secondComponent - firstComponent)*vectorBetweenBalls;
                    newFirstComponent.Normalize();
                    newSecondComponent.Normalize();
                    Vector = newSecondComponent;
                    ball.Vector = newFirstComponent;

                    var newSpeed = Speed > ball.Speed ? Speed : ball.Speed;
                    Speed = newSpeed;
                    ball.Speed = newSpeed;
                    collision = true;
                }
            }
            return collision;
        }

        private bool CheckForBorderBounce(Vector2 newPosition, GraphicsDeviceManager graphics, float maxX, float maxY)
        {
            maxX = graphics.GraphicsDevice.Viewport.Width;
            maxY = graphics.GraphicsDevice.Viewport.Height;

            var colision = false;
            if (newPosition.X + Texture.Width > maxX)
            {
                Vector = Vector2.Reflect(Vector, new Vector2(1, 0));
                Position = new Vector2(maxX - Texture.Width, newPosition.Y);
                colision = true;
            }
            if (newPosition.X < 0)
            {
                Vector = Vector2.Reflect(Vector, new Vector2(1, 0));
                Position = new Vector2(0, newPosition.Y);
                colision = true;
            }

            if (newPosition.Y + Texture.Height > maxY)
            {
                Vector = Vector2.Reflect(Vector, new Vector2(0, 1));
                Position = new Vector2(newPosition.X, maxY - Texture.Height);
                colision = true;
            }
            if (newPosition.Y < 0)
            {
                Vector = Vector2.Reflect(Vector, new Vector2(0, 1));
                Position = new Vector2(newPosition.X, 0);
                colision = true;
            }

            if (!colision)
            {
                Position = newPosition;
            }

            if (Speed > 0)
            {
                Speed -= 1;
            }
            if (Speed < 0)
            {
                Speed = 0;
            }
            return colision;
        }

        public void UpdateInput(Point targetPoint)
        {
            var ballRect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width,
                        Texture.Height);
            var ballMid = new Vector2(ballRect.X + Texture.Width / 2, ballRect.Y + Texture.Height / 2);
            if (!ballRect.Contains(targetPoint)) return;
            Vector2 newVector;
            if (targetPoint.X > ballMid.X)
            {
                if (targetPoint.Y > ballMid.Y)
                {
                    newVector = new Vector2(ballMid.X - targetPoint.X, ballMid.Y - targetPoint.Y);
                }
                else
                {
                    newVector = new Vector2(ballMid.X - targetPoint.X, ballMid.Y - targetPoint.Y);
                }
            }
            else
            {
                if (targetPoint.Y > ballMid.Y)
                {
                    newVector = new Vector2(ballMid.X - targetPoint.X, ballMid.Y - targetPoint.Y);
                }
                else
                {
                    newVector = new Vector2(ballMid.X - targetPoint.X, ballMid.Y - targetPoint.Y);
                }
            }
            newVector.Normalize();
            Vector = newVector;
            Speed = 500;
        }
    }
}
