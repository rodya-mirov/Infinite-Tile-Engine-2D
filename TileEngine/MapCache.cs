using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine.Utilies;

namespace TileEngine
{
    public class MapCache<MapCellType>
        where MapCellType : MapCell
    {
        /// <summary>
        /// These should be set large enough so that the map can comfortably be drawn
        /// on a single cache load (or else the cache is actually hurting you!) but
        /// small enough that you don't have much extra (or it's just extra work to
        /// keep it current, and no gain)
        /// </summary>
        public const int cacheWidth = 256;
        public const int cacheHeight = 256;

        private MapCellType[,] cache;
        private TileMap<MapCellType> map;

        /// <summary>
        /// The bounds for what is contained.  Note that, unlike array
        /// indices, the bottom-right corner, (xMax, yMax), is
        /// actually a valid index for the array, in the interest
        /// of readability.
        /// </summary>
        private int xMin, yMin;
        private int xMax
        {
            get { return xMin + cacheWidth - 1; }
            set { xMin = value - cacheWidth + 1; }
        }
        private int yMax
        {
            get { return yMin + cacheHeight - 1; }
            set { yMin = value - cacheHeight + 1; }
        }
        
        /// <summary>
        /// These are the magic that make this a toroidal array!
        /// xStartIndex represents the point in the index where
        /// x=xMin, and yStartIndex is where y=yMin
        /// </summary>
        private int xStartIndex, yStartIndex;

        /// <summary>
        /// Constructs a new cache for the given map and fully loads it,
        /// starting at the specified upper-left corner.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public MapCache(TileMap<MapCellType> map, int startX = 0, int startY = 0)
        {
            this.map = map;

            cache = new MapCellType[cacheWidth, cacheHeight];

            this.xMin = startX;
            this.yMin = startY;

            rebuildCache();
        }

        /// <summary>
        /// This builds the entire cache from scratch, maintaining
        /// the xMin and yMin values.
        /// </summary>
        /// <param name="map"></param>
        private void rebuildCache()
        {
            this.xStartIndex = 0;
            this.yStartIndex = 0;

            for (int x = 0; x < cacheWidth; x++)
            {
                for (int y = 0; y < cacheHeight; y++)
                {
                    cache[x, y] = map.MakeMapCell(x + xMin, y + yMin);
                }
            }
        }

        /// <summary>
        /// Whether or not the specified point is in the cache.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
        public MapCellType Get(int x, int y)
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
        private void smoothGuarantee(int x, int y)
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

        /// <summary>
        /// Determines whether it would be faster to do a
        /// SmoothGuarantee or just completely rebuild
        /// the cache, then does the better of the two.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Guarantee(int x, int y)
        {
            int smoothCost = 0;
            if (x < xMin)
                smoothCost += cacheHeight * (xMin - x);
            if (x > xMax)
                smoothCost += cacheHeight * (x - xMax);
            if (y < yMin)
                smoothCost += cacheWidth * (yMin - y);
            if (y > yMax)
                smoothCost += cacheWidth * (y - yMax);

            int rebuildCost = cacheWidth * cacheHeight;

            if (smoothCost < rebuildCost)
            {
                smoothGuarantee(x, y);
            }
            else
            {
                if (x < xMin)
                    xMin = x;
                else if (x > xMax)
                    xMax = x;

                if (y < yMin)
                    yMin = y;
                else if (y > yMax)
                    yMax = y;

                rebuildCache();
            }
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
