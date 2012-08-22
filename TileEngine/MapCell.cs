using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class MapCell
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        /// <summary>
        /// The list of tiles which exist "at ground level"
        /// </summary>
        public List<int> BaseTiles = new List<int>();

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
        /// <param name="bottomTileID"></param>
        public MapCell(int bottomTileID, int x, int y)
        {
            AddBaseTile(bottomTileID);

            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructs a copy of the given cell, located
        /// at the new coordinates.
        /// </summary>
        /// <param name="toCopy"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public MapCell(MapCell toCopy, int newX, int newY)
        {
            BaseTiles = new List<int>(toCopy.BaseTiles);
            HeightTiles = new List<int>(toCopy.HeightTiles);
            TopperTiles = new List<int>(toCopy.TopperTiles);

            this.X = newX;
            this.Y = newY;
        }

        /// <summary>
        /// Draws the cell.
        /// 
        /// If the font is supplied and non-null, coordinates will be drawn in the center of the cell.
        /// </summary>
        /// <param name="spriteBatch">An active spritebatch to draw the cell</param>
        /// <param name="xDrawPosition">The left-x pixel coordinate to draw at</param>
        /// <param name="yDrawPosition">The top-y pixel coordinate to draw at</param>
        /// <param name="startingDepth">The depth value of the bottom-most part of the cell.</param>
        /// <param name="heightRowDepthMod">The amount to decrease the depth with each stacked tile.</param>
        /// <param name="font">The font to draw the coordinates in</param>
        public void DrawCell(SpriteBatch spriteBatch, int xDrawPosition, int yDrawPosition,
            float startingDepth, float heightRowDepthMod)
        {
            float depthMod = 0;

            //Now draw the base tiles
            foreach (int tileID in this.BaseTiles)
            {
                spriteBatch.Draw(
                    Tile.TileSetTexture, //tiles texture
                    new Rectangle( //drawing region, offset for x and y
                        xDrawPosition,
                        yDrawPosition,
                        Tile.TileWidth,
                        Tile.TileHeight),
                    Tile.GetSourceRectangle(tileID), //source rectangle for this tile
                    Color.White, //no tint
                    0.0f, //no rotation
                    Vector2.Zero, //origin vector; 0 means do nothing in particular
                    SpriteEffects.None, //no sprite effects
                    startingDepth - depthMod
                    );

                depthMod += heightRowDepthMod;
            }

            //represents how deep in the HeightTiles stack we are
            int heightRow = 0;

            //then the height tiles
            foreach (int tileID in this.HeightTiles)
            {
                spriteBatch.Draw(
                    Tile.TileSetTexture, //texture
                    new Rectangle( //drawing region; like before, but move the Y up by Tile.HeightTileOffset each time we pile another HeightTile
                        xDrawPosition,
                        yDrawPosition - (heightRow * Tile.HeightTileOffset),
                        Tile.TileWidth,
                        Tile.TileHeight),
                    Tile.GetSourceRectangle(tileID), //tile source rectangle
                    Color.White, //no tint
                    0.0f, //no rotation
                    Vector2.Zero, //no use of the origin vector
                    SpriteEffects.None, //no sprite effects
                    startingDepth - depthMod - ((float)heightRow * heightRowDepthMod) //the base depth, minus the offset from the height piling
                    );

                heightRow++;
                depthMod += heightRowDepthMod;
            }

            //note heightRow is retained after that loop, so is now "the row above all the HeightTiles"

            //And finally, the topper tiles
            foreach (int tileID in this.TopperTiles)
            {
                spriteBatch.Draw(
                    Tile.TileSetTexture,
                    new Rectangle(
                        xDrawPosition,
                        yDrawPosition - (heightRow * Tile.HeightTileOffset),
                        Tile.TileWidth,
                        Tile.TileHeight),
                    Tile.GetSourceRectangle(tileID),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    startingDepth - depthMod - ((float)heightRow * heightRowDepthMod)
                    );

                depthMod += heightRowDepthMod;
            }

            /*
            //as a topper, draw the highlighted if necessary
            if (isHighlighted)
            {
                spriteBatch.Draw(
                    Tile.TileSetTexture,
                    new Rectangle(
                        xDrawPosition,
                        yDrawPosition - (heightRow * Tile.HeightTileOffset),
                        Tile.TileWidth,
                        Tile.TileHeight),
                    Tile.GetSourceRectangle((int)TileType.HIGHLIGHTED),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    startingDepth - ((float)heightRow * heightRowDepthMod)
                    );
            } */
        }

        /// <summary>
        /// Sorts by X-coordinate, then sub-sorts by Y-coordinate.
        /// Returns 0 iff the coordinates are the same.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(MapCell other)
        {
            if (this.X != other.X)
                return this.X - other.X;

            return this.Y - other.Y;
        }
    }

    public struct SortedPoint : IComparable<SortedPoint>
    {
        public int X, Y;

        public SortedPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public SortedPoint(MapCell cell)
        {
            this.X = cell.X;
            this.Y = cell.Y;
        }

        public int CompareTo(SortedPoint other)
        {
            if (this.X != other.X)
                return this.X - other.X;
            else
                return this.Y - other.Y;
        }
    }
}
