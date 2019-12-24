using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        static int noDistinctColors = MST.FindDistinctColors();
        static double MST_Sum = MST.FindMinimumSpanningTree();
        static Dictionary<int, KeyValuePair<int, double>> MSTree = MST.GetMST();
        static Dictionary<int, RGBPixel> MSTHelper = MST.GetMSTHelper();
        public static Dictionary<int, RGBPixel> represntativeColor = new Dictionary<int, RGBPixel>(noDistinctColors);//each original color as key and value as rep color ofeach cluster

        // To be seen by DFS and DFSHelper fn
        static Stack<int> clusterColors = new Stack<int>(noDistinctColors); //for holding nodes in each cluster separately in dfs
        static Dictionary<int, bool> discovered = MST.visited; //to check in dfs if node was visited
        static Dictionary<int, List<int>> clusteredGraph = new Dictionary<int, List<int>>(noDistinctColors); //graph after removal of k - 1 edges, contains clusters
        
        public static void ProduceKClusters(int k)
        {
            List<KeyValuePair<double, int>> edges = MST.GetEdges(); //1
            //remove k - 1 edges to obtain k clusters 
            //mb2ash mwgod lma bn add f mst//+ the first edge in graph (contains root node where MST started with no destination and distance as infinity
            for (int i = k - 1; i < edges.Count; ++i) //E  = V - 1 as MST
            {
                int node1 = edges[i].Value;
                int node2 = MSTree[node1].Key;
                if (!clusteredGraph.ContainsKey(node1))//1
                {
                    clusteredGraph.Add(node1, new List<int>(noDistinctColors)); //1             
                }
                if (!clusteredGraph.ContainsKey(node2))//1
                {
                    clusteredGraph.Add(node2, new List<int>(noDistinctColors));//1
                }
                clusteredGraph[node1].Add(node2);//1
                clusteredGraph[node2].Add(node1);//1
            }
            //loop on dic that has all nodes to make sure all nodes are included in clustGraph
            //clusters that contain only one node are discovered here
            List<KeyValuePair<int, RGBPixel>> vertices = MSTHelper.ToList();
            foreach (KeyValuePair<int, RGBPixel> node in vertices)
            {
                if (!clusteredGraph.ContainsKey(node.Key))//1
                {
                    clusteredGraph.Add(node.Key, new List<int>(0));  //1  //size = zero as it forms a cluster by itself with no other nodes           
                }
            }
        }
        public static List<RGBPixel> DFS() //V + E > V
        {
            int k = 25666;
            ProduceKClusters(k);   
            List<RGBPixel> pallette = new List<RGBPixel>(k); // initialize the size by clus
            int clusterRed = 0, clusterGreen = 0, clusterBlue = 0;
            MarkAllNodesUnDiscovered();
            // traversing each cluster
            List<KeyValuePair<int, List<int>>> clusters = clusteredGraph.ToList(); //o clusters size, D
            foreach (KeyValuePair<int, List<int>> vertex in clusters) //E+V //V
            {
                if (!discovered[vertex.Key]) //o1
                {
                    clusterRed = clusterGreen = clusterBlue = 0;
                    DFSHelper(vertex.Key, ref clusterRed, ref clusterGreen, ref clusterBlue);
                    RGBPixel newColor = paletteGeneration(clusterRed, clusterGreen, clusterBlue, ref pallette);
                    FindRepresentativeColor(newColor);
                }
                else
                {
                    continue;
                }
            }
            return pallette;
        }
        static void DFSHelper(int S, ref int clusterRed, ref int clusterGreen, ref int clusterBlue)
        {
            discovered[S] = true; //1
            clusterColors.Push(S); //1

            CalculateNewClusterColor(S, ref clusterRed, ref clusterGreen, ref clusterBlue); //1
            List<int> adjNodes = clusteredGraph[S]; //children of S
            foreach (int node in adjNodes) 
            {
                if (!discovered[node])
                {
                    DFSHelper(node, ref clusterRed, ref clusterGreen, ref clusterBlue);
                }
            }         
        }
        static void CalculateNewClusterColor(int S, ref int clusterRed, ref int clusterGreen, ref int clusterBlue) //1
        {
            byte red = MSTHelper[S].red;
            byte green = MSTHelper[S].green;
            byte blue = MSTHelper[S].blue;

            //calc new color of cluster while traversing
            clusterRed += red;
            clusterGreen += green;
            clusterBlue += blue;
        }
        /// <summary>
        /// initialization of the discovered list by false
        /// </summary>
        /// <param name="S"></param>
        /// <param name="clusterRed"></param>
        /// <param name="clusterGreen"></param>
        /// <param name="clusterBlue"></param>
        static void MarkAllNodesUnDiscovered() //1
        {
            List<KeyValuePair<int, List<int>>> clusters = clusteredGraph.ToList();
            foreach (KeyValuePair<int, List<int>> node in clusters) //D
            {
                int key = node.Key;
                discovered[key] = false; //o1
            }
        }
        static void FindRepresentativeColor(RGBPixel newColor) //max D
        {
            while (clusterColors.Count != 0)
            {
                int basicColor = clusterColors.Pop(); //1
                represntativeColor.Add(basicColor, newColor); //o1            
            }
        }
        static RGBPixel paletteGeneration(int clusterRed, int clusterGreen, int clusterBlue, ref List<RGBPixel> pallette) //1
        {
            RGBPixel newColor;
            int clusterNodesCount = clusterColors.Count;  // indicates # Distinct colors in this cluster 
            newColor.red = (byte)(clusterRed / clusterNodesCount); //1
            newColor.blue = (byte)(clusterBlue / clusterNodesCount); //1
            newColor.green = (byte)(clusterGreen / clusterNodesCount); //1
            pallette.Add(newColor); //1
            return newColor; //1
        }
    }
}