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

        /// <summary>
        /// Returns true if and only if the supplied points are adjacent (that is,
        /// one coordinate is shared and the other differs by exactly 1).
        /// Note it returns false if the supplied points are the same.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public override bool CanMoveFromSquareToSquare(int startX, int startY, int endX, int endY)
        {
            return Math.Abs(startX - endX) + Math.Abs(startY - endY) == 1;
        }

        /// <summary>
        /// Just returns a list of all adjacent points.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public override IEnumerable<Point> GetAdjacentPoints(int X, int Y)
        {
            yield return new Point(X - 1, Y);
            yield return new Point(X + 1, Y);
            yield return new Point(X, Y - 1);
            yield return new Point(X, Y + 1);
        }
    }
}
