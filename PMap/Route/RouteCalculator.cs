//by Tolga Birdal

using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace PMapCore.Route
{
    ///http://www.codeproject.com/Articles/24816/A-Fast-Priority-Queue-Implementation-of-the-Dijkst
    public class RouteCalculator
    {
        /// <summary>
        /// Egy node-hoz tartozó szomszédos node-ok visszaadása
        /// </summary>
        /// <param name="startingNode"></param>
        /// <returns></returns>
        public delegate List<int> GetNodeNeigbors(int startingNode);

        /// <summary>
        /// Két node közötti költség visszadása
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public delegate float GetInternodeCost(int start, int finish);

        
        /// <summary>
        /// Visszaadja, ki van-e számolva az összes kérdéses node-ra az optimalizált útvonal
        /// </summary>
        /// <returns></returns>
        public delegate bool IsComputeAllPaths(int p_computedNode);
        
        /// <summary> 
        /// Creates an instance of the <see cref="Dijkstra"/> class. 
        /// </summary> 
        /// <param name="p_totalNodeCount"> 
        /// The total number of nodes in the graph. 
        /// </param> 
        /// <param name="p_internodeCost"> 
        /// The delegate that can provide the cost of a transition between 
        /// any two nodes. 
        /// </param> 
        /// <param name="p_neigbors"> 
        /// An optional delegate that can provide a small subset of nodes 
        /// that a given node may be connected to. 
        /// </param> 
        public RouteCalculator(int p_totalNodeCount, GetInternodeCost p_internodeCost, GetNodeNeigbors p_neigbors)
        {
            if (p_totalNodeCount < 3) throw new ArgumentOutOfRangeException("totalNodeCount", p_totalNodeCount, "Expected a minimum of 3.");
            if (p_internodeCost == null) throw new ArgumentNullException("p_internodeCost");
            InternodeCost = p_internodeCost;
            Neigbors = p_neigbors;
            TotalNodeCount = p_totalNodeCount;
        }

        private  GetInternodeCost InternodeCost { get;  set; }
        private GetNodeNeigbors Neigbors { get;  set; }
        private int TotalNodeCount { get;  set; }

        private int NodeStart   { get;  set; }
        private int NodeFinish   { get;  set; }

        public class RouteCalcResult
        {
            /// <summary> 
            /// Prepares a Dijkstra results package. 
            /// </summary> 
            /// <param name="minimumPath"> 
            /// The minimum path array, where each array element index corresponds  
            /// to a node designation, and the array element value is a pointer to 
            /// the node that should be used to travel to this one. 
            /// </param> 
            /// <param name="minimumDistance"> 
            /// The minimum distance from the starting node to the given node. 
            /// </param> 
            public RouteCalcResult(int[] minimumPath, float[] minimumDistance)
            {
                MinimumDistance = minimumDistance;
                MinimumPath = minimumPath;
            }

            /// The minimum path array, where each array element index corresponds  
            /// to a node designation, and the array element value is a pointer to 
            /// the node that should be used to travel to this one. 
            public readonly int[] MinimumPath;

            /// The minimum distance from the starting node to the given node. 
            public readonly float[] MinimumDistance;
        }

        public class QueueElement : IComparable
        {
            public int index;
            public float weight;

            public QueueElement() { }
            public QueueElement(int i, float val)
            {
                index = i;
                weight = val;
            }

            public int CompareTo(object obj)
            {
                QueueElement outer = (QueueElement)obj;

                if (this.weight > outer.weight)
                    return 1;
                else if (this.weight < outer.weight)
                    return -1;
                else return 0;
            } 
        }


        public virtual int[] CalcOneOptimizedPath(int start, int finish)
        {
            NodeStart = start;
            NodeFinish = finish;
            RouteCalcResult results = CalcAllOptimizedPaths(start, IsComputeFinish);
            if (results == null)
                return new int[0];
            else
                return GetOptimizedPath(start, finish, results.MinimumPath);

        }

        private bool IsComputeFinish(int p_computedNode)
        {
            return NodeFinish > 0 && NodeFinish == p_computedNode;
        }


        // start: The node to use as a starting location. 
        // A struct containing both the minimum distance and minimum path 
        // to every node from the given <paramref name="start"/> node. 
        public virtual RouteCalcResult CalcAllOptimizedPaths(int start, IsComputeAllPaths p_isComputeAllPaths)
        {
            // Initialize the distance to every node from the starting node. 
            float[] d = GetStartingInternodeCost(start);

            // Initialize best path to every node as from the starting node. 
            int[] p = GetStartingOptimizedPath(start);
            BasicHeap Q = new BasicHeap();
            //FastHeap Q = new FastHeap(TotalNodeCount);

            //Ha az számolandó pontnak nincs szomszédja, kilépünk
            if (Neigbors(start).Count == 0)
                return null;


            for (int i = 0; i != TotalNodeCount; i++)
                Q.Push(i, d[i]);

            while (Q.Count != 0)
            {
                int v = Q.Pop();

                if (p_isComputeAllPaths( v))
                    break;

                foreach (int w in Neigbors(v))
                {
                    if (w < 0 || w > Q.Count - 1) continue;

                    float cost = InternodeCost(v, w);
                    if (cost < float.MaxValue && d[v] + cost < d[w]) // don't let wrap-around negatives slip by 
                    {
                        // We have found a better way to get at relative 
                        d[w] = d[v] + cost; // record new distance 
                        p[w] = v;
                        Q.Push(w, d[w]);
                    }
                }
            }

            return new RouteCalcResult(p, d);
        }


   

        // Finds an array of nodes that provide the shortest path 
        // from one given node to another. 
        // ShortestPath : P array of the completed algorithm:
        // The list of nodes that provide the one step at a time path from 
        public virtual int[] GetOptimizedPath(int start, int finish, int[] optimizedPath)
        {
            Stack<int> path = new Stack<int>();

            try
            {


                do
                {
                    path.Push(finish);
                    finish = optimizedPath[finish]; // step back one step toward the start point 
                }
                while (finish != start);

                return path.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new int[0];
            }
        }

        // Initializes the P array for the algorithm. 
        // A fresh P array will set every single node's source node to be  
        // the starting node, including the starting node itself. 
        protected virtual int[] GetStartingOptimizedPath(int startingNode)
        {
            int[] p = new int[TotalNodeCount];
            for (int i = 0; i < p.Length; i++)
                p[i] = startingNode;
            return p;
        }

        // Initializes the D array for the start of the algorithm.
        // The traversal cost for every node will be set to impossible 
        // (int.MaxValue) unless a connecting edge is found between the 
        // starting node and the node in question.
        protected virtual float[] GetStartingInternodeCost(int start)
        {
            float[] subset = new float[TotalNodeCount];
            for (int i = 0; i != subset.Length; i++)
                subset[i] = float.MaxValue; // all are unreachable 
            subset[start] = 0; // zero cost from start to start 
            foreach (int nearby in Neigbors(start))
                subset[nearby] = InternodeCost(start, nearby);
            return subset;
        }

    }
}