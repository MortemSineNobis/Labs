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
        ChessGame game = new ChessGame();
        public MainWindow()
        {
            InitializeComponent();
            DrawChessmen();
        }

        private void DrawChessmen()
        {
            var size = grid.Children[0].DesiredSize.Width;
            var chessmen = game.Draw(size);

            foreach (var chessman in chessmen)
            {
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    var item = grid.Children[i];
                    if (item is PackIcon)
                    {
                        var obj = item as PackIcon;
                        if ((int)obj.GetValue(Grid.RowProperty) == chessman.Y
                            && (int)obj.GetValue(Grid.ColumnProperty) == chessman.X)
                        {
                            obj.Foreground = chessman.icon.Foreground;
                            obj.Kind = chessman.icon.Kind;
                        }
                    }
                }
            }
        }

        private void DrawBackground()
        {
            var back = game.DrawBack();
            foreach (var background in back)
            {
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    var item = grid.Children[i];
                    if (item is PackIcon)
                    {
                        var obj = item as PackIcon;
                        if ((int)obj.GetValue(Grid.RowProperty) == background.Y
                            && (int)obj.GetValue(Grid.ColumnProperty) == background.X)
                        {
                            obj.Background = background.b;
                        }
                    }
                }
            }
        }

        private void PackIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in grid.Children)
            {
                PackIcon item1 = item as PackIcon;
                if (item1!=null)
                    Console.WriteLine(item1.Background);
            }
            ReDrawBoard();
            PackIcon pi = sender as PackIcon;
            Console.WriteLine(pi.Background);
            if (pi.Kind == PackIconKind.None && pi.Background.ToString() != "#FFEBECD0")
                game.cancel();
            var x = (int)pi.GetValue(Grid.ColumnProperty);
            var y = (int)pi.GetValue(Grid.RowProperty);
            game.SetHoldedNode(x, y);
            DrawBackground();
            Console.WriteLine(game.playerState);
        }

        private void ReDrawBoard()
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                BrushConverter bc = new BrushConverter();
                var item = grid.Children[i] as PackIcon;
                if (item != null)
                {
                    item.Kind = PackIconKind.None;
                    if ((int)item.GetValue(Grid.RowProperty) % 2 == 0)
                    {
                        if ((int)item.GetValue(Grid.ColumnProperty) % 2 == 0)
                            item.Background = (Brush)bc.ConvertFrom("#ebecd0");
                        else
                            item.Background = (Brush)bc.ConvertFrom("#779556");
                    }
                    else
                    {
                        if ((int)item.GetValue(Grid.ColumnProperty) % 2 == 1)
                            item.Background = (Brush)bc.ConvertFrom("#ebecd0");
                        else
                            item.Background = (Brush)bc.ConvertFrom("#779556");
                    }
                }
            }
            DrawChessmen();
        }
    }
}
