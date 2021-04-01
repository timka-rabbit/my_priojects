using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointClass;

namespace MathClass
{
    /// <summary>
    /// Класс рандомайзера
    /// </summary>
    public static class Rand
    {
        public static Random rand;

        static Rand()
        {
            rand = new Random();
        }
    }

    /// <summary>
    /// Класс поиска расстояния
    /// </summary>
    public static class Dist
    {
        /// <summary>
        /// Расстояние между точками в Евклидовой метрике
        /// </summary>
        /// <param name="p1"> Первая точка </param>
        /// <param name="p2"> Вторая точка </param>
        /// <returns> Расстояние между точками в Евклидовой метрике </returns>
        public static double GetEuclideanDist(IPoint2D p1, IPoint2D p2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(p1.X - p2.X), 2) + Math.Pow(Math.Abs(p1.Y - p2.Y), 2));
        }

        /// <summary>
        /// Расстояние между точками в ортогональной (Манхэттенской) метрике
        /// </summary>
        /// <param name="p1"> Первая точка </param>
        /// <param name="p2"> Вторая точка </param>
        /// <returns> Расстояние между точками в Манхэттенской метрике </returns>
        public static double GetOrthogonalDist(IPoint2D p1, IPoint2D p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }
}
