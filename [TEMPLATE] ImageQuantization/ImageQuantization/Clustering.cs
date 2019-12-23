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
        public static Dictionary<string, RGBPixel> represntativeColor = new Dictionary<string, RGBPixel>(noDistinctColors);//init later

        // To be seen by DFS fn && pelletteGeneration fn
        static Stack<string> clusterColors = new Stack<string>(noDistinctColors);
        static Dictionary<string, bool> discovered = new Dictionary<string, bool>(noDistinctColors);
        //agdd fkra
        static Dictionary<string, List<string>> clusteredGraph = new Dictionary<string, List<string>>(noDistinctColors);
        static int DescendingOrder(KeyValuePair<double, string> node1, KeyValuePair<double, string> node2)
        {
            return node2.Key.CompareTo(node1.Key); //for descending order
        }
        static List<KeyValuePair<double, string>> ExtractMSTEdges()
        {
            List<KeyValuePair<double, string>> edges = new List<KeyValuePair<double, string>>(noDistinctColors); //need to che k
            List<KeyValuePair<string, KeyValuePair<string, double>>> vertices = MSTree.ToList(); //D
            //for loop approaches an O(V) operation, V is the number of vertices
            foreach (KeyValuePair<string, KeyValuePair<string, double>> node in vertices) //D
            {
                double distance = node.Value.Value;
                String source = node.Key;
                KeyValuePair<double, string> edge = new KeyValuePair<double, string>(distance, source);
                edges.Add(edge);//1
            }
            edges.Sort(DescendingOrder);//V lg V
            return edges;
        }
        public static int ProduceKClusters(int k)
        {
            List<KeyValuePair<double, string>> edges = ExtractMSTEdges();
            //ConvertDirectedMSTtoUndirected(edges);
            for (int i = k - 1; i < edges.Count; ++i) //k*D///kda kolo d
            {
                string node1 = edges[i].Value;
                string node2 = MSTree[node1].Key;
                if (!clusteredGraph.ContainsKey(node1))//1
                {
                    clusteredGraph.Add(node1, new List<string>(noDistinctColors));     //1             
                }
                if (!clusteredGraph.ContainsKey(node2))//1
                {
                    clusteredGraph.Add(node2, new List<string>(noDistinctColors));//1
                }
                clusteredGraph[node1].Add(node2);//1
                clusteredGraph[node2].Add(node1);//1
            }
            List<KeyValuePair<string, RGBPixel>> vertices = MSTHelper.ToList();
            foreach(KeyValuePair<string, RGBPixel> node in vertices)
            {
                if (!clusteredGraph.ContainsKey(node.Key))//1
                {
                    clusteredGraph.Add(node.Key, new List<string>(0));     //1             
                }
            }
            return clusteredGraph.Count;
        }
        public static List<RGBPixel> paletteGeneration()
        {
            int k = 2284;
            int noClusters = ProduceKClusters(k);   
            List<RGBPixel> pallette = new List<RGBPixel>(k); // initialize the size by clus

            // initialization of the discovered list by false
            List<KeyValuePair<string, List<string>>> clusters = clusteredGraph.ToList(); //o clusters size, D
            foreach (KeyValuePair<string, List<string>> node in clusters) //D
            {
                string key = node.Key;
                discovered.Add(key, false); //o1
                ///represntativeColor.Add(key, new RGBPixel()); //o1
            }
            // traversing each cluster
            foreach (KeyValuePair<string, List<string>> vertex in clusters) //E+V //V
            {
                if (!discovered[vertex.Key]) //o1
                {
                    //clusterColors = new Stack<string>(noDistinctColors);//init

                    int clusterRed = 0;
                    int clusterGreen = 0;
                    int clusterBlue = 0;

                    DFS(vertex.Key, ref clusterRed, ref clusterGreen, ref clusterBlue); 

                    //  the average ??
                    RGBPixel newColor;
                    int clusterNodesCount = clusterColors.Count;  // indicates # Distinct colors in this cluster 
                    newColor.red = (byte)(clusterRed / clusterNodesCount);
                    newColor.blue = (byte)(clusterBlue / clusterNodesCount);
                    newColor.green = (byte)(clusterGreen / clusterNodesCount);
                    pallette.Add(newColor); //1


                    // to extract the stack values before the next loop 
                    while (clusterColors.Count != 0)//anaa //d max
                    {
                        string basicColor = clusterColors.Pop(); //1
                        represntativeColor.Add(basicColor, newColor); //o1        //1              
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

            List<string>adjNodes = clusteredGraph[S];
            foreach (string node in adjNodes)
            {
                if (!discovered[node])
                {
                    DFS(node, ref clusterRed, ref clusterGreen, ref clusterBlue);
                }
            }
            
        }
    }
}