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
        private MapSaved cellTemporaryOverrides;

        public TileMap()
        {
            cellCache = new MapCache(this);
            cellSaved = new MapSaved(this);
            cellTemporaryOverrides = new MapSaved(this);
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
            MapCell cell;

            cell = cellTemporaryOverrides.GetCell(x, y);
            if (cell != null)
                return cell;

            cell = cellSaved.GetCell(x, y);
            if (cell != null)
                return cell;

            cellCache.Guarantee(x, y);
            return cellCache.Get(x, y);
        }

        /// <summary>
        /// Constructs the MapCell at the given coordinates;
        /// does not consult the cache.  The default behavior
        /// just makes the upper-left tile at every space.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual MapCell MakeMapCell(int x, int y)
        {
            return new MapCell(0, x, y);
        }

        /// <summary>
        /// This cancels all existing overrides
        /// </summary>
        public void ClearOverrides()
        {
            cellTemporaryOverrides.Clear();
        }

        /// <summary>
        /// Places a copy of the given cell at the intended coordinates,
        /// into the set of overrides.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void SetOverride(MapCell cell, int newX, int newY)
        {
            cellTemporaryOverrides.SaveExternalCell(cell, newX, newY);
        }

        /// <summary>
        /// Clears the override cell at the specified positon, if any.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearOverrideAtPosition(int x, int y)
        {
            cellTemporaryOverrides.ClearCellAtPosition(x, y);
        }
    }
}
