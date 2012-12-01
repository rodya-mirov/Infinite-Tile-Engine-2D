using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;

namespace TileEngineDemo
{
    public class TileMapExtension : TileMap<MapCell>
    {
        /// <summary>
        /// The "procedural generation" of this map is
        /// just "uniform grass every damn where."
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MapCell MakeMapCell(int x, int y)
        {
            return new MapCell(0, x, y);
        }
    }
}
