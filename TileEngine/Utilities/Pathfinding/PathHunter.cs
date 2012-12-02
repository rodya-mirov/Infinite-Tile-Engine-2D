using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TileEngine.Utilities.DataStructures;

namespace TileEngine.Utilities.Pathfinding
{
    public class PathHunter
    {
        /// <summary>
        /// Just making it so you can't instantiate these things I guess
        /// </summary>
        protected PathHunter()
        {
        }

        /// <summary>
        /// This constructs a path from the specified start point to
        /// some point in the set of destinations.
        /// 
        /// Will only construct paths up to a certain maximum COST;
        /// as of now, going from one square to another costs 1 unit.
        /// So the maximum cost is the maximum path length.
        /// 
        /// Returns null if no path was found.
        /// 
        /// Running time (if no path is found, the worst case) is O(k+n^2), where
        ///        n=maxCost
        ///        k=destinations.Count
        /// 
        /// Assumes no negative cost paths, as in Djikstra's algorithm
        /// </summary>
        /// <param name="startPoint">The point we start from.</param>
        /// <param name="goalPoints">The set of possible destinations we could be happy with.</param>
        /// <param name="maxCost">Maximum cost of any path.</param>
        /// <param name="manager">The WorldManager that we use to do our pathing.</param>
        /// <param name="startTime">The time we start the algorithm.</param>
        /// <returns>The shortest path from here to somewhere in there; returns null iff there is no path of cost less than maxCost.</returns>
        public static Path GetPath<T, S, M>(Point startPoint, HashSet<Point> goalPoints, int maxCost, TileMapManager<T, S, M> manager, GameTime startTime)
            where T : InGameObject
            where S : MapCell, Translatable<S>
            where M : TileMap<S>, new()
        {
            return PathHunter.GetPath<T, S, M>(startPoint, goalPoints, maxCost, manager, startTime.TotalGameTime);
        }

        /// <summary>
        /// This constructs a path from the specified start point to
        /// some point in the set of destinations.
        /// 
        /// Will only construct paths up to a certain maximum COST;
        /// as of now, going from one square to another costs 1 unit.
        /// So the maximum cost is the maximum path length.
        /// 
        /// Returns null if no path was found.
        /// 
        /// Running time (if no path is found, the worst case) is O(k+n^2), where
        ///        n=maxCost
        ///        k=destinations.Count
        /// 
        /// Assumes no negative cost paths, as in Djikstra's algorithm
        /// </summary>
        /// <param name="startPoint">The point we start from.</param>
        /// <param name="goalPoints">The set of possible destinations we could be happy with.</param>
        /// <param name="maxCost">Maximum cost of any path.</param>
        /// <param name="manager">The WorldManager that we use to do our pathing.</param>
        /// <param name="startTime">The time we start the algorithm.</param>
        /// <returns>The shortest path from here to somewhere in there; returns null iff there is no path of cost less than maxCost.</returns>
        public static Path GetPath<T, S, M>(Point startPoint, HashSet<Point> goalPoints, int maxCost, TileMapManager<T, S, M> manager, TimeSpan startTime)
            where T : InGameObject
            where S : MapCell, Translatable<S>
            where M : TileMap<S>, new()
        {
            //check for trivialities- we can't find a path to nowhere
            if (goalPoints.Count() == 0)
                return null;

            // ... and we don't need to search if we're already there
            if (goalPoints.Contains(startPoint))
                return new Path(startPoint);

            //now set up a list of points we've seen before, so as to not
            //check the same position a billion times
            Dictionary<Point, int> bestDistancesFound = new Dictionary<Point, int>();
            bestDistancesFound[startPoint] = 0;

            Heap<Path> heap = null;

            if (goalPoints.Count() == 1)
            {
                //looks funny, but as an unindexed collection, I don't know a better way
                foreach (Point p in goalPoints)
                    heap = new PathToPointHeap(p);
            }
            else
            {
                heap = new PathHeap();
            }

            heap.Add(new Path(startPoint));

            while (heap.Count > 0)
            {
                //if there's new passability information, start the process over
                if (manager.LastGeneralPassabilityUpdate != null && //if it's null, we're definitely OK
                    startTime < manager.LastGeneralPassabilityUpdate)
                {
                    startTime = manager.LastGeneralPassabilityUpdate;

                    heap.Clear();
                    heap.Add(new Path(startPoint));

                    bestDistancesFound = new Dictionary<Point, int>();
                    bestDistancesFound[startPoint] = 0;
                }

                Path bestPath = heap.Pop();

                //if we didn't cap out our path length yet,
                //check out the adjacent points to form longer paths
                if (bestPath.Cost < maxCost)
                {
                    int newCost = bestPath.Cost + 1;

                    //for each possible extension ...
                    foreach (Point adj in manager.GetAdjacentPoints(bestPath.End.X, bestPath.End.Y))
                    {
                        //if we hit a destination, great, stop
                        if (goalPoints.Contains(adj))
                            return new Path(bestPath, adj, newCost);

                        //don't bother adding this possible extension back on unless
                        //it's either a completely new point or a strict improvement over another path
                        if (bestDistancesFound.Keys.Contains(adj) && bestDistancesFound[adj] <= newCost)
                            continue;

                        //otherwise, this is a perfectly serviceable path extension and we should look into it
                        bestDistancesFound[adj] = newCost;

                        heap.Add(new Path(bestPath, adj, newCost));
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Just a Heap of Path objects, where better means exclusively lower cost.
    /// </summary>
    class PathHeap : Heap<Path>
    {
        public override bool isBetter(Path a, Path b)
        {
            return a.Cost < b.Cost;
        }
    }

    /// <summary>
    /// Like the PathHeap, but with a direction, so that we can estimate the cost
    /// of a path to be the cost so far, plus the Manhattan distance from the end
    /// of our path to the goal point.
    /// </summary>
    class PathToPointHeap : Heap<Path>
    {
        private int startX, startY;

        public PathToPointHeap(Point start)
            : base()
        {
            this.startX = start.X;
            this.startY = start.Y;
        }

        public override bool isBetter(Path a, Path b)
        {
            int ca = a.Cost + Math.Abs(a.End.X - startX) + Math.Abs(a.End.Y - startY);
            int cb = b.Cost + Math.Abs(b.End.X - startX) + Math.Abs(b.End.Y - startY);

            return ca < cb;
        }
    }
}
