using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BouncingBallsCommon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace BouncingBallsForBabies
{
    public class BouncingBallsForBabiesManager
    {
        private float _maxX = -1f;
        private float _maxY = -1f;
        private MouseState _lastMouseState;
        private bool InsanityMode = false;
        private int NumberOfSimultaneousBugs = 3;

        public List<Texture2D> BallTextures { get; set; }
        public Texture2D BugTexture { get; set; }
        public Texture2D BugSquishedTexture { get; set; }
        public Texture2D WoodTexture { get; set; }
        public List<Ball> Balls { get; set; }
        public SpriteFont ScoreFont { get; set; }
        public List<Bug> Bugs { get; set; }

        public List<SoundEffect> BounceSounds { get; set; }
        public List<SoundEffect> SquishSounds { get; set; }

        public BouncingBallsForBabiesManager()
        {
            _lastMouseState = Mouse.GetState();
            Balls = new List<Ball>();
            Bugs = new List<Bug>();
        }
        
        internal void LoadContent(ContentManager content)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            var values = Enumerable.Range(1, 8).OrderBy(x => rng.Next()).ToArray();

            BallTextures = new List<Texture2D>{
                content.Load<Texture2D>(string.Format(@"sprites\ball{0}", values[0])),
                content.Load<Texture2D>(string.Format(@"sprites\ball{0}", values[1])),
                content.Load<Texture2D>(string.Format(@"sprites\ball{0}", values[2]))
            };
            BugTexture = content.Load<Texture2D>(@"sprites\bug");
            BugSquishedTexture = content.Load<Texture2D>(@"sprites\bug2");
            WoodTexture = content.Load<Texture2D>(@"sprites\wood texture");
            BounceSounds = new List<SoundEffect>
            {
                content.Load<SoundEffect>(@"sounds\bounce1"),
                content.Load<SoundEffect>(@"sounds\bounce2"),
                content.Load<SoundEffect>(@"sounds\bounce3"),
                content.Load<SoundEffect>(@"sounds\bounce4"),
            };
            SquishSounds = new List<SoundEffect>
            {
                content.Load<SoundEffect>(@"sounds\squish1"),
                content.Load<SoundEffect>(@"sounds\squish2"),
                content.Load<SoundEffect>(@"sounds\squish3"),
                content.Load<SoundEffect>(@"sounds\squish4"),
            };
            ScoreFont = content.Load<SpriteFont>(@"fonts\Comic Sans");
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            if (!Balls.Any())
            {
                Balls.Add(new Ball(new Vector2(132, 132), new Vector2(0f, 0f), 0, BallTextures[0], Color.White));
                Balls.Add(new Ball(new Vector2(232, 232), new Vector2(0f, 0f), 0, BallTextures[1], Color.White));
                Balls.Add(new Ball(new Vector2(332, 332), new Vector2(0f, 0f), 0, BallTextures[2], Color.White));
            }
            Bugs.ForEach(bug =>
            {
                if (bug.ExpireTime != TimeSpan.MinValue && bug.ExpireTime <= gameTime.TotalGameTime)
                {
                    Bugs.Remove(bug);
                }
            });
            if(Bugs.Count < NumberOfSimultaneousBugs || InsanityMode)
            {
                Bugs.Add(new Bug(new Vector2(Utility.GetRandomInt(0, graphics.GraphicsDevice.Viewport.Width - BugTexture.Width), 
                    Utility.GetRandomInt(0, graphics.GraphicsDevice.Viewport.Height - BugTexture.Height)), 
                    BugTexture, BugSquishedTexture, Color.White));
            }
            UpdateInput();
            UpdatePositions(gameTime,graphics);
        }

        private void UpdateInput()
        {
            var currentMouseState = Mouse.GetState();
            var currentKeyboardState = Keyboard.GetState();
            if (_lastMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var ball in Balls)
                {
                    ball.UpdateInput(new Point(currentMouseState.X, currentMouseState.Y));
                }
            }
            while (TouchPanel.IsGestureAvailable)
            {
                var gestures = TouchPanel.ReadGesture();
                switch (gestures.GestureType)
                {
                    case GestureType.Tap:
                    case GestureType.DoubleTap:
                    {
                        foreach (var ball in Balls)
                        {
                            ball.UpdateInput(new Point((int)gestures.Position.X, (int)gestures.Position.Y));
                        }
                        break;
                    }
                }
            }

            InsanityMode = currentKeyboardState.IsKeyDown(Keys.Space);
            if (currentKeyboardState.IsKeyDown(Keys.D1) || currentKeyboardState.IsKeyDown(Keys.NumPad1))
            {
                NumberOfSimultaneousBugs = 1;
            }
            if(currentKeyboardState.IsKeyDown(Keys.D2) || currentKeyboardState.IsKeyDown(Keys.NumPad2))
            {
                NumberOfSimultaneousBugs = 2;
            }
            if(currentKeyboardState.IsKeyDown(Keys.D3) || currentKeyboardState.IsKeyDown(Keys.NumPad3))
            {
                NumberOfSimultaneousBugs = 3;
            }
            if(currentKeyboardState.IsKeyDown(Keys.D4) || currentKeyboardState.IsKeyDown(Keys.NumPad4))
            {
                NumberOfSimultaneousBugs = 4;
            }
            if(currentKeyboardState.IsKeyDown(Keys.D5) || currentKeyboardState.IsKeyDown(Keys.NumPad5))
            {
                NumberOfSimultaneousBugs = 5;
            }
            if(currentKeyboardState.IsKeyDown(Keys.D0) || currentKeyboardState.IsKeyDown(Keys.NumPad0))
            {
                NumberOfSimultaneousBugs = 0;
            }
            _lastMouseState = currentMouseState;
        }

        public void UpdatePositions(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            var collision = false;
            var squish = false;
            foreach (var ball in Balls)
            {
                var col = ball.UpdatePosition(gameTime, graphics, _maxX, _maxY, Balls.Where(bs => !bs.Equals(ball)).ToList());
                if (col)
                {
                    collision = true;
                }
            }
            foreach (var bug in Bugs)
            {
                if (!bug.Squished)
                {
                    var crushingBall = bug.CheckForSquish(Balls);
                    if(crushingBall != null)
                    {
                        crushingBall.Score++;
                        bug.Squished = true;
                        bug.ExpireTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(5);
                        squish = true;
                    }
                }
            }
            if (squish)
            {
                SquishSounds[Utility.GetRandomInt(0, SquishSounds.Count)].Play();
            }
            if (collision)
            {
                BounceSounds[Utility.GetRandomInt(0, BounceSounds.Count)].Play();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.Draw(WoodTexture, new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();

            spriteBatch.DrawString(ScoreFont, "Leaderboard", new Vector2(10, 10), Color.DarkGreen);
            var scoreboardPoint = new Point(32, 36);
            for(int n = 0; n < Balls.Count; n++)
            {
                spriteBatch.Draw(Balls[n].Texture, new Rectangle(scoreboardPoint.X, scoreboardPoint.Y + n * 32, 32, 32), Balls[n].Colour);
                spriteBatch.DrawString(ScoreFont, Balls[n].Score.ToString(), new Vector2(scoreboardPoint.X + 40, scoreboardPoint.Y + n * 32), Color.DarkGreen);
            }

            foreach (var bug in Bugs)
            {
                spriteBatch.Draw(bug.Squished ? bug.SquishedTexture : bug.Texture, bug.Position, bug.Colour);
            }

            foreach (var ball in Balls)
            {
                spriteBatch.Draw(ball.Texture, ball.Position, ball.Colour);
            }
        }
    }
}
