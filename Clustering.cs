using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        static int noDistinctColors = 0;
        static double MST_Sum = 0;
        //static List<bool> visited = new List<bool>();
        static RGBPixel[,] Buffer = ImageOperations.GetImage();     
        static int imageHeight = Buffer.GetLength(0);
        static int imageWidth = Buffer.GetLength(1);
        static int maxDistinctNum = Math.Max(imageHeight, imageWidth);
        static Dictionary<string, KeyValuePair<string, double>> distinctColors = new Dictionary<string, KeyValuePair<string, double>>(imageHeight*imageWidth);
        static Dictionary<string, RGBPixel> distinctHelper = new Dictionary<string, RGBPixel>(imageWidth*imageHeight);
        static Dictionary<string, bool> visited = new Dictionary<string, bool>();
        static string color;
       // static int k=0;
        public static int GetDistinctColors()
        {
            for (int i = 0; i < imageHeight; ++i)
            {
                for (int j = 0; j < imageWidth; ++j)
                {
                    color = (Buffer[i, j].red).ToString() + (Buffer[i, j].green).ToString() + (Buffer[i, j].blue).ToString();
                    //int key = Int32.Parse(color);
                    if(!distinctHelper.ContainsKey(color))
                    {
                        RGBPixel node = Buffer[i, j];
                        
                        distinctColors.Add(color, new KeyValuePair<string, double>("",double.MaxValue));
                        distinctHelper.Add(color, node);
                        noDistinctColors++;
                        visited.Add(color, false);
                      
                    }
                }
            }
            return noDistinctColors;
        }
        public static double GetMinimumSpanningTree()
        {
            //the minimum node  by which we start calculating min from....
            KeyValuePair<string, KeyValuePair<string, double>> next = distinctColors.ElementAt(0);

            //temp to get the minimum node at each iteration...
            KeyValuePair<string, KeyValuePair<string, double>> temp =new KeyValuePair<string, KeyValuePair<string, double>>();


            //each iteration we relax a node......O(D)
            for (int i = 0; i < distinctColors.Count - 1; i++)
            {
                visited[next.Key] = true;
                double minedge = double.MaxValue;

                //O(D)
                foreach(KeyValuePair<string, KeyValuePair<string, double>> node in distinctColors.ToList())
                {
            //check wether the distance must be calculated or not.......
                    if (node.Equals(next)|| visited[node.Key]==true)
                    {
                        continue;
                    }
                   
                    double distance = Math.Sqrt(Math.Pow((distinctHelper[next.Key].red - distinctHelper[node.Key].red), 2)
                                              + Math.Pow((distinctHelper[next.Key].blue - distinctHelper[node.Key].blue), 2)
                                              + Math.Pow((distinctHelper[next.Key].green - distinctHelper[node.Key].green), 2));

                    //updating the map.......
                    if (distinctColors[node.Key].Value> distance)
                    {

                        distinctColors[node.Key] = new KeyValuePair<string, double>(next.Key, distance);
                       
                    }
                    //getting the next minimum.....
                    if (distinctColors[node.Key].Value < minedge)
                    {
                        minedge = distinctColors[node.Key].Value;
                        temp=node;
                    }

                  
                }

                next=temp;
               

                MST_Sum += minedge;

            }
            return MST_Sum;
        }
    }
}