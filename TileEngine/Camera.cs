using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    /// <summary>
    /// This determines draw positions, etc. to aid in drawing.  The coordinates are all
    /// integers (often as Points) instead of floats; the reason is that floats lose
    /// precision as their values increase, and we get weird glitches at super-large coordinates.
    /// This defeats the purpose of the infinite world!
    /// </summary>
    public static class Camera
    {
        /// <summary>
        /// This should represent the width of the viewable window,
        /// and should be set early on to reflect that.  Not updated
        /// automatically!
        /// </summary>
        public static int ViewWidth { get; set; }

        /// <summary>
        /// This should represent the height of the viewable window,
        /// and should be set early on to reflect that.  Not updated
        /// automatically!
        /// </summary>
        public static int ViewHeight { get; set; }

        public static Point DisplayOffset { get; set; }

        private static Point location = Point.Zero;

        /// <summary>
        /// This Vector2 represents the "upper left corner" of the viewable window.
        /// </summary>
        public static Point Location
        {
            get
            {
                return location;
            }
            private set
            {
                location.X = value.X;
                location.Y = value.Y;
            }
        }

        /// <summary>
        /// Transforms "in-world" coordinates to "drawable" coordinates.
        /// </summary>
        /// <param name="worldPosition">The in-world coordinates</param>
        /// <returns>The drawable coordinates</returns>
        public static Point WorldToScreen(Point worldPosition)
        {
            return new Point(
                worldPosition.X - Location.X + DisplayOffset.X,
                worldPosition.Y - Location.Y + DisplayOffset.Y
                );
        }

        /// <summary>
        /// Transforms "drawable" coordinates to "in-world" coordinates.
        /// </summary>
        /// <param name="screenPosition">The drawable coordinates</param>
        /// <returns>The in-world coordinates</returns>
        public static Point ScreenToWorld(Point screenPosition)
        {
            return new Point(
                screenPosition.X + Location.X - DisplayOffset.X,
                screenPosition.Y + Location.Y - DisplayOffset.Y
                );
        }

        /// <summary>
        /// Translates the camera by the specified offset
        /// </summary>
        /// <param name="offset"></param>
        public static void Move(Point offset)
        {
            Move(offset.X, offset.Y);
        }

        /// <summary>
        /// Translates the camera by the specified offset
        /// </summary>
        /// <param name="offset"></param>
        public static void Move(int xOffset, int yOffset)
        {
            Location = new Point(
                Location.X + xOffset,
                Location.Y + yOffset
                );
        }
    }
}
