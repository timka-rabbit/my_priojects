using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Geometry;
using Visual;
using Drawer;
using StopperLength;
using Command;

namespace UI
{
    public partial class Form1 : Form
    {
        Graphics g;  // контекст отрисовки
        List<VisualCurve> curves; // список кривых
        Random rand;
        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            g = panel1.CreateGraphics();
            new Form_Instance(this).Execute();
        }
        ~Form1()
        {
            g.Dispose();
        }

        /// <summary>
        /// Операция начального состояния формы
        /// </summary>
        class Form_Instance : ACommand
        {
            Form1 owner;
            public Form_Instance(Form1 owner)
            {
                this.owner = owner;
            }

            protected override void doExecute()
            {
                owner.curves = new List<VisualCurve>();
            }

            protected override ACommand Clone()
            {
                return new Form_Instance(owner);
            }
        }

        /// <summary>
        /// Операция добавления кривой
        /// </summary>
        class Items_Add : ACommand
        {
            VisualCurve curve;
            Form1 owner;
            public Items_Add(VisualCurve curve, Form1 owner)
            {
                this.curve = curve;
                this.owner = owner;
            }

            protected override void doExecute()
            {
                owner.curves.Add(curve);   
            }

            protected override ACommand Clone()
            {
                return new Items_Add(curve, owner);
            }
        }

        /// <summary>
        /// Операция перемещения кривой
        /// </summary>
        class Items_Move : ACommand
        {
            int index;
            IPoint new_p;
            Form1 owner;
            public Items_Move(int index, IPoint new_p, Form1 owner)
            {
                this.index = index;
                this.new_p = (IPoint)new_p.Clone();
                this.owner = owner;
            }

            protected override void doExecute()
            {
                VisualCurve c = VisualCurve.CreateVisualCurve(new MoveTo((ICurve)owner.curves[index].Curve.Clone(), new_p));
                owner.curves.RemoveAt(index);
                owner.curves.Add(c);
            }

            protected override ACommand Clone()
            {
                return new Items_Move(index, new_p, owner);
            }
        }

        /// <summary>
        /// Отрисовка кривых из списка
        /// </summary>
        /// <param name="list"> Список кривых </param>
        private void Draw(List<VisualCurve> list)
        {
            IDrawer draw = new DrawGraphics(g, 1);
            foreach (VisualCurve c in list)
                c.Draw(draw);
        }

        /// <summary>
        /// Генерация новой кривой при нажатии кнопки
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            VisualCurve cu = GenerateCurve(rand.Next(1, 3));
            new Items_Add(cu.Clone(), this).Execute();
            cu.Draw(new DrawGraphics(g, 1));
        }

        /// <summary>
        /// Генерация кривой 
        /// </summary>
        private VisualCurve GenerateCurve(int type)
        {
            int w = panel1.Width / 2 - 10;
            int h = panel1.Height / 2 - 10;
            if (type == 1)
                return VisualCurve.CreateVisualLine(new Line(new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h))), new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h)))));
            else
                return VisualCurve.CreateVisualBezier(new Bezier(new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h))), new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h))), new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h))), new Geometry.Point(correctX(rand.Next(-w, w)), correctY(rand.Next(-h, h)))));
        }

        /// <summary>
        /// Корректировка координаты X
        /// </summary>
        private float correctX(float x)
        {
            return x + (float)panel1.Width / 2;
        }

        /// <summary>
        /// Корректировка координаты Y
        /// </summary>
        private float correctY(float y)
        {
            return (float)panel1.Height / 2 - y; 
        }

        /// <summary>
        /// Отмена последней операции на Ctrl+Z, возврат отмены на Ctrl+Y
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
            {
                CM.Instance.Undo();
                panel1.Refresh();
                Draw(curves);
            }
            if (e.KeyData == (Keys.Control | Keys.Y))
            {
                if (CM.Instance.Redo())
                {
                    panel1.Refresh();
                    Draw(curves);
                }
            }
        }

       /*------------------------------------------------------------------------------------*/

        bool moving = false;
        bool hover = false;
        bool hoverCurve = false;
        VisualCurve movingCurve;
        List<VisualCurve> movingList;
        IPoint p_offset;

        void CopyCurves(ref List<VisualCurve> list)
        {
            list = new List<VisualCurve>();
            foreach (VisualCurve c in curves)
                list.Add((VisualCurve)(c.Clone()));
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            CopyCurves(ref movingList);
            foreach (VisualCurve c in movingList)
            {
                if (c.Path.IsOutlineVisible(e.Location, new Pen(Color.Black, 15)))
                {
                    c.GetPoint(0, out p_offset);
                    p_offset.X -= e.X;
                    p_offset.Y -= e.Y;
                    movingCurve = c;
                    break;
                }
            }
            if (movingCurve != null)
                moving = true;
        }
        private void DrawMove(List<VisualCurve> list, VisualCurve move)
        {
            IDrawer draw = new DrawGraphics(g, 1);
            IDrawer drawMove = new DrawGraphics(g, 4);
            int ind = list.IndexOf(move);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != ind)
                    list[i].Draw(draw);
                else list[i].Draw(drawMove);
            }
        }

        /// <summary>
        /// Перемещение кривой при нажатии на неё мышкой
        /// </summary>
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                IPoint new_p = new Geometry.Point(e.X + p_offset.X, e.Y + p_offset.Y);
                movingCurve.Curve = new MoveTo(movingCurve.Curve, new_p);
                panel1.Refresh();
                DrawMove(movingList, movingCurve);
            }
            else
            {
                foreach (VisualCurve c in curves)
                {
                    if (hoverCurve = c.Path.IsOutlineVisible(e.Location, new Pen(Color.Black, 15)))
                    {
                        VisualCurve cu = VisualCurve.CreateVisualCurve(c.Clone());
                        c.Draw(new DrawGraphics(g, 3));
                        hover = true;
                        break;
                    }
                }
                if (!hoverCurve && hover)
                {
                    panel1.Refresh();
                    Draw(curves);
                    hover = false;
                }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
           if(moving)
           {
               IPoint new_p = new Geometry.Point(e.X + p_offset.X, e.Y + p_offset.Y);
               new Items_Move(movingList.IndexOf(movingCurve), new_p, this).Execute();
               movingCurve = null;
               moving = false;
               panel1.Refresh();
               Draw(curves);
           }
        }
    }
}
