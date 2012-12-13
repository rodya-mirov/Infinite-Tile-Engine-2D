using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TileEngine.Utilities;

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
    public class MapSaved<MapCellType>
        where MapCellType : MapCell, Copyable<MapCellType>
    {
        private Dictionary<Point, MapCellType> savedCells;
        private TileMap<MapCellType> map;

        public MapSaved(TileMap<MapCellType> map)
        {
            this.map = map;

            savedCells = new Dictionary<Point, MapCellType>();
        }

        /// <summary>
        /// Returns the cell at the specified coordinates, assuming
        /// the cell is saved somewhere.  Returns null if not.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapCellType GetCell(int x, int y)
        {
            Point p = new Point(x, y);

            if (savedCells.ContainsKey(p))
                return savedCells[p];

            return null;
        }

        /// <summary>
        /// Saves a shifted copy of an externally constructed cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void SaveExternalCell(MapCellType cell, int newX, int newY)
        {
            Point sp = new Point(newX, newY);
            Copyable<MapCellType> cc = (Copyable<MapCellType>)cell;
            savedCells[sp] = cc.Copy();
        }

        /// <summary>
        /// Constructs a shifted copy of the specified block, located
        /// at the specified (upper left) corner, and saves the result.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        public void SaveExternalBlock(MapCellType[,] block, int xmin, int ymin)
        {
            int width = block.GetLength(0);
            int height = block.GetLength(1);

            MapCellType cell;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Copyable<MapCellType> copyable = block[x, y];
                    cell = copyable.Copy();

                    Point sp = new Point(x + xmin, y + ymin);
                    savedCells[sp] = cell;
                }
            }
        }

        /// <summary>
        /// Completely wipes out all saved cells, irrevocably!
        /// USE WITH CAUTION
        /// </summary>
        public void Clear()
        {
            savedCells.Clear();
        }

        /// <summary>
        /// Clears any saved cell at the specified position.  Does nothing
        /// if there was nothing there.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearCellAtPosition(int x, int y)
        {
            Point sp = new Point(x, y);
            if (savedCells.ContainsKey(sp))
                savedCells.Remove(sp);
        }
    }
}
