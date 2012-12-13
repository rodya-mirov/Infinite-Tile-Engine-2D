using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TileEngine.Utilities.DataStructures
{
    /// <summary>
    /// Maintains a "sorted" list, where it's efficient to add something in,
    /// but actually it's only sorting them a little bit.
    /// 
    /// Strictly speaking, this is a binary heap, with all that entails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Heap<T> : IEnumerable<T>
    {
        /// <summary>
        /// Underlying array.
        /// 
        /// Pre/postcondition for this array: For all n, baseList[n] > max(baseList[2n+1], baseList[2n+2])
        ///     this makes it a binary tree, where the children of the node at n are 2n+1 and 2n+2
        /// 
        /// Performance of a binary heap:
        ///     add(T item):    O(log(n))
        ///     peek():         O(1)
        ///     pop():          O(log(n))
        /// </summary>
        protected List<T> baseList;

        public Heap(int capacity = 50)
        {
            baseList = new List<T>(capacity);
        }

        public int Count
        {
            get { return baseList.Count; }
        }

        /// <summary>
        /// Returns the top element of the heap, but does not
        /// change anything about the heap.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return baseList[0];
        }

        /// <summary>
        /// Returns the top element of the heap and removes
        /// it from the heap.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T output = baseList[0];

            baseList[0] = baseList[baseList.Count - 1];
            reheapify();

            baseList.RemoveAt(baseList.Count - 1);
            return output;
        }

        /// <summary>
        /// Assuming everything EXCEPT the top satisfies the heap
        /// property, re-heapify so that everything satisfies the
        /// heap property.
        /// </summary>
        /// <returns></returns>
        protected int reheapify()
        {
            int n = 0;
            int count = baseList.Count;

            T trickleDown = baseList[0];

            //so long as the current node has children, trickle down
            //until it's a working heap again
            while (2 * n + 1 < count)
            {
                //we know left is a well-defined child, or else we would have stopped
                int left = 2 * n + 1;

                //but right may not be
                int right = 2 * n + 2;

                int biggestChild = left;
                if (right < count && isBetter(baseList[right], baseList[left]))
                    biggestChild = right;

                if (isBetter(baseList[biggestChild], trickleDown))
                {
                    baseList[n] = baseList[biggestChild];
                    n = biggestChild;
                }
                else
                    break;
            }

            baseList[n] = trickleDown;
            return n;
        }

        /// <summary>
        /// Puts the supplied item somewhere in the list.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int n = baseList.Count;

            baseList.Add(item);

            while (n > 0)
            {
                int prev = (n - 1 - ((n - 1) % 2)) / 2; //the predecessor index
                if (isBetter(item, baseList[prev]))
                {
                    baseList[n] = baseList[prev];
                    n = prev;
                }
                else
                    break;
            }

            baseList[n] = item;
        }

        /// <summary>
        /// Returns true if and only if a is (strictly) better than b.
        /// 
        /// If you want this to be a min-heap, better means smaller.
        /// If you want this to be a max-heap, better means bigger.
        /// 
        /// If you want the prettiest pony to be at the top of the heap,
        /// then better means prettier.  Go forth and implement.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public abstract bool isBetter(T a, T b);

        /// <summary>
        /// Clears the heap completely.
        /// </summary>
        public void Clear()
        {
            baseList.Clear();
        }

        /// <summary>
        /// Yields the elements of the Heap in no particular
        /// order.  In particular, is not guaranteed to give
        /// the best element first!
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T t in baseList)
                yield return t;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
