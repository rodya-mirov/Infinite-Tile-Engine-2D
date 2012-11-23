using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public abstract class TileMapManager
    {
        SpriteBatch spriteBatch;

        public SpriteFont Font { get; set; }

        public TileMap MyMap { get; protected set; }

        public String ContentLocation { get; protected set; }

        public Game game { get; protected set; }

        #region Drawing Information
        /// <summary>
        /// Rigged up to make (0, 0) in pixel coordinates default
        /// to the center of square (0, 0)
        /// </summary>
        int baseOffsetX;
        int baseOffsetY;

        int squaresWideToDraw;
        int squaresTallToDraw;

        /// <summary>
        /// We'll reduce the z-layer of each successive "height" tile
        /// by this very small number, so it gets painted on top of its fellows,
        /// but won't impact anything in front of it.
        /// </summary>
        float heightRowDepthMod = 0.0000001f;
        #endregion

        public TileMapManager(Game game, String contentLocation)
        {
            this.ContentLocation = contentLocation;
            this.game = game;

            baseOffsetX = -Tile.TileStepX;
            baseOffsetY = -Tile.TileHeight + Tile.TileStepY;
        }

        public virtual void Initialize()
        {
            MyMap = makeMap();
        }

        protected virtual TileMap makeMap()
        {
            return new TileMap();
        }

        public virtual void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            Tile.TileSetTexture = game.Content.Load<Texture2D>(ContentLocation);

            Camera.DisplayOffset = new Point(baseOffsetX, baseOffsetY);
        }

        public void SetViewDimensions(int width, int height)
        {
            Camera.ViewWidth = width;
            Camera.ViewHeight = height;

            squaresWideToDraw = 2 + (int)((Math.Abs(baseOffsetX) + width) / (Tile.TileStepX * 2));
            squaresTallToDraw = (int)((Math.Abs(baseOffsetY) + height) / (Tile.TileStepY * 2));
        }

        public void SetViewDimensions(GraphicsDeviceManager graphics)
        {
            this.SetViewDimensions(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public virtual void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            processMouseMovement(ms);

            updateInGameObjects();
        }

        protected virtual void updateInGameObjects()
        {
            foreach (InGameObject obj in InGameObjects)
                obj.Update();
        }

        /// <summary>
        /// MouseSquare is the most recently moused-over (in-game) square coordinate.
        /// This is updated automatically during the Update loop.
        /// </summary>
        public int MouseSquareX { get; set; }
        /// <summary>
        /// MouseSquare is the most recently moused-over (in-game) square coordinate.
        /// This is updated automatically during the Update loop.
        /// </summary>
        public int MouseSquareY { get; set; }
        /// <summary>
        /// MouseSquare is the most recently moused-over (in-game) square coordinate.
        /// This is updated automatically during the Update loop.
        /// </summary>
        public Point MouseSquare
        {
            get { return new Point(MouseSquareX, MouseSquareY); }
        }

        private int mouseScreenX { get; set; }
        private int mouseScreenY { get; set; }

        private void processMouseMovement(MouseState ms)
        {
            updateMousePosition(ms.X, ms.Y);
        }

        private void updateMousePosition(int mouseX, int mouseY)
        {
            //Premature optimization is the mother of weird errors
            /*
            if (this.mouseScreenX == mouseX && this.mouseScreenY == mouseY)
                return;
             */

            this.mouseScreenX = mouseX;
            this.mouseScreenY = mouseY;

            int properOffsetX = -Camera.Location.X;
            int properOffsetY = -Camera.Location.Y;

            int gameRawMouseX = mouseX - properOffsetX;
            int gameRawMouseY = mouseY - properOffsetY;

            int gridPosX = Numerical.intDivide(gameRawMouseX, Tile.TileStepX);
            int gridPosY = Numerical.intDivide(gameRawMouseY, Tile.TileStepY);

            //now two cases: define the grid parity to be gridPosX+gridPosY % 2
            //if it's even, our square is divided like this: /
            //    then either we're a lower-right corner or an upper-left corner
            //if it's odd, our square is divided like this: \
            //    then either we're an upper-right corner or a lower-left corner
            //we'll transform everything into an upper-left corner

            int gridParity = Numerical.Mod(gridPosX + gridPosY, 2);

            int inSquareX = gameRawMouseX - Tile.TileStepX * gridPosX;
            int inSquareY = gameRawMouseY - Tile.TileStepY * gridPosY;

            if (gridParity == 0)
            {
                //UL corner (below the / diagonal)
                if ((inSquareY - Tile.TileStepY) * Tile.TileStepX >= -inSquareX * Tile.TileStepY)
                {
                    //do nothing
                }
                else //DR corner
                {
                    gridPosX -= 1;
                    gridPosY -= 1;
                }
            }
            else
            {
                //UR corner (below the \ diagonal)
                if (inSquareY * Tile.TileStepX >= Tile.TileStepY * inSquareX)
                {
                    gridPosX -= 1;
                }
                else //DL corner
                {
                    gridPosY -= 1;
                }
            }

            //the parity is 0, guaranteed by the above, so there's no issue of rounding errors
            int relativeSquareX = Numerical.intDivide(gridPosX + gridPosY, 2) + 1;
            int relativeSquareY = Numerical.intDivide(gridPosX - gridPosY, 2);

            this.MouseSquareX = relativeSquareX;
            this.MouseSquareY = relativeSquareY;
        }

        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            float maxdepth = ((squaresWideToDraw + 1) + ((squaresTallToDraw + 1) * Tile.TileWidth)) * 10;

            //converts pixels to steps
            int leftX = Numerical.intDivide(Camera.Location.X, Tile.TileStepX);
            int topY = Numerical.intDivide(Camera.Location.Y, Tile.TileStepY);

            //if the sum is now odd, we will get weird errors, so just move over a little
            //we will fix the inelegance with more offset
            if (Numerical.Mod(leftX + topY, 2) == 1)
                leftX--;

            int firstX = (leftX + topY) / 2;
            int firstY = (leftX - topY) / 2;

            int offsetX = Camera.Location.X - (firstX + firstY) * Tile.TileStepX - baseOffsetX;
            int offsetY = Camera.Location.Y - (firstX - firstY) * Tile.TileStepY - baseOffsetY;

            drawTileMapCells(maxdepth, firstX, firstY, offsetX, offsetY);
            drawInGameObjects(maxdepth, firstX, firstY, offsetX, offsetY);

            spriteBatch.End();
        }

        protected void drawInGameObjects(float maxdepth, int firstX, int firstY, int offsetX, int offsetY)
        {
            foreach (InGameObject obj in InGameObjects)
            {
                Rectangle sourceRectangle = obj.SourceRectangle;

                Rectangle destinationRectangle = obj.MakeDestinationRectangle(firstX, firstY, offsetX, offsetY);

                Rectangle objBox = obj.InWorldSquareBoundingBox;

                float depth = calculateDepthOffset(maxdepth, objBox.Right - firstX, objBox.Top - firstY) - heightRowDepthMod;

                spriteBatch.Draw(
                    obj.Texture,
                    destinationRectangle,
                    sourceRectangle,
                    Color.White, //no tint
                    0f, //no rotation
                    Vector2.Zero, //don't use origin
                    SpriteEffects.None,
                    depth);
            }
        }

        protected virtual IEnumerable<InGameObject> InGameObjects
        {
            get
            {
                yield break;
            }
        }

        private void drawTileMapCells(float maxdepth, int firstX, int firstY, int offsetX, int offsetY)
        {
            int xDrawPosition, yDrawPosition;

            MapCell cellToDraw;

            for (int xPlusYHalf = 0; xPlusYHalf < squaresWideToDraw; xPlusYHalf++)
            {
                for (int xMinusYHalf = 0; xMinusYHalf < squaresTallToDraw; xMinusYHalf++)
                {
                    int x = xPlusYHalf + xMinusYHalf;
                    int y = xPlusYHalf - xMinusYHalf;

                    //we draw it once, move x over by one, then draw again
                    for (int i = 0; i < 2; i++)
                    {
                        xDrawPosition = (x + y) * Tile.TileStepX - offsetX;
                        yDrawPosition = (x - y) * Tile.TileStepY - offsetY;

                        //we really are moving around with those firstX and firstY values
                        cellToDraw = MyMap.GetMapCell(firstX + x, firstY + y);

                        cellToDraw.DrawCell(spriteBatch,
                            xDrawPosition,
                            yDrawPosition,
                            calculateDepthOffset(maxdepth, x, y),
                            heightRowDepthMod);

                        x++;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the depthOffset that should be used, given a maxdepth value
        /// and LOCAL (that is, for drawing purposes) x and y coordinates.
        /// </summary>
        /// <param name="maxdepth"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static float calculateDepthOffset(float maxdepth, int x, int y)
        {
            return 0.7f - ((x - (y * Tile.TileWidth)) / maxdepth);
        }

        #region Pathing Assistance
        /// <summary>
        /// Returns an enumeration of the points which can be directly moved to
        /// from the specified point on the map.  Any point (at all!) should be in this
        /// enumeration if and only if it will pass CanMoveFromSquareToSquare
        /// with the specified start point.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public abstract IEnumerable<Point> GetAdjacentPoints(int X, int Y);

        
        /// <summary>
        /// Determines whether one can move directly from the start square to the end square.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public abstract bool CanMoveFromSquareToSquare(int startX, int startY, int endX, int endY);
        #endregion
    }
}
