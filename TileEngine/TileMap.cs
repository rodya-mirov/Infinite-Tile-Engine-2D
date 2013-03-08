using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine.Utilities;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public abstract class TileMap<MapCellType>
        where MapCellType : MapCell, Copyable<MapCellType>
    {
        public readonly bool LimitedMap;
        /// <summary>
        /// Note that the bounds are XMin <= x < XMax, YMin <= y < YMax
        /// </summary>
        public readonly int XMin, XMax, YMin, YMax;

        public bool IsValidCellIndex(int x, int y)
        {
            if (!LimitedMap)
                return true;

            return XMin <= x && x < XMax && YMin <= y && y < YMax;
        }

        private MapCache<MapCellType> visualCellCache;
        private MapSaved<MapCellType> savedRealCells;
        private MapSaved<MapCellType> visualOverrideCells;

        protected virtual bool UseCaching { get { return true; } }

        protected TileMap(bool limited,
            int xmin = int.MinValue, int xmax = int.MaxValue, int ymin = int.MinValue, int ymax = int.MaxValue)
        {
            this.LimitedMap = limited;
            if (limited)
            {
                this.XMin = xmin;
                this.YMin = ymin;
                this.XMax = xmax;
                this.YMax = ymax;
            }
            else
            {
                this.XMin = int.MinValue;
                this.YMin = int.MinValue;
                this.XMax = int.MaxValue;
                this.YMax = int.MaxValue;
            }

            savedRealCells = new MapSaved<MapCellType>(this);
            visualOverrideCells = new MapSaved<MapCellType>(this);
        }

        public void SetUpCache()
        {
            if (UseCaching)
                visualCellCache = new MapCache<MapCellType>(this);
        }

        #region Constructed Cell Adding
        /// <summary>
        /// Saves a copy of the attached cell at the specified coordinates
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void AddConstructedCell(MapCellType cell, int newX, int newY)
        {
            savedRealCells.SaveExternalCell(cell, newX, newY);
        }

        public void AddConstructedBlock(MapCellType[,] cells, int leftX, int topY)
        {
            savedRealCells.SaveExternalBlock(cells, leftX, topY);
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

            cell = visualOverrideCells.GetCell(x, y);
            if (cell != null)
                return cell;

            cell = savedRealCells.GetCell(x, y);
            if (cell != null)
                return cell;

            if (UseCaching)
            {
                visualCellCache.Guarantee(x, y);
                return visualCellCache.Get(x, y);
            }
            else
            {
                return this.MakeMapCell(x, y);
            }
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

            cell = savedRealCells.GetCell(x, y);
            if (cell != null)
                return cell;

            return MakeMapCell(x, y);
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
            visualOverrideCells.Clear();
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
            visualOverrideCells.SaveExternalCell(cell, newX, newY);
        }

        /// <summary>
        /// Clears the override cell at the specified positon, if any.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearVisualOverrideAtPosition(int x, int y)
        {
            visualOverrideCells.ClearCellAtPosition(x, y);
        }
    }
}
