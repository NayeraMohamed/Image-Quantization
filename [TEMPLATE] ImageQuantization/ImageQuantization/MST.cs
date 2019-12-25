using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class MST
    {
        
        /// <summary>
        /// Gets all DistinctColors from 2D array and fills Dictionary of MST with all of them
        /// </summary>
        /// <param name="Buffer">Image array</param>
        /// <param name="distinctColors"> Dictionary that contains MinSpanningTree</param>
        /// <param name="distinctHelper">Helper Dictionary for keeping struct values concatenated as integer (Key) 
        /// and struct itself as value</param>
        /// <param name="visited">Dictionary that checks if nodes are visited in MinSpanningTree</param>
        /// <param name="color">Dictionary key</param>
        /// <param name="minVertix">Any node that we start Minimum Spaning from</param>
        /// <returns>number DistinctColors</returns>
        /// Time Complexity: O(N^2)
        /// Space Complexity: N
        public static int FindDistinctColors(RGBPixel[,] Buffer, ref Dictionary<int, KeyValuePair<int, double>> distinctColors, 
            ref Dictionary<int, RGBPixel> distinctHelper, ref Dictionary<int, bool> visited, 
            ref int color, ref KeyValuePair<int, KeyValuePair<int, double>> minVertix)
        {
            int noDistinctColors = 0; //O(1)
            int imageWidth = ImageOperations.GetWidth(Buffer); //O(1)
            int imageHeight = ImageOperations.GetHeight(Buffer); //O(1)
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
                        distinctHelper.Add(color, node); //O(1)
                        visited.Add(color, false); //node initially not visited O(1)

                        noDistinctColors++; //O(1) 
                    }
                }
            }
            //to avoid using Dic.First(), Vertex to start MST from is generated here
            minVertix = new KeyValuePair<int, KeyValuePair<int, double>>(color, new KeyValuePair<int, double>(0, double.MaxValue)); //O(1)
            return noDistinctColors; //O(1)
        }

       
        

        /// <summary>
        /// Calculates distances between all vertices and each other, 
        /// and finds Minimum Spanning Tree using Prim's Algorithm
        /// </summary>
        /// <param name="distinctColors">Dictionary that contains MinSpanningTree</param>
        /// <param name="distinctHelper">Helper Dictionary for keeping struct values concatenated as int (Key) 
        /// and the struct as Value</param>
        /// <param name="visited">Dictionary that checks if nodes are visited in MinSpanningTree</param>
        /// <param name="color">Dictionary key</param>
        /// <param name="minVertix">Any node that we start from the Minimum Spanning</param>
        /// <param name="edges">List that contains mimimum spanning edges to be sorted</param>
        /// <returns>MST Sum</returns>
        ///Time Complexity: O(V^2) = O(E), V is the no of vertices, 
        /// E is the no of Edges in graph, approaches V^2 as it is a dense graph
        public static double FindMinimumSpanningTree(ref Dictionary<int, KeyValuePair<int, double>> distinctColors,
            Dictionary<int, RGBPixel> distinctHelper, Dictionary<int, bool> visited, 
            int color, KeyValuePair<int, KeyValuePair<int, double>> minVertix, ref List<KeyValuePair<double, int>> edges)
        {
            double MST_Sum = 0; //O(1)
            //
            int v; //O(1)
            double minEdge; //O(1)
            KeyValuePair<int, double> edge = new KeyValuePair<int, double>(); //O(1)
           
            //currentMin >  to get the minimum vertix at each iteration
            KeyValuePair<int, KeyValuePair<int, double>> currentMin = new KeyValuePair<int, KeyValuePair<int, double>>(); //O(1)
            RGBPixel nodeColor, minVertexColor; //O(1)


            //ToList() > Converts distinctColors Dictionary to List to be able to iterate over it,
            //approaches an O(V) operation, V is the number of vertices
            List<KeyValuePair<int, KeyValuePair<int, double>>> vertices = distinctColors.ToList();
           
            //Outer for loop approaches an O(V) operation, V is the number of vertices,
            //each iteration we relax a vertix
            for (int i = 0; i < distinctColors.Count - 1; i++)
            {
                v = minVertix.Key; //O(1)
                visited[v] = true; //O(1)
                minEdge = double.MaxValue; //O(1)

                 //Inner for loop approaches an O(V) operation, V is the number of vertices
                foreach (KeyValuePair<int, KeyValuePair<int, double>> node in vertices)
                {
                    int n = node.Key; //O(1)
                    nodeColor = distinctHelper[n]; //O(1)
                    minVertexColor = distinctHelper[v]; //O(1)

                    //check whether the distance must be calculated or not
                    //continue if node is already visited or it is the same node where the min distance is calc relative to it
                    if ((n == v) || visited[n])
                    {
                        continue;
                    }
                    //Sqrt() approaches O(1) operation
                    int d = (minVertexColor.red - nodeColor.red) * (minVertexColor.red - nodeColor.red)
                                              + (minVertexColor.blue - nodeColor.blue) * (minVertexColor.blue - nodeColor.blue)
                                              + (minVertexColor.green - nodeColor.green) * (minVertexColor.green - nodeColor.green);
                    double distance = Math.Sqrt(d); //O(1)
                    //
                    edge = distinctColors[n]; //O(1)
                    //update distance value if less distance is calculated
                    if (edge.Value > distance) //O(1)
                    {
                        distinctColors[n] = new KeyValuePair<int, double>(v, distance); //O(1)
                    }
                    //finding current min in inner loop relative to chosen vertex in outer loop
                    if (distinctColors[n].Value < minEdge) //O(1)
                    {
                        minEdge = distinctColors[n].Value; //O(1)
                        currentMin = node; //O(1)
                    }
                }
                minVertix = currentMin; //O(1)
                KeyValuePair<double, int> MSTEdge = new KeyValuePair<double, int>(minEdge, minVertix.Key); //O(1)
                edges.Add(MSTEdge);//O(1)
                MST_Sum += minEdge; //O(1)
            }
            edges.Sort(DescendingOrder); //O(VlgV)
            return MST_Sum; //O(1)
        }
        //Time Complexity: O(1)
        static int DescendingOrder(KeyValuePair<double, int> node1, KeyValuePair<double, int> node2)
        {
            return node2.Key.CompareTo(node1.Key); //for descending order O(1)
        }
        

        /// <summary>
        /// Function that performs math operation on the R G B of the pixel to generate a unique key
        /// </summary>
        /// <param name="x">The R of the pixel</param>
        /// <param name="y">The G of the pixel</param>
        /// <param name="z">The B of the pixel</param>
        /// <returns>unique key</returns>
        /// Time Complexity: O(1)
        static int CantorPairing(int x, int y, int z)
        {
            int res1 = (((x + y) * (x + y + 1)) / 2) + y; //O(1)
            int res2 = (((res1 + z) * (res1 + z + 1)) / 2) + z; //O(1)
            return res2; //O(1)
        }

        
        
        /// <summary>
        /// Function that repaints the image with the new representative color of each cluster
        /// </summary>
        /// <param name="Buffer">Image 2D array</param>
        /// <param name="represntativeColor">Dictionary that carries every distinct node with its new color</param>
        /// <returns>The quantized image</returns>
        /// Time Complexity: O(N^2)

        public static RGBPixel[,] Coloring(ref RGBPixel[,] Buffer, Dictionary<int, RGBPixel> represntativeColor) 
        {
            
            int imageWidth = ImageOperations.GetWidth(Buffer); //O(1)
            int imageHeight = ImageOperations.GetHeight(Buffer); //O(1)
            RGBPixel[,] modifiedImage = Buffer; //O(1)
            //Outer for loop approaches an O(N) operation, N is the image height
            for (int i = 0; i < imageHeight; ++i)
            {
                //Inner for loop approaches an O(N) operation, N is the image width
                for (int j = 0; j < imageWidth; ++j)
                {
                    RGBPixel node = Buffer[i, j]; //O(1)
                    int color = CantorPairing(node.red, node.green, node.blue); //O(1)
                    RGBPixel newColor = represntativeColor[color]; //O(1)
                    modifiedImage[i, j] = newColor; //O(1)
                }
            }
            return modifiedImage; //O(1)
        }
    }
}