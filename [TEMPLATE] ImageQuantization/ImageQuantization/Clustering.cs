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
        static List<KeyValuePair<double, string>> edges = new List<KeyValuePair<double, string>>(MSTree.Count); //need to che k

        static int DescendingOrder(KeyValuePair<double, string> node1, KeyValuePair<double, string> node2)
        {
            return node2.Key.CompareTo(node1.Key); //for descending order
        }
        static void ExtractMSTEdges()
        {
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
        }
        public static void ProduceKClusters(int k)
        {
            ExtractMSTEdges();
            for(int i = 0; i < k - 1; ++i)
            {
                string source = edges[i].Value;
                MSTree.Remove(source);
            }
        }
    }
}