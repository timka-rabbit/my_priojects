using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using Geometry;
using Drawer;
using StopperLength;

namespace Visual
{
    /// <summary>
    /// Интерфейс отрисовки кривой
    /// </summary>
    public interface IDrawable
    {
        GraphicsPath Path { get; }
        void Draw(IDrawer draw);
    }

    /// <summary>
    /// Класс отрисовки кривой
    /// </summary>
    public class VisualCurve : ICurve, IDrawable
    {
        ICurve curve;
        GraphicsPath path;
        VisualCurve(ICurve curve)
        {
            this.curve = curve;
            path = new GraphicsPath();
        }
        public VisualCurve Clone()
        {
            VisualCurve c = new VisualCurve((ICurve)(curve.Clone()));
            c.path = this.Path;
            return c;
        }
        object ICloneable.Clone()
        {
            VisualCurve c = new VisualCurve((ICurve)(curve.Clone()));
            c.path = this.Path;
            return c;
        }
        public GraphicsPath Path { get { return path; } }
        public ICurve Curve { get { return curve; } set { curve = value; } }

        public double GetLength(double t, IStopper stop)
        {
            return curve.GetLength(t, stop);
        }
        public double GetT(double len, IStopper stop)
        {
            return curve.GetT(len, stop);
        }
        public void GetPoint(double t, out IPoint p)
        {
            curve.GetPoint(t, out p);
        }
        public AComposite GetComposite()
        {
            return curve.GetComposite();
        }
        public bool CompareTo(ICurve curve)
        {
            return this.curve.CompareTo(curve);
        }
        public void Draw(IDrawer draw)
        {
            IPoint p1, p2, end;
            this.GetPoint(0, out p1);
            this.GetPoint(1, out end);
            p2 = (IPoint)p1.Clone();
            draw.DrawStart(p1.X, p1.Y); 
            int i;
            for (i = 1; i < Params.n; i++)
            {
                this.GetPoint((double)i / Params.n, out p2);
                if (p2.CompareTo(end))
                    break;
                draw.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
                path.AddLine(p1.X, p1.Y, p2.X, p2.Y);
                p1 = (IPoint)p2.Clone();
            }
            draw.DrawEnd(p1.X, p1.Y, end.X, end.Y);
            path.AddLine(p1.X, p1.Y, end.X, end.Y);
        }
        static public VisualCurve CreateVisualLine(Line line)
        {
            return new VisualCurve(line);
        }
        static public VisualCurve CreateVisualBezier(Bezier bezier)
        {
            return new VisualCurve(bezier);
        }
        static public VisualCurve CreateVisualCurve(ICurve curve)
        {
            return new VisualCurve(curve);
        }
    }
}
