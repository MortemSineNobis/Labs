using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Chess.Engine.Chessmans
{
    public class Bishop : Chessman
    {
        /// <summary>
        /// Represents the directions of movement
        /// </summary>
        private Direction[] directions = new Direction[4];

        public Bishop(PlayerColor color)
            : base(color)
        {
            for (int i = 0; i < 4; i++)
            {
                directions[i] = null;
            }
        }

        public Bishop(Chessman promote)
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
            //Open up left direction and listen to it
            directions[0] = new Direction(this, -1, 1);
            //Open up right direction and listen to it
            directions[1] = new Direction(this, 1, 1);
            //Open down left direction and listen to it
            directions[2] = new Direction(this, -1, -1);
            //Open down right direction and listen to it
            directions[3] = new Direction(this, 1, -1);
        }

        public override bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            foreach (Direction direction in directions)
            {
                //If any direction can hit the blocked return false
                if (!direction.IsBlockedIfMove(from, to, blocked)) return false;
            }

            return true;
        }

        public override PackIcon Picture => new PackIcon
        {
            VerticalAlignment = VerticalAlignment.Center,
            Kind = PackIconKind.ChessBishop,
            Margin = new Thickness(0),
            Height = 512,
            Width = 512
        };
    }
}
