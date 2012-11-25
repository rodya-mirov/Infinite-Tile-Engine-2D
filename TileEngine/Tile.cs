using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    /// <summary>
    /// A completely static class representing the
    /// information about the tiles which make up
    /// the world.  Includes a single static texture,
    /// as well as integers representing the various
    /// sizes.
    /// </summary>
    public static class Tile
    {
        static public Texture2D TileSetTexture;

        /// <summary>
        /// The width in pixels of each tile (graphic!)
        /// </summary>
        static public int TileWidth = 64;

        /// <summary>
        /// The height in pixels of each tile (graphic!)
        /// </summary>
        static public int TileHeight = 64;

        /// <summary>
        /// The amount to change the x-coordinate when drawing the next tile in a row
        /// </summary>
        static public int TileStepX = 32;

        /// <summary>
        /// The amount to change the y-coordinate when drawing the next row
        /// </summary>
        static public int TileStepY = 16;

        /// <summary>
        /// The height represented by each tile in MapCell.HeightTiles
        /// </summary>
        static public int HeightTileOffset = 32;

        /// <summary>
        /// For in-game coordinates, this is how "tall" the tile is,
        /// in the y-dimension.
        /// </summary>
        static public int TileInGameHeight = 32;

        /// <summary>
        /// Literally and always TileInGameHeight/2
        /// </summary>
        static public int TileInGameHeightHalf
        {
            get { return TileInGameHeight / 2; }
            set { TileInGameHeight = value * 2; }
        }

        /// <summary>
        /// For in-game coordinates, this is how "wide" the tile is,
        /// in the x-dimension.
        /// </summary>
        static public int TileInGameWidth = 32;

        /// <summary>
        /// Literally and always TileInGameWidth/2
        /// </summary>
        static public int TileInGameWidthHalf
        {
            get { return TileInGameWidth / 2; }
            set { TileInGameWidth = value * 2; }
        }

        /// <summary>
        /// Returns the rectangle needed to draw the tile at a specific index
        /// </summary>
        /// <param name="tileIndex">The index of the tile to draw, in terms of the tile grid in the source image</param>
        /// <returns>The source Rectangle required to draw it</returns>
        static public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tilesPerRow = TileSetTexture.Width / TileWidth;

            int tileY = tileIndex / tilesPerRow;
            int tileX = tileIndex % tilesPerRow;

            return new Rectangle(tileX * TileWidth, tileY * TileHeight, TileWidth, TileHeight);
        }
    }
}
