using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine.Utilities;

namespace TileEngine
{
    public abstract class TileMap<MapCellType>
        where MapCellType : MapCell, Translatable<MapCellType>
    {
        public const int randomSeed = 121213;
        private MapCache<MapCellType> cellCache;
        private MapSaved<MapCellType> cellSaved;
        private MapSaved<MapCellType> cellVisualOverrides;

        public TileMap()
        {
            cellCache = new MapCache<MapCellType>(this);
            cellSaved = new MapSaved<MapCellType>(this);
            cellVisualOverrides = new MapSaved<MapCellType>(this);
        }

        #region Constructed Cell Adding
        public void AddConstructedCell(MapCellType cell)
        {
            cellSaved.SaveExternalCell(cell);
        }

        public void AddConstructedCell(MapCellType cell, int newX, int newY)
        {
            cellSaved.SaveExternalCell(cell, newX, newY);
        }

        public void AddConstructedBlock(MapCellType[,] cells)
        {
            cellSaved.SaveExternalBlock(cells);
        }

        public void AddConstructedBlock(MapCellType[,] cells, int newX, int newY)
        {
            cellSaved.SaveExternalBlock(cells, newX, newY);
        }
        #endregion

        /// <summary>
        /// Gets the MapCell located at the (true) coordinates X and Y.
        /// This may return Visual Override cells, so if this is not desired,
        /// please use GetRealMapCell instead.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCellType GetVisualMapCell(int x, int y)
        {
            MapCellType cell;

            cell = cellVisualOverrides.GetCell(x, y);
            if (cell != null)
                return cell;

            cell = cellSaved.GetCell(x, y);
            if (cell != null)
                return cell;

            cellCache.Guarantee(x, y);
            return cellCache.Get(x, y);
        }

        /// <summary>
        /// This gets the (underlying) MapCell at the true coordinates
        /// X and Y.  The difference between this and GetMapCell is that
        /// this ignores the VisualOverride cells.  Used for passability,
        /// etc. purposes, rather than drawing purposes.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCellType GetRealMapCell(int x, int y)
        {
            MapCellType cell;

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
        public abstract MapCellType MakeMapCell(int x, int y);

        /// <summary>
        /// This cancels all existing visual overrides
        /// </summary>
        public void ClearVisualOverrides()
        {
            cellVisualOverrides.Clear();
        }

        /// <summary>
        /// Places a copy of the given cell at the intended coordinates,
        /// into the set of overrides.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void SetVisualOverride(MapCellType cell, int newX, int newY)
        {
            cellVisualOverrides.SaveExternalCell(cell, newX, newY);
        }

        /// <summary>
        /// Clears the override cell at the specified positon, if any.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearVisualOverrideAtPosition(int x, int y)
        {
            cellVisualOverrides.ClearCellAtPosition(x, y);
        }
    }
}
