using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Engine.Pieces;

namespace Chess.Engine
{
    public class ChessBoard
    {
        private ChessPiece[,] boardArray;
        private const int COLUMNS = 8;
        private int ROWS = 8;

        public ChessBoard()
        {
            SetupBoard();
        }

        public int GetLength(int l)
        {
            return boardArray.GetLength(l);
        }

        public ChessPiece this[int x, int y]
        {
            get { return boardArray[x, y]; }
        }

        private ChessBoard SetupBoard()
        {
            boardArray = new ChessPiece[COLUMNS, ROWS];
            string[] playerPeices = {
                "Rook", "Knight", "Bishop", "Queen",
                "King", "Bishop", "Knight", "Rook",
                "Pawn", "Pawn", "Pawn", "Pawn",
                "Pawn", "Pawn", "Pawn", "Pawn" };
            var King = new King();
            Console.WriteLine(King.GetType());
            for (int i = 0; i < COLUMNS; i++)
            {
                // Player 0 pieces
                boardArray[i, 0] = (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess.Engine.Pieces." + playerPeices[i]));
                boardArray[i, 1] = (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess.Engine.Pieces." + playerPeices[i + COLUMNS]));
                // Player 1 pieces
                boardArray[i, ROWS - 1] = (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess.Engine.Pieces." + playerPeices[i]), new object[] { 1 });
                boardArray[i, ROWS - 2] = (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess.Engine.Pieces." + playerPeices[i + COLUMNS]), new object[] { 1 });
            }
            return this;
        }

        /// <summary>
        /// Рассчитывает действия, доступные для шахматной фигуры в наборе координат.
        /// </summary>
        /// <param name="x">Количество клеток справа от нижнего левого квадрата</param>
        /// <param name="y">Количество клеток над нижним левым квадратом</param>
        /// <param name="ignoreCheck">Не проверять на угрозы королю</param>
        /// <param name="attackActions">Рассчет возможных атак</param>
        /// <param name="moveActions">Рассчет возможных перемещений</param>
        /// <param name="boardArray">Поле (доска)</param>
        /// <returns>Возвращает список возможных перемещений</returns>
        public IEnumerable<Point> PieceActions(int x, int y, bool ignoreCheck = false, bool attackActions = true, bool moveActions = true, ChessPiece[,] boardArray = null)
        {
            if (boardArray == null)
            {
                boardArray = this.boardArray;
            }

            bool[,] legalActions = new bool[boardArray.GetLength(0), boardArray.GetLength(1)];
            List<Point> availableActions = new List<Point>();
            ChessPiece movingPeice = boardArray[x, y];

            if (attackActions)
            {
                foreach (Point[] direction in movingPeice.AvailableAttacks)
                {
                    foreach (Point attackPoint in direction)
                    {
                        Point adjustedPoint = new Point(attackPoint.x + x, attackPoint.y + y);
                        if (ValidatePoint(adjustedPoint))
                        {
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null
                                && boardArray[adjustedPoint.x, adjustedPoint.y].Player ==
                                movingPeice.Player) break;
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null)
                            {
                                AddMove(availableActions, new Point(x, y), adjustedPoint, ignoreCheck);
                                break;
                            }
                        }
                    }
                }
            }

            if (moveActions)
            {
                foreach (Point[] direction in movingPeice.AvailableMoves)
                {
                    foreach (Point movePoint in direction)
                    {
                        Point adjustedPoint = new Point(movePoint.x + x, movePoint.y + y);
                        if (ValidatePoint(adjustedPoint))
                        {
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null) break;
                            AddMove(availableActions, new Point(x, y), adjustedPoint, ignoreCheck);
                        }
                    }
                }
            }

            if (movingPeice is King && ((King)movingPeice).CanCastle)
            {
                int rookX = 0;
                if (boardArray[rookX, y] is Rook && ((Rook)boardArray[rookX, y]).CanCastle)
                {
                    bool missedCondition = false;
                    foreach (int rangeX in Enumerable.Range(rookX + 1, Math.Abs(rookX - x) - 1))
                    {
                        if (boardArray[rangeX, y] != null) missedCondition = true;
                    }
                    missedCondition = missedCondition || KingInCheck(movingPeice.Player);
                    if (!missedCondition)
                        AddMove(availableActions, new Point(x, y), new Point(x - 2, y), ignoreCheck);
                }
                rookX = COLUMNS - 1;
                if (boardArray[rookX, y] is Rook && ((Rook)boardArray[rookX, y]).CanCastle)
                {
                    bool missedCondition = false;
                    foreach (int rangeX in Enumerable.Range(x + 1, Math.Abs(rookX - x) - 1))
                    {
                        if (boardArray[rangeX, y] != null) missedCondition = true;
                    }
                    missedCondition = missedCondition || KingInCheck(movingPeice.Player);
                    if (!missedCondition)
                        AddMove(availableActions, new Point(x, y), new Point(x + 2, y), ignoreCheck);
                }
            }

            if (movingPeice is Pawn)
            {
                Pawn pawn = (Pawn)movingPeice;
                int flipDirection = 1;

                if (pawn.Player == 1) flipDirection = -1;
                if (pawn.CanEnPassantLeft)
                {
                    Point attackPoint;
                    attackPoint = ChessPiece.GetDiagnalMovementArray(1, DiagnalDirection.FORWARD_LEFT)[0];
                    attackPoint.y *= flipDirection;
                    attackPoint.y += y;
                    attackPoint.x += x;
                    if (ValidatePoint(attackPoint))
                    {
                        AddMove(availableActions, new Point(x, y), attackPoint, ignoreCheck);
                    }
                }

                if (pawn.CanEnPassantRight)
                {
                    Point attackPoint;
                    attackPoint = ChessPiece.GetDiagnalMovementArray(1, DiagnalDirection.FORWARD_RIGHT)[0];
                    attackPoint.y *= flipDirection;
                    attackPoint.y += y;
                    attackPoint.x += x;
                    if (ValidatePoint(attackPoint))
                    {
                        AddMove(availableActions, new Point(x, y), attackPoint, ignoreCheck);
                    }
                }
            }

            return availableActions;
        }

        private void AddMove(List<Point> availableActions, Point fromPoint, Point toPoint, bool ignoreCheck = false)
        {
            bool kingInCheck = false;

            if (!ignoreCheck)
            {
                ChessPiece movingPiece = boardArray[fromPoint.x, fromPoint.y];
                ChessPiece[,] boardArrayBackup = (ChessPiece[,])boardArray.Clone();
                ActionPiece(fromPoint, toPoint, true);
                kingInCheck = KingInCheck(movingPiece.Player);
                boardArray = boardArrayBackup;
            }

            if (ignoreCheck || !kingInCheck) availableActions.Add(toPoint);
        }

        public bool KingInCheck(int player)
        {
            for (int x = 0; x < COLUMNS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    ChessPiece chessPiece = boardArray[x, y];
                    if (chessPiece != null
                        && chessPiece.Player == player
                        && chessPiece is King)
                    {
                        if (CheckSquareVulnerable(x, y, player))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            throw new Exception("King wasn't found!");
        }

        public IEnumerable<Point> PieceActions(Point position, bool ignoreCheck = false, bool attackActions = true, bool moveActions = true, ChessPiece[,] boardArray = null)
        {
            return PieceActions(position.x, position.y, ignoreCheck, attackActions, moveActions, boardArray);
        }

        /// <summary>
        /// Перемещение фигуры на доске.
        /// </summary>
        /// <param name="fromX">Начальное положение фигуры по x.</param>
        /// <param name="fromY">Начальное положение фигуры по y.</param>
        /// <param name="toX">Конечное положение фигуры по x.</param>
        /// <param name="toY">Конечное положение фигуры по y.</param>
        /// <returns>Возвращает true при удачном исходе, в противном случае - false.</returns>
        public bool ActionPiece(int fromX, int fromY, int toX, int toY)
        {
            return ActionPiece(new Point(fromX, fromY), new Point(toX, toY));
        }

        /// <summary>
        /// Перемещение фигуры на доске.
        /// </summary>
        /// <param name="from">НАчальное положение фигуры.</param>
        /// <param name="to">Конечное положение фигуры.</param>
        /// <returns>Возвращает true при удачном исходе, в противном случае - false.</returns>
        public bool ActionPiece(Point from, Point to, bool bypassValidaiton = false)
        {
            if (bypassValidaiton || PieceActions(from).Contains(to))
            {
                ChessPiece movingPeice = boardArray[from.x, from.y];
                if (movingPeice is Pawn)
                {
                    Pawn pawn = (Pawn)movingPeice;
                    // If this was a double jump, check enpassant
                    if (Math.Abs(from.y - to.y) == 2)
                    {
                        int adjasentX = to.x - 1;
                        if (adjasentX > -1
                            && boardArray[adjasentX, to.y] != null
                            && boardArray[adjasentX, to.y].Player != movingPeice.Player
                            && boardArray[adjasentX, to.y] is Pawn)
                        {
                            if (!bypassValidaiton)
                                ((Pawn)boardArray[adjasentX, to.y]).CanEnPassantRight = true;
                        }
                        adjasentX += 2;
                        if (adjasentX < COLUMNS
                            && boardArray[adjasentX, to.y] != null
                            && boardArray[adjasentX, to.y].Player != movingPeice.Player
                            && boardArray[adjasentX, to.y] is Pawn)
                        {
                            if (!bypassValidaiton)
                                ((Pawn)boardArray[adjasentX, to.y]).CanEnPassantLeft = true;
                        }
                    }
                    if (from.x != to.x && boardArray[to.x, to.y] == null)
                    {
                        boardArray[to.x, from.y] = null;
                    }

                    if (!bypassValidaiton) // Пешка не может ходить на 2 клетки после первого хода ей.
                        pawn.CanDoubleJump = false;
                }
                if (movingPeice is CastlePiece)
                {
                    CastlePiece rookOrKing = (CastlePiece)movingPeice;
                    if (!bypassValidaiton) 
                        rookOrKing.CanCastle = false;
                }
                if (movingPeice is King)
                {
                    King king = (King)movingPeice;
                    if (from.x - to.x == 2)
                    {   // Move rook for Queenside castle
                        boardArray[to.x + 1, from.y] = boardArray[0, from.y];
                        boardArray[0, from.y] = null;
                    }
                    if (from.x - to.x == -2)
                    {   // Move rook for Kingside castle
                        boardArray[to.x - 1, from.y] = boardArray[COLUMNS - 1, from.y];
                        boardArray[COLUMNS - 1, from.y] = null;
                    }
                }
                movingPeice.CalculateMoves();
                boardArray[from.x, from.y] = null;
                boardArray[to.x, to.y] = movingPeice;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка клетки на уязвимость перед противником
        /// </summary>
        /// <param name="player">Текущий игрок</param>
        /// <param name="boardArray">Поле (доска).</param>
        /// <returns>Возвращает true, если клетка может быть атакована</returns>
        public bool CheckSquareVulnerable(int squareX, int squareY, int player, ChessPiece[,] boardArray = null)
        {
            if (boardArray == null)
            {
                boardArray = this.boardArray;
            }

            for (int x = 0; x < boardArray.GetLength(0); x++)
            {
                for (int y = 0; y < boardArray.GetLength(1); y++)
                {
                    if (boardArray[x, y] != null && boardArray[x, y].Player != player)
                    {
                        foreach (Point point in PieceActions(x, y, true, true, false, boardArray))
                        {
                            if (point.x == squareX && point.y == squareY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ValidateRange(int value, int high, int low = -1)
        {
            return value > low && value < high;
        }

        public bool ValidateX(int value)
        {
            return ValidateRange(value, boardArray.GetLength(0));
        }

        public bool ValidateY(int value)
        {
            return ValidateRange(value, boardArray.GetLength(1));
        }

        public bool ValidatePoint(Point point)
        {
            return ValidateX(point.x) && ValidateY(point.y);
        }
    }
}
