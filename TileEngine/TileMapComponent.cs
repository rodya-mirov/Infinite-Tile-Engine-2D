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
        public TileMap MyMap { get; set; }

        int baseOffsetX = 0;
        int baseOffsetY = 0;

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

            baseOffsetX = -Tile.TileWidth;
            baseOffsetY = -Tile.TileHeight;
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

            squaresWideToDraw = (int)((Math.Abs(baseOffsetX) + width) / Tile.TileStepX) + 3;
            squaresTallToDraw = (int)((Math.Abs(baseOffsetY) + height) / Tile.TileStepY) + 2;
        }

        public void SetViewDimensions(GraphicsDeviceManager graphics)
        {
            this.SetViewDimensions(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        private bool spaceDown = false;

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

            if (ks.IsKeyDown(Keys.Space) && !spaceDown)
            {
                Console.WriteLine(Camera.Location.X + ", " + Camera.Location.Y);
                spaceDown = true;

                int firstX = Camera.Location.X / Tile.TileStepX;
                int firstY = Camera.Location.Y / Tile.TileStepY;

                int offsetX = Camera.Location.X - firstX * Tile.TileStepX;
                int offsetY = Camera.Location.Y - firstY * Tile.TileStepY;

                Console.WriteLine(firstX + ", " + firstY);
                Console.WriteLine(offsetX + ", " + offsetY);

                Console.WriteLine();
            }
            else
            {
                spaceDown = false;
            }

            if (ks.IsKeyDown(Keys.A))
                throw new Exception();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            float maxdepth = ((squaresWideToDraw + 1) + ((squaresTallToDraw + 1) * Tile.TileWidth)) * 10;
            float depthOffset;

            int firstX = Camera.Location.X / Tile.TileStepX - 1;
            int firstY = Camera.Location.Y / Tile.TileStepY - 1;

            int offsetX = Camera.Location.X - firstX * Tile.TileStepX;
            int offsetY = Camera.Location.Y - firstY * Tile.TileStepY;

            for (int y = 0; y < squaresTallToDraw; y++)
            {
                //Move the odd rows over a bit
                int rowOffset = 0;

                //checks if it's odd
                if (((firstY + y) & 1) == 1)
                    rowOffset = Tile.OddRowXOffset;

                for (int x = 0; x < squaresWideToDraw; x++)
                {
                    //the depth is just based on the loop itself
                    depthOffset = 0.7f - ((x + (y * Tile.TileWidth)) / maxdepth);

                    //but we really are moving around with those firstX / firstY
                    MapCell currentCell = MyMap.GetMapCell(firstX + x, firstY + y);

                    //Now draw the base tiles
                    foreach (int tileID in currentCell.BaseTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture, //tiles texture
                            new Rectangle( //drawing region, offset for x and y
                                (x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.TileStepY) - offsetY + baseOffsetY,
                                Tile.TileWidth,
                                Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID), //source rectangle for this tile
                            Color.White, //no tint
                            0.0f, //no rotation
                            Vector2.Zero, //origin vector; 0 means do nothing in particular
                            SpriteEffects.None, //no sprite effects
                            depthOffset
                            );
                    }

                    //then the height tiles
                    int heightRow = 0;

                    foreach (int tileID in currentCell.HeightTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture, //texture
                            new Rectangle( //drawing region; like before, but move the Y up by Tile.HeightTileOffset each time we pile another HeightTile
                                (x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.TileStepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset),
                                Tile.TileWidth,
                                Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID), //tile source rectangle
                            Color.White, //no tint
                            0.0f, //no rotation
                            Vector2.Zero, //no use of the origin vector
                            SpriteEffects.None, //no sprite effects
                            depthOffset - ((float)heightRow * heightRowDepthMod) //the base depth, minus the offset from the height piling
                            );
                        heightRow++;
                    }

                    //And finally, the topper tiles
                    foreach (int tileID in currentCell.TopperTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture,
                            new Rectangle(
                                (x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.TileStepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset),
                                Tile.TileWidth,
                                Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod)
                            );
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
