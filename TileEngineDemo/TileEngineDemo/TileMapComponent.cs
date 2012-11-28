using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TileEngine;

namespace TileEngineDemo
{
    public class TileMapComponent<InGameObjectType> : DrawableGameComponent where InGameObjectType : InGameObject
    {
        private TileMapManager<InGameObjectType> mapManager { get; set; }

        public TileMapComponent(Game1 game, TileMapManager<InGameObjectType> map)
            : base(game)
        {
            this.mapManager = map;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            mapManager.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mapManager.Update(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();

            mapManager.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            mapManager.Draw(gameTime);
        }
    }
}
