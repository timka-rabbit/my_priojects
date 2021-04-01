using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using PointClass;
using AlgorithmClass;
using MathClass;

namespace Graphic_UI
{
    public partial class Form1 : Form
    {
        List<IPoint2D> points;
        Graphics gr_1; // Алгоритм Краскала
        Graphics gr_2; // Муравьиный алгоритм
        bool isShownMode = false; // переменная для запрета ввода новых точек в режиме показа
        public Form1()
        {
            InitializeComponent();
            points = new List<IPoint2D>();
            gr_1 = panel1.CreateGraphics();
            gr_2 = panel2.CreateGraphics();
            this.Size = new System.Drawing.Size(435, 517);
            panel2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button1.Visible = true;
            button2.Visible = false;
            button1.Enabled = false;
        }

        /// <summary>
        /// Добавление вершины кликом левой кнопки мыши
        /// </summary>
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left && !isShownMode)
            {
                points.Add(new Point2D(e.Location.X, e.Location.Y));
                gr_1.FillEllipse(Brushes.Black, e.Location.X - 5, e.Location.Y - 5, 10, 10);
                gr_1.DrawString((points.Count).ToString(), new System.Drawing.Font("Consolas", 11f), Brushes.Black, new PointF((float)e.Location.X, (float)e.Location.Y - 20f));
                if(points.Count > 1)
                    button1.Enabled = true;
            }
        }

        /// <summary>
        /// Построение дерева
        /// </summary>
        private async void button1_Click(object sender, EventArgs e)
        {
            isShownMode = true;
            this.Size = new System.Drawing.Size(848, 517);
            panel2.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            button1.Visible = false;
            button2.Visible = true;

            // итерационный алгоритм улучшения построения начального дерева Штейнера (локал оптимизация)
            /*----------------------------------------Алгоритм Краскала----------------------------------------*/
            
            var info_kruskal = await RunConstructTree<IPoint2D>(gr_1, points, KruskalAlgorithm<IPoint2D>.Solve);
            textBox1.Text = info_kruskal.Item1;
            textBox3.Text = info_kruskal.Item2;

            /*------------------------------------------Полный перебор-----------------------------------------*/

            //var info_brute = await RunConstructTree<IPoint2D>(gr_2, points, BruteForce<IPoint2D>.Solve);
            //textBox2.Text = info_brute.Item1;
            //textBox4.Text = info_brute.Item2;

            /*---------------------------------------Муравьиный алгоритм---------------------------------------*/

            var info_ant = await RunConstructTree<IPoint2D>(gr_2, points, AntColonyAlgorithm<IPoint2D>.Solve);
            textBox2.Text = info_ant.Item1;
            textBox4.Text = info_ant.Item2;
        }

        /// <summary>
        /// Обёртка для асинхронного запуска выполнения алгоритма построения дерева 
        /// </summary>
        /// <typeparam name="T"> Тип точек </typeparam>
        /// <param name="gr"> Контекст отрисовки </param>
        /// <param name="points"> Опорные точки </param>
        /// <param name="func"> Алгоритм построения дерева </param>
        /// <returns> Длина дерева и время построения в виде строк </returns>
        async Task<Tuple<string, string>> RunConstructTree<T>(Graphics gr, List<T> points, Func<List<T>, dynamic> func)
        {
            Stopwatch sw = new Stopwatch();   
            sw.Start();
            dynamic tree = await Task.Run(() => func(points));
            sw.Stop();
            string road = string.Empty;
            string time = string.Empty;
            if (tree != null)
            {
                road = string.Format("{0:f2}", this.PrintTree(gr, tree));
                TimeSpan time_ant = sw.Elapsed;
                time = time_ant.Hours + " h : " + time_ant.Minutes + " m : " + string.Format("{0:f2}", (time_ant.Seconds + time_ant.Milliseconds / (double)1000)) + " s";
            }
            return new Tuple<string,string>(road, time);
        }

        /// <summary>
        /// Отрисовка дерева на экране и подсчёт его длины
        /// </summary>
        /// <param name="gr"> Контекст области отрисовки </param>
        /// <param name="tree"> Список рёбер дерева </param>
        /// <returns> Длина дерева </returns>
        double PrintTree(Graphics gr, dynamic tree)
        {
            double sum = 0;
            gr.Clear(Color.White);
            for (int i = 0; i < points.Count; i++)
            {
                gr.FillEllipse(Brushes.Black, (float)points[i].X - 5, (float)points[i].Y - 5, 10, 10);
                gr.DrawString((i + 1).ToString(), new System.Drawing.Font("Consolas", 11f), Brushes.Black, new PointF((float)points[i].X, (float)points[i].Y - 20f));
            }
            for (int i = 0; i < tree.Count; i++)
            {
                Pen pen = new Pen(Color.FromArgb(Rand.rand.Next(50, 200), Rand.rand.Next(50, 200), Rand.rand.Next(50, 200)), 3f);
                gr.DrawLine(pen, new PointF((float)tree[i].Item1.X, (float)tree[i].Item1.Y), new PointF((float)tree[i].Item2.X, (float)tree[i].Item2.Y));
                sum += MathClass.Dist.GetOrthogonalDist(tree[i].Item1, tree[i].Item2);
            }
            return sum;
        }

        /// <summary>
        /// Очистка панелей и возврат на панель ввода точек
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            isShownMode = false;
            this.Size = new System.Drawing.Size(435, 517);
            gr_1.Clear(panel1.BackColor);
            gr_2.Clear(panel2.BackColor);
            points.Clear();
            textBox1.Clear();
            textBox2.Clear();
            panel2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button1.Visible = true;
            button2.Visible = false;
            button1.Enabled = false;
        }
    }
}
