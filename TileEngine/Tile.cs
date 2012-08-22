using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileEngine
{
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
        /// The amount of extra offset to put on the tile if the row has an odd index
        /// (this gives an alternating effect)
        /// </summary>
        static public int OddRowXOffset = 32;

        /// <summary>
        /// The height represented by each tile in MapCell.HeightTiles
        /// </summary>
        static public int HeightTileOffset = 32;

        /// <summary>
        /// Returns the rectangle needed to draw the tile at a specific index
        /// </summary>
        /// <param name="tileIndex">The index of the tile to draw, in terms of the tile grid in the source image</param>
        /// <returns>The source Rectangle required to draw it</returns>
        static public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tileY = tileIndex / (TileSetTexture.Width / TileWidth);
            int tileX = tileIndex % (TileSetTexture.Width / TileWidth);

            return new Rectangle(tileX * TileWidth, tileY * TileHeight, TileWidth, TileHeight);
        }


        /// <summary>
        /// Returns the rectangle needed to draw the tile at a specific index
        /// </summary>
        /// <param name="type">The special name of a type of tile</param>
        /// <returns>The source Rectangle required to draw it</returns>
        static public Rectangle GetSourceRectangle(TileType type)
        {
            return GetSourceRectangle((int)type);
        }
    }

    //Each row has 10 things in it, things are numbered in reading order from 0
    //so the first in each row are numbered 0, 10, 20, etc.
    public enum TileType
    {
        //these I drew :P
        GRASS = 0,
        WATER = 1,

        HIGHLIGHTED = 10

        //the building tiles are hard to label individually so I won't
    };
}
