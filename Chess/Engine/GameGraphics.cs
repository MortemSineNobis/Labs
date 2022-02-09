using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Chess.Engine
{
    class GameGraphics
    {
        public static PackIcon GetIcon(ChessBoard.Cell cell, double size)
        {
            var res = cell.Chessman.Picture;

            res.Foreground = cell.Chessman.Color == PlayerColor.White ? Brushes.White : Brushes.Black;
            res.Height = res.Width = size;
            
            return res;
        }

        public static Brush SetBackground(Moves moves)
        {
            var b =  moves switch
            {
                Moves.Normal => Brushes.Transparent,
                Moves.NoMoves => Brushes.DarkRed,
                Moves.Move => Brushes.Green,
                Moves.Selected => Brushes.Yellow,
                _ => Brushes.Transparent
            };

            return b;
        }
    }
}
