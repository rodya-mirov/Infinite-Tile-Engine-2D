using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Utilies;
using TileEngine.Utilities;

namespace TileEngine
{
    public abstract class TileMapManager<InGameObjectType, MapCellType, MapType>
        where InGameObjectType : InGameObject
        where MapCellType : MapCell, Translatable<MapCellType>
        where MapType : TileMap<MapCellType>, new()
    {
        SpriteBatch spriteBatch;

        public SpriteFont Font { get; set; }

        public MapType MyMap { get; protected set; }

        public String ContentLocation { get; protected set; }

        public Game game { get; protected set; }

        #region Passability Information
        /// <summary>
        /// Represents the latest time that the passability
        /// information was changed.  This is, by definition and
        /// implementation, the maximum of LastEasierPassabilityUpdate
        /// and LastHarderPassabilityUpdate.
        /// 
        /// Is null if and only if passability has never been updated.
        /// </summary>
        public TimeSpan LastGeneralPassabilityUpdate
        {
            get
            {
                if (LastEasierPassabilityUpdate == null)
                    return LastHarderPassabilityUpdate;
                else if (LastHarderPassabilityUpdate == null)
                    return LastEasierPassabilityUpdate;
                else if (LastEasierPassabilityUpdate > LastHarderPassabilityUpdate)
                    return LastEasierPassabilityUpdate;
                else
                    return LastHarderPassabilityUpdate;
            }
        }

        /// <summary>
        /// Indicate that passability has been changed.
        /// This triggers both PassabilityMadeEasier(currentTime)
        /// and PassabilityMadeHarder(currentTime), and is therefore
        /// something of a blunt instrument.
        /// </summary>
        /// <param name="currentTime"></param>
        public void PassabilityChanged(GameTime currentTime)
        {
            PassabilityMadeEasier(currentTime);
            PassabilityMadeHarder(currentTime);
        }

        /// <summary>
        /// The latest time that passability was made easier.
        /// That is, when there were more paths available than before.
        /// </summary>
        public TimeSpan LastEasierPassabilityUpdate { get; protected set; }

        /// <summary>
        /// Indicate that passability has been made easier; that
        /// is, there are more allowable paths than before.
        /// </summary>
        /// <param name="currentTime"></param>
        public void PassabilityMadeEasier(GameTime currentTime)
        {
            this.LastEasierPassabilityUpdate = currentTime.TotalGameTime;
        }

        /// <summary>
        /// The latest time that passability was made harder.
        /// That is, when there were fewer paths available than before.
        /// </summary>
        public TimeSpan LastHarderPassabilityUpdate { get; protected set; }

        /// <summary>
        /// Indicate that passability has been made harder; that
        /// is, there are fewer allowable paths than before.
        /// </summary>
        /// <param name="currentTime"></param>
        public void PassabilityMadeHarder(GameTime currentTime)
        {
            this.LastHarderPassabilityUpdate = currentTime.TotalGameTime;
        }
        #endregion

        #region Drawing Information
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
        }

        public virtual void Initialize()
        {
            MyMap = makeMap();
        }

        protected virtual MapType makeMap()
        {
            return new MapType();
        }

        public virtual void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            Tile.TileSetTexture = game.Content.Load<Texture2D>(ContentLocation);

