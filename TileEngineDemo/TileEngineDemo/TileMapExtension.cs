using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;

namespace TileEngineDemo
{
    public class TileMapExtension : TileMap<MapCell>
    {
        protected override bool UseCaching { get { return true; } }

        public TileMapExtension()
            : base()
        {
            grassTile = new MapCell(0);
        }

        private MapCell grassTile;

        /// <summary>
        /// The "procedural generation" of this map is
        /// just "uniform grass every damn where."
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MapCell MakeMapCell(int x, int y)
        {
            if (grassTile == null)
            {
                throw new InvalidProgramException();
            }
            return grassTile;
        }
    }
}
