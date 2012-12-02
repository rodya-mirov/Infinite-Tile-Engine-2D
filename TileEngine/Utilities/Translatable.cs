using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine.Utilities
{
    public interface Translatable<T>
    {
        /// <summary>
        /// Makes a copy of this object, located at the new coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        T CopyAt(int x, int y);
    }
}
