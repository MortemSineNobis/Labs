using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine
{
    public abstract class ChessPiece
    {
        protected const int MAX_DISTANCE = 7;

        // Пешки
        protected bool canEnPassantLeft;
        protected bool canEnPassantRight;
        protected bool canDoubleJump;

        // ОСтальные
        protected bool canCastle; // Ладья и король
        protected Point[][] availableMoves; // Массивы для перемещения ([направление][рассояние])
        protected Point[][] availableAttacks; // Возможные атаки
        private int player;

        public Point[][] AvailableMoves
        {
            get { return availableMoves; }
        }

        public Point[][] AvailableAttacks
        {
            get { return availableAttacks; }
        }

        public int Player
        {
            get { return player; }
            set { player = value; }
        }

        public abstract ChessPiece CalculateMoves();

        /// <summary>
        /// Получает относительные координаты движения по горизонтали и вертикали.
        /// Используется королем, королевой, пешкой, ладьей.
        /// </summary>
        /// <param name="distance">Расстояние в заданном направление</param>
        /// <param name="direction">Направление относитьно текущей позиции</param>
        /// <returns>Возвращает массив возможных перемещений по вертикали и горизонтали.</returns>
        public static Point[] GetMovementArray(int distance, Direction direction)
        {
            Point[] movement = new Point[distance];
            int xPosition = 0;
            int yPosition = 0;

            for (int i = 0; i < distance; i++)
            {
                switch (direction)
                {
                    case Direction.FORWARD:
                        yPosition++;
                        break;
                    case Direction.BACKWARD:
                        yPosition--;
                        break;
                    case Direction.LEFT:
                        xPosition++;
                        break;
                    case Direction.RIGHT:
                        xPosition--;
                        break;
                    default:
                        break;
                }
                movement[i] = new Point(xPosition, yPosition);
            }
            return movement;
        }

        /// <summary>
        /// Получает относительные координаты движения по диагонали.
        /// Используется королем, королевой, пешкой, слоном.
        /// </summary>
        /// <param name="distance">Расстояние в заданном направление</param>
        /// <param name="direction">Направление относитьно текущей позиции</param>
        /// <returns>Возвращает массив возможных перемещений по диагонали.</returns>
        public static Point[] GetDiagnalMovementArray(int distance, DiagnalDirection direction)
        {
            Point[] attack = new Point[distance];
            int xPosition = 0;
            int yPosition = 0;

            for (int i = 0; i < distance; i++)
            {
                switch (direction)
                {
                    case DiagnalDirection.FORWARD_LEFT:
                        xPosition--;
                        yPosition++;
                        break;
                    case DiagnalDirection.FORWARD_RIGHT:
                        xPosition++;
                        yPosition++;
                        break;
                    case DiagnalDirection.BACKWARD_LEFT:
                        xPosition--;
                        yPosition--;
                        break;
                    case DiagnalDirection.BACKWARD_RIGHT:
                        xPosition++;
                        yPosition--;
                        break;
                    default:
                        break;
                }
                attack[i] = new Point(xPosition, yPosition);
            }
            return attack;
        }
    }
}
