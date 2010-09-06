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
        private Dictionary<string, Image> m_monsters;

        private List<Image> m_terrainList;
        private List<Image> m_objectList;

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

            LoadMonsters();

            m_terrainList = new List<Image>();
            m_objectList = new List<Image>();

            m_grid = CreateMapGrid(MapHeight, MapWidth);

            MapCanvas.Children.Add(m_grid);
            Canvas.SetLeft(m_grid, 3);
            Canvas.SetTop(m_grid, 3);
        }

        public void Setup(IMap map, IPlayer player)
        {
            m_map = map;
            m_player = player;
            Draw();
        }

        private Image LoadImage(string filename)
        {
            Image image = new Image();
            image.Source = ResourceHelper.GetBitmap(filename);
            return image;
        }

        private void LoadMonsters()
        {
            m_monsters = new Dictionary<string, Image>();
            m_monsters["Goblin"] = LoadImage("Images/Monsters/goblin.png");
            m_monsters["Hobgoblin"] = LoadImage("Images/Monsters/hobgoblin.png");
            m_monsters["Kobold"] = LoadImage("Images/Monsters/kobold.png");
            m_monsters["Orc"] = LoadImage("Images/Monsters/orc.png");
            m_monsters["Orc Knight"] = LoadImage("Images/Monsters/orc_knight.png");
            m_monsters["Rat"] = LoadImage("Images/Monsters/rat.png");
            m_monsters["Wolf"] = LoadImage("Images/Monsters/wolf.png");
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

        public void Draw()
        {
            MageCrawlPoint upperLeftViewPoint = UpperLeftViewPoint;
            foreach (Image image in m_terrainList)
            {
                int x = Grid.GetColumn(image) + upperLeftViewPoint.X;
                int y = Grid.GetRow(image) + upperLeftViewPoint.Y;
                if (m_map.IsPointOnMap(x, y))
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

            m_objectList.ForEach(i => m_grid.Children.Remove(i));
            m_objectList.Clear();

            DrawMonsters();
        }

        private void DrawMonsters()
        {
            MageCrawlPoint upperLeft = UpperLeftViewPoint;
            foreach (IMonster m in m_map.Monsters.Where(m => IsPointDrawable(m.Position)))
            {
                Image image = GetImageForMonster(m);
                m_objectList.Add(image);
                m_grid.Children.Add(image);
                Grid.SetColumn(image, m.Position.X - upperLeft.X);
                Grid.SetRow(image, m.Position.Y - upperLeft.Y);
            }
        }

        private Image GetImageForMonster(IMonster m)
        {
            Image i = new Image();
            switch (m.BaseType)
            {
                case "Wolf":
                    i.Source = m_monsters["Wolf"].Source;
                    break;
                case "Orc Barbarian":
                    i.Source = m_monsters["Orc"].Source;
                    break;
                case "Orc Warrior":
                    i.Source = m_monsters["Orc Knight"].Source;
                    break;
                case "Goblin Healer":
                    i.Source = m_monsters["Goblin"].Source;
                    break;
                case "Goblin Slinger":
                    i.Source = m_monsters["Hobgoblin"].Source;
                    break;
                case "Kobold":
                    i.Source = m_monsters["Kobold"].Source;
                    break;
                case "Giant Rat":
                    i.Source = m_monsters["Rat"].Source;
                    break;
                default:
                    throw new InvalidOperationException("GetImageForMonster - can't find image for: " + m.BaseType);
            }
            return i;
        }

        private bool IsPointDrawable(MageCrawlPoint p)
        {
            MageCrawlPoint upperLeft = UpperLeftViewPoint;
            return p.X >= upperLeft.X && p.Y >= upperLeft.Y && p.X < upperLeft.X + MapWidth && p.Y < upperLeft.Y + MapHeight;
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
