using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class TileMap
    {
        public const int randomSeed = 121213;
        private MapCache cellCache;

        public TileMap()
        {
            cellCache = new MapCache(this);
        }

        /// <summary>
        /// Gets the MapCell located at the (true) coordinates X and Y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCell GetMapCell(int x, int y)
        {
            cellCache.Guarantee(x, y);
            return cellCache.Get(x, y);
        }

        /// <summary>
        /// Constructs the MapCell at the given coordinates;
        /// does not consult the cache.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCell MakeMapCell(int x, int y)
        {
            Random random = new Random(randomSeed + (x ^ y));

            MapCell output = new MapCell(random.Next(0, 6));

            output.X = x;
            output.Y = y;

            return output;
        }
    }
}
