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
        private int drawsSinceReset, updatesSinceReset;
        string drawnFPS, logicFPS;

        private TimeSpan timeSpan;
        private TimeSpan interval;

        public SpriteFont Font { get; set; }
        private SpriteBatch batch;

        private Vector2 position1, position2, position3, position4;

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

            drawsSinceReset = 0;
            drawnFPS = "D FPS: 0";
            logicFPS = "L FPS: 0";

            position1 = new Vector2(30, 30);
            position2 = new Vector2(30, 31);
            position3 = new Vector2(30, 50);
            position4 = new Vector2(30, 51);

            Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            updatesSinceReset++;
        }

        public override void Draw(GameTime gameTime)
        {
            drawsSinceReset++;
            timeSpan += gameTime.ElapsedGameTime;

            if (timeSpan > interval)
            {
                drawnFPS = "Draws FPS: " + ((2 * drawsSinceReset).ToString());
                logicFPS = "Logic FPS: " + ((2 * updatesSinceReset).ToString());
                timeSpan -= interval;

                drawsSinceReset = 0;
                updatesSinceReset = 0;
            }

            if (Visible)
            {
                batch.Begin();
                batch.DrawString(Font, drawnFPS, position2, Color.Black);
                batch.DrawString(Font, drawnFPS, position1, Color.White);
                batch.DrawString(Font, logicFPS, position4, Color.Black);
                batch.DrawString(Font, logicFPS, position3, Color.White);
                batch.End();
            }

            base.Draw(gameTime);
        }
    }
}
