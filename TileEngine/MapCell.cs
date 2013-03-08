using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Utilities;

namespace TileEngine
{
    public class MapCell : Copyable<MapCell>
    {
        protected SortedDictionary<int, Queue<int>> Tiles;

        #region Border information
        /// <summary>
        /// Whether to use borders at all.  Default is false.
        /// </summary>
        protected virtual bool UseBorders { get { return false; } }

        /// <summary>
        /// If using borders, whether to use the left border.
        /// 
        /// Default is true.
        /// </summary>
        protected virtual bool UseLeftBorder { get { return true; } }

        /// <summary>
        /// If using borders, the tile index for the left border, which visually,
        /// is the "upper left".
        /// 
        /// Default throws an error.
        /// </summary>
        protected virtual int LeftBorderTileIndex { get { throw new NotImplementedException(); } }

        /// <summary>
        /// If using borders, whether to use the right border.
        /// 
        /// Default is true.
        /// </summary>
        protected virtual bool UseRightBorder { get { return true; } }

        /// <summary>
        /// If using borders, the tile index for the right border, which visually,
        /// is the "lower right".
        /// 
        /// Default throws an error.
        /// </summary>
        protected virtual int RightBorderTileIndex { get { throw new NotImplementedException(); } }

        /// <summary>
        /// If using borders, whether to use the top border.
        /// 
        /// Default is true.
        /// </summary>
        protected virtual bool UseTopBorder { get { return true; } }

        /// <summary>
        /// If using borders, the tile index for the top border, which visually,
        /// is the "lower left".
        /// 
        /// Default throws an error.
        /// </summary>
        protected virtual int TopBorderTileIndex { get { throw new NotImplementedException(); } }

        /// <summary>
        /// If using borders, whether to use the bottom border.
        /// 
        /// Default is true.
        /// </summary>
        protected virtual bool UseBottomBorder { get { return true; } }

        /// <summary>
        /// If using borders, the tile index for the bottom border, which visually,
        /// is the "upper right".
        /// 
        /// Default throws an error.
        /// </summary>
        protected virtual int BottomBorderTileIndex { get { throw new NotImplementedException(); } }
        #endregion

        /// <summary>
        /// This is how much to translate drawing the BASE of
        /// the tile, vertically.  It's intended to be an elevation,
        /// so positive numbers will appear higher up.
        /// </summary>
        protected int VisualElevation = 0;

        public SortedDictionary<int, Queue<int>> TilesCopy()
        {
            SortedDictionary<int, Queue<int>> tilesCopy = new SortedDictionary<int, Queue<int>>();
            foreach(int level in Tiles.Keys)
            {
                tilesCopy[level] = new Queue<int>(Tiles[level]);
            }

            return tilesCopy;
        }

        /// <summary>
        /// Constructs a new MapCell with the specified base tile at the specified coordinates.
        /// </summary>
        /// <param name="baseTile"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MapCell(int baseTile) : this()
        {
            Tiles[0] = new Queue<int>();

            Tiles[0].Enqueue(baseTile);
        }

        /// <summary>
        /// Like the public constructor, but doesn't set up a base tile.
        /// Not to be used lightly :P
        /// </summary>
        protected MapCell()
        {
            Tiles = new SortedDictionary<int, Queue<int>>();
        }

        /// <summary>
        /// Copies this cell!
        /// </summary>
        /// <returns></returns>
        public MapCell Copy()
        {
            MapCell output = new MapCell();

            foreach (int key in this.Tiles.Keys)
            {
                Queue<int> tileLevel = new Queue<int>();

                foreach (int tile in this.Tiles[key])
                    tileLevel.Enqueue(tile);

                output.Tiles[key] = tileLevel;
            }

            return output;
        }

        /// <summary>
        /// Adds a tile to the specified height level of the Cell
        /// </summary>
        /// <param name="tileID"></param>
        /// <param name="level"></param>
        public virtual void AddTile(int tileID, int level)
        {
            if (!Tiles.Keys.Contains(level))
                Tiles[level] = new Queue<int>();

            Tiles[level].Enqueue(tileID);
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
        /// <param name="tint">The color to tint the cell</param>
        public void DrawCell(SpriteBatch spriteBatch, int xDrawPosition, int yDrawPosition,
            float startingDepth, float heightRowDepthMod, Color tint)
        {
            float depth = startingDepth;
            int heightOffset = 0;
            int lastLevel = 0;

            foreach (int level in Tiles.Keys)
            {
                lastLevel = level;
                heightOffset = level * Tile.HeightTileOffset;

                foreach (int tileID in Tiles[level])
                {
                    spriteBatch.Draw(
                        Tile.TileSetTexture,
                        new Rectangle(
                            xDrawPosition,
                            yDrawPosition - heightOffset - VisualElevation,
                            Tile.TileWidth,
                            Tile.TileHeight
                            ),
                        Tile.GetSourceRectangle(tileID),
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth - ((float)level) * heightRowDepthMod);

                    depth -= heightRowDepthMod;
                }
            }

            if (UseBorders)
            {
                if (UseLeftBorder)
                {
                    spriteBatch.Draw(
                        Tile.TileSetTexture,
                        new Rectangle(
                            xDrawPosition,
                            yDrawPosition - heightOffset - VisualElevation,
                            Tile.TileWidth,
                            Tile.TileHeight
                            ),
                        Tile.GetSourceRectangle(LeftBorderTileIndex),
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth - ((float)lastLevel) * heightRowDepthMod);
                }

                if (UseRightBorder)
                {
                    spriteBatch.Draw(
                        Tile.TileSetTexture,
                        new Rectangle(
                            xDrawPosition,
                            yDrawPosition - heightOffset - VisualElevation,
                            Tile.TileWidth,
                            Tile.TileHeight
                            ),
                        Tile.GetSourceRectangle(RightBorderTileIndex),
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth - ((float)lastLevel) * heightRowDepthMod);
                }

                if (UseTopBorder)
                {
                    spriteBatch.Draw(
                        Tile.TileSetTexture,
                        new Rectangle(
                            xDrawPosition,
                            yDrawPosition - heightOffset - VisualElevation,
                            Tile.TileWidth,
                            Tile.TileHeight
                            ),
                        Tile.GetSourceRectangle(TopBorderTileIndex),
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth - ((float)lastLevel) * heightRowDepthMod);
                }

                if (UseBottomBorder)
                {
                    spriteBatch.Draw(
                        Tile.TileSetTexture,
                        new Rectangle(
                            xDrawPosition,
                            yDrawPosition - heightOffset - VisualElevation,
                            Tile.TileWidth,
                            Tile.TileHeight
                            ),
                        Tile.GetSourceRectangle(BottomBorderTileIndex),
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth - ((float)lastLevel) * heightRowDepthMod);
                }
            }
        }
    }
}
