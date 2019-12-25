using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            int imageWidth = ImageOperations.GetWidth(ImageMatrix);
            int imageHeight = ImageOperations.GetHeight(ImageMatrix);
            int maxDistinctNum = 60000;//ImageOperations.GetHeight(ImageMatrix) * ImageOperations.GetWidth(ImageMatrix) + 10;
            Dictionary<int, KeyValuePair<int, double>> MSTree = new Dictionary<int, KeyValuePair<int, double>>(maxDistinctNum);
            Dictionary<int, RGBPixel> distinctHelper = new Dictionary<int, RGBPixel>(maxDistinctNum);
            Dictionary<int, bool> visited = new Dictionary<int, bool>(maxDistinctNum);
            int color = 0;
            KeyValuePair<int, KeyValuePair<int, double>> minVertix = new KeyValuePair<int, KeyValuePair<int, double>>();
            int noDistinctColors = MST.FindDistinctColors(ImageMatrix, ref MSTree, ref distinctHelper, ref visited, ref color, ref minVertix);
            
            List<KeyValuePair<double, int>> edges = new List<KeyValuePair<double, int>>(maxDistinctNum);
            double MST_Sum = MST.FindMinimumSpanningTree(ref MSTree, distinctHelper, visited, color, minVertix, ref edges);
            
            int k = 192;
            Dictionary<int, RGBPixel> represntativeColor = new Dictionary<int, RGBPixel>(maxDistinctNum);
            Dictionary<int, List<int>> clusteredGraph = Clustering.ProduceKClusters(k, edges, MSTree, maxDistinctNum, visited);
            Clustering.DFS(clusteredGraph, visited, maxDistinctNum, k, ref represntativeColor, distinctHelper);
            
            MST.Coloring(ref ImageMatrix, represntativeColor);
            
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);          
        }    
    }
}