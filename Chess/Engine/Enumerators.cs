using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine
{
    public struct Point
    {
        public int x;
        public int y;

        /// <summary>
        /// Конструктор класса точка
        /// </summary>
        /// <param name="x">Позиция по горизонтали</param>
        /// <param name="y">Позиция по вертикали</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Вывод координат
        /// </summary>
        /// <returns>Возвращает координаты в формате (x, y)</returns>
        public override string ToString()
        {
            return (String.Format("({0}, {1})", x, y));
        }
    }

    public enum Direction
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT
    }

    public enum DiagnalDirection
    {
        FORWARD_LEFT,
        FORWARD_RIGHT,
        BACKWARD_LEFT,
        BACKWARD_RIGHT
    }
}
