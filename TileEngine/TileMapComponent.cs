using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public class TileMapComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        public SpriteFont Font { get; set; }

        public TileMap MyMap { get; set; }

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

        public TileMapComponent(Game game)
            : base(game)
        {
            MyMap = new TileMap();

            baseOffsetX = -Tile.TileStepX;
            baseOffsetY = -Tile.TileHeight + Tile.TileStepY;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Tile.TileSetTexture = Game.Content.Load<Texture2D>(@"Textures\TileSets\part4_tileset");

            Camera.DisplayOffset = new Point(baseOffsetX, baseOffsetY);

            base.LoadContent();
        }

        public void SetViewDimensions(int width, int height)
        {
            Camera.ViewWidth = width;
            Camera.ViewHeight = height;

            squaresWideToDraw = 1 + (int)((Math.Abs(baseOffsetX) + width) / (Tile.TileStepX * 2));
            squaresTallToDraw = (int)((Math.Abs(baseOffsetY) + height) / (Tile.TileStepY * 2));
        }

        public void SetViewDimensions(GraphicsDeviceManager graphics)
        {
            this.SetViewDimensions(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Left))
                Camera.Move(-2, 0);

            if (ks.IsKeyDown(Keys.Right))
                Camera.Move(2, 0);

            if (ks.IsKeyDown(Keys.Up))
                Camera.Move(0, -2);

            if (ks.IsKeyDown(Keys.Down))
                Camera.Move(0, 2);

            if (ks.IsKeyDown(Keys.A))
                throw new Exception();

            base.Update(gameTime);
        }

        /// <summary>
        /// Transforms "world coordinates" to "pixel coordinates".
        /// 
        /// The "positive x" world vector (1, 0) is (Tile.TileStepX, Tile.TileStepY) in pixels
        /// The "positive y" world vector (0, 1) is (Tile.TileStepX, -Tile.TileStepY) in pixels
        /// 
        /// So the transformation matrix is [ [TSX, TSY], [TSX, -TSY] ]
        /// </summary>
        /// <param name="worldCoordinates"></param>
        /// <returns></returns>
        public Vector2 WorldToPixel(Vector2 worldCoordinates)
        {
            return new Vector2(
                Tile.TileStepX * (worldCoordinates.X + worldCoordinates.Y),
                Tile.TileStepY * (worldCoordinates.X - worldCoordinates.Y)
                );
        }

        /// <summary>
        /// Transforms "pixel coordinates" into "world coordinates"
        /// 
        /// The transformation matrix above has the following inverse:
        /// 
        ///     [ [.5/TSX, .5/TSY], [.5/TSX, -.5/TSY] ]
        ///     
        /// Which will invert the transform and send pixel coordinates
        /// into game-world coordinates.
        /// </summary>
        /// <param name="pixelCoords"></param>
        /// <returns></returns>
        public Vector2 PixelToWorld(Vector2 pixelCoords)
        {
            return new Vector2(
                pixelCoords.X / (Tile.TileStepX * 2f) + pixelCoords.Y / (Tile.TileStepY * 2f),
                pixelCoords.X / (Tile.TileStepX * 2f) - pixelCoords.Y / (Tile.TileStepY * 2f)
                );
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            float maxdepth = ((squaresWideToDraw + 1) + ((squaresTallToDraw + 1) * Tile.TileWidth)) * 10;
            float depthOffset;

            //converts pixels to steps
            int leftX = Numerical.intDivide(Camera.Location.X, Tile.TileStepX);
            int topY = Numerical.intDivide(Camera.Location.Y, Tile.TileStepY);

            //if the sum is now odd, we will get weird errors, so just move over a little
            //we will fix the inelegance with more offset
            if (Numerical.Mod(leftX + topY, 2) == 1)
                leftX--;

            int firstX = (leftX + topY) / 2;
            int firstY = (leftX - topY) / 2;

            int offsetX = Camera.Location.X - (firstX + firstY) * Tile.TileStepX;
            int offsetY = Camera.Location.Y - (firstX - firstY) * Tile.TileStepY;

            int xDrawPosition, yDrawPosition;

            MapCell cellToDraw;

            for (int xPlusYHalf = 0; xPlusYHalf < squaresWideToDraw; xPlusYHalf++)
            {
                for (int xMinusYHalf = 0; xMinusYHalf < squaresTallToDraw; xMinusYHalf++)
                {
                    int x = xPlusYHalf + xMinusYHalf;
                    int y = xPlusYHalf - xMinusYHalf;

                    /* Yes, yes the code is repeated ... just change both if necessary */

                    xDrawPosition = (x + y) * Tile.TileStepX - offsetX + baseOffsetX;
                    yDrawPosition = (x - y) * Tile.TileStepY - offsetY + baseOffsetY;

                    //we really are moving around with those firstX and firstY values
                    cellToDraw = MyMap.GetMapCell(firstX + x, firstY + y);

                    //the depth is just based on the loop itself
                    depthOffset = 0.7f - ((x + (y * Tile.TileWidth)) / maxdepth);

                    cellToDraw.DrawCell(spriteBatch, xDrawPosition, yDrawPosition, depthOffset, heightRowDepthMod, Font);

                    x++;

                    /* Yes, yes the code is repeated ... just change both if necessary */

                    xDrawPosition = (x + y) * Tile.TileStepX - offsetX + baseOffsetX;
                    yDrawPosition = (x - y) * Tile.TileStepY - offsetY + baseOffsetY;

                    //we really are moving around with those firstX and firstY values
                    cellToDraw = MyMap.GetMapCell(firstX + x, firstY + y);

                    //the depth is just based on the loop itself
                    depthOffset = 0.7f - ((x + (y * Tile.TileWidth)) / maxdepth);

                    cellToDraw.DrawCell(spriteBatch, xDrawPosition, yDrawPosition, depthOffset, heightRowDepthMod, Font);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
