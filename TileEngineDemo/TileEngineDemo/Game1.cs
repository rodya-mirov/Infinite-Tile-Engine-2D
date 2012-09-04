using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;

namespace TileEngineDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        TileMapComponent tileMapComponent;

        public Game1()
        {
            this.IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            tileMapComponent = new TileMapComponent(this, @"Textures\TileSets\TileSheet");
            Components.Add(tileMapComponent);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            tileMapComponent.SetViewDimensions(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            SpriteFont font = Content.Load<SpriteFont>("Fonts/Segoe");
            tileMapComponent.Font = font;

            BuildDefaultTiles();
        }

        private void BuildDefaultTiles()
        {
            MapCell[,] houseBlock = constructHouse();
            tileMapComponent.MyMap.AddConstructedBlock(houseBlock, 5, 5);
            tileMapComponent.MyMap.AddConstructedBlock(houseBlock, 5, 7);
            tileMapComponent.MyMap.AddConstructedBlock(houseBlock, 5, 10);
            tileMapComponent.MyMap.AddConstructedBlock(houseBlock, 5, 12);
        }

        /// <summary>
        /// A pre-made method which shows how to construct a house
        /// </summary>
        /// <returns></returns>
        private MapCell[,] constructHouse()
        {
            MapCell[,] houseBlock = new MapCell[4, 2];

            MapCell cell;

            //0, 0, left corner
            cell = new MapCell(70, 0, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[0, 0] = cell;

            //0, 2, top corner
            cell = new MapCell(70, 0, 1);
            cell.AddHeightTile(51);
            houseBlock[0, 1] = cell;

            //1, 0, bottom wall
            cell = new MapCell(70, 1, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[1, 0] = cell;

            //1, 1, top wall
            cell = new MapCell(70, 1, 1);
            cell.AddHeightTile(60);
            houseBlock[1, 1] = cell;

            //2, 0, bottom wall
            cell = new MapCell(70, 2, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[2, 0] = cell;

            //2, 1, top wall
            cell = new MapCell(70, 2, 1);
            cell.AddHeightTile(60);
            houseBlock[2, 1] = cell;

            //3, 0, right corner
            cell = new MapCell(70, 3, 0);
            cell.AddHeightTile(91);
            cell.AddHeightTile(31);
            houseBlock[3, 0] = cell;

            //3, 1, bottom corner
            cell = new MapCell(70, 3, 1);
            cell.AddHeightTile(94);
            cell.AddHeightTile(37);
            houseBlock[3, 1] = cell;

            //send it out!
            return houseBlock;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();

            processMovement(ks);

            base.Update(gameTime);
        }

        private void processMovement(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
                Camera.Move(-2, 0);

            if (ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
                Camera.Move(2, 0);

            if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
                Camera.Move(0, -2);

            if (ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
                Camera.Move(0, 2);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
