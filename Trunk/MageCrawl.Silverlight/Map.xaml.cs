using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class Map : UserControl
    {
        private Canvas m_canvas;
        private Grid m_grid;

        private Image m_blank;
        private Image m_wall;

        private IMap m_map;

        public Map()
        {
            InitializeComponent();
            m_canvas = (Canvas)FindName("Canvas");
            m_blank = LoadImage("Images//blank.bmp");
            m_wall = LoadImage("Images//dngn_rock_wall_07.bmp");

            m_grid = CreateMapGrid(13, 17);
            m_grid.ShowGridLines = true;

            m_canvas.Children.Add(m_grid);
            Canvas.SetLeft(m_grid, 6);
            Canvas.SetTop(m_grid, 20);
        }

        private Grid CreateMapGrid(int rows, int cols)
        {
            Grid grid = new Grid();
            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(32);
                grid.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(32);
                grid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < cols; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    Image image = new Image();
                    grid.Children.Add(image);
                    Grid.SetColumn(image, i);
                    Grid.SetRow(image, j);
                }
            }

            return grid;
        }

        private Image LoadImage(string filename)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(filename, UriKind.Relative));
            return image;
        }

        public void Setup(IMap map)
        {
            m_map = map;
            Draw();
        }

        public void Draw()
        {
            foreach (Image image in m_grid.Children)
            {
                int x = Grid.GetColumn(image);
                int y = Grid.GetRow(image);
                image.Source = m_wall.Source;
            }
        }
    }
}
