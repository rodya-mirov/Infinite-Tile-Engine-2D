using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;
using TileEngine.Utilities;

namespace TileEngineDemo
{
    public class MapCellExtension : MapCell, Copyable<MapCellExtension>
    {
        #region Border Information
        protected override bool UseBorders { get { return true; } }

        private bool useLeftBorder, useRightBorder, useTopBorder, useBottomBorder;
        protected override bool UseLeftBorder { get { return useLeftBorder; } }
        protected override bool UseRightBorder { get { return useRightBorder; } }
        protected override bool UseTopBorder { get { return useTopBorder; } }
        protected override bool UseBottomBorder { get { return useBottomBorder; } }

        protected override int LeftBorderTileIndex { get { return 11; } }
        protected override int RightBorderTileIndex { get { return 12; } }
        protected override int TopBorderTileIndex { get { return 10; } }
        protected override int BottomBorderTileIndex { get { return 13; } }
        #endregion

        public MapCellExtension(int baseTile, bool left, bool right, bool top, bool bottom)
            : base(baseTile)
        {
            this.useLeftBorder = left;
            this.useRightBorder = right;
            this.useTopBorder = top;
            this.useBottomBorder = bottom;
        }

        protected MapCellExtension()
            : base()
        {
        }

        public new MapCellExtension Copy()
        {
            MapCellExtension output = new MapCellExtension();
            output.Tiles = this.TilesCopy();
            return output;
        }
    }
}
