using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    /// <summary>
    /// This is a bit like the cache, but only saves when it's explicitly told to.
    /// Adds whole blocks at a time!
    /// </summary>
    public class MapSaved
    {
        private List<SavedBlock> savedBlocks;
        private TileMap map;

        public MapSaved(TileMap map)
        {
            this.map = map;

            savedBlocks = new List<SavedBlock>();
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
            foreach (SavedBlock block in savedBlocks)
            {
                if (block.containsCell(x, y))
                    return block.getCell(x, y);
            }

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
            MapCell[,] cells = new MapCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[xmin + x, ymin + y] = map.GetMapCell(xmin + x, ymin + y);
                }
            }

            savedBlocks.Add(new SavedBlock(cells));
        }

        /// <summary>
        /// Writes down a constructed block of MapCells to be permanently saved.
        /// </summary>
        /// <param name="block"></param>
        public void SaveExternalBlock(MapCell[,] block)
        {
            savedBlocks.Add(new SavedBlock(block));
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

            MapCell[,] newBlock = new MapCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newBlock[x, y] = new MapCell(block[x, y], xmin + x, ymin + y);
                }
            }

            savedBlocks.Add(new SavedBlock(newBlock));

        }
    }

    public struct SavedBlock
    {
        MapCell[,] blocks;
        int xmin, ymin, width, height;

        public SavedBlock(MapCell[,] blocks)
        {
            this.blocks = blocks;
            this.xmin = blocks[0,0].X;
            this.ymin = blocks[0,0].Y;

            this.width = blocks.GetLength(0);
            this.height = blocks.GetLength(1);
        }

        public bool containsCell(int x, int y)
        {
            return xmin <= x && x < xmin + width && ymin <= y && y < ymin + height;
        }

        public MapCell getCell(int x, int y)
        {
            return blocks[x - xmin, y - ymin];
        }
    }
}
