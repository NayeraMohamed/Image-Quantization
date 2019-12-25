using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        static Dictionary<int, bool> discovered; //Checks in DFS if node is visited
        static Dictionary<int, List<int>> clusteredGraph; //Graph after removal of k-1 edges, contains clusters
        static int nDistinctColors;
        static Stack<int> clusterColors = new Stack<int>(nDistinctColors); //Holds nodes in each cluster seperatly in DFS
        //distinctHelper > Helper Dictionary for keeping struct values concatenated as int (Key) and the struct as Value
        static Dictionary<int, RGBPixel> MSTHelper; 
 
        /// <summary>
        /// Removes highest k-1 edges and generates graph with nodes that form the clusters
        /// </summary>
        /// <param name="k">Required Number of clusters</param>
        /// <param name="edges">List that contains mimimum spanning edges to be sorted</param>
        /// <param name="MSTree">distinctColors Dictionary that contains MinSpanningTree</param>
        /// <param name="maxDistinctColors">The max number of distinict colors that could occur 
        /// to intialize dictionaries and lists with</param>
        /// <param name="visited">Dictionary that checks if nodes are visited in MinSpanningTree</param>
        /// <returns>Graph after removal of k-1 edges, contains clusters</returns>
        /// Time Complexity: O(K+D) = O(D)
        public static Dictionary<int, List<int>> ProduceKClusters(int k, List<KeyValuePair<double, int>> edges, 
            Dictionary<int, KeyValuePair<int, double>> MSTree, int maxDistinctColors, Dictionary<int, bool> visited)
        {
            discovered = visited;//O(1)
            nDistinctColors = maxDistinctColors; //O(1)
            Dictionary<int, List<int>> clusteredGraph = new Dictionary<int, List<int>>(maxDistinctColors); //O(1)
            //remove k - 1 edges to obtain k clusters 
            //mb2ash mwgod lma bn add f mst
            //+ the first edge in graph (contains root node where MST started with no destination and distance as infinity
            for (int i = k - 1; i < edges.Count; ++i) //E  = V - 1 as MST //O(D)
            {
                int node1 = edges[i].Value; //O(1)
                int node2 = MSTree[node1].Key; //O(1)
                if (!clusteredGraph.ContainsKey(node1))//O(1)
                {
                    clusteredGraph.Add(node1, new List<int>(maxDistinctColors)); //O(1)     
                    discovered[node1] = false; //O(1)
                }
                if (!clusteredGraph.ContainsKey(node2)) //O(1)
                {
                    clusteredGraph.Add(node2, new List<int>(maxDistinctColors)); //O(1)
                    discovered[node2] = false; //O(1)
                }
                clusteredGraph[node1].Add(node2); //O(1)
                clusteredGraph[node2].Add(node1); //O(1)
            }
            //loop on removed edges to make sure all nodes are included in clustGraph
            //clusters that contain only one node are discovered here
            for (int i = 0; i < k; ++i) //O(K)
            {
                int node1 = edges[i].Value; //O(1)
                int node2 = MSTree[node1].Key; //O(1)
                if (!clusteredGraph.ContainsKey(node1)) //O(1)
                {
                    clusteredGraph.Add(node1, new List<int>(maxDistinctColors)); //O(1) 
                    discovered[node1] = false; //O(1)
                }
                if (!clusteredGraph.ContainsKey(node2) && node2 != 0) //O(1)
                {
                    clusteredGraph.Add(node2, new List<int>(maxDistinctColors)); //O(1)
                    discovered[node2] = false; //O(1)
                }
            }
            return clusteredGraph; //O(1)
        }
        /// <summary>
        /// Traverses the clusteredGraph to get each cluster individually and calculates its 
        /// representative color to ceate the palette
        /// </summary>
        /// <param name="clusteredGraphh">Graph after removal of k-1 edges, contains clusters</param>
        /// <param name="discovered">Checks if node is visited</param>
        /// <param name="maxDistinctColors">The max number of distinict colors that could occur 
        /// to intialize dictionaries and lists with</param>
        /// <param name="k">Required number of clusters</param>
        /// <param name="represntativeColor">Dictionary that carries every distinct node with its new color</param>
        /// <param name="distinctHelper">Helper Dictionary for keeping struct values concatenated as int (Key)
        /// and the struct as Value</param>
        /// <returns>The palette</returns>
        /// Time Complexity: E+V =V = O(D)
        public static List<RGBPixel> DFS(Dictionary<int, List<int>> clusteredGraphh, Dictionary<int, bool> discovered,
            int maxDistinctColors, int k, ref Dictionary<int, RGBPixel> represntativeColor,
            Dictionary<int, RGBPixel> distinctHelper)
        {
            clusteredGraph = clusteredGraphh; //O(1)
            MSTHelper = distinctHelper; //O(1)
            List<RGBPixel> pallette = new List<RGBPixel>(k); // initialize the size by clus //O(1)
            int clusterRed = 0, clusterGreen = 0, clusterBlue = 0; //O(1)
            // traversing each cluster
            List<KeyValuePair<int, List<int>>> clusters = clusteredGraph.ToList(); //o clusters size, //O(D)
            foreach (KeyValuePair<int, List<int>> vertex in clusters) //E+V //V
            {
                if (!discovered[vertex.Key]) //o1
                {
                    clusterRed = clusterGreen = clusterBlue = 0; //O(1)
                    DFSHelper(vertex.Key, ref clusterRed, ref clusterGreen, ref clusterBlue); //O(adj(vertex.key))
                    int clusterNodesCount = clusterColors.Count; //O(1)
                    RGBPixel newColor = paletteGeneration(clusterRed, clusterGreen, clusterBlue, ref pallette, clusterNodesCount); //O(1)
                    FindRepresentativeColor(newColor, ref represntativeColor); //O(1)
                }
                else
                {
                    continue; //O(1)
                }
            }
            return pallette; //O(1)
        }
        /// <summary>
        /// Traverses the adjacent nodes of each node
        /// </summary>
        /// <param name="S">The key node to traverse its adjacent nodes</param>
        /// <param name="clusterRed">The summation of red color intensity</param>
        /// <param name="clusterGreen">The summation of green color intensity</param>
        /// <param name="clusterBlue">The summation of blue color intensity</param>
        static void DFSHelper(int S, ref int clusterRed, ref int clusterGreen, ref int clusterBlue)
        {
            discovered[S] = true; //O(1)
            clusterColors.Push(S); //O(1)

            CalculateNewClusterColor(S, ref clusterRed, ref clusterGreen, ref clusterBlue); //O(1)
            List<int> adjNodes = clusteredGraph[S]; //children of S //O(1)
            foreach (int node in adjNodes) //O(adj(S))
            {
                if (!discovered[node]) //O(1)
                {
                    DFSHelper(node, ref clusterRed, ref clusterGreen, ref clusterBlue); 
                }
            }         
        }
        /// <summary>
        /// Adds the intensity of red, blue and green of each new node in the cluster
        /// </summary>
        /// <param name="S">The key node which sums its red,blue, and green intensties</param>
        /// <param name="clusterRed">The summation of red color intensity</param>
        /// <param name="clusterGreen">The summation of green color intensity</param>
        /// <param name="clusterBlue">The summation of blue color intensity</param>
        /// Time Complexity O(1)
        static void CalculateNewClusterColor(int S, ref int clusterRed, ref int clusterGreen, ref int clusterBlue) 
        {
            byte red = MSTHelper[S].red; //O(1)
            byte green = MSTHelper[S].green; //O(1)
            byte blue = MSTHelper[S].blue; //O(1)

            //calc new color of cluster while traversing
            clusterRed += red; //O(1)
            clusterGreen += green; //O(1)
            clusterBlue += blue; //O(1)
        }
        /// <summary>
        /// Constructs the represntativeColor dictionary
        /// </summary>
        /// <param name="newColor">The Cluster representative color</param>
        /// <param name="represntativeColor">Dictionary that has each original color as key 
        /// and rep color of each cluster as value</param>
        /// Time Complexity: O(D)
        static void FindRepresentativeColor(RGBPixel newColor, ref Dictionary<int, RGBPixel> represntativeColor) //max D
        {
            while (clusterColors.Count != 0) //O(stackSize) -> max O(D)
            {
                int basicColor = clusterColors.Pop(); //O(1)
                represntativeColor.Add(basicColor, newColor); //O(1)           
            }
        }
        /// <summary>
        /// Calculates the representative color of each cluster
        /// </summary>
        /// <param name="clusterRed">Summation of all red intensties in the cluster</param>
        /// <param name="clusterGreen">Summation of all green intensties in the cluster</param>
        /// <param name="clusterBlue">Summation of all blue intensties in the cluster</param>
        /// <param name="pallette">List of all the representative colors</param>
        /// <param name="clusterNodesCount">The number of nodes in the cluster</param>
        /// <returns>The palette</returns>
        /// Time Complexity: O(1)
        static RGBPixel paletteGeneration(int clusterRed, int clusterGreen, int clusterBlue, ref List<RGBPixel> pallette, 
            int clusterNodesCount) 
        {
            RGBPixel newColor; //O(1)
            newColor.red = (byte)(clusterRed / clusterNodesCount); //O(1)
            newColor.blue = (byte)(clusterBlue / clusterNodesCount); //O(1)
            newColor.green = (byte)(clusterGreen / clusterNodesCount); //O(1)
            pallette.Add(newColor); //O(1)
            return newColor; //O(1)
        }
    }
}