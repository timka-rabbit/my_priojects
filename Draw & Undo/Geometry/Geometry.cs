using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopperLength;
using System.Collections;


namespace Geometry
{
    /// <summary>
    /// Пользовательские параметры
    /// </summary>
    public struct Params
    {
        /// <summary>
        /// Детализация отрисовки
        /// </summary>
        public const int n = 50;
        /// <summary>
        /// Параметр середины кривой
        /// </summary>
        public const double t = 0.5;
    }


    /// <summary>
    /// Интерфейс для работы с точками
    /// </summary>
    public interface IPoint : ICloneable
    {
        /// <summary>
        /// Получает и возвращает координату X
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// Получает и возвращает координату Y
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// Сравнивает исходную точку с другой покоординатно
        /// </summary>
        bool CompareTo(IPoint p);
    }


    /// <summary>
    /// Интерфейс для работы с кривыми
    /// </summary>
    public interface ICurve : ICloneable
    {
        /// <summary>
        /// Получаем точку кривой по параметру t
        /// </summary>
        void GetPoint(double t, out IPoint p);

        /// <summary>
        /// Получаем длину кривой по параметру t
        /// </summary>
        double GetLength(double t, IStopper stop);

        /// <summary>
        /// Получаем параметр t по длине кривой
        /// </summary>
        double GetT(double len, IStopper stop);

        /// <summary>
        /// Проверка, является ли кривая составным объектом
        /// </summary>
        AComposite GetComposite();

        /// <summary>
        /// Сравнивает исходную кривую с другой
        /// </summary>
        bool CompareTo(ICurve curve);
    }

    /// <summary>
    /// Класс точки
    /// </summary>
    public class Point : IPoint
    {
        float x;
        float y;
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public bool CompareTo(IPoint p)
        {
            if (this.X == p.X && this.Y == p.Y)
                return true;
            return false;
        }

        public object Clone()
        {
            return new Point(x, y);
        }
    }

    /// <summary>
    /// Абстрактный класс кривой
    /// </summary>
    public abstract class ACurve : ICurve
    {
        public abstract void GetPoint(double t, out IPoint p);

