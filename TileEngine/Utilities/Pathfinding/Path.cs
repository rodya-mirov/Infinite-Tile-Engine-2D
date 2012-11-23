using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine.Utilities.Pathfinding
{
    public class Path
    {
        private PathNode pointsTraveled;

        /// <summary>
        /// This is an ordered list of points traveled, where the head is the
        /// first (start) point, the tail is the last (end) point, and there is
        /// an assumption that it is legal to travel between point I and point I+1
        /// </summary>
        public IEnumerable<Point> PointsTraveled()
        {
            foreach (Point p in pointsTraveled.GetPointsInPath())
                yield return p;
        }

        /// <summary>
        /// The start point of the path
        /// </summary>
        public Point Start
        {
            get { return pointsTraveled.startPoint; }
        }

        /// <summary>
        /// The end point of the path
        /// </summary>
        public Point End
        {
            get { return pointsTraveled.endPoint; }
        }

        /// <summary>
        /// The "cost" of the path.  Whatever this represents, we want it to be low.
        /// </summary>
        public int Cost
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new path from a start point and cost.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="baseCost"></param>
        public Path(Point startPoint, int baseCost = 0)
        {
            pointsTraveled = new PathNode(startPoint);

            Cost = baseCost;
        }

        /// <summary>
        /// Constructs a new path, extending the old path
        /// by adding one point and the cost of traveling
        /// from the end of the old path to this point.
        /// </summary>
        /// <param name="startPath"></param>
        /// <param name="nextPoint"></param>
        /// <param name="newCost"></param>
        public Path(Path startPath, Point nextPoint, int newCost)
        {
            pointsTraveled = new PathNode(startPath.pointsTraveled, nextPoint);

            Cost = newCost;
        }

        private class PathNode
        {
            public PathNode previousPath;
            public Point endPoint;
            public Point startPoint;

            public PathNode(Point singlePoint)
            {
                this.previousPath = null;
                this.endPoint = singlePoint;
                this.startPoint = singlePoint;
            }

            public PathNode(PathNode previousPath, Point endPoint)
            {
                this.previousPath = previousPath;
                this.endPoint = endPoint;
                this.startPoint = previousPath.startPoint;
            }

            public IEnumerable<Point> GetPointsInPath()
            {
                if (previousPath != null)
                {
                    foreach (Point p in previousPath.GetPointsInPath())
                        yield return p;
                }

                yield return endPoint;
            }
        }
    }
}
