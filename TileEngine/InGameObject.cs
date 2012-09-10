using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public abstract class InGameObject
    {
        /// <summary>
        /// General update method; should be called once per timestep.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// The current texture we should be drawing
        /// </summary>
        public abstract Texture2D Texture { get; }

        /// <summary>
        /// The soruce rectangle we should be drawing from
        /// </summary>
        public abstract Rectangle SourceRectangle { get; }

        /// <summary>
        /// The x-coordinate square this best occupies
        /// </summary>
        public virtual int squareX
        {
            get
            {
                return Numerical.intDivide(xPositionWorld + Tile.TileInGameWidthHalf, Tile.TileInGameWidth);
            }
        }

        /// <summary>
        /// The y-coordinate square this best occupies
        /// </summary>
        public virtual int squareY
        {
            get
            {
                return Numerical.intDivide(yPositionWorld + Tile.TileInGameHeightHalf, Tile.TileInGameHeight);
            }
        }

        /// <summary>
        /// The x-coordinate in in-game pixels (this can be confusing
        /// for humans to parse, but if you treat the game world like a
        /// square grid and just enjoy the automatic tilt, it works fine)
        /// </summary>
        public abstract int xPositionWorld { get; }

        /// <summary>
        /// The y-coordinate in in-game pixels (this can be confusing
        /// for humans to parse, but if you treat the game world like a
        /// square grid and just enjoy the automatic tilt, it works fine)
        /// </summary>
        public abstract int yPositionWorld { get; }

        /// <summary>
        /// Calculates the un-translated xposition to draw this sprite at
        /// </summary>
        public virtual int xPositionDraw
        {
            get
            {
                return Numerical.intDivide(xPositionWorld * Tile.TileStepX, Tile.TileInGameWidth)
                    + Numerical.intDivide(yPositionWorld * Tile.TileStepX, Tile.TileInGameHeight);
            }
        }

        /// <summary>
        /// Calculates the un-translated yposition to draw this sprite at
        /// </summary>
        public virtual int yPositionDraw
        {
            get
            {
                return Numerical.intDivide(xPositionWorld * Tile.TileStepY, Tile.TileInGameWidth)
                    - Numerical.intDivide(yPositionWorld * Tile.TileStepY, Tile.TileInGameHeight);
            }
        }

        /// <summary>
        /// Calculates the location where the this thing should be drawn
        /// </summary>
        /// <param name="firstX"></param>
        /// <param name="firstY"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public virtual Rectangle MakeDestinationRectangle(int firstX, int firstY, int offsetX, int offsetY)
        {
            Rectangle source = SourceRectangle;

            Rectangle output = new Rectangle();

            output.X = xPositionDraw - offsetX - (firstX + firstY) * Tile.TileStepX;

            output.Y = yPositionDraw - offsetY - (firstX - firstY) * Tile.TileStepY;

            output.Height = source.Width;
            output.Width = source.Height;

            return output;
        }
    }
}
