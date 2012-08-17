using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
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

        public static Vector2 DisplayOffset { get; set; }

        private static Vector2 location = Vector2.Zero;

        /// <summary>
        /// This Vector2 represents the "upper left corner" of the viewable window.
        /// </summary>
        public static Vector2 Location
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
        public static Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return worldPosition - Location + DisplayOffset;
        }

        /// <summary>
        /// Transforms "drawable" coordinates to "in-world" coordinates.
        /// </summary>
        /// <param name="screenPosition">The drawable coordinates</param>
        /// <returns>The in-world coordinates</returns>
        public static Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return screenPosition + Location - DisplayOffset;
        }

        /// <summary>
        /// Translates the camera by the specified offset
        /// </summary>
        /// <param name="offset"></param>
        public static void Move(Vector2 offset)
        {
            Location += offset;
        }

        /// <summary>
        /// Translates the camera by the specified offset
        /// </summary>
        /// <param name="offset"></param>
        public static void Move(float xOffset, float yOffset)
        {
            Location += new Vector2(xOffset, yOffset);
        }
    }
}
