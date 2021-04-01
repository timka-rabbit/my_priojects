using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointClass;
using MathClass;

namespace AlgorithmClass
{
    /// <summary>
    /// Родительский класс алгоритмов построения дерева Штейнера
    /// </summary>
    /// <typeparam name="T"> Тип точек </typeparam>
    public class ShteinerAlghorithm<T>
        where T : class, IPoint2D
    {
        /// <summary>
        /// Получение суммарной длины дерева
        /// </summary>
        /// <param name="x"> Вектор решения </param>
        /// <param name="lens"> Длины Рёбер </param>
        /// <returns></returns>
        protected static double Q(int[] x, double[] lens)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * lens[i];
            return sum;
        }

        
        /// <summary>
        /// Проверка на связность набора точек
        /// </summary>
        /// <param name="x"> Массив взятых точек </param>
        /// <param name="pairs_of_verts"> Пары точек </param>
        /// <param name="points"> Исходные точки </param>
        /// <returns> true - есть связность, false - нет </returns>
        protected static bool Check(int[] x, List<Tuple<T, T>> pairs_of_verts, List<T> points)
        {
            // из множества всех пар берём те, которые выбрались в векторе решений
            var pairs_entered = new List<Tuple<T, T>>();
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1)
                {
                    pairs_entered.Add(pairs_of_verts[i]);
                }
            }
            if (pairs_entered.Count == 0)
                return false;

            // выбираем все уникальные точки из пар
            List<T> unique_points = new List<T>();
            for (int i = 0; i < pairs_entered.Count; i++)
            {
                if (unique_points.Find(p => p.Equals(pairs_entered[i].Item1)) == null)
                    unique_points.Add(pairs_entered[i].Item1);
                if (unique_points.Find(p => p.Equals(pairs_entered[i].Item2)) == null)
                    unique_points.Add(pairs_entered[i].Item2);
            }

            // формируем матрицу связности для полученных уникальных точек
            double[,] matrix = new double[unique_points.Count, unique_points.Count];
            for (int i = 0; i < pairs_entered.Count; i++)
            {
                int num_1 = unique_points.FindIndex(p => p.Equals(pairs_entered[i].Item1));
                int num_2 = unique_points.FindIndex(p => p.Equals(pairs_entered[i].Item2));
                matrix[num_1, num_2] = matrix[num_2, num_1] = 1;
            }

            bool[] exist = new bool[points.Count];
            bool[] visited = new bool[matrix.GetLength(0)];
            // поиск в ширину
            CheckConnectIter(0, matrix, unique_points, points, ref visited, ref exist);
            // если посетили все вершины и все исходные точки, то связность есть
            if (!visited.Contains(false) && !exist.Contains(false))
            {
                int[,] matrix_points = new int[points.Count, points.Count];
                visited = new bool[matrix.GetLength(0)];
                // проверяем на наличие циклов
                if (CyclesSearch(matrix).Count == 0)
                    return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Поиск в ширину на связность
        /// </summary>
        /// <param name="vert"> Номер посещаемой точки </param>
        /// <param name="matrix"> Матрица смежности всех точек </param>
        /// <param name="unique_points"> Все точки </param>
        /// <param name="points"> Исходные точки </param>
        /// <param name="visited"> Посещение всех точек </param>
        /// <param name="exist"> Вхождение исходных точек </param>
        private static void CheckConnectIter(int vert, double[,] matrix, List<T> unique_points, List<T> points, ref bool[] visited, ref bool[] exist)
        {
            // помечаем текущую точку
            visited[vert] = true;
            // проверяем является ли этаточка одной из исходных
            int point_num = points.FindIndex(p => p.Equals(unique_points[vert]));
            if (point_num != -1)
                exist[point_num] = true;
            // итерация поиска
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (vert != i && matrix[vert, i] != 0 && !visited[i])
                    CheckConnectIter(i, matrix, unique_points, points, ref visited, ref exist);
            }
        }

        /// <summary>
        /// Метод поиска циклов в графе (в глубину)
        /// </summary>
        /// <returns> Список циклов </returns>
        protected static List<List<int>> CyclesSearch(double[,] matr)
        {
            int N = matr.GetLength(0);
            int[] color = new int[N];
            var cycles = new List<List<int>>();
            for (int i = 0; i < N; i++)
            {
                for (int k = 0; k < N; k++)
                    color[k] = 1;
                List<int> cycle = new List<int>();
                cycle.Add(i);
                DFScycle(matr, i, i, color, -1, cycle, ref cycles);
            }
            return cycles;
        }

        /// <summary>
        /// Ищет номер вершины
        /// </summary>
        /// <param name="num"> Номер ребра </param>
        /// <param name="pos"> Номер вершины (true - первая, false - вторая) </param>
        private static int getVert(double[,] matr, int num, bool pos)
        {
            int N = matr.GetLength(0);
            int edgesCount = 0;
            int i_0 = 0;
            int j_0 = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (matr[i, j] != 0)
                    {
                        edgesCount++;
                        if (num == edgesCount - 1)
                        {
                            i_0 = i;
                            j_0 = j;
                            break;
                        }
                    }
                }
                if (num == edgesCount - 1)
                    break;
            }
            return pos ? i_0 : j_0;
        }

        /// <summary>
        /// Метод сравнения двух списков
        /// </summary>
        private static bool ListEquals(List<int> l1, List<int> l2)
        {
            if (l1.Count != l2.Count)
                return false;
            for (int i = 0; i < l1.Count; i++)
                if (l1[i] != l2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Количество рёбер в графе
        /// </summary>
        /// <param name="matr"> Матрица смежности </param>
        /// <returns> Количество рёбер в графе </returns>
        private static int Edges(double[,] matr)
        {
            int N = matr.GetLength(0);
            int edgesCount = 0;
            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    if (matr[i, j] != 0)
                        edgesCount++;
            return edgesCount;
        }

        /// <summary>
        /// Итерация поиска в глубину для поиска циклов
        /// </summary>
        /// <param name="startV"> Начальная вершина рассматриваемого ребра </param>
        /// <param name="endV"> Конечная вершина цикла </param>
        /// <param name="color"> Цвета вершин </param>
        /// <param name="unavailableEdge"> Ребро, по которому не нужно переходить на следующей итерации </param>
        /// <param name="cycle"> Текущий цикл </param>
        /// <param name="cycles"> Список циклов </param>
        private static void DFScycle(double[,] matr, int startV, int endV, int[] color, int unavailableEdge, List<int> cycle, ref List<List<int>> cycles)
        {
            if (cycles.Count > 0)
                return;
            //если startV == endV, то эту вершину перекрашивать не нужно, иначе мы в нее не вернемся, а вернуться необходимо
            if (startV != endV)
                color[startV] = 2;
            else if (cycle.Count >= 2)
            {
                cycle.Reverse();
                bool flag = false; //проверка на палиндром для этого цикла графа в списке цмклов
                for (int i = 0; i < cycles.Count; i++)
                {
                    if (ListEquals(cycles[i], cycle))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    cycle.Reverse();
                    cycles.Add(cycle);
                }
                return;
            }

            int countEdges = Edges(matr);

            for (int w = 0; w < countEdges; w++)
            {
                if (w == unavailableEdge) // если мы переходили по этому ребру на
                    continue;             // предыдущей итерации, то пропускаем его
                int v1 = getVert(matr, w, true);
                int v2 = getVert(matr, w, false);
                if (color[v2] == 1 && v1 == startV)
                {
                    List<int> cycleNew = new List<int>(cycle);
                    cycleNew.Add(v2);
                    DFScycle(matr, v2, endV, color, w, cycleNew, ref cycles);
                    color[v2] = 1;
                }
                else if (color[v1] == 1 && v2 == startV)
                {
                    List<int> cycleNew = new List<int>(cycle);
                    cycleNew.Add(v1);
                    DFScycle(matr, v1, endV, color, w, cycleNew, ref cycles);
                    color[v1] = 1;
                }
            }
        }
    }

    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Алгоритм Краскала построения минимального остовного дерева
    /// </summary>
    /// <typeparam name="T"> Тип точек </typeparam>
    public class KruskalAlgorithm<T> : ShteinerAlghorithm<T>
        where T : class, IPoint2D
    {
        /// <summary>
        /// Построение дерева штейнера алгоритмом Краскала
        /// </summary>
        /// <param name="points"> Опорные точки для построения </param>
        /// <returns> Список рёбер дерева </returns>
        public static dynamic Solve(List<T> points)
        {
            if (points.Count < 2)
                return null;

            var pairs = new List<Tuple<Tuple<T, T>, double>>();

            // составляем пары вершин
            for (int i = 0; i < points.Count; i++)
                for (int j = i + 1; j < points.Count; j++)
                    pairs.Add(new Tuple<Tuple<T, T>, double>(new Tuple<T, T>(points[i], points[j]), Dist.GetOrthogonalDist(points[i], points[j])));

            // упорядочиваем пары вершин по длинам рёбер
            pairs.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));

            var road = new List<Tuple<Tuple<T, T>, double>>();

            double[,] matr = new double[points.Count, points.Count];

            int[] parity = new int[points.Count];
            for (int i = 0; i < pairs.Count; i++)
            {
                if (road.Count < 2)
                {
                    int vert_1_ind = points.IndexOf(pairs[i].Item1.Item1);
                    int vert_2_ind = points.IndexOf(pairs[i].Item1.Item2);
                    road.Add(pairs[i]);
                    parity[vert_1_ind]++;
                    parity[vert_2_ind]++;
                    matr[vert_1_ind, vert_2_ind] = matr[vert_2_ind, vert_1_ind] = pairs[i].Item2;
                }
                // проверка на цикличность
                else
                {
                    int vert_1_ind = points.IndexOf(pairs[i].Item1.Item1);
                    int vert_2_ind = points.IndexOf(pairs[i].Item1.Item2);
                    parity[vert_1_ind]++;
                    parity[vert_2_ind]++;
                    matr[vert_1_ind, vert_2_ind] = matr[vert_2_ind, vert_1_ind] = pairs[i].Item2;
                    // проверка на наличие цикла
                    if (CyclesSearch(matr).Count > 0)
                    {
                        parity[vert_1_ind]--;
                        parity[vert_2_ind]--;
                        matr[vert_1_ind, vert_2_ind] = matr[vert_2_ind, vert_1_ind] = 0;
                    }
                    else road.Add(pairs[i]);
                }

                // проверка на связность графа
                bool[] visited = new bool[points.Count];
                WidthSearch(points.IndexOf(pairs[0].Item1.Item1), matr, ref visited);
                if (parity.ToList().IndexOf(0) == -1 && visited.ToList().IndexOf(false) == -1)
                    break;
            }

            var new_road = new List<Tuple<T, T>>();
            for (int i = 0; i < road.Count; i++)
            {
                new_road.Add(new Tuple<T, T>
                    (new Point2D(road[i].Item1.Item1.X, road[i].Item1.Item1.Y) as T, new Point2D(road[i].Item1.Item1.X, road[i].Item1.Item2.Y) as T));
                new_road.Add(new Tuple<T, T>
                    (new Point2D(road[i].Item1.Item1.X, road[i].Item1.Item2.Y) as T, new Point2D(road[i].Item1.Item2.X, road[i].Item1.Item2.Y) as T));
            }

            // вырожденный случай, когда все точки находятся на одной линии
            if (points.FindAll(p => p.X == points[0].X).Count == points.Count ||
                points.FindAll(p => p.Y == points[0].Y).Count == points.Count)
                return new_road;

            for (int i = 0; i < new_road.Count; i++)
            {
                int ind_x = new_road.FindIndex(t => (!t.Item1.Equals(new_road[i].Item1) && !t.Item2.Equals(new_road[i].Item2))
                    && (t.Item1.Y == new_road[i].Item1.Y) && (t.Item2.Y == new_road[i].Item2.Y) && (t.Item1.Y == new_road[i].Item2.Y));
                if (ind_x > -1)
                {
                    if ((new_road[ind_x].Item1.X == new_road[i].Item1.X)
                        && (new_road[ind_x].Item1.X <= new_road[i].Item2.X)
                        && (new_road[ind_x].Item2.X <= new_road[i].Item2.X))
                    {
                        new_road[i] = new Tuple<T, T>
                            (new Point2D(new_road[ind_x].Item2.X, new_road[ind_x].Item2.Y) as T, new Point2D(new_road[i].Item2.X, new_road[i].Item2.Y) as T);
                    }
                    else if ((new_road[ind_x].Item2.X == new_road[i].Item2.X)
                        && (new_road[ind_x].Item1.X <= new_road[i].Item2.X)
                        && (new_road[ind_x].Item1.X <= new_road[i].Item1.X))
                    {
                        new_road[i] = new Tuple<T, T>
                            (new Point2D(new_road[ind_x].Item1.X, new_road[ind_x].Item1.Y) as T, new Point2D(new_road[i].Item2.X, new_road[i].Item2.Y) as T);
                    }
                }
                else
                {
                    int ind_y = new_road.FindIndex(t => (!t.Item1.Equals(new_road[i].Item1) && !t.Item2.Equals(new_road[i].Item2))
                    && (t.Item1.X == new_road[i].Item1.X) && (t.Item2.X == new_road[i].Item2.X) && (t.Item1.X == new_road[i].Item2.X));
                    if (ind_y > -1)
                    {
                        if ((new_road[ind_y].Item1.Y == new_road[i].Item1.Y)
                            && (new_road[ind_y].Item1.Y <= new_road[i].Item2.Y)
                            && (new_road[ind_y].Item2.Y <= new_road[i].Item2.Y))
                        {
                            new_road[i] = new Tuple<T, T>
                                (new Point2D(new_road[ind_y].Item2.X, new_road[ind_y].Item2.Y) as T, new Point2D(new_road[i].Item2.X, new_road[i].Item2.Y) as T);
                        }
                        else if ((new_road[ind_y].Item2.Y == new_road[i].Item2.Y)
                            && (new_road[ind_y].Item1.Y <= new_road[i].Item2.Y)
                            && (new_road[ind_y].Item1.Y <= new_road[i].Item1.Y))
                        {
                            new_road[i] = new Tuple<T, T>
                                (new Point2D(new_road[ind_y].Item1.X, new_road[ind_y].Item1.Y) as T, new Point2D(new_road[i].Item2.X, new_road[i].Item2.Y) as T);
                        }
                    }
                }
            }

            // разделяем все рёбра на линии, лежащие на одной оси
            var solution = new List<Tuple<T, T>>();
            for (int i = 0; i < new_road.Count; i++)
            {
                if ((new_road[i].Item1.X == new_road[i].Item2.X) || (new_road[i].Item1.Y == new_road[i].Item2.Y))
                {
                    solution.Add(new_road[i]);
                }
                else
                {
                    solution.Add(new Tuple<T, T>(
                        new Point2D(new_road[i].Item1.X, new_road[i].Item1.Y) as T,
                        new Point2D(new_road[i].Item1.X, new_road[i].Item2.Y) as T));
                    solution.Add(new Tuple<T, T>(
                        new Point2D(new_road[i].Item1.X, new_road[i].Item2.Y) as T,
                        new Point2D(new_road[i].Item2.X, new_road[i].Item2.Y) as T));
                }
            }

            // отсортируем правильно все пары точек (чтобы первая точка была выше/правее второй)
            for(int i = 0; i < solution.Count; i++)
            {
                if(solution[i].Item1.X == solution[i].Item2.X)
                {
                    if(solution[i].Item1.Y <= solution[i].Item2.Y)
                    {
                        solution.Insert(i, new Tuple<T, T>(solution[i].Item2, solution[i].Item1));
                        solution.RemoveAt(i + 1);
                    }
                }
                else if(solution[i].Item1.Y == solution[i].Item2.Y)
                {
                    if (solution[i].Item1.X <= solution[i].Item2.X)
                    {
                        solution.Insert(i, new Tuple<T, T>(solution[i].Item2, solution[i].Item1));
                        solution.RemoveAt(i + 1);
                    }
                }
            }

            // разделяем накладывающиеся рёбра
            int k = 0;
            while (k < solution.Count)
            {
                if(solution[k].Item1.Equals(solution[k].Item2))
                {
                    solution.RemoveAt(k);
                }
                if (solution[k].Item1.X == solution[k].Item2.X)
                {
                    int ind = solution.FindLastIndex(edge => ((edge.Item1.X == edge.Item2.X)
                        && (edge.Item1.Y == solution[k].Item1.Y || edge.Item2.Y == solution[k].Item2.Y)));
                    if (ind != -1 && ind != k)
                    {
                        if (solution[k].Item2.Equals(solution[ind].Item2))
                        {
                            if (solution[k].Item1.Y < solution[ind].Item1.Y)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[ind].Item1.X, solution[ind].Item1.Y) as T,
                                    new Point2D(solution[k].Item1.X, solution[k].Item1.Y) as T));
                                solution.RemoveAt(ind);
                                k = -1;
                            }
                            else if (solution[k].Item1.Y > solution[ind].Item1.Y)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[k].Item1.X, solution[k].Item1.Y) as T,
                                    new Point2D(solution[ind].Item1.X, solution[ind].Item1.Y) as T));
                                solution.RemoveAt(k);
                                k = -1;
                            }
                            else
                            {
                                solution.RemoveAt(k);
                                k = -1;
                            }
                        }
                        else if (solution[k].Item1.Equals(solution[ind].Item1))
                        {
                            if (solution[k].Item2.Y < solution[ind].Item2.Y)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[ind].Item2.X, solution[ind].Item2.Y) as T,
                                    new Point2D(solution[k].Item2.X, solution[k].Item2.Y) as T));
                                solution.RemoveAt(k);
                                k = -1;
                            }
                            else if (solution[k].Item2.Y > solution[ind].Item2.Y)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[k].Item2.X, solution[k].Item2.Y) as T,
                                    new Point2D(solution[ind].Item2.X, solution[ind].Item2.Y) as T));
                                solution.RemoveAt(ind);
                                k = -1;
                            }
                            else
                            {
                                solution.RemoveAt(k);
                                k = -1;
                            }
                        }
                    }
                }

                else if (solution[k].Item1.Y == solution[k].Item2.Y)
                {
                    int ind = solution.FindLastIndex(edge => ((edge.Item1.Y == edge.Item2.Y)
                          && (edge.Item1.X == solution[k].Item1.X || edge.Item2.X == solution[k].Item2.X)));
                    if (ind != -1 && ind != k)
                    {
                        if (solution[k].Item2.Equals(solution[ind].Item2))
                        {
                            if (solution[k].Item1.X < solution[ind].Item1.X)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[ind].Item1.X, solution[ind].Item1.Y) as T,
                                    new Point2D(solution[k].Item1.X, solution[k].Item1.Y) as T));
                                solution.RemoveAt(ind);
                                k = -1;
                            }
                            else if (solution[k].Item1.X > solution[ind].Item1.X)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[k].Item1.X, solution[k].Item1.Y) as T,
                                    new Point2D(solution[ind].Item1.X, solution[ind].Item1.Y) as T));
                                solution.RemoveAt(k);
                                k = -1;
                            }
                            else
                            {
                                solution.RemoveAt(k);
                                k = -1;
                            }
                        }
                        else if (solution[k].Item1.Equals(solution[ind].Item1))
                        {
                            if (solution[k].Item2.X < solution[ind].Item2.X)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[ind].Item2.X, solution[ind].Item2.Y) as T,
                                    new Point2D(solution[k].Item2.X, solution[k].Item2.Y) as T));
                                solution.RemoveAt(k);
                                k = -1;
                            }
                            else if (solution[k].Item2.X > solution[ind].Item2.X)
                            {
                                solution.Add(new Tuple<T, T>(
                                    new Point2D(solution[k].Item2.X, solution[k].Item2.Y) as T,
                                    new Point2D(solution[ind].Item2.X, solution[ind].Item2.Y) as T));
                                solution.RemoveAt(ind);
                                k = -1;
                            }
                            else
                            {
                                solution.RemoveAt(k);
                                k = -1;
                            }
                        }
                    }
                }
                k++;
            }

            return solution;
        }

        /// <summary>
        /// Поиск в ширину для проверки связности
        /// </summary>
        /// <param name="vert"></param>
        /// <param name="matr"></param>
        /// <param name="visited"></param>
        protected static void WidthSearch(int vert, double[,] matr, ref bool[] visited)
        {
            visited[vert] = true;
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                if (vert != i && matr[vert, i] != 0 && !visited[i])
                    WidthSearch(i, matr, ref visited);
            }
        }
    }

    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Алгоритма полного перебора постороения минимального дерева Штейнера
    /// </summary>
    /// <typeparam name="T"> Тип элементов </typeparam>
    public class BruteForce<T> : ShteinerAlghorithm<T>
        where T : class, IPoint2D
    {
        /// <summary>
        /// Построение дерева Штейнера полным перебором
        /// </summary>
        /// <param name="points"> Список вершин графа </param>
        /// <returns> Список рёбер </returns>
        public static dynamic Solve(List<T> points)
        {
            if (points.Count < 2)
                return null;

            /*********************************************************************************/
            /* 1) Необходимо построить сеточный граф на основе исходных точек                */
            /* 2) Затем вызвать полный перебор всех решений, т.е. перебирать                 */
            /*    все возможные комбинации рёбер                                             */
            /* Количество рёбер рассматриваемых для решения урезано до половины от сеточного */
            /* графа, т.к. "дорога", длинной больше этой, точно не будет оптимальной         */
            /*********************************************************************************/

            List<double> x_list = new List<double>();
            List<double> y_list = new List<double>();
            for(int i = 0; i < points.Count; i++)
            {
                x_list.Add(points[i].X);
                y_list.Add(points[i].Y);
            }
            x_list.Sort();
            y_list.Sort();
            x_list = x_list.Distinct().ToList();
            y_list = y_list.Distinct().ToList();

            int width = x_list.Count - 1;
            int height = y_list.Count - 1;

            // количество рёбер = сумма чётностей вершин пополам
            int countEdges = (4 * 2 + 2 * 3 * (height - 1) + 2 * 3 * (width - 1) + 4 * (height - 1) * (width - 1)) / 2;
            
            // заполняем массивы длин рёбер и пар точек
            double[] lens = new double[countEdges];
            var pairs_of_verts = new Tuple<T, T>[countEdges]; 
            for(int i = 0; i <= height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    lens[i * width + i * (width + 1) + j] = x_list[j + 1] - x_list[j];
                    pairs_of_verts[i * width + i * (width + 1) + j] = new Tuple<T, T>(new Point2D(x_list[j], y_list[i]) as T, new Point2D(x_list[j + 1], y_list[i]) as T);
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j <= width; j++)
                {
                    lens[(i + 1) * width + i * (width + 1) + j] = y_list[i + 1] - y_list[i];
                    pairs_of_verts[(i + 1) * width + i * (width + 1) + j] = new Tuple<T, T>(new Point2D(x_list[j], y_list[i]) as T, new Point2D(x_list[j], y_list[i + 1]) as T);
                }
            }

            // вырожденный случай, когда все точки находятся на одной линии
            if (x_list.Count == 1 || y_list.Count == 1)
                return pairs_of_verts.ToList();

            int[] x = new int[countEdges];
            // решаем полным перебором
            int[] solution = BruteIter(x, 0, lens, pairs_of_verts.ToList(), points);

            // сохраняем пары вершин (рёбра), которые входят в решение
            var solution_edges = new List<Tuple<T, T>>();
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] == 1)
                    solution_edges.Add(pairs_of_verts[i]);
            }

            return solution_edges;
        }

        /// <summary>
        /// Полный перебор
        /// </summary>
        /// <param name="x"> Вектор решения </param>
        /// <param name="pos"> Позиция в векторе </param>
        /// <param name="lens"> Длины рёбер </param>
        /// <param name="pairs_of_verts"> Пары вершин </param>
        /// <param name="points"> Исходные точки </param>
        /// <returns> Вектор решения </returns>
        private static int[] BruteIter(int[] x, int pos, double[] lens, List<Tuple<T, T>> pairs_of_verts, List<T> points)
        {
            if (x.Sum() > points.Count * 2)
                return null;

            // если дошли до конца массива решений, то
            // проверяем граф, полученный данным решением, на связность 
            if (pos == x.Length)
            {
                if (Check(x, pairs_of_verts, points))
                {
                    return x;
                }
                else return null;
            }
            else
            {
                int[] x_right = new int[x.Length];
                x.CopyTo(x_right, 0);
                x_right[pos] = 1;

                // рекурсивно запускаем 2 следующих итерации: с взятым следующим ребром и без него
                int[] x_1 = BruteIter(x, pos + 1, lens, pairs_of_verts, points);
                int[] x_2 = BruteIter(x_right, pos + 1, lens, pairs_of_verts, points);
                if (x_1 != null && x_2 == null)
                    return x_1;
                else if (x_1 == null && x_2 != null)
                    return x_2;
                else if (x_1 != null && x_2 != null)
                    return (Q(x_1, lens) < Q(x_2, lens)) ? x_1 : x_2;
                else return x_1;
            }
        }
    }

    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// Муравьиный алгоритм постороения минимального дерева Штейнера
    /// </summary>
    /// <typeparam name="T"> Тип элементов </typeparam>
    public class AntColonyAlgorithm<T> : ShteinerAlghorithm<T>
        where T : class, IPoint2D
    {
        /// <summary>
        /// Построение дерева Штейнера муравьиным алгоритмом
        /// </summary>
        /// <param name="points"> Список вершин графа </param>
        /// <returns> Список рёбер </returns>
        public static dynamic Solve(List<T> points)
        {
            if (points.Count < 2)
                return null;

            List<double> x_list = new List<double>();
            List<double> y_list = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                x_list.Add(points[i].X);
                y_list.Add(points[i].Y);
            }
            x_list.Sort();
            y_list.Sort();
            x_list = x_list.Distinct().ToList();
            y_list = y_list.Distinct().ToList();

            int width = x_list.Count - 1;
            int height = y_list.Count - 1;

            // количество рёбер = сумма чётностей вершин пополам
            int countEdges = (4 * 2 + 2 * 3 * (height - 1) + 2 * 3 * (width - 1) + 4 * (height - 1) * (width - 1)) / 2;
            
            // заполняем массивы длин рёбер и пар точек
            double[] lens = new double[countEdges];
            var pairs_of_verts = new Tuple<T, T>[countEdges];
            for (int i = 0; i <= height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    lens[i * width + i * (width + 1) + j] = x_list[j + 1] - x_list[j];
                    pairs_of_verts[i * width + i * (width + 1) + j] = new Tuple<T, T>(new Point2D(x_list[j], y_list[i]) as T, new Point2D(x_list[j + 1], y_list[i]) as T);
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j <= width; j++)
                {
                    lens[(i + 1) * width + i * (width + 1) + j] = y_list[i + 1] - y_list[i];
                    pairs_of_verts[(i + 1) * width + i * (width + 1) + j] = new Tuple<T, T>(new Point2D(x_list[j], y_list[i]) as T, new Point2D(x_list[j], y_list[i + 1]) as T);
                }
            }

            // вырожденный случай, когда все точки находятся на одной линии
            if (x_list.Count == 1 || y_list.Count == 1)
                return pairs_of_verts.ToList();

            /***********************************************************************************************/
            /* Изначально имеем набор рёбер сетки с равными вероятностями взятия                           */
            /* 1) Выбираем случайно рёбра сетки, пока не получится связное дерево                          */
            /* 2) Сохраняем его и проделываем с этим же набором вероятностей ещё несколько таких итераций  */
            /* 3) Выбираем из них лучшее (по значению критерия) и корректируем вероятности выпадения       */
            /* 4) Проделываем шаги (1)-(3) для этих вероятностей выпадения, и т.д до какого-то останова    */ 
            /***********************************************************************************************/

            int stop_iterations = 30;       // количество итераций обучения мураьиного алгоритма
            int current_iteration = 0;      // текущее количество итераций
            int max_count_of_roads = 2;     // количество сохраняемых решений на каждой итерации
            int current_count_of_roads = 0; // текущее количество сохранённых решений


            // будем сохранять лучшие решения и их списки вероятностей рёбер
            var save_solutions = new List<Tuple<int[], List<int>>>();

            // лист вероятностей (лежат номера вершин)
            var list_propapility = new List<int>();
            for (int i = 0; i < countEdges; i++)
                list_propapility.Add(i);

            // сохраняем первый набор "вероятностей"
            save_solutions.Add(new Tuple<int[], List<int>>(new List<int>(new int[countEdges]).ToArray(), new List<int>(list_propapility)));

            // запускаем установленное число итераций "обучения" алгоритма
            while (current_iteration < stop_iterations)
            {
                var list_solutions = new List<int[]>(); // список для сохранения решений
                current_count_of_roads = 0; // обнуляем счётчик сохранённых решений
                // находим установленное число решений в одном "поколении обучения"
                while (current_count_of_roads < max_count_of_roads)
                {
                    int[] x_temp = new int[countEdges];
                    var temp_prob_list = new List<int>(save_solutions.Last().Item2);
                    int sas = 0;
                    do
                    {
                        sas++;
                        x_temp = new int[countEdges];
                        temp_prob_list = new List<int>(save_solutions.Last().Item2);
                        int random_choices = Rand.rand.Next(points.Count - 1, countEdges / 2 + 1); // количество слуйно выбираемых рёбер
                        int rand_edge;
                        while (random_choices > 0)
                        {
                            rand_edge = temp_prob_list[Rand.rand.Next(0, temp_prob_list.Count)]; // рандомим номер ребра
                            temp_prob_list.RemoveAll(num => num == rand_edge); // удаляем все номера, равные выпавшему (т.к. уже взяли его)
                            x_temp[rand_edge] = 1; // берём это ребро
                            random_choices--;
                        }
                    }
                    while (!Check(x_temp, pairs_of_verts.ToList(), points)); // пока не получим связное дерево, рандомим
                    // удаляем лишние рёбра
                    x_temp = DeleteExcessEdges(x_temp, pairs_of_verts.ToList(), points);

                    list_solutions.Add(new List<int>(x_temp).ToArray()); // сохраняем это решение
                    current_count_of_roads++;
                }

                // сохраняем лучшее из полученных решений
                int[] best_solution = new int[countEdges];
                double min_q = double.MaxValue;
                for (int i = 0; i < list_solutions.Count; i++)
                {
                    double q = Q(list_solutions[i], lens);
                    if (q < min_q)
                    {
                        min_q = q;
                        list_solutions[i].CopyTo(best_solution, 0);
                    }
                }
                // обновляем вероятности
                var prob_list = new List<int>(save_solutions.Last().Item2);
                for (int i = 0; i < best_solution.Length; i++)
                {
                    if (best_solution[i] == 1)
                        prob_list.Insert(Rand.rand.Next(0, prob_list.Count), i);
                }
                // сохраняем лучшее решение и вероятности
                save_solutions.Add(new Tuple<int[], List<int>>(new List<int>(best_solution).ToArray(), new List<int>(prob_list)));
                current_iteration++;
            }
            
            // удаляем первое (пустое) решение
            save_solutions.RemoveAt(0);

            // выбираем минимальное из полученных решений среди всех итераций
            double min = double.MaxValue;
            int min_ind = 0;
            for (int i = 0; i < save_solutions.Count; i++)
            {
                double q = Q(save_solutions[i].Item1, lens);
                if (q < min)
                {
                    min = q;
                    min_ind = i;
                }
            }

            // записываем решение в виде рёбер
            var solution_edges = new List<Tuple<T, T>>();
            for (int i = 0; i < save_solutions[min_ind].Item1.Length; i++)
            {
                if (save_solutions[min_ind].Item1[i] == 1)
                    solution_edges.Add(pairs_of_verts[i]);
            }

            return solution_edges;
        }

        /// <summary>
        /// Удаление лишних рёбер из решения построенного связного дерева
        /// </summary>
        /// <param name="x"> Текущее решение </param>
        /// <param name="pairs_of_verts"> Рёбра - пары вершин </param>
        /// <param name="points"> Исходные точки </param>
        /// <returns> Решение после удаления лишних рёбер </returns>
        private static int[] DeleteExcessEdges(int[] x, List<Tuple<T, T>> pairs_of_verts, List<T> points)
        {
            /*******************************************************************/
            /* Необходимо удалить лишние рёбра в связном дереве.               */
            /* Лишним будем считать ребро, которое приходит в какую-то точку   */
            /* единожны, не считая изначальные точки, которые нужно соединить. */
            /*******************************************************************/
            List<T> unique_points;
            do
            {
                var solution_pairs = new List<Tuple<Tuple<T, T>, int>>();
                // выбираем из множества рёбер рёбра решения
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] == 1)
                    {
                        solution_pairs.Add(new Tuple<Tuple<T, T>, int>(pairs_of_verts[i], i));
                    }
                }

                // выписываем все точки
                List<T> pair_points = new List<T>();
                for (int i = 0; i < solution_pairs.Count; i++)
                {
                    pair_points.Add(solution_pairs[i].Item1.Item1);
                    pair_points.Add(solution_pairs[i].Item1.Item2);
                }

                // ищем одиночные точки, не являющиеся исходными, их нужно удалить
                unique_points = new List<T>();
                for (int i = 0; i < pair_points.Count; i++)
                {
                    if (pair_points.FindAll(p => p.Equals(pair_points[i])).Count == 1 && !points.Contains(pair_points[i]))
                    {
                        unique_points.Add(pair_points[i]);
                    }
                }

                // записываем номера рёбер, в которых фигурируют найденные точки
                List<int> nums = new List<int>();
                for (int i = 0; i < solution_pairs.Count; i++)
                {
                    if (unique_points.Contains(solution_pairs[i].Item1.Item1) || unique_points.Contains(solution_pairs[i].Item1.Item2))
                    {
                        nums.Add(solution_pairs[i].Item2);
                    }
                }

                // убираем эти рёбра из решения
                for (int i = 0; i < nums.Count; i++)
                {
                    x[nums[i]] = 0;
                }
                // проделываем данную операци, пока есть такие точки
            } while (unique_points.Count > 0);
            
            return x;
        }
    }
}