            Camera.DisplayOffset = new Point(0, 0);
        }

        public void SetViewDimensions(int width, int height)
        {
            Camera.ViewWidth = width;
            Camera.ViewHeight = height;

            squaresWideToDraw = 2 + (int)((width) / (Tile.TileStepX * 2));
            squaresTallToDraw = 3 + (int)((height) / (Tile.TileStepY * 2));
        }

        public void SetViewDimensions(GraphicsDeviceManager graphics)
        {
            this.SetViewDimensions(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public virtual void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            processMouseMovement(ms);

            updateInGameObjects(gameTime);
        }

        protected virtual void updateInGameObjects(GameTime gameTime)
        {
            foreach (InGameObjectType obj in InGameObjects())
                obj.Update(gameTime);
        }

        /// <summary>
        /// MouseWorldX is the x-coordinate of MouseWorldPosition.
        /// </summary>
        public int MouseWorldX { get; set; }
        /// <summary>
        /// MouseWorldY is the y-coordinate of MouseWorldPosition.
        /// </summary>
        public int MouseWorldY { get; set; }
        /// <summary>
        /// MouseWorldPosition is the most recently moused-over
        /// in-world (pixel) coordinate.  This is updated automatically
        /// during the update loop.
        /// </summary>
        public Point MouseWorldPosition
        {
            get { return new Point(MouseWorldX, MouseWorldY); }
        }

        /// <summary>
        /// MouseSquare is the most recently moused-over (in-game) square coordinate.
        /// This is updated automatically during the Update loop.
        /// </summary>
        public int MouseSquareX
        {
            get { return Numerical.intDivide(MouseWorldX, Tile.TileInGameWidth); }
        }

        /// <summary>
        /// MouseSquare is the most recently moused-over (in-game) square coordinate.
        /// This is updated automatically during the Update loop.
        /// </summary>
        public int MouseSquareY
        {
            get { return Numerical.intDivide(MouseWorldY, Tile.TileInGameHeight); }
        }

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

        /// <summary>
        /// Matrix transforms!  Turning mouse screen coordinates into
        /// in-game pixel coordinates, then scaling down to square coordinates.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        private void updateMousePosition(int mouseX, int mouseY)
        {
            this.mouseScreenX = mouseX;
            this.mouseScreenY = mouseY;

            int translatedMouseX = mouseX + Camera.Location.X;
            int translatedMouseY = mouseY + Camera.Location.Y;

            int wxTsx = Numerical.intDivide(Tile.TileInGameWidth * translatedMouseX, Tile.TileStepX);
            int wyTsy = Numerical.intDivide(Tile.TileInGameWidth * translatedMouseY, Tile.TileStepY);
            int hxTsx = Numerical.intDivide(Tile.TileInGameHeight * translatedMouseX, Tile.TileStepX);
            int hyTsy = Numerical.intDivide(Tile.TileInGameHeight * translatedMouseY, Tile.TileStepY);

            MouseWorldX = Numerical.intDivide(wxTsx + wyTsy, 2);
            MouseWorldY = Numerical.intDivide(hxTsx - hyTsy, 2);
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

            int firstX = (leftX + topY) / 2 - 1;
            int firstY = (leftX - topY) / 2;

            int offsetX = Camera.Location.X - (firstX + firstY) * Tile.TileStepX;
            int offsetY = Camera.Location.Y - (firstX - firstY) * Tile.TileStepY;

            drawTileMapCells(maxdepth, firstX, firstY, offsetX, offsetY);
            drawInGameObjects(maxdepth, firstX, firstY, offsetX, offsetY);

            spriteBatch.End();
        }

        protected void drawInGameObjects(float maxdepth, int firstX, int firstY, int offsetX, int offsetY)
        {
            foreach (InGameObject obj in InGameObjects())
            {
                Rectangle sourceRectangle = obj.SourceRectangle;

                Rectangle destinationRectangle = obj.MakeDestinationRectangle(firstX, firstY, offsetX, offsetY);

                Rectangle objBox = obj.InWorldSquareBoundingBox;

                float depth = calculateDepthOffset(maxdepth, objBox.Right - firstX - 1, objBox.Top - firstY);
                depth -= heightRowDepthMod; //since it's above the ground, level with the first level of walls

                spriteBatch.Draw(
                    obj.Texture,
                    destinationRectangle,
                    sourceRectangle,
                    obj.Tint,
                    0f, //no rotation
                    Vector2.Zero, //don't use origin
                    SpriteEffects.None,
                    depth);
            }
        }

        protected virtual IEnumerable<InGameObjectType> InGameObjects()
        {
            yield break;
        }

        /// <summary>
        /// This can be toggled to change whether cell coordinates are drawn.
        /// </summary>
        protected bool displayCellCoordinates = false;

        private void drawTileMapCells(float maxdepth, int firstX, int firstY, int offsetX, int offsetY)
        {
            int xDrawPosition, yDrawPosition;

            offsetX += Tile.TileVisualOffsetX;
            offsetY += Tile.TileVisualOffsetY;

            MapCellType cellToDraw;

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

                        int xIndex = firstX + x;
                        int yIndex = firstY + y;

                        //we really are moving around with those firstX and firstY values
                        cellToDraw = MyMap.GetMapCell(firstX + x, firstY + y);

                        cellToDraw.DrawCell(spriteBatch,
                            xDrawPosition,
                            yDrawPosition,
                            calculateDepthOffset(maxdepth, x, y),
                            heightRowDepthMod);

                        if (displayCellCoordinates && (Font != null))
                        {
                            String s = "(" + (firstX + x) + "," + (firstY + y) + ")";
                            Vector2 measurement = Font.MeasureString(s);

                            Vector2 drawPosition = new Vector2(
                                xDrawPosition + Tile.TileWidth / 2 - measurement.X / 2 + Tile.TileVisualOffsetX,
                                yDrawPosition - measurement.Y / 2 + Tile.TileVisualOffsetY
                                );

                            spriteBatch.DrawString(Font, s, drawPosition, Color.White);
                        }

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
