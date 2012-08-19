using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class MapCache
    {
        /// <summary>
        /// These should be set large enough so that the map can comfortably be drawn
        /// on a single cache load (or else the cache is actually hurting you!) but
        /// small enough that you don't have much extra (or it's just extra work to
        /// keep it current, and no gain)
        /// </summary>
        public const int cacheWidth = 64;
        public const int cacheHeight = 64;

        private MapCell[,] cache;
        private TileMap map;

        /// <summary>
        /// The bounds for what is contained.  Note that, unlike array
        /// indices, the bottom-right corner, (xMax, yMax), is
        /// actually a valid index for the array, in the interest
        /// of readability.
        /// </summary>
        private int xMin, yMin;
        
        private int xStartIndex, yStartIndex;

        /// <summary>
        /// Constructs a new cache for the given map and fully loads it,
        /// starting at the specified upper-left corner.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public MapCache(TileMap map, int startX = 0, int startY = 0)
        {
            this.map = map;

            cache = new MapCell[cacheWidth, cacheHeight];

            this.xStartIndex = 0;
            this.yStartIndex = 0;

            this.xMin = startX;
            this.yMin = startY;

            for (int x = 0; x < cacheWidth; x++)
            {
                for (int y = 0; y < cacheHeight; y++)
                {
                    cache[x, y] = map.MakeMapCell(x + xMin, y + yMin);
                }
            }
        }

        public bool Contains(int x, int y)
        {
            return (xMin <= x && x < xMin + cacheWidth && yMin <= y && y < yMin + cacheHeight);
        }

        /// <summary>
        /// Returns the cached value for the MapCell at (x, y).
        /// Assumes this value is actually contained in the cache
        /// and may have undesired behavior if not (anything from
        /// array bounds exceptions to just returning wrong data).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCell Get(int x, int y)
        {
            return cache[
                Numerical.Mod(x - xMin + xStartIndex, cacheWidth),
                Numerical.Mod(y - yMin + yStartIndex, cacheHeight)
                ];
        }

        /// <summary>
        /// Forces the cache to contain the MapCell for (x, y), and
        /// does a minimal amount of changes so that it does.
        /// 
        /// Specifically, adds rows/columns in the appropriate direction
        /// so that the cache gradually moves to the specified coordinate.
        /// Does nothing if (x,y) is already in the cache.
        /// 
        /// Teleportation is not friendly with this model!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Guarantee(int x, int y)
        {
            while (y < yMin)
                addTopRow();

            while (y >= yMin + cacheHeight)
                addBottomRow();

            while (x < xMin)
                addLeftColumn();

            while (x >= xMin + cacheWidth)
                addRightColumn();
        }

        private void addTopRow()
        {
            //allows for a new top row
            yMin--;

            //fixes the indexing so that the old data is still indexed correctly
            yStartIndex = Numerical.Mod(yStartIndex - 1, cacheHeight);

            //now fix the new top row
            for (int x = 0; x < cacheWidth; x++)
            {
                cache[Numerical.Mod(x + xStartIndex, cacheWidth), yStartIndex] = map.MakeMapCell(x + xMin, yMin);
            }
        }

        private void addBottomRow()
        {
            //set up the new bottom row, which replaces the old top row
            for (int x = 0; x < cacheWidth; x++)
            {
                cache[Numerical.Mod(x + xStartIndex, cacheWidth), yStartIndex] = map.MakeMapCell(x + xMin, yMin + cacheHeight);
            }

            //updates the minimum
            yMin++;

            //fixes the indexing
            yStartIndex = Numerical.Mod(yStartIndex + 1, cacheHeight);
        }

        private void addLeftColumn()
        {
            //allows for the next left column
            xMin--;

            //fixes the indexing
            xStartIndex = Numerical.Mod(xStartIndex - 1, cacheWidth);

            //fix the new left column
            for (int y = 0; y < cacheHeight; y++)
            {
                cache[xStartIndex, Numerical.Mod(y + yStartIndex, cacheHeight)] = map.MakeMapCell(xMin, y + yMin);
            }
        }

        private void addRightColumn()
        {
            //set up the new right column, which replaces the old left column
            for (int y = 0; y < cacheHeight; y++)
            {
                cache[xStartIndex, Numerical.Mod(y + yStartIndex, cacheHeight)] = map.MakeMapCell(xMin + cacheWidth, y + yMin);
            }

            //updates the minimum
            xMin++;

            //fixes the indexing
            xStartIndex = Numerical.Mod(xStartIndex + 1, cacheWidth);
        }
    }
}
