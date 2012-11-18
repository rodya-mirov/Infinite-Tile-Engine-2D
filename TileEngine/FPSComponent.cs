using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class FPSComponent : DrawableGameComponent
    {
        private int numFramesSinceReset;
        string fps;

        private TimeSpan timeSpan;
        private TimeSpan interval;

        public SpriteFont Font { get; set; }
        private SpriteBatch batch;

        private Vector2 position1, position2;

        private Color defaultColor = Color.White;

        public bool Visible { get; set; }

        /// <summary>
        /// Construct a new FPSComponent, which will automagically
        /// compute and draw its FPS without need for outside interference.
        /// 
        /// Before writing will occur, you must set the font.
        /// Color defaults to White, but can be changed.
        /// </summary>
        /// <param name="game"></param>
        public FPSComponent(Game game)
            : base(game)
        {
            timeSpan = TimeSpan.Zero;
            interval = TimeSpan.FromSeconds(0.5);

            this.Font = Font;
            batch = new SpriteBatch(game.GraphicsDevice);

            numFramesSinceReset = 0;
            fps = "FPS: 0";

            position1 = new Vector2(30, 30);
            position2 = new Vector2(30, 31);

            Visible = true;
        }

        public override void Draw(GameTime gameTime)
        {
            numFramesSinceReset++;
            timeSpan += gameTime.ElapsedGameTime;

            if (timeSpan > interval)
            {
                fps = "FPS: " + ((2 * numFramesSinceReset).ToString());
                timeSpan -= interval;
                numFramesSinceReset = 0;
            }

            if (Visible)
            {
                batch.Begin();
                batch.DrawString(Font, fps, position2, Color.Black);
                batch.DrawString(Font, fps, position1, Color.White);
                batch.End();
            }

            base.Draw(gameTime);
        }
    }
}
