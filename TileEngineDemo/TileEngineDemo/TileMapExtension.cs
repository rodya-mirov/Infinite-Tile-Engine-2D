using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;

namespace TileEngineDemo
{
    public class TileMapExtension : TileMap<MapCellExtension>
    {
        protected override bool UseCaching { get { return true; } }

        public TileMapExtension()
            : base(false)
        {
            grassTiles = new MapCellExtension[16];
            for (int x = 0; x < 16; x++)
            {
                grassTiles[x] = new MapCellExtension(0, (x & 1) == 1, (x & 2) == 2, (x & 4) == 4, (x & 8) == 8);
            }
        }

        private MapCellExtension[] grassTiles;

        /// <summary>
        /// The "procedural generation" of this map is
        /// just "uniform grass every damn where."
        /// 
        /// The tricky bits are making conditional borders happen.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MapCellExtension MakeMapCell(int x, int y)
        {
            int index = 0;

            int xMod = (x % 4);
            if (xMod < 0) xMod += 4;

            int yMod = (y % 4);
            if (yMod < 0) yMod += 4;

            if (xMod == 0) index += 1;
            if (xMod == 3) index += 2;
            if (yMod == 0) index += 4;
            if (yMod == 3) index += 8;

            return grassTiles[index];
        }
    }
}
