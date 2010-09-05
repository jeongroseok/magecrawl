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

using MageCrawlPoint = Magecrawl.Utilities.Point;

namespace MageCrawl.Silverlight
{
    public partial class Map : UserControl
    {
        private Grid m_grid;

        private Image m_floor;
        private Image m_wall;
        private List<Image> m_playerParts;
        private List<Image> m_terrainList;

        private IMap m_map;
        private IPlayer m_player;

        private const int MapWidth = 17;
        private const int MapHeight = 15;
        private const int CenterX = (MapWidth - 1) / 2;
        private const int CenterY = (MapHeight - 1) / 2;

        public Map()
        {
            InitializeComponent();
            m_floor = LoadImage("Images/Terrain/grey_dirt3.png");
            m_wall = LoadImage("Images/Terrain/brick_dark3.png");
            m_playerParts = new List<Image>() { LoadImage("Images/Player/gray.png"), LoadImage("Images/Player/human_m.png"), 
                LoadImage("Images/Player/gandalf_g.png"), LoadImage("Images/Player/middle_brown3.png"), 
                LoadImage("Images/Player/glove_grayfist.png"), LoadImage("Images/Player/wizard_blackred.png") };

            m_terrainList = new List<Image>();

            m_grid = CreateMapGrid(MapHeight, MapWidth);

            MapCanvas.Children.Add(m_grid);
            Canvas.SetLeft(m_grid, 3);
            Canvas.SetTop(m_grid, 3);
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
                    m_terrainList.Add(image);
                    grid.Children.Add(image);
                    Grid.SetColumn(image, i);
                    Grid.SetRow(image, j);
                }
            }

            foreach (Image image in m_playerParts)
            {
                grid.Children.Add(image);
                Grid.SetColumn(image, CenterX);
                Grid.SetRow(image, CenterY);
            }

            return grid;
        }

        private Image LoadImage(string filename)
        {
            Image image = new Image();
            image.Source = ResourceHelper.GetBitmap(filename);
            return image;
        }

        public void Setup(IMap map, IPlayer player)
        {
            m_map = map;
            m_player = player;
            Draw();
        }

        public void Draw()
        {
            MageCrawlPoint upperLeftViewPoint = UpperLeftViewPoint;
            foreach (Image image in m_terrainList)
            {
                int x = Grid.GetColumn(image) + upperLeftViewPoint.X;
                int y = Grid.GetRow(image) + upperLeftViewPoint.Y;
                if (m_map.IsPointOnMap(new MageCrawlPoint(x, y)))
                {
                    if (m_map.GetTerrainAt(x, y) == TerrainType.Floor)
                    {
                        image.Source = m_floor.Source;
                    }
                    else
                    {
                        image.Source = m_wall.Source;
                    }
                }
                else
                {
                    image.Source = null;
                }
            }

            DrawPlayer();
        }

        private void DrawPlayer()
        {
            BitmapImage i = new BitmapImage();
            foreach (Image image in m_playerParts)
            {
                Grid.SetColumn(image, CenterX);
                Grid.SetRow(image, CenterY);
            }
        }

        public MageCrawlPoint UpperLeftViewPoint
        {
            get
            {
                return m_player.Position - new MageCrawlPoint(CenterX, CenterY);
            }
        }
    }
}
