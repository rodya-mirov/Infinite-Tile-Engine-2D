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
        private MapSaved cellSaved;

        public TileMap()
        {
            cellCache = new MapCache(this);
            cellSaved = new MapSaved(this);
        }

        #region Constructed Cell Adding
        public void AddConstructedCell(MapCell cell)
        {
            cellSaved.SaveExternalCell(cell);
        }

        public void AddConstructedCell(MapCell cell, int newX, int newY)
        {
            cellSaved.SaveExternalCell(cell, newX, newY);
        }

        public void AddConstructedBlock(MapCell[,] cells)
        {
            cellSaved.SaveExternalBlock(cells);
        }

        public void AddConstructedBlock(MapCell[,] cells, int newX, int newY)
        {
            cellSaved.SaveExternalBlock(cells, newX, newY);
        }
        #endregion

        /// <summary>
        /// Gets the MapCell located at the (true) coordinates X and Y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCell GetMapCell(int x, int y)
        {
            MapCell cell = cellSaved.GetCell(x, y);
            if (cell != null)
                return cell;

            cellCache.SmoothGuarantee(x, y);
            return cellCache.Get(x, y);
        }

        /// <summary>
        /// Constructs the MapCell at the given coordinates;
        /// does not consult the cache.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual MapCell MakeMapCell(int x, int y)
        {
            return new MapCell((int)TileType.GRASS, x, y);
        }
    }
}
