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
        static RGBPixel[,] Buffer = ImageOperations.GetImage();     
        static int imageHeight = Buffer.GetLength(0);
        static int imageWidth = Buffer.GetLength(1);
        static int maxDistinctNum = Math.Max(imageHeight, imageWidth);
        static Dictionary<RGBPixel, KeyValuePair<RGBPixel, double>> distinctColors = new Dictionary<RGBPixel, KeyValuePair<RGBPixel, double>>(maxDistinctNum);  
        
        public static int GetDistinctColors()
        {
            for (int i = 0; i < imageHeight; ++i)
            {
                for (int j = 0; j < imageWidth; ++j)
                {
                    if(!distinctColors.ContainsKey(Buffer[i, j]))
                    {
                        RGBPixel node = Buffer[i, j];
                        distinctColors.Add(node, new KeyValuePair<RGBPixel, double>());
                        noDistinctColors++;
                    }
                }
            }
            return noDistinctColors;
        }
        public static double GetMinimumSpanningTree()
        {

            return MST_Sum;
        }
    }
}