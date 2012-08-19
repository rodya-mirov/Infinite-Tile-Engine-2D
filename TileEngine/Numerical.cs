using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public static class Numerical
    {
        /// <summary>
        /// Returns the "correct" x%y (that is, the
        /// positive remainder of x/y).  Assumes
        /// y>0, but is made to deal with x<=0.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Mod(int x, int y)
        {
            int r = x % y;

            return (r < 0) ? r + y : r;
        }

        /// <summary>
        /// Returns the "correct" x%y (that is, the
        /// positive remainder of x/y).  Assumes
        /// y>0, but is made to deal with x<=0.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Mod(float x, float y)
        {
            float r = x % y;

            return (r < 0) ? r + y : r;
        }

        /// <summary>
        /// Returns the "correct" rounding DOWN
        /// of x/y, assuming y>0.  That is, the largest
        /// integer k such that k*y <= x.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int intDivide(float x, float y)
        {
            int r = (int)(x / y);

            return (r * y > x) ? r - 1 : r;
        }
        
        /// <summary>
        /// Returns the "correct" rounding DOWN
        /// of x/y, assuming y>0.  That is, the largest
        /// integer k such that k*y <= x.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int intDivide(int x, int y)
        {
            int r = (int)(x / y);

            return (r * y > x) ? r - 1 : r;
        }

        /// <summary>
        /// This takes a number between 0 and 1 and applies
        /// a smoothing to it (still increasing and between 0 and 1)
        /// so that the resulting function is smooth and has
        /// first and second derivatives 0 at the endpoints (0 and 1).
        /// 
        /// The formula itself is 6t^5 - 15t^4 + 10t^3
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float QuinticScale(float t)
        {
            return ((6 * t - 15) * t + 10) * t * t * t;
        }

        /// <summary>
        /// Returns a random floating point number between 0 and 1 using
        /// the two seeds in a repeatable way.
        /// </summary>
        /// <param name="xSeed"></param>
        /// <param name="ySeed"></param>
        /// <returns></returns>
        public static float GetRandom(int xSeed, int ySeed)
        {
            Random r = new Random(xSeed ^ ySeed);
            return (float)r.NextDouble();
        }
    }
}
