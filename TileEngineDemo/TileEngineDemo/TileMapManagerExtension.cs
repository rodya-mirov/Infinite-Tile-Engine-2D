using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;
using Microsoft.Xna.Framework;

namespace TileEngineDemo
{
    public class TileMapManagerExtension : TileMapManager
    {
        protected List<InGameObject> objects;

        public TileMapManagerExtension(Game game, String contentLocation)
            : base(game, contentLocation)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            objects = new List<InGameObject>();
            objects.Add(new NPCexample(0, 0, MyMap));
            objects.Add(new NPCexample(100, 100, MyMap));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            NPCexample.LoadContent(this.game);
        }

        protected override IEnumerable<InGameObject> InGameObjects
        {
            get
            {
                foreach (InGameObject obj in objects)
                    yield return obj;
            }
        }
    }
}
