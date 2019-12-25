using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        static Dictionary<int, bool> discovered;
        static Dictionary<int, List<int>> clusteredGraph;
        static int nDistinctColors;
        static Stack<int> clusterColors = new Stack<int>(nDistinctColors);
        static Dictionary<int, RGBPixel> MSTHelper;  
        public static Dictionary<int, List<int>> ProduceKClusters(int k, List<KeyValuePair<double, int>> edges, Dictionary<int, KeyValuePair<int, double>> MSTree, int maxDistinctColors, Dictionary<int, bool> visited)
        {
            discovered = visited;
            nDistinctColors = maxDistinctColors;
            Dictionary<int, List<int>> clusteredGraph = new Dictionary<int, List<int>>(maxDistinctColors);
            //remove k - 1 edges to obtain k clusters 
            //mb2ash mwgod lma bn add f mst//+ the first edge in graph (contains root node where MST started with no destination and distance as infinity
            for (int i = k - 1; i < edges.Count; ++i) //E  = V - 1 as MST
            {
                int node1 = edges[i].Value;
                int node2 = MSTree[node1].Key;
                if (!clusteredGraph.ContainsKey(node1))//1
                {
                    clusteredGraph.Add(node1, new List<int>(maxDistinctColors)); //1      
                    discovered[node1] = false;
                }
                if (!clusteredGraph.ContainsKey(node2))//1
                {
                    clusteredGraph.Add(node2, new List<int>(maxDistinctColors));//1
                    discovered[node2] = false;
                }
                clusteredGraph[node1].Add(node2);//1
                clusteredGraph[node2].Add(node1);//1
            }
            //loop on removed edges to make sure all nodes are included in clustGraph
            //clusters that contain only one node are discovered here
            for (int i = 0; i < k; ++i) //K
            {
                int node1 = edges[i].Value;
                int node2 = MSTree[node1].Key;
                if (!clusteredGraph.ContainsKey(node1))//1
                {
                    clusteredGraph.Add(node1, new List<int>(maxDistinctColors)); //1 
                    discovered[node1] = false;
                }
                if (!clusteredGraph.ContainsKey(node2) && node2 != 0)//1
                {
                    clusteredGraph.Add(node2, new List<int>(maxDistinctColors));//1
                    discovered[node2] = false;
                }
            }
            return clusteredGraph;
        }
        public static List<RGBPixel> DFS(Dictionary<int, List<int>> clusteredGraphh, Dictionary<int, bool> discovered, int maxDistinctColors, int k, ref Dictionary<int, RGBPixel> represntativeColor, Dictionary<int, RGBPixel> distinctHelper) //V + E > V
        {
            clusteredGraph = clusteredGraphh;
            MSTHelper = distinctHelper;
            List<RGBPixel> pallette = new List<RGBPixel>(k); // initialize the size by clus
            int clusterRed = 0, clusterGreen = 0, clusterBlue = 0;
            // traversing each cluster
            List<KeyValuePair<int, List<int>>> clusters = clusteredGraph.ToList(); //o clusters size, D
            foreach (KeyValuePair<int, List<int>> vertex in clusters) //E+V //V
            {
                if (!discovered[vertex.Key]) //o1
                {
                    clusterRed = clusterGreen = clusterBlue = 0;
                    DFSHelper(vertex.Key, ref clusterRed, ref clusterGreen, ref clusterBlue);
                    int clusterNodesCount = clusterColors.Count;
                    RGBPixel newColor = paletteGeneration(clusterRed, clusterGreen, clusterBlue, ref pallette, clusterNodesCount);
                    FindRepresentativeColor(newColor, ref represntativeColor);
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
        static void FindRepresentativeColor(RGBPixel newColor, ref Dictionary<int, RGBPixel> represntativeColor) //max D
        {
            while (clusterColors.Count != 0)
            {
                int basicColor = clusterColors.Pop(); //1
                represntativeColor.Add(basicColor, newColor); //o1            
            }
        }
        static RGBPixel paletteGeneration(int clusterRed, int clusterGreen, int clusterBlue, ref List<RGBPixel> pallette, int clusterNodesCount) //1
        {
            RGBPixel newColor; 
            newColor.red = (byte)(clusterRed / clusterNodesCount); //1
            newColor.blue = (byte)(clusterBlue / clusterNodesCount); //1
            newColor.green = (byte)(clusterGreen / clusterNodesCount); //1
            pallette.Add(newColor); //1
            return newColor; //1
        }
    }
}