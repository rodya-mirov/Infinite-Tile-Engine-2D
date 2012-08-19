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
        public int X { get; set; }
        public int Y { get; set; }

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
        public void DrawCell(SpriteBatch spriteBatch, int xDrawPosition, int yDrawPosition, float startingDepth, float heightRowDepthMod, SpriteFont font = null)
        {
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
                    startingDepth
                    );
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
                    startingDepth - ((float)heightRow * heightRowDepthMod) //the base depth, minus the offset from the height piling
                    );
                heightRow++;
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
                    startingDepth - ((float)heightRow * heightRowDepthMod)
                    );
            }
        }
    }
}
