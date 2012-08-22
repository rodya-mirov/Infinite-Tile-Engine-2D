using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    /// <summary>
    /// This is a bit like the cache, but only saves when it's explicitly told to.
    /// Adds whole blocks at a time!
    /// 
    /// The underlying structure is a sorted list of cells with their coordinates,
    /// though.  This is actually faster than the blocks, in general, because there's
    /// no good way to sort the blocks, and no obvious way to defragment block
    /// storage.
    /// </summary>
    public class MapSaved
    {
        private SortedList<SortedPoint, MapCell> savedCells;
        private TileMap map;

        public MapSaved(TileMap map)
        {
            this.map = map;

            savedCells = new SortedList<SortedPoint, MapCell>();
        }

        /// <summary>
        /// Returns the cell at the specified coordinates, assuming
        /// the cell is saved somewhere.  Returns null if not.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCell GetCell(int x, int y)
        {
            SortedPoint p = new SortedPoint(x, y);

            if (savedCells.ContainsKey(p))
                return savedCells[p];

            return null;
        }

        /// <summary>
        /// Transcribes a block on the existing map and saves it permanently.
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveExistingBlock(int xmin, int ymin, int width, int height)
        {
            MapCell cell;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cell = map.GetMapCell(xmin + x, ymin + y);
                    savedCells.Add(new SortedPoint(cell), cell);
                }
            }
        }

        /// <summary>
        /// Saves an externally constructed cell.
        /// </summary>
        /// <param name="cell"></param>
        public void SaveExternalCell(MapCell cell)
        {
            savedCells.Add(new SortedPoint(cell), cell);
        }

        /// <summary>
        /// Saves a shifted copy of an externally constructed cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void SaveExternalCell(MapCell cell, int newX, int newY)
        {
            MapCell newCell = new MapCell(cell, newX, newY);

            savedCells.Add(new SortedPoint(newCell), newCell);
        }

        /// <summary>
        /// Writes down a constructed block of MapCells to be permanently saved.
        /// </summary>
        /// <param name="block"></param>
        public void SaveExternalBlock(MapCell[,] block)
        {
            foreach (MapCell cell in block)
                savedCells.Add(new SortedPoint(cell), cell);
        }

        /// <summary>
        /// Constructs a shifted copy of the specified block, located
        /// at the specified (upper left) corner, and saves the result.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        public void SaveExternalBlock(MapCell[,] block, int xmin, int ymin)
        {
            int width = block.GetLength(0);
            int height = block.GetLength(1);

            MapCell cell;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cell = new MapCell(block[x, y], xmin + x, ymin + y);
                    savedCells.Add(new SortedPoint(cell), cell);
                }
            }
        }
    }
}
