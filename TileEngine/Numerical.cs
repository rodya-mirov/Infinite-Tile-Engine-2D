using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public static class Numerical
    {
        public static int mod(int x, int y)
        {
            if (y < 0)
                y = -y;

            int r = x % y;
            return r > 0 ? r : r + y;
        }

        public static float mod(float x, float y)
        {
            if (y < 0)
                y = -y;

            float r = x % y;
            return r > 0 ? r : r + y;
        }

        public static long mod(long x, long y)
        {
            if (y < 0)
                y = -y;

            long r = x % y;
            return r > 0 ? r : r + y;
        }
    }
}
