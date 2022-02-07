using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.Chessmans
{
    public class Rook : Chessman
    {
        /// <summary>
        /// Represents the directions of movement
        /// </summary>
        private Direction[] directions = new Direction[4];

        public Rook(PlayerColor color)
            : base(color)
        {
            for (int i = 0; i < 4; i++)
            {
                directions[i] = null;
            }
        }

        public Rook(Chessman promote)
            : this(promote.Color)
        {
            Moved = promote.Moved;
        }

        public override IEnumerable<ChessBoard.Cell> PossibleMoves
        {
            get
            {
                foreach (Direction direction in directions)
                {
                    foreach (ChessBoard.Cell cell in direction.GetPossibleMoves())
                    {
                        yield return cell;
                    }
                }
            }
        }

        public override void Recalculate()
        {
            //Open upward direction and listen to it
            directions[0] = new Direction(this, 0, 1);
            //Open downward direction and listen to it
            directions[1] = new Direction(this, 0, -1);
            //Open leftward direction and listen to it
            directions[2] = new Direction(this, -1, 0);
            //Open rightward direction and listen to it
            directions[3] = new Direction(this, 1, 0);
        }

        public override bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            //If any direction can hit the blocked return false
            foreach (Direction direction in directions)
            {
                //If any direction can hit the blocked return false
                if (!direction.IsBlockedIfMove(from, to, blocked)) return false;
            }

            return true;
        }

        public override char Char => 'R';
    }
}
