using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public static class Numerical
    {
        public static int Mod(int x, int y)
        {
            int r = x % y;

            return (r < 0) ? r + y : r;
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
            return 1;
            throw new NotImplementedException();
        }
    }
}
