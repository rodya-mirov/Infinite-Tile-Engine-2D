using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class TileMap
    {
        public const int randomSeed = 121213;

        public TileMap()
        {
        }

        public MapCell GetMapCell(int x, int y)
        {
            Random random = new Random(randomSeed + (x^y));

            MapCell output = new MapCell(random.Next(0, 6));
            output.Intensity = makeIntensity(x, y);

            return output;
        }

        private const int numOctaves = 2; //keep this relatively small

        /// <summary>
        /// Constructs an intensity value using a grid-based value noise algorithm.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private float makeIntensity(int x, int y)
        {
            float output = 0;

            int multiplier = (1 << numOctaves); //2 ** numOctaves

            int leftX, rightX;
            int topY, bottomY;

            float UL, UR, DL, DR;
            float topAverage, bottomAverage, average;

            float fMult;

            while (multiplier > 0)
            {
                fMult = (float)multiplier;

                leftX = x - Numerical.Mod(x, multiplier);
                rightX = leftX + multiplier;

                topY = y - Numerical.Mod(y, multiplier);
                bottomY = topY + multiplier;

                UL = Numerical.GetRandom(leftX, topY);
                UR = Numerical.GetRandom(rightX, topY);
                DL = Numerical.GetRandom(leftX, bottomY);
                DR = Numerical.GetRandom(rightX, bottomY);

                topAverage = UL * Numerical.QuinticScale((x - leftX) / multiplier) + UR * Numerical.QuinticScale((rightX - x) / multiplier);
                bottomAverage = DL * Numerical.QuinticScale((x - leftX) / multiplier) + DR * Numerical.QuinticScale((rightX - x) / multiplier);
                average = topAverage * Numerical.QuinticScale((y - topY) / multiplier) + bottomAverage * Numerical.QuinticScale((bottomY - y) / multiplier);

                output += average * fMult;

                multiplier = multiplier >> 1; //cut a digit off
            }

            return output;
        }
    }
}
