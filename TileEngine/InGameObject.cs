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
        /// Constructs a rectangle bounding the object in the world.
        /// The default behavior is a zero-width, zero-height box
        /// centered at the supplied xPositionWorld,yPositionWorld
        /// </summary>
        /// <returns></returns>
        public virtual Rectangle InWorldPixelBoundingBox
        {
            get
            {
                return new Rectangle(xPositionWorld, yPositionWorld, 0, 0);
            }
        }

        /// <summary>
        /// Constructs a rectangle of all the squares which this object
        /// touches.  Note that just because two objects touch the same
        /// square does not necessarily mean they touch each other!
        /// This is a very coarse bounding box.
        /// </summary>
        public virtual Rectangle InWorldSquareBoundingBox
        {
            get
            {
                Rectangle pixelBox = InWorldPixelBoundingBox;
                Rectangle output = new Rectangle();

                output.X = FindXSquare(pixelBox.X, pixelBox.Y);
                output.Y = FindYSquare(pixelBox.X, pixelBox.Y);

                int otherCornerX = FindXSquare(pixelBox.Right, pixelBox.Bottom);
                int otherCornerY = FindYSquare(pixelBox.Right, pixelBox.Bottom);

                output.Width = otherCornerX - output.X;
                output.Height = otherCornerY - output.Y;

                return output;
            }
        }
        
        /// <summary>
        /// Finds the square coordinate of this object.
        /// </summary>
        /// <returns></returns>
        public Point SquareCoordinate()
        {
            return new Point(
                FindXSquare(xPositionWorld, yPositionWorld),
                FindYSquare(xPositionWorld, yPositionWorld)
                );
        }

        /// <summary>
        /// Calculates the X-Square coordinate from a given (in-game pixel) point
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        protected int FindXSquare(int xPos, int yPos)
        {
            return Numerical.intDivide(xPos + Tile.TileInGameWidthHalf, Tile.TileInGameWidth);
        }

        /// <summary>
        /// Calculates the Y-Square coordinate from a given (in-game pixel) point
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        protected int FindYSquare(int xPos, int yPos)
        {
            return Numerical.intDivide(yPos + Tile.TileInGameHeightHalf, Tile.TileInGameHeight);
        }

        /// <summary>
        /// Calculates the X-Ingame-pixel coordinate from a given square point
        /// </summary>
        /// <param name="xSquare"></param>
        /// <param name="ySquare"></param>
        /// <returns></returns>
        protected int FindXCoordinate(int xSquare, int ySquare)
        {
            return xSquare * Tile.TileInGameWidth;
        }

        /// <summary>
        /// Calculates the Y-Ingame-pixel coordinate from a given square point
        /// </summary>
        /// <param name="xSquare"></param>
        /// <param name="ySquare"></param>
        /// <returns></returns>
        protected int FindYCoordinate(int xSquare, int ySquare)
        {
            return ySquare * Tile.TileInGameHeight;
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
