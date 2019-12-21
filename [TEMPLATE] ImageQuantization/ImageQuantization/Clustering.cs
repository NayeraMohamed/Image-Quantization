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
        public static void ProduceKClusters(int k)
        {
            List<KeyValuePair<double, string>> edges = ExtractMSTEdges();
            for(int i = 0; i < k - 1; ++i)
            {
                string source = edges[i].Value;
                MSTree.Remove(source);
            }
        }
          public static List<RGBPixel> paletteGeneration()
        {
            List<RGBPixel> pallette  = new List<RGBPixel>(); // initialize the size by K clusters 
             
            // initialization of the discovered list 
            List<KeyValuePair<string, KeyValuePair<string, double>>> clusters = MSTree.ToList();
            foreach (KeyValuePair<string, KeyValuePair<string, double>> node in clusters)
            {
                string key = node.Key;
                bool visited = false;
                discovered.Add(key, visited);
            }


            // traversing each cluster
            List<KeyValuePair<string, bool>> vertices = discovered.ToList();
            foreach (KeyValuePair<string, bool> vertex in vertices)
            {
                if (vertex.Value == false)
                {
                    clusterColors = new Stack<string>();
                    int clusterNodesCount = 0;  // indicates # Distinct colors in this cluster 
                    int clusterRed = 0;
                    int clusterGreen = 0;
                    int clusterBlue = 0;

                    DFS(vertex.Key, clusterRed, clusterGreen, clusterBlue, clusterNodesCount);
                    
                    // is  calc the average correct ??
                    RGBPixel newColor;
                    newColor.red = (byte)(clusterRed / clusterNodesCount);
                    newColor.blue = (byte)(clusterBlue / clusterNodesCount);
                    newColor.green = (byte)(clusterGreen / clusterNodesCount);
                    pallette.Add(newColor);


                    // to extract the stack values before the next loop 
                    //
                 }

                else
                    continue;
            }
            return pallette;
        }

        public static void DFS(string S, int clusterRed, int clusterGreen, int clusterBlue, int count)
        {
            discovered[S] = true;
            clusterColors.Push(S);
            count++;

            byte red = MSTHelper[S].red;
            byte green = MSTHelper[S].green;
            byte blue = MSTHelper[S].blue;

            clusterRed += red;
            clusterGreen += green;
            clusterBlue += blue;

            string adjacentNode = MSTree[S].Key;
            if (discovered[adjacentNode] == false)
            {
                DFS(adjacentNode, clusterRed, clusterGreen, clusterBlue, count);
            }

        }
    }
}
