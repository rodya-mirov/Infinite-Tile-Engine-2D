using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class TileMap
    {
        public const int randomSeed = 121213;
        private Random random;

        public TileMap()
        {
            this.random = new Random(randomSeed);
        }

        public MapCell GetMapCell(int x, int y)
        {
            MapCell output = new MapCell(0, 0);

            return output;
        }
    }
}
