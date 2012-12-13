using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine.Utilities
{
    public interface Copyable<T>
    {
        T Copy();
    }
}
