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
        private int drawsSinceReset;
        string drawnFPS;

        private TimeSpan timeSpan;
        private TimeSpan interval;

        public SpriteFont Font { get; set; }
        private SpriteBatch batch;

        private Vector2 position1, position2;

        private Color defaultColor = Color.White;

        public void ToggleVisible()
        {
            this.Visible = !this.Visible;
        }

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

            drawsSinceReset = 0;
            drawnFPS = "FPS: 0";

            position1 = new Vector2(60, 30);
            position2 = new Vector2(60, 31);

            Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            drawsSinceReset++;
            timeSpan += gameTime.ElapsedGameTime;

            if (timeSpan > interval)
            {
                drawnFPS = "FPS: " + ((2 * drawsSinceReset).ToString());
                timeSpan -= interval;

                drawsSinceReset = 0;
            }

            batch.Begin();
            batch.DrawString(Font, drawnFPS, position2, Color.Black);
            batch.DrawString(Font, drawnFPS, position1, Color.White);
            batch.End();

            base.Draw(gameTime);
        }
    }
}
