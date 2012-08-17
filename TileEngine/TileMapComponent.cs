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

        TileMap myMap = new TileMap();

        int squaresAcross = 40;
        int squaresDown = 40;

        int baseOffsetX = -32;
        int baseOffsetY = -64;

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

            Camera.DisplayOffset = new Vector2(baseOffsetX, baseOffsetY);

            base.LoadContent();
        }

        public void SetViewDimensions(int width, int height)
        {
            Camera.ViewWidth = width;
            Camera.ViewHeight = height;
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


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            float maxdepth = ((squaresAcross + 1) + ((squaresDown + 1) * Tile.TileWidth)) * 10;
            float depthOffset;

            int firstX = (int)(Camera.Location.X / Tile.TileStepX);
            int firstY = (int)(Camera.Location.Y / Tile.TileStepY);

            int offsetX = (int)(Numerical.mod(Camera.Location.X, Tile.TileStepX));
            int offsetY = (int)(Numerical.mod(Camera.Location.Y, Tile.TileStepY));

            for (int y = 0; y < squaresDown; y++)
            {
                //Move the odd rows over a bit
                int rowOffset = 0;
                if (Numerical.mod(firstY + y, 2) == 1)
                    rowOffset = Tile.OddRowXOffset;

                for (int x = 0; x < squaresAcross; x++)
                {
                    //the depth is just based on the loop itself
                    depthOffset = 0.7f - ((x + (y * Tile.TileWidth)) / maxdepth);

                    //but we really are moving around with those firstX / firstY
                    MapCell currentCell = myMap.GetMapCell(firstX + x, firstY + y);

                    //Now draw the base tiles
                    foreach (int tileID in currentCell.BaseTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture, //tiles texture
                            new Rectangle( //drawing region, offset for x and y
                                (x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.TileStepY) - offsetY + baseOffsetY + currentCell.BaseHeight,
                                Tile.TileWidth,
                                Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID), //source rectangle for this tile
                            Color.White, //no tint
                            0.0f, //no rotation
                            Vector2.Zero, //origin vector; 0 means do nothing in particular
                            SpriteEffects.None, //no sprite effects
                            1.0f //layer depth is 1, so the bottom of the bottom
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
                                (y * Tile.TileStepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset) + currentCell.BaseHeight,
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
                                (y * Tile.TileStepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset) + currentCell.BaseHeight,
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
