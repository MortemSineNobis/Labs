using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chess.Engine;
using MaterialDesignThemes.Wpf;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessBoard chessBoard = new ChessBoard();
        Engine.Point selectedPiece = new Engine.Point();
        int selectedPlayer = -1;
        bool player = true;
        public MainWindow()
        {
            InitializeComponent();
            DrawPieces(chessBoard);
        }

        private void PackIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedPlayer == -1)
                if (!CanMove(sender)) return;
            DrawPieces(chessBoard);
            if (!(sender is PackIcon)) return;
            var button = sender as PackIcon;
            
            button.Background = Brushes.Blue;
            var a = GetPositionFromControl(sender);

            if (!(button.Tag is ChessPiece))
            {
                if (selectedPlayer > -1)
                {
                    var res = chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.x - 1, a.y - 1);
                    selectedPlayer = -1;
                    if ((selectedPiece.x != (a.x - 1) || selectedPiece.y != (a.y - 1)) && res)
                    {
                        LogHistory($"{(char)('a' + selectedPiece.x)}{7 - selectedPiece.y + 1}{(char)('a' + a.x - 1)}{7 - a.y + 2}");
                        player = !player;
                    }
                    
                    DrawPieces(chessBoard);
                }
                return;
            }

            ChessPiece chessPiece = (ChessPiece)button.Tag;
            Console.WriteLine("({2}, {3}) - {0} from team {1}", chessPiece.GetType(), chessPiece.Player, a.x - 1, a.y - 1);

            if (selectedPlayer > -1 && selectedPlayer != chessPiece.Player)
            {
                var res = chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.x - 1, a.y - 1);
                selectedPlayer = -1;

                if ((selectedPiece.x != (a.x - 1) || selectedPiece.y != (a.y - 1)) && res)
                {
                    LogHistory($"{(char)('a' + selectedPiece.x)}{7 - selectedPiece.y + 1}{(char)('a' + a.x - 1)}{7 - a.y + 2}");
                    player = !player;
                }

                DrawPieces(chessBoard);
            }
            else
            {
                selectedPlayer = chessPiece.Player;
                selectedPiece.x = a.x - 1;
                selectedPiece.y = a.y - 1;
                foreach (Engine.Point point in chessBoard.PieceActions(a.x - 1, a.y - 1))
                {
                    var actionButton = GetControlFromPosition(point.x + 1, point.y + 1);
                    BrushConverter bc = new BrushConverter();
                    actionButton.Background = Brushes.LightSeaGreen;
                    Console.WriteLine("~({0}, {1})", point.x, point.y);
                }
            }
        }

        private PackIcon GetControlFromPosition(int x, int y)
        {
            foreach (var item in grid.Children)
            {
                var obj = item as PackIcon;
                if (obj != null)
                    if ((int)obj.GetValue(Grid.RowProperty) == y-1 && (int)obj.GetValue(Grid.ColumnProperty) == x-1)
                        return obj;
            }
            return null;
        }
        private Engine.Point GetPositionFromControl(object sender)
        {
            var obj = sender as PackIcon;
            if (obj != null)
            {
                var x = (int)obj.GetValue(Grid.ColumnProperty) + 1;
                var y = (int)obj.GetValue(Grid.RowProperty) + 1;
                return new Engine.Point(x, y);
            }
            return new Engine.Point(-1,-1);
        }

        private void DrawPieces(ChessBoard board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var button = GetControlFromPosition(x + 1, y + 1);
                    if (button != null)
                    {
                        BrushConverter bc = new BrushConverter();
                        button.Background = (Brush)bc.ConvertFromString(button.Uid);
                        if (board[x, y] != null)
                        {
                            ChessPiece chessPiece = board[x, y];
                            button.Tag = chessPiece;
                            button.Kind = GetKind(chessPiece);
                            if (chessPiece.Player == 1) button.Foreground = Brushes.White;
                            else button.Foreground = Brushes.Black;
                        }
                        else
                        {
                            button.Kind = PackIconKind.None;
                            button.Tag = null;
                        }
                    }
                }
            }
        }

        private PackIconKind GetKind(ChessPiece chessPiece)
        {
            return chessPiece.GetType().ToString() switch
            {
                "Chess.Engine.Pieces.Rook" => PackIconKind.ChessRook,
                "Chess.Engine.Pieces.Pawn" => PackIconKind.ChessPawn,
                "Chess.Engine.Pieces.Knight" => PackIconKind.ChessKnight,
                "Chess.Engine.Pieces.Bishop" => PackIconKind.ChessBishop,
                "Chess.Engine.Pieces.Queen" => PackIconKind.ChessQueen,
                "Chess.Engine.Pieces.King" => PackIconKind.ChessKing,
                _=> PackIconKind.None
            };
        }

        private bool CanMove(object sender)
        {
            var obj = sender as PackIcon;
            if (obj == null) return false;
            var color = obj.Foreground.ToString() == "#FFFFFFFF";
            return !(player ^ color);
        }

        private void LogHistory(string move)
        {
            var playerStr = player ? "W" : "B";
            history.Items.Add($"{playerStr}: {move}");
        }
    }
}
