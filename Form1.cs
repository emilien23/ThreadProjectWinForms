using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace ThreadProjectWinForms
{
    public partial class Form1 : Form
    {
        static float[,] a;
        static float[,] b;
        static int m = 1;
        List<Thread> list_th;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(text_size.Text);
            long[] mill = startParallel(n);
            chart_parallel.Series[0].Points.Clear();
            create_Chart(mill);
        }
        public void create_Chart(long[] mill)
        {
            for (int i = 0; i < mill.Length; i++)
                chart_parallel.Series[0].Points.AddXY(i + 1, mill[i]);
        }
        public long[] startParallel(int n)
        {
            long[] millisec = new long[8];

            m = 1;

            for (; m <= 8; m++)
            {
                a = new float[n, n];
                b = new float[n, n];

                a = fillRandomValues(a, n);
                b = fillRandomValues(b, n);

                list_th = new List<Thread>(m);
                int th = 1, shag = 0, ostatok = n;

                if (n % m == 0)
                    shag = n / m;
                else
                    shag = n / m + 1;
                int istart = 0, iend = shag;

                for (int i = 0; i < m; i++)
                {
                    list_th.Add(new Thread(delegate() { process(istart, 0, iend, n); }));
                    istart += shag;
                    ostatok -= shag;
                    if (ostatok != 0 && ostatok % (m - th) == 0)
                        shag = n / m;

                    iend += shag;
                    th++;
                }
                foreach(Thread t in list_th)
                {
                    Stopwatch sw = new Stopwatch();
                    
                    sw.Start();
                    t.Start();
                    sw.Stop();
                    millisec[m-1] = sw.ElapsedTicks;
                }
            }
            return millisec;
        }
         float[,] fillRandomValues(float[,] matrix, int n)
        {
            Random rand = new Random();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = rand.Next();
            return matrix;
        }
         float[,] sequentialProcessing(float[,] a, float[,] b, int n)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    a[i, j] = Convert.ToSingle(Math.Log(Math.Cos(Convert.ToDouble(a[i, j] + b[i, j]))));
            return a;
        }
         void process(int istart, int jstart, int iend, int jend)
        {
            if (istart < jend)
            {
                for (int i = istart; i < iend; i++)
                    for (int j = jstart; j < jend; j++)
                        a[i, j] = Convert.ToSingle(Math.Log(Math.Cos(Convert.ToDouble(a[i, j] + b[i, j]))));
            }
        }
         private void button1_Click(object sender, EventArgs e)
         {
             int n = Convert.ToInt32(text_size.Text);
             a = new float[n, n];
             b = new float[n, n];

             a = fillRandomValues(a, n);
             b = fillRandomValues(b, n);


             Stopwatch sw = new Stopwatch();
             sw.Start();

             a = sequentialProcessing(a, b, n);
             sw.Stop();

             label2.Text =  "Затраченное время " + sw.ElapsedMilliseconds.ToString();
         }
    }
}
