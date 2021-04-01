using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Drawer
{
    /// <summary>
    /// Интерфейс рисования
    /// </summary>
    public interface IDrawer
    {
        /// <summary>
        /// Отрисовка начальной точки
        /// </summary>
        void DrawStart(float x, float y);

        /// <summary>
        /// Отрисовка линии
        /// </summary>
        void DrawLine(float x1, float y1, float x2, float y2);

        /// <summary>
        /// Отрисовка конечной точки
        /// </summary>
        void DrawEnd(float x1, float y1, float x2, float y2);
    }

    /// <summary>
    /// Класс отрисовки на графический контексте
    /// </summary>
    public class DrawGraphics : IDrawer
    {
        Graphics g;
        int style;
        public DrawGraphics(Graphics g, int style)
        {
            this.g = g;
            this.style = style;
        }
        public void DrawStart(float x, float y)
        {
            switch(style)
            { 
                case 1: 
                    g.FillPie(new Pen(Color.Green).Brush, x - 5, y - 5, 10, 10, 0, 360);
                    break;
                case 2:
                    g.FillRectangle(new Pen(Color.Black) { DashPattern = new float[] { 2f, 1f } }.Brush, new RectangleF(new System.Drawing.PointF(x - 3, y - 3), new Size(6, 6)));
                    break;
                case 3:
                    g.FillPie(new Pen(Color.DarkTurquoise).Brush, x - 5, y - 5, 10, 10, 0, 360);
                    break;
                default:
                    g.FillPie(new Pen(Color.Orange).Brush, x - 5, y - 5, 10, 10, 0, 360);
                    break;
            }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (style)
            {
                case 1:
                    g.DrawLine(new Pen(Color.Green, 2f), new PointF(x1, y1), new PointF(x2, y2));
                    break;
                case 2:
                    using (Pen pen = new Pen(Color.Black, 2f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash, DashPattern = new float[] { 2f, 1f } })
                    {
                        g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
                    }
                    break;
                case 3:
                    g.DrawLine(new Pen(Color.DarkTurquoise, 2f), new PointF(x1, y1), new PointF(x2, y2));
                    break;
                default:
                    g.DrawLine(new Pen(Color.Orange, 2f), new PointF(x1, y1), new PointF(x2, y2));
                    break;
            }
        }
        public void DrawEnd(float x1, float y1, float x2, float y2)
        {
            switch (style)
            {
                case 1:
                    using (Pen pen = new Pen(Color.Green, 2f) { CustomEndCap = new AdjustableArrowCap(4f, 8f) })
                    {
                        g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
                    }
                    break;
                case 2:
                    using (Pen pen = new Pen(Color.Black, 2f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash, DashPattern = new float[] { 2f, 1f } })
                    {
                        g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
                        g.FillRectangle(new Pen(Color.Black).Brush, new RectangleF(new System.Drawing.PointF(x2 - 3, y2 - 3), new Size(6, 6)));
                    }
                    break;
                case 3:
                    using (Pen pen = new Pen(Color.DarkTurquoise, 2f) { CustomEndCap = new AdjustableArrowCap(4f, 8f) })
                    {
                        g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
                    }
                    break;
                default:
                    using (Pen pen = new Pen(Color.Orange, 2f) { CustomEndCap = new AdjustableArrowCap(4f, 8f) })
                    {
                        g.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Класс отрисовки в SVG файл
    /// </summary>
    public class DrawSVG : IDrawer
    {
        string _fileName;
        int style;
        public DrawSVG(string fileName, int style)
        {
            this._fileName = fileName;
            this.style = style;
            using (StreamWriter file = new StreamWriter(fileName, false))
            {
                file.Write("<svg xmlns=\"http://www.w3.org/2000/svg\">\n</svg>");
               
            }
            if (style == 1)
                Rewriting("\n<defs>\n<marker id=\"arrowhead\" markerWidth=\"10\" markerHeight=\"7\" refX=\"6.5\" refY=\"3.5\" orient=\"auto\">" +
                         "\n<polygon points=\"0 1, 7 3.5, 0 6\" fill=\"green\"/>\n</marker>\n</defs>");
            else if(style == 4)
                Rewriting("\n<defs>\n<marker id=\"arrowhead\" markerWidth=\"10\" markerHeight=\"7\" refX=\"6.5\" refY=\"3.5\" orient=\"auto\">" +
                         "\n<polygon points=\"0 1, 7 3.5, 0 6\" fill=\"orchid\"/>\n</marker>\n</defs>");
        }
        void Rewriting(string str)
        {
            string[] text = File.ReadAllLines(this._fileName, Encoding.Default).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            using (StreamWriter file = new StreamWriter(_fileName, false))
            {
                int k = text.Count();
                int i;
                for (i = 0; i < k - 1; i++)
                    file.Write("\n" + text[i]);
                file.Write(str);
                file.Write("\n" + text[k - 1]);
            }
        }
        string Str(float a)
        {
            return a.ToString().Replace(',', '.');
        }
        public void DrawStart(float x, float y)
        {
            switch (style)
            {
                case 1:
                    Rewriting("\n<circle r=\"5\" cx=\"" + Str(x) + "\" cy=\"" + Str(y) + "\" fill=\"green\"/>");
                    break;
                case 2:
                    Rewriting("\n<rect x=\"" + Str(x - 3) + "\" y=\"" + Str(y - 3) + "\" width=\"6\" height=\"6\" fill=\"black\"/>");
                    break;
                case 3:
                    Rewriting("\n<circle r=\"5\" cx=\"" + Str(x) + "\" cy=\"" + Str(y) + "\" fill=\"darkturquoise\"/>");
                    break;
                default:
                    float h = (int)(12 * Math.Pow(3, 0.5) / 2);
                    Rewriting("\n<polygon points=\"" + Str(x - 6) + " " + Str(y - h / 3) + ", " + Str(x + 6) + " " + Str(y - h / 3) + ", " + Str(x) + " " + Str(y + 2 * h / 3) + "\" fill=\"orchid\"/>");
                    break;
            }
        }
        public virtual void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (style)
            {
                case 1:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"green\" stroke-width=\"2\"/>");
                    break;
                case 2:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"black\" stroke-width=\"2\" stroke-dasharray=\"4\"/>");
                    break;
                case 3:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"darkturquoise\" stroke-width=\"2\"/>");
                    break;
                default:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"orchid\" stroke-width=\"2\"/>");
                    break;
            }
        }
        public void DrawEnd(float x1, float y1, float x2, float y2)
        {
            switch (style)
            {
                case 1:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"green\" stroke-width=\"2\" marker-end=\"url(#arrowhead)\"/>");
                    break;
                case 2:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"black\" stroke-width=\"2\" stroke-dasharray=\"4\"/>" +
                    "\n<rect x=\"" + Str(x2 - 3) + "\" y=\"" + Str(y2 - 3) + "\" width=\"6\" height=\"6\" fill=\"black\"/>");
                    break;
                case 3:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"darkturquoise\" stroke-width=\"2\"/>");
                    Rewriting("\n<circle r=\"5\" cx=\"" + Str(x2) + "\" cy=\"" + Str(y2) + "\" fill=\"darkturquoise\"/>");
                    break;
                default:
                    Rewriting("\n<line x1=\"" + Str(x1) + "\" y1=\"" + Str(y1) + "\" x2=\"" + Str(x2) + "\" y2=\"" + Str(y2) + "\" stroke=\"orchid\" stroke-width=\"2\" marker-end=\"url(#arrowhead)\"/>");
                    break;
            }
        }
    }

}
