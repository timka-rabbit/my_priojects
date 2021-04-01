using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopperLength
{
    /// <summary>
    /// Интерфейс для работы с остановом
    /// </summary>
    public interface IStopper
    {
        bool Stop(double L, double t, double param);
    }

    /// <summary>
    /// Класс сравнение для получения параметра t
    /// </summary>
    public class StopT : IStopper
    {
        public bool Stop(double len, double t, double param)
        {
            return len < param;
        }
    }

    /// <summary>
    /// Класс сравнение для получения длины L
    /// </summary>
    public class StopLength : IStopper
    {
        public bool Stop(double len, double t, double param)
        {
            return t < param;
        }
    }
}
