using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class MST
    {
        static RGBPixel[,] Buffer = ImageOperations.GetImage(); //matrix that contains image pixels
        static int imageHeight = Buffer.GetLength(0);
        static int imageWidth = Buffer.GetLength(1);

        static int maxDistinctNum = imageHeight * imageWidth; //max no of distinct colors that can be found in matrix
        /*All Dictionaries initialized with maxDistinctNum as it is the max no of elements that can be added 
        to the Dictionary (Dictionary capacity) to avoid O(N) complexity when adding new elements*/
        //distinctColors > Dictionary that contains MinSpanningTree
        static Dictionary<int, KeyValuePair<int, double>> distinctColors = new Dictionary<int, KeyValuePair<int, double>>(maxDistinctNum);
        //distinctHelper > Helper Dictionary for keeping struct values concatenated as string (Key) to increase hashFunction operations
        static Dictionary<int, RGBPixel> distinctHelper = new Dictionary<int, RGBPixel>(maxDistinctNum);
        //visited > Dictionary to check visited nodes in MinSpnningTree
        public static Dictionary<int, bool> visited = new Dictionary<int, bool>(maxDistinctNum);

        //
        static KeyValuePair<int, KeyValuePair<int, double>> minVertix;
        static int color;
        static List<KeyValuePair<double, int>> edges = new List<KeyValuePair<double, int>>(noDistinctColors); // all edges in graph
        //
        static int noDistinctColors = 0; //actual no of distinct colors, calculated in GetDistinctColors() function
        static double MST_Sum = 0;

        /// <summary>
        /// Gets all DistinctColors from 2D array and fills Dictionary of MST with all of them
        /// </summary>
        /// <returns>number of DistinctColors</returns>
        /// Time Complexity: O(N^2), N is the image width (or height)
        /// Space Complexity: N, N is the image width (or height)
        public static int FindDistinctColors()
        {
            //Outer for loop approaches an O(N) operation, N is the image height
            for (int i = 0; i < imageHeight; ++i)
            {
                //Inner for loop approaches an O(N) operation, N is the image width
                for (int j = 0; j < imageWidth; ++j)
                {
                    RGBPixel node = Buffer[i, j]; //O(1)
                    //gets unique int for each color to be used as dictionary key by using Cantor Pairing
                    color = CantorPairing(node.red, node.green, node.blue);
                    if (!distinctHelper.ContainsKey(color)) //ContainsKey() approaches an O(1) operation
                    {
                        //Add() approaches an O(1) operation if Count is less than the capacity,
                        //guaranteed to run in O(1) as map is initialized with max capacity
                        distinctColors.Add(color, new KeyValuePair<int, double>(0, double.MaxValue));  //node initialized with max distance
                        distinctHelper.Add(color, node);
                        visited.Add(color, false); //node initially not visited

                        noDistinctColors++; //O(1) 
                    }
                }
            }
            //to avoid using Dic.First(), Vertex to start MST from is generated here
            minVertix = new KeyValuePair<int, KeyValuePair<int, double>>(color, new KeyValuePair<int, double>(0, double.MaxValue));
            return noDistinctColors; //O(1)
        }

        /// <summary>
        /// Calculates distances between all vertices and each other, 
        /// and finds Minimum Spanning Tree using Prim's Algorithm
        /// </summary>
        /// <returns>Sum of MST</returns>
        /// Time Complexity: O(V^2) = O(E), V is the no of vertices, 
        /// E is the no of Edges in graph, approaches V^2 as it is a dense graph
        public static double FindMinimumSpanningTree()
        {
            //
            int v; 
            double minEdge;
            KeyValuePair<int, double> edge = new KeyValuePair<int, double>();
            //
            //the minimum node by which we start calculating min from,
            //initialized with source vertix where we start MST from
            //minVertix = distinctColors.First(); //O(1)

            //currentMin >  to get the minimum vertix at each iteration
            KeyValuePair<int, KeyValuePair<int, double>> currentMin = new KeyValuePair<int, KeyValuePair<int, double>>();
            RGBPixel nodeColor, minVertexColor;
            //Outer for loop approaches an O(V) operation, V is the number of vertices,
            //each iteration we relax a vertix
            for (int i = 0; i < distinctColors.Count - 1; i++)
            {
                v = minVertix.Key;
                visited[v] = true;
                minEdge = double.MaxValue;

                //ToList() > Converts distinctColors Dictionary to List to be able to iterate over it,
                //approaches an O(V) operation, V is the number of vertices
                List<KeyValuePair<int, KeyValuePair<int, double>>> vertices = distinctColors.ToList();

                //Inner for loop approaches an O(V) operation, V is the number of vertices
                foreach (KeyValuePair<int, KeyValuePair<int, double>> node in vertices)
                {
                    nodeColor = distinctHelper[node.Key];
                    minVertexColor = distinctHelper[minVertix.Key];
                    //check whether the distance must be calculated or not
                    //continue if node is already visited or it is the same node where the min distance is calc relative to it
                    if (((nodeColor.red == minVertexColor.red) &&
                        (nodeColor.blue == minVertexColor.blue) &&
                        (nodeColor.green == minVertexColor.green))
                        || visited[node.Key])
                    {
                        continue;
                    }
                    //Sqrt() approaches O(1) operation
                    double distance = Math.Sqrt((minVertexColor.red - nodeColor.red) * (minVertexColor.red - nodeColor.red)
                                              + (minVertexColor.blue - nodeColor.blue) * (minVertexColor.blue - nodeColor.blue)
                                              + (minVertexColor.green - nodeColor.green) * (minVertexColor.green - nodeColor.green));
                    //
                    edge = distinctColors[node.Key];
                    //
                    //update distance value if less distance is calculated
                    if (edge.Value > distance)
                    {
                        distinctColors[node.Key] = new KeyValuePair<int, double>(minVertix.Key, distance);
                    }
                    //finding current min in inner loop relative to chosen vertex in outer loop
                    if (distinctColors[node.Key].Value < minEdge)
                    {
                        minEdge = distinctColors[node.Key].Value;
                        currentMin = node;
                    }
                }
                minVertix = currentMin;
                KeyValuePair<double, int> MSTEdge = new KeyValuePair<double, int>(minEdge, minVertix.Key);//1
                edges.Add(MSTEdge);//1
                MST_Sum += minEdge;
            }
            edges.Sort(DescendingOrder); //V lg V
            return MST_Sum; //O(1)
        }
        static int DescendingOrder(KeyValuePair<double, int> node1, KeyValuePair<double, int> node2)
        {
            return node2.Key.CompareTo(node1.Key); //for descending order
        }
        public static List<KeyValuePair<double, int>> GetEdges()
        {
            return edges;
        }
        public static Dictionary<int, KeyValuePair<int, double>> GetMST()
        {
            return distinctColors;
        }
        public static Dictionary<int, RGBPixel> GetMSTHelper()
        {
            return distinctHelper;
        }
        static int CantorPairing(int x, int y, int z)
        {
            int res1 = (((x + y) * (x + y + 1)) / 2) + y;
            int res2 = (((res1 + z) * (res1 + z + 1)) / 2) + z;
            return res2;
        }

        public static RGBPixel[,] dispImage() //n squared //size of image
        {
            Clustering.DFS();
            RGBPixel[,] modifiedImage = Buffer;
            //Outer for loop approaches an O(N) operation, N is the image height
            for (int i = 0; i < imageHeight; ++i)
            {
                //Inner for loop approaches an O(N) operation, N is the image width
                for (int j = 0; j < imageWidth; ++j)
                {
                    RGBPixel node = Buffer[i, j]; //O(1)
                    int color = CantorPairing(node.red, node.green, node.blue);
                    RGBPixel newColor = Clustering.represntativeColor[color];
                    modifiedImage[i, j] = newColor;
                }
            }
            return modifiedImage;
        }
    }
}