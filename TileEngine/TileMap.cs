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

            constructDefaultTiles();
        }

        /// <summary>
        /// This constructs any default, specially designed
        /// tiles that we need to have.  They are saved into
        /// the MapSaved blocks and not considered in the
        /// procedural generation stuff.
        /// </summary>
        private void constructDefaultTiles()
        {
            constructHouse();
        }

        private void constructHouse()
        {
            MapCell[,] houseBlock = new MapCell[4, 2];

            MapCell cell;

            //0, 0, left corner
            cell = new MapCell(70, 0, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[0, 0] = cell;

            //0, 2, top corner
            cell = new MapCell(70, 0, 1);
            cell.AddHeightTile(51);
            houseBlock[0, 1] = cell;

            //1, 0, bottom wall
            cell = new MapCell(70, 1, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[1, 0] = cell;

            //1, 2, top wall
            cell = new MapCell(70, 1, 1);
            cell.AddHeightTile(60);
            houseBlock[1, 1] = cell;

            //2, 0, bottom wall
            cell = new MapCell(70, 2, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[2, 0] = cell;

            //2, 2, top wall
            cell = new MapCell(70, 2, 1);
            cell.AddHeightTile(60);
            houseBlock[2, 1] = cell;

            //3, 0, right corner
            cell = new MapCell(70, 2, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[3, 0] = cell;

            //3, 2, bottom corner
            cell = new MapCell(70, 2, 1);
            cell.AddHeightTile(94);
            cell.AddHeightTile(37);
            houseBlock[3, 1] = cell;

            //and save several copies!
            cellSaved.SaveExternalBlock(houseBlock, 0, 0);
            cellSaved.SaveExternalBlock(houseBlock, 0, 2);
            cellSaved.SaveExternalBlock(houseBlock, 0, 5);
            cellSaved.SaveExternalBlock(houseBlock, 0, 7);
        }

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
        public MapCell MakeMapCell(int x, int y)
        {
            MapCell output = new MapCell((int)TileType.GRASS, x, y);

            return output;
        }
    }
}
