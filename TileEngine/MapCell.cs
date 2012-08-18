using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class MapCell
    {
        /// <summary>
        /// The list of tiles which exist "at ground level"
        /// </summary>
        public List<int> BaseTiles = new List<int>();

        /// <summary>
        /// A stand-in for a generated value
        /// </summary>
        public float Intensity { get; set; }

        /// <summary>
        /// A list of tiles which effectively pile on top of
        /// each other to form a tall tile.
        /// </summary>
        public List<int> HeightTiles = new List<int>();

        /// <summary>
        /// A list of tiles which effective live at the top of the
        /// pile of height tiles.
        /// </summary>
        public List<int> TopperTiles = new List<int>();

        /// <summary>
        /// Returns or sets the bottom-most tile in the base
        /// </summary>
        public int TileID
        {
            get { return BaseTiles.Count > 0 ? BaseTiles[0] : 0; }
            set
            {
                if (BaseTiles.Count > 0)
                    BaseTiles[0] = value;
                else
                    AddBaseTile(value);
            }
        }

        /// <summary>
        /// Adds a new BaseTile on top of the existing base tiles.
        /// </summary>
        /// <param name="tileID"></param>
        public void AddBaseTile(int tileID)
        {
            BaseTiles.Add(tileID);
        }

        /// <summary>
        /// Adds a new HeightTile on top of the existing height tiles.
        /// </summary>
        /// <param name="tileID"></param>
        public void AddHeightTile(int tileID)
        {
            HeightTiles.Add(tileID);
        }

        /// <summary>
        /// Adds a new TopperTile on top of the existing topper tiles.
        /// </summary>
        /// <param name="tileID"></param>
        public void AddTopperTile(int tileID)
        {
            TopperTiles.Add(tileID);
        }

        /// <summary>
        /// Constructs a new MapCell with the supplied bottom-most tile.
        /// </summary>
        /// <param name="tileID"></param>
        public MapCell(int tileID)
        {
            TileID = tileID;
        }
    }
}