        public double GetLength(double t, IStopper stop)
        {
            if (t == 0)
                return 0;
            IPoint p1, p2;
            double a = 0;
            this.GetPoint(a, out p1);
            double len = 0;
            int i;
            for (i = 1; stop.Stop(len, (double)i / Params.n, t); i++)
            {
                a = (double)i / Params.n;
                this.GetPoint(a, out p2);
                len += Math.Pow(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2), 0.5);
                p1 = p2;
            }
            t = len * a / t;
            return t;
        }
        public double GetT(double len, IStopper stop)
        {
            return GetLength(len, stop);
        }
        public AComposite GetComposite()
        {
            return null;
        }
        public abstract bool CompareTo(ICurve curve);
        public abstract object Clone();
    }

    /// <summary>
    /// Класс линии
    /// </summary>
    public class Line : ACurve
    {
        IPoint a;
        IPoint b;
        public Line(IPoint a, IPoint b)
        {
            this.a = a;
            this.b = b;
        }
        public override void GetPoint(double t, out IPoint p)
        {
            float x_0 = (float)((1 - t) * a.X + t * b.X);
            float y_0 = (float)((1 - t) * a.Y + t * b.Y);
            p = new Point(x_0, y_0);
        }
        public override bool CompareTo(ICurve curve)
        {
            if (!(curve is Line))
                return false;
            else
            {
                if (this.a.CompareTo((curve as Line).a) && this.b.CompareTo((curve as Line).b))
                    return true;
                else return false;
            }
        }

        public override object Clone()
        {
            return new Line((IPoint)a.Clone(), (IPoint)b.Clone());
        }
    }

    /// <summary>
    /// Класс кривой Безье
    /// </summary>
    public class Bezier : ACurve
    {
        IPoint a;
        IPoint b;
        IPoint c;
        IPoint d;
        public Bezier(IPoint a, IPoint c, IPoint d, IPoint b)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
        public override void GetPoint(double t, out IPoint p)
        {
            float x_0 = (float)(Math.Pow(1 - t, 3) * a.X + 3 * t * Math.Pow(1 - t, 2) * c.X + 3 * t * t * (1 - t) * d.X + t * t * t * b.X);
            float y_0 = (float)(Math.Pow(1 - t, 3) * a.Y + 3 * t * Math.Pow(1 - t, 2) * c.Y + 3 * t * t * (1 - t) * d.Y + t * t * t * b.Y);
            p = new Point(x_0, y_0);
        }
        public override bool CompareTo(ICurve curve)
        {
            if (!(curve is Bezier))
                return false;
            else
            {
                if (this.a.CompareTo((curve as Bezier).a) && this.b.CompareTo((curve as Bezier).b) && this.c.CompareTo((curve as Bezier).c) && this.d.CompareTo((curve as Bezier).d))
                    return true;
                else return false;
            }
        }

        public override object Clone()
        {
            return new Bezier((IPoint)a.Clone(), (IPoint)c.Clone(), (IPoint)d.Clone(), (IPoint)b.Clone());
        }
    }

    /// <summary>
    /// Имитирует часть кривой от t_start до t_finish
    /// </summary>
    public class Fragment : ICurve
    {
        ICurve comp;
        double t_start;
        double t_finish;
        public Fragment(ICurve comp, double t_start, double t_finish)
        {
            this.comp = comp;
            this.t_start = t_start;
            this.t_finish = t_finish;
        }
        public void GetPoint(double t, out IPoint p)
        {
            if (t <= t_start)
                comp.GetPoint(t_start, out p);
            else if (t >= t_finish)
                comp.GetPoint(t_finish, out p);
            else comp.GetPoint(t, out p);
        }
        public double GetLength(double t, IStopper stop)
        {
            if (t <= t_start)
                return 0;
            else if (t >= t_finish)
                return comp.GetLength(t_finish, stop) - comp.GetLength(t_start, stop);
            else return comp.GetLength(t, stop) - comp.GetLength(t_start, stop);
        }
        public double GetT(double len, IStopper stop)
        {
            double t = comp.GetT(len, stop);
            if (t <= t_start)
                return 0;
            else if (t >= t_finish)
                return 1;
            else return t - t_start;
        }

        public AComposite GetComposite()
        {
            return comp.GetComposite();
        }
        public bool CompareTo(ICurve curve)
        {
            if (!(curve is Fragment))
                return false;
            else
            {
                if ((curve as Fragment).comp.CompareTo(comp) && (this.t_start == (curve as Fragment).t_start) && (this.t_finish == (curve as Fragment).t_finish))
                    return true;
                else return false;
            }
        }
        public object Clone()
        {
            return new Fragment((ICurve)comp.Clone(), t_start, t_finish);
        }
    }

    /// <summary>
    /// Имитирует сдвиг кривой в новую точку
    /// </summary>
    public class MoveTo : ICurve
    {
        ICurve comp;
        IPoint new_p;
        public MoveTo(ICurve comp, IPoint new_p)
        {
            this.comp = comp;
            this.new_p = new_p;
        }
        public void GetPoint(double t, out IPoint p)
        {
            if (t == 0)
                p = new_p;
            else
            {
                comp.GetPoint(0, out p);
                float x_offset = new_p.X - p.X;
                float y_offset = new_p.Y - p.Y;
                comp.GetPoint(t, out p);
                p.X += x_offset;
                p.Y += y_offset;
            }
        }
        public double GetLength(double t, IStopper stop)
        {
            return comp.GetLength(t, stop);
        }

        public double GetT(double len, IStopper stop)
        {
            return comp.GetT(len, stop);
        }

        public AComposite GetComposite()
        {
            return comp.GetComposite();
        }

        public bool CompareTo(ICurve curve)
        {
            if(curve is MoveTo)
                return (curve as MoveTo).comp.CompareTo(this.comp);
            else return curve.CompareTo(comp);
        }
        public object Clone()
        {
            return new MoveTo((ICurve)comp.Clone(), new_p);
        }
    }


    public interface Iterable
    {
        int Iterate(Func<ICurve, bool> funс);
    }

    public abstract class AComposite : ICurve, Iterable
    {
        protected List<ICurve> items;
        public int Count { get { return items.Count; } }
        public abstract void GetPoint(double t, out IPoint p);
        public abstract double GetLength(double t, IStopper stop);
        public abstract double GetT(double len, IStopper stop);
        public abstract bool Add(ICurve curve);
        public abstract object Clone();
        public bool RemoveLast()
        {
            if (Count > 0)
            {
                items.RemoveAt(Count - 1);
                return true;
            }
            else return false;
        }
        public AComposite GetComposite()
        {
            return this;
        }
        public bool CompareTo(ICurve curve)
        {
            if (!(curve is AComposite))
                return false;
            else
            {
                if ((curve as AComposite).Count != this.Count)
                    return false;
                else
                {
                    for (int i = 0; i < Count; i++)
                        if (!items[i].CompareTo((curve as AComposite).items[i]))
                            return false;
                    return true;
                }
            }
        }
        class Iterator
        {
            AComposite agg;
            int pos = -1;
            public Iterator(AComposite agg)
            {
                this.agg = agg;
            }
            public object Current { get { return agg.items[pos]; } }
            public bool MoveNext()
            {
                if (pos + 1 < agg.items.Count)
                {
                    pos++;
                    return true;
                }
                else
                {
                    Reset();
                    return false;
                }
            }
            public void Reset() { pos = -1; }
        }
        public int Iterate(Func<ICurve, bool> funс)
        {
            int k = 0;
            Iterator iter = new Iterator(this);
            iter.Reset();
            while (iter.MoveNext())
            {
                ICurve c = iter.Current as ICurve;
                if (c.GetComposite() != null)
                    k += c.GetComposite().Iterate(funс);
                else
                {
                    funс(c);
                    k++;
                }
            }
            return k;
        }

        public List<ICurve> Distinct()
        {
            List<ICurve> dubl = this.Dublicates();
            List<ICurve> list = new List<ICurve>(items);
            if (dubl.Count > 0)
            {    
                while (dubl.Count > 0)
                {
                    foreach (ICurve l in list)
                    {
                        if(l.CompareTo(dubl[0]))
                        {
                            list.Remove(l);
                            dubl.RemoveAt(0);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        public List<ICurve> Dublicates()
        {
            List<ICurve> list = new List<ICurve>();
            this.Iterate((curve) =>
            {
                int i = 0;
                this.Iterate((c) =>
                {
                    if (c.CompareTo(curve))
                    {
                        i++;
                    }
                    return true;
                });
                if ((list.Count == 0) || (list.Count > 0 && i > 1 && list.Where(x => x.CompareTo(curve)).ToList().Count > 0))
                    list.Add(curve);
                return true;
            });
            return list;
        }
    }

    /// <summary>
    /// Создаёт цепочку из двух кривых и позволяет работать с ними, как с одной
    /// </summary>
    public class Link : AComposite
    {
        public Link(ICurve curve_1, ICurve curve_2)
        {
            items = new List<ICurve>();
            items.Add(curve_1);
            IPoint p;
            curve_1.GetPoint(1, out p);
            items.Add(new MoveTo(curve_2, p));
        }
        public override object Clone()
        {
            return new Link((ICurve)items[0].Clone(), (ICurve)items[1].Clone());
        }
        public override void GetPoint(double t, out IPoint p)
        {
            if (Count > 0)
            {
                double L = this.GetLength(1, new StopLength());
                double l = items[0].GetLength(1, new StopLength());
                if (t <= l / L)
                    items[0].GetPoint(t * L / l, out p);
                else
                    items[1].GetPoint((t - l / L) * ((double)1 / (1 - l / L)), out p);
            }
            else throw new ArgumentNullException("Count", "Кривых нет!");
        }
      
        public override double GetLength(double t, IStopper stop)
        {
            if (Count > 0 && t >0)
            {
                if (t <= 0.5)
                    return items[0].GetLength(t * 2, stop);
                else
                    return items[0].GetLength(1, stop) + items[1].GetLength((t - 0.5) * 2, stop);
            }
            else return 0;
        }
        public override double GetT(double len, IStopper stop)
        {
            if (Count > 0 && len > 0)
            {
                double l = items[0].GetLength(1, new StopLength());
                if (len <= l)
                    return items[0].GetT(len, stop);
                else
                    return items[1].GetT(len - l, stop);
            }
            else return 0;
        }
        public override bool Add(ICurve curve)
        {
            if (curve == null || Count == 2)
                return false;
            items.Add(curve);
            if (Count > 1)
            {
                IPoint p;
                items[Count - 2].GetPoint(1, out p);
                items[Count - 1] = new MoveTo(items.Last(), p);
            }
            return true;
        }
    }

    /// <summary>
    /// Создаёт цепучку из набора кривых и позволяет работать с ними, как с одной
    /// </summary>
    public class Chain : AComposite
    {
        public Chain(params ICurve[] curves)
        {
            items = new List<ICurve>();
            if (curves.Count() > 1)
            {
                items.Add(curves[0]);
                IPoint p;
                for (int i = 1; i < curves.Count(); i++)
                {
                    items[i - 1].GetPoint(1, out p);
                    items.Add(new MoveTo(curves[i], p));
                }
            }
            else items = curves.ToList();
        }
        public override object Clone()
        {
            List<ICurve> list = new List<ICurve>();
            foreach (ICurve c in items)
                list.Add((ICurve)c.Clone());
            return new Chain(list.ToArray());
        }
        public override void GetPoint(double t, out IPoint p)
        {
            if (Count > 0)
            {
                int k = 0;
                for (double i = 0, j = (double)1 / Count; !(t >= i && t <= j) && j < 1; i += (double)1 / Count, j += (double)1 / Count)
                    k++;
                items[k].GetPoint((t - (double)k / Count) * Count, out p);
            }
            else throw new ArgumentNullException("Count", "Кривых нет!");
        }

        public override double GetLength(double t, IStopper stop)
        {
            if (Count > 0 && t > 0)
            {

                int k = 0;
                for (double i = 0, j = (double)1 / Count; !(t >= i && t <= j) && j < 1; i += (double)1 / Count, j += (double)1 / Count)
                    k++;
                return items.FindAll(curve => items.IndexOf(curve) < k).ToList().Sum(curve => curve.GetLength(1, stop)) + items[k].GetLength((t - (double)k / Count) * Count, stop);
            }
            else return 0;
        }

        public override double GetT(double len, IStopper stop)
        {
            if (Count > 0 && len > 0)
            {
                IStopper st = new StopLength();
                int k = 0;
                double l1 = 0;
                for (double i = 0, j = (double)1 / Count, l2 = items[k].GetLength(1, st); !(len >= l1 && len <= l2) && j < 1; l1 = l2, l2 += items[k++].GetLength(1, st), i += (double)1 / Count, j += (double)1 / Count) ;
                return (double)k / Count + items[k].GetT(len - l1, stop);
            }
            else return 0;
        }

        public override bool Add(ICurve curve)
        {
            if (curve == null)
                return false;
            items.Add(curve);
            if (Count > 1)
            {
                IPoint p;
                items[Count - 2].GetPoint(1, out p);
                items[Count - 1] = new MoveTo(items.Last(), p);
            }
            return true;
        }
    }
}
