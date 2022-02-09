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
    public class King : Chessman
    {
        /// <summary>
        /// Represents the directions of movement
        /// </summary>
        private Direction[] directions = new Direction[8];

        /// <summary>
        /// Shows if we can castle to the left
        /// </summary>
        private bool canCastleLeft;

        /// <summary>
        /// Shows if we can castle to the right
        /// </summary>
        private bool canCastleRight;

        public King(PlayerColor color)
            : base(color)
        {
            for (int i = 0; i < 8; i++)
            {
                directions[i] = null;
            }
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

                    if (canCastleLeft)
                    {
                        yield return Parent.Parent.GetCell(2, (Color == PlayerColor.White) ? 0 : 7);
                    }

                    if (canCastleRight)
                    {
                        yield return Parent.Parent.GetCell(6, (Color == PlayerColor.White) ? 0 : 7);
                    }
                }
            }
        }

        public override void Recalculate()
        {
            //If moved castling is not possible anymore an we should also remove listeners
            if (!Moved)
            {
                //Set it to true we'll set it to false if it wasn't true
                canCastleLeft = true;

                //Checks if the left rook is still in place and haven't moved yet
                ChessBoard.Cell leftRookCell = Parent.Parent.GetCell(0, (Color == PlayerColor.White) ? 0 : 7);
                if (leftRookCell.Chessman == null || !(leftRookCell.Chessman is Rook) || leftRookCell.Chessman.Color != Color || leftRookCell.Chessman.Moved)
                    canCastleLeft = false;
                else
                {
                    //Checks pieces that could block the castle
                    for (int i = 1; i <= 3; i++)
                    {
                        if (Parent.Parent.GetCell(i, (Color == PlayerColor.White) ? 0 : 7).Chessman != null)
                            canCastleLeft = false;
                    }
                }

                //Set it to true we'll set it to false if it wasn't true
                canCastleRight = true;

                //Checks if the right rook is still in place and haven't moved yet
                ChessBoard.Cell rightRookCell = Parent.Parent.GetCell(7, (Color == PlayerColor.White) ? 0 : 7);
                if (rightRookCell.Chessman == null || !(rightRookCell.Chessman is Rook) || rightRookCell.Chessman.Color != Color || rightRookCell.Chessman.Moved)
                    canCastleRight = false;
                else
                {
                    //Checks pieces that could block the castle
                    for (int i = 5; i <= 6; i++)
                    {
                        if (Parent.Parent.GetCell(i, (Color == PlayerColor.White) ? 0 : 7).Chessman != null)
                            canCastleRight = false;
                    }
                }
            }

            //Open upward direction and listen to it
            directions[0] = new Direction(this, 0, 1, 1);
            //Open downward direction and listen to it
            directions[1] = new Direction(this, 0, -1, 1);
            //Open leftward direction and listen to it
            directions[2] = new Direction(this, -1, 0, 1);
            //Open rightward direction and listen to it
            directions[3] = new Direction(this, 1, 0, 1);
            //Open up left direction and listen to it
            directions[4] = new Direction(this, -1, 1, 1);
            //Open up right direction and listen to it
            directions[5] = new Direction(this, 1, 1, 1);
            //Open down left direction and listen to it
            directions[6] = new Direction(this, -1, -1, 1);
            //Open down right direction and listen to it
            directions[7] = new Direction(this, 1, -1, 1);
        }

        public override bool IsBlockedIfMove(ChessBoard.Cell from, ChessBoard.Cell to, ChessBoard.Cell blocked)
        {
            foreach (Direction direction in directions)
            {
                //If any direction can hit the blocked return false
                if (!direction.IsBlockedIfMove(from, to, blocked))
                    return false;
            }

            return true;
        }

        public override PackIcon Picture => new PackIcon
        {
            VerticalAlignment = VerticalAlignment.Center,
            Kind = PackIconKind.ChessKing,
            Margin = new Thickness(0),
            Height = 512,
            Width = 512
        };
    }
}
