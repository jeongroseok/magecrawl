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
        private Grid m_grid;

        private Image m_blank;
        private Image m_wall;

        private IMap m_map;

        public Map()
        {
            InitializeComponent();
            m_blank = LoadImage("Images/grey_dirt3.png");
            m_wall = LoadImage("Images/brick_dark3.png");

            m_grid = CreateMapGrid(13, 17);

            MapCanvas.Children.Add(m_grid);
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
            image.Source = ResourceHelper.GetBitmap(filename);
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
                if (m_map.GetTerrainAt(x,y) == TerrainType.Floor)
                    image.Source = m_blank.Source;
                else
                    image.Source = m_wall.Source;
            }
        }
    }
}
