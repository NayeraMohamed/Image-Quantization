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
        static Dictionary<string, KeyValuePair<string, double>> MSTree = MST.GetMST();
        static Dictionary<string, RGBPixel> MSTHelper = MST.GetMSTHelper();
        public static Dictionary<string, RGBPixel> represntativeColor = new Dictionary<string, RGBPixel>();//init later

        // To be seen by DFS fn && pelletteGeneration fn
        static Stack<string> clusterColors;
        static Dictionary<string, bool> discovered = new Dictionary<string, bool>(noDistinctColors);

        static int DescendingOrder(KeyValuePair<double, string> node1, KeyValuePair<double, string> node2)
        {
            return node2.Key.CompareTo(node1.Key); //for descending order
        }
        static List<KeyValuePair<double, string>> ExtractMSTEdges()
        {
            List<KeyValuePair<double, string>> edges = new List<KeyValuePair<double, string>>(MSTree.Count); //need to che k
            List<KeyValuePair<string, KeyValuePair<string, double>>> vertices = MSTree.ToList();
            //for loop approaches an O(V) operation, V is the number of vertices
            foreach (KeyValuePair<string, KeyValuePair<string, double>> node in vertices)
            {
                double distance = node.Value.Value;
                String source = node.Key;
                KeyValuePair<double, string> edge = new KeyValuePair<double, string>(distance, source);
                edges.Add(edge);
            }
            edges.Sort(DescendingOrder);
            return edges;
        }
        public static int ProduceKClusters(int k)
        {
            List<KeyValuePair<double, string>> edges = ExtractMSTEdges();
            for (int i = 0; i < k; ++i)
            {
                string source = edges[i].Value;
                double x = edges[i].Key;
                MSTree.Remove(source);
            }
            return MSTree.Count();
        }
        public static List<RGBPixel> paletteGeneration()
        { 
            int noClusters = ProduceKClusters(1);
            List<RGBPixel> pallette = new List<RGBPixel>(noClusters); // initialize the size by K clusters 

            // initialization of the discovered list by false
            List<KeyValuePair<string, RGBPixel>> clusters = MSTHelper.ToList();
            foreach (KeyValuePair<string, RGBPixel> node in clusters)
            {
                string key = node.Key;
                bool visited = false;
                discovered.Add(key, visited);
                represntativeColor.Add(key, node.Value);
            }


            // traversing each cluster
            List<KeyValuePair<string, KeyValuePair<string, double>>> vertices = MSTree.ToList();
            foreach (KeyValuePair<string, KeyValuePair<string, double>> vertex in vertices)
            {
                if (discovered[vertex.Key] == false)
                {
                    clusterColors = new Stack<string>();//init

                    int clusterRed = 0;
                    int clusterGreen = 0;
                    int clusterBlue = 0;

                    DFS(vertex.Key, ref clusterRed, ref clusterGreen, ref clusterBlue);

                    //  the average ??
                    RGBPixel newColor = new RGBPixel();
                    int clusterNodesCount = clusterColors.Count;  // indicates # Distinct colors in this cluster 
                    newColor.red = (byte)(clusterRed / clusterNodesCount);
                    newColor.blue = (byte)(clusterBlue / clusterNodesCount);
                    newColor.green = (byte)(clusterGreen / clusterNodesCount);
                    pallette.Add(newColor);


                    // to extract the stack values before the next loop 
                    while (clusterColors.Count != 0)//anaa
                    {
                        string basicColor = clusterColors.Pop();
                        
                        represntativeColor[basicColor] = newColor;
                        
                    }
                }

                else
                {
                    continue;
                }
            }

            return pallette;
        }

        static void DFS(string S, ref int clusterRed, ref int clusterGreen, ref int clusterBlue)
        {
            discovered[S] = true;
            clusterColors.Push(S);


            byte red = MSTHelper[S].red;
            byte green = MSTHelper[S].green;
            byte blue = MSTHelper[S].blue;

            clusterRed += red;
            clusterGreen += green;
            clusterBlue += blue;

            if(MSTree.ContainsKey(S))
            {
                string adjacentNode = MSTree[S].Key;
                if (discovered[adjacentNode] == false)
                {
                    DFS(adjacentNode, ref clusterRed, ref clusterGreen, ref clusterBlue);
                }
            }
            
        }
    }
}