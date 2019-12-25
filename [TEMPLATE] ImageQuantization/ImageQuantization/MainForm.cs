using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
            var stopwatch = Stopwatch.StartNew(); 
            int imageWidth = ImageOperations.GetWidth(ImageMatrix); //O(1)
            int imageHeight = ImageOperations.GetHeight(ImageMatrix); //O(1)
            int maxDistinctNum = 60000; //Maximum number of distinic in the given test cases //O(1)

            //Dictionary that contains MinSpanningTree
            Dictionary<int, KeyValuePair<int, double>> MSTree = new Dictionary<int, KeyValuePair<int, double>>(maxDistinctNum);//O(1)

            //Helper Dictionary for keeping struct values concatenated as integer (Key) and the struct as Value
            Dictionary<int, RGBPixel> distinctHelper = new Dictionary<int, RGBPixel>(maxDistinctNum);//O(1)
            //Checks visited nodes in minimum spanning tree
            Dictionary<int, bool> visited = new Dictionary<int, bool>(maxDistinctNum);//O(1)
            int color = 0; //O(1)
            //Any node that we start from the Minimum Spanning
            KeyValuePair<int, KeyValuePair<int, double>> minVertix = new KeyValuePair<int, KeyValuePair<int, double>>();//O(1)
            //O(D^2)
            int noDistinctColors = MST.FindDistinctColors(ImageMatrix, ref MSTree, ref distinctHelper, ref visited, ref color, ref minVertix);
            //List that contains mimimum spanning edges sorted
            List<KeyValuePair<double, int>> edges = new List<KeyValuePair<double, int>>(maxDistinctNum); //O(1)
            double MST_Sum = MST.FindMinimumSpanningTree(ref MSTree, distinctHelper, visited, color, minVertix, ref edges); //O(D^2)

            int k = int.Parse(txtGetK.Text); //O(N)
            //Dictionary carries each original color as key and representative color of each cluster as value
            Dictionary<int, RGBPixel> represntativeColor = new Dictionary<int, RGBPixel>(maxDistinctNum);
            //O(K+D) = O(D)
            Dictionary<int, List<int>> clusteredGraph = Clustering.ProduceKClusters(k, edges, MSTree, maxDistinctNum, visited);
            //O(E+V) =O(V) = O(D)
            Clustering.DFS(clusteredGraph, visited, maxDistinctNum, k, ref represntativeColor, distinctHelper);
            //O(N^2)
            MST.Coloring(ref ImageMatrix, represntativeColor);
            
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            stopwatch.Stop();
            double Miliseconds = (stopwatch.Elapsed.TotalMilliseconds);
            txtMili.Text=Miliseconds.ToString(); //O(1)
            double Seconds = (stopwatch.Elapsed.TotalSeconds);
            txtSec.Text = Seconds.ToString(); //O(1)
            txtDistinct.Text = noDistinctColors.ToString(); //O(1)
            txtMST.Text = MST_Sum.ToString(); //O(1)
        }

        private void txtGetK_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }    
    }
}