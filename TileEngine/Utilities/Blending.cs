using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine.Utilities
{
    public static class Blending
    {
        public static Color MultiplyBlend(Color a, Color b)
        {
            Vector4 av = a.ToVector4();
            Vector4 bv = b.ToVector4();

            return new Color(av * bv);
        }
    }
}
