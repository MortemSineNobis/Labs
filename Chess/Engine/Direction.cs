using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine
{
    /// <summary>
    /// Contains possible moves and handles line of sight checks
    /// </summary>
    public class Direction
    {
        /// <summary>
        /// The chessman whose moves are represented by this object
        /// </summary>
        public Chessman Chessman
        {
            private set;
            get;
        }

        /// <summary>
        /// The X-direction
        /// </summary>
        public int X
        {
            private set;
            get;
        }

        /// <summary>
        /// The Y-direction
        /// </summary>
        public int Y
        {
            private set;
            get;
        }

        /// <summary>
        /// The possible moves you can make in this direction including the line of sight blocking chessman that may or may not be hittable.
        /// See also <seealso cref="GetPossibleMoves"/>
        /// </summary>
        private List<ChessBoard.Cell> possibleMoves;

        /// <summary>
        /// The possible moves you can make in this direction
        /// </summary>
        /// <param name="enemyHittable">Are the enemy chessmans hittable</param>
        /// <returns>An enumeration of possible moves</returns>
        public IEnumerable<ChessBoard.Cell> GetPossibleMoves(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                yield break;

            for (int i = 0; i < possibleMoves.Count - 1; i++)
            {
                yield return possibleMoves[i];
            }

            if (possibleMoves.Last().Chessman == null)
                yield return possibleMoves.Last();
            else if (enemyHittable && possibleMoves.Last().Chessman.Color != Chessman.Color)
                yield return possibleMoves.Last();
        }

        /// <summary>
        /// The count of possible moves
        /// </summary>
        /// <param name="enemyHittable">Are the enemy chessmans hittable</param>
        /// <returns>The count of possible moves</returns>
        public int GetPossibleMoveCount(bool enemyHittable = true)
        {
            if (possibleMoves.Count == 0)
                return 0;

            if (possibleMoves.Last().Chessman == null)
                return possibleMoves.Count;
            else if (!enemyHittable || possibleMoves.Last().Chessman.Color == Chessman.Color)
                return possibleMoves.Count - 1;
            else
                return possibleMoves.Count;
        }

        /// <summary>
        /// The number of moves that we could take, considering no blocking or out of board.
        /// </summary>
        public int DesiredCount
        {
            private set;
            get;
        }

        /// <summary>
        /// Tells if the direction should update the hit graph of possible move cells
        /// </summary>
        private bool updateHitGraph;

        public Direction(Chessman chessman, int x, int y, int desiredCount = 8, bool updateHitGraph = true)
        {
            Chessman = chessman;
            X = x;
            Y = y;
            DesiredCount = desiredCount;
            this.updateHitGraph = updateHitGraph;

            possibleMoves = new List<ChessBoard.Cell>();
            possibleMoves.AddRange(chessman.Parent.OpenLineOfSight(x, y, desiredCount));

            foreach (ChessBoard.Cell move in possibleMoves)
            {
                if (updateHitGraph)
                    move.HitBy.Add(Chessman);
            }
        }

        /// <summary>
        /// Tells if the moved chessman on the cell changed the hit state of the blocked 
        /// </summary>
        /// <param name="from">Where the chessman stands right now</param>
        /// <param name="to">Where the chessman is moved</param>
        /// <param name="blocked">Hit tests this chessman</param>
        /// <returns>If blocked is hittable after moving the from</returns>
        public bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            if (possibleMoves.Contains(blocked) && !possibleMoves.Contains(to))
            {
                //The blocked is hittable to begin with and we don't block it with a new blocker
                //To may still equal blocked but direction should not care about that
                return false;
            }
            else if (possibleMoves.Contains(from))
            {
                int toIndex = possibleMoves.IndexOf(to);
                if (0 <= toIndex && toIndex < possibleMoves.Count - 1)
                    return true; //The blocker closer to the chessman
                else
                {
                    //If we moved further
                    foreach (ChessBoard.Cell move in from.OpenLineOfSight(X, Y, DesiredCount - possibleMoves.Count))
                    {
                        if (move == to) //The blocker moved into the new path
                            return true;
                        if (move == blocked) //The blocked is hittable
                            return false;
                    }
                }
            }

            //Happens when the blocker was not cotained and the blocked was not contained a perfect combination for nothing happening
            return true;
        }
    }
}
