﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileEngineDemo
{
    public class NPCexample : InGameObject
    {
        private TileMap map { get; set; }
        private static MapCell HighlightCell { get; set; }

        public NPCexample(int x, int y, TileMap map)
        {
            xPos = x;
            yPos = y;

            this.map = map;
        }

        protected static Texture2D NPCtexture;
        protected static Rectangle[] sources;

        public static void LoadContent(Game game)
        {
            if (NPCtexture == null)
                NPCtexture = game.Content.Load<Texture2D>(@"Textures\NPCs\LineArt");

            sources = new Rectangle[4];
            for (int i = 0; i < 4; i++)
                sources[i] = new Rectangle(0, 64 * i, 64, 64);

            HighlightCell = new MapCell(1, 0, 0);
        }

        private const int waitForTurn = 90;
        private int ticksSinceTurn = waitForTurn;
        private int sourceIndex = 0;

        private int xVel = 0;
        private int yVel = 0;

        public override void Update()
        {
            ticksSinceTurn++;

            map.ClearOverrideAtPosition(squareX, squareY);

            if (ticksSinceTurn >= waitForTurn)
            {
                ticksSinceTurn = 0;

                switch (sourceIndex)
                {
                    case 0:
                        sourceIndex = 1;
                        xVel = 1;
                        yVel = 0;
                        break;

                    case 1:
                        sourceIndex = 3;
                        xVel = 0;
                        yVel = 1;
                        break;

                    case 2:
                        sourceIndex = 0;
                        xVel = 0;
                        yVel = -1;
                        break;

                    case 3:
                        sourceIndex = 2;
                        xVel = -1;
                        yVel = 0;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            xPos += xVel;
            yPos += yVel;

            map.SetOverride(HighlightCell, squareX, squareY);
        }

        public override Texture2D Texture
        {
            get
            {
                return NPCtexture;
            }
        }

        protected int xPos { get; set; }
        protected int yPos { get; set; }

        public override int xPositionWorld
        {
            get { return xPos; }
        }

        public override int yPositionWorld
        {
            get { return yPos; }
        }

        public override Rectangle SourceRectangle
        {
            get { return sources[sourceIndex]; }
        }
    }
}