using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TileEngine;

namespace TileEngineDemo
{
    public class TileMapComponent : DrawableGameComponent
    {
        private TileMapManager map { get; set; }

        public TileMapComponent(Game1 game, TileMapManager map)
            : base(game)
        {
            this.map = map;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            map.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            map.Update(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();

            map.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            map.Draw(gameTime);
        }
    }
}
