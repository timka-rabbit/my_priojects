using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointClass
{
    /// <summary>
    /// Интерфейс точки в двучмерной системе координат
    /// </summary>
    public interface IPoint2D
    {
        /// <summary>
        /// Координата X
        /// </summary>
        double X { get; }

        /// <summary>
        /// Координата Y
        /// </summary>
        double Y { get; }
    }

    /// <summary>
    /// Класс точки в двучмерной системе координат
    /// </summary>
    public class Point2D : IPoint2D
    {
        double x; // координата X
        double y; // координата Y

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Point2D()
        {
            this.x = 0;
            this.y = 0;
        }

        /// <summary>
        /// Конструктор, принимающий 2 координаты
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        public Point2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="p"> Точка для копирования координат </param>
        public Point2D(IPoint2D p)
        {
            this.x = p.X;
            this.y = p.Y;
        }

        /// <summary>
        /// Координата X точки
        /// </summary>
        public double X { get { return x; } }

        /// <summary>
        /// Координата Y точки
        /// </summary>
        public double Y { get { return y; } }

        /// <summary>
        /// Сравнивает данную точку с другой
        /// </summary>
        /// <param name="p"> Другая точка для сравнения </param>
        /// <returns> Результат сравнения (true - равны, false - не равны) </returns>
        public bool Equals(IPoint2D p)
        {
            if (p == null)
                return false;
            return (this.X == p.X) && (this.Y == p.Y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IPoint2D);
        }
    }
}
