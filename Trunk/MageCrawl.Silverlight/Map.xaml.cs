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
using Magecrawl.Utilities;

namespace MageCrawl.Silverlight
{
    public partial class Map : UserControl
    {
        private Grid m_grid;

        private List<Image> m_playerParts;
        private Dictionary<string, Image> m_images;

        private List<Image> m_overlayImages;
        private List<Image> m_terrainList;
        private List<Image> m_objectList;
        private Image m_viewCursor;

        private IGameEngine m_engine;

        private const int MapWidth = 17;
        private const int MapHeight = 15;
        private const int CenterX = (MapWidth - 1) / 2;
        private const int CenterY = (MapHeight - 1) / 2;

        public Map()
        {
            InitializeComponent();

            m_playerParts = new List<Image>() { LoadImage("Images/Player/gray.png"), LoadImage("Images/Player/human_m.png"), 
                LoadImage("Images/Player/gandalf_g.png"), LoadImage("Images/Player/middle_brown3.png"), 
                LoadImage("Images/Player/glove_grayfist.png"), LoadImage("Images/Player/wizard_blackred.png") };

            LoadImages();

            TargetablePoints = new List<EffectivePoint>();

            m_terrainList = new List<Image>();
            m_objectList = new List<Image>();
            m_overlayImages = new List<Image>();

            m_grid = CreateMapGrid(MapHeight, MapWidth);

            MapCanvas.Children.Add(m_grid);
            Canvas.SetLeft(m_grid, 3);
            Canvas.SetTop(m_grid, 3);
        }

        public void Setup(IGameEngine engine)
        {
            m_engine = engine;
            InTargettingMode = false;
            Draw();
        }

        private bool m_targettingMode;
        public bool InTargettingMode
        {
            get
            {
                return m_targettingMode;
            }
            set
            {
                m_targettingMode = value;
                TargetPoint = m_engine.Player.Position;
                UseViewCursor = false;
            }
        }
        
        public MageCrawlPoint TargetPoint { get; set; }

        public List<EffectivePoint> TargetablePoints { get; set; }

        public bool UseViewCursor
        {
            set
            {
                m_viewCursor = value ? m_images["Cursor"] : m_images["CursorRed"];
            }
        }

        private Image LoadImage(string filename)
        {
            Image image = new Image();
            image.Source = ResourceHelper.GetBitmap(filename);
            return image;
        }

        private void LoadImages()
        {
            m_images = new Dictionary<string, Image>();
            m_images["Goblin"] = LoadImage("Images/Monsters/goblin.png");
            m_images["Hobgoblin"] = LoadImage("Images/Monsters/hobgoblin.png");
            m_images["Kobold"] = LoadImage("Images/Monsters/kobold.png");
            m_images["Orc"] = LoadImage("Images/Monsters/orc.png");
            m_images["Orc Knight"] = LoadImage("Images/Monsters/orc_knight.png");
            m_images["Rat"] = LoadImage("Images/Monsters/rat.png");
            m_images["Wolf"] = LoadImage("Images/Monsters/wolf.png");

            m_images["Floor"] = LoadImage("Images/Terrain/grey_dirt3.png");
            m_images["Wall"] = LoadImage("Images/Terrain/brick_dark3.png");
            m_images["OutOfSight"] = LoadImage("Images/Other/out_of_sight.png");

            m_images["Chest"] = LoadImage("Images/MapObjects/chest.png");
            m_images["Fountain"] = LoadImage("Images/MapObjects/dngn_blue_fountain.png");
            m_images["OpenDoor"] = LoadImage("Images/MapObjects/dngn_open_door.png");
            m_images["ClosedDoor"] = LoadImage("Images/MapObjects/dngn_closed_door.png");
            m_images["LastStairsDown"] = LoadImage("Images/MapObjects/dngn_enter.png");
            m_images["LastStairsUp"] = LoadImage("Images/MapObjects/dngn_return.png");
            m_images["StairsDown"] = LoadImage("Images/MapObjects/stone_stairs_down.png");
            m_images["StairsUp"] = LoadImage("Images/MapObjects/stone_stairs_up.png");

            m_images["Potion"] = LoadImage("Images/Items/brilliant_blue.png");
            m_images["Wand"] = LoadImage("Images/Items/gem_bronze.png");
            m_images["Scroll"] = LoadImage("Images/Items/scroll.png");
            
            m_images["Axe"] = LoadImage("Images/Items/battle_axe1.png");
            m_images["MediumBoots"] = LoadImage("Images/Items/boots1_brown.png");
            m_images["HeavyBoots"] = LoadImage("Images/Items/boots3_stripe.png");
            m_images["Dagger"] = LoadImage("Images/Items/dagger.png");
            m_images["Dagger"] = LoadImage("Images/Items/dagger.png");
            m_images["MediumHelm"] = LoadImage("Images/Items/elven_leather_helm.png");
            m_images["LightGloves"] = LoadImage("Images/Items/glove1.png");
            m_images["MediumGloves"] = LoadImage("Images/Items/glove2.png");
            m_images["HeavyHelm"] = LoadImage("Images/Items/helmet1_visored.png");
            m_images["Bow"] = LoadImage("Images/Items/longbow.png");
            m_images["HeavyChestArmor"] = LoadImage("Images/Items/plate_mail1.png");
            m_images["Staff"] = LoadImage("Images/Items/quarterstaff.png");
            m_images["MediumChestArmor"] = LoadImage("Images/Items/ring_mail2.png");
            m_images["LightChestArmor"] = LoadImage("Images/Items/robe2.png");
            m_images["Sword"] = LoadImage("Images/Items/short_sword2.png");
            m_images["Spear"] = LoadImage("Images/Items/spear1_elven.png");
            m_images["Spear"] = LoadImage("Images/Items/spear1_elven.png");
            m_images["LightBoots"] = LoadImage("Images/Items/urand_assassin.png");
            m_images["HeavyGloves"] = LoadImage("Images/Items/urand_war.png");
            m_images["LightHelm"] = LoadImage("Images/Items/wizard_hat1.png");

            m_images["Cursor"] = LoadImage("Images/Other/cursor.png");
            m_images["CursorRed"] = LoadImage("Images/Other/cursor_red.png");
            m_images["CursorGreen"] = LoadImage("Images/Other/cursor_green.png");
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
            TileVisibility[,] visibility = m_engine.GameState.CalculateTileVisibility();

            MageCrawlPoint upperLeftViewPoint = UpperLeftViewPoint;

            // Clear object layer
            m_objectList.ForEach(i => m_grid.Children.Remove(i));
            m_objectList.Clear();

            foreach (Image image in m_terrainList)
            {
                int x = Grid.GetColumn(image) + upperLeftViewPoint.X;
                int y = Grid.GetRow(image) + upperLeftViewPoint.Y;

                if (m_engine.Map.IsPointOnMap(x, y))
                {
                    if (visibility[x, y] != TileVisibility.Unvisited)
                    {
                        if (m_engine.Map.GetTerrainAt(x, y) == TerrainType.Floor)
                        {
                            image.Source = m_images["Floor"].Source;
                        }
                        else
                        {
                            image.Source = m_images["Wall"].Source;
                        }

                        if (visibility[x, y] == TileVisibility.Visited)
                        {
                            // But not visible now
                            AddObjectLayerImage(GetOutOfSightImage(), Grid.GetColumn(image), Grid.GetRow(image));
                        }
                    }
                    else
                    {
                        image.Source = null;
                    }
                }
                else
                {
                    image.Source = null;
                }

            }

            DrawMapObjects(visibility, m_engine);
            DrawItems(visibility);
            DrawMonsters(visibility);
            DrawPlayer();
            DrawCursor();
            DrawTargettingOverlay();
        }

        private void DrawMonsters(TileVisibility[,] visibility)
        {
            MageCrawlPoint upperLeft = UpperLeftViewPoint;
            foreach (IMonster m in m_engine.Map.Monsters.Where(m => IsPointDrawable(m.Position)))
            {
                if (visibility[m.Position.X, m.Position.Y] == TileVisibility.Visible)
                {
                    AddObjectLayerImage(GetImageForMonster(m), m.Position.X - upperLeft.X, m.Position.Y - upperLeft.Y);
                }
            }
        }
        
        private void DrawMapObjects(TileVisibility[,] visibility, IGameEngine engine)
        {
            MageCrawlPoint upperLeft = UpperLeftViewPoint;
            foreach (IMapObject o in m_engine.Map.MapObjects.Where(o => IsPointDrawable(o.Position)))
            {
                if (visibility[o.Position.X, o.Position.Y] != TileVisibility.Unvisited)
                {
                    AddObjectLayerImage(GetImageForMapObject(o, engine), o.Position.X - upperLeft.X, o.Position.Y - upperLeft.Y);
                }
            }
        }

        private void DrawItems(TileVisibility[,] visibility)
        {
            MageCrawlPoint upperLeft = UpperLeftViewPoint;
            foreach (var i in m_engine.Map.Items.Where(i => IsPointDrawable(i.Second)))
            {
                if (visibility[i.Second.X, i.Second.Y] == TileVisibility.Visible)
                {
                    AddObjectLayerImage(GetImageForItem(i.First), i.Second.X - upperLeft.X, i.Second.Y - upperLeft.Y);
                }
            }
        }

        private void DrawPlayer()
        {
            if (IsPointDrawable(m_engine.Player.Position))
            {
                MageCrawlPoint upperLeft = UpperLeftViewPoint;
                foreach (Image image in m_playerParts)
                {
                    // Make sure we're the "last" in the list so we draw first
                    m_grid.Children.Remove(image);
                    m_grid.Children.Add(image);
                    Grid.SetColumn(image, m_engine.Player.Position.X - upperLeft.X);
                    Grid.SetRow(image, m_engine.Player.Position.Y - upperLeft.Y);
                }
            }
            else
            {
                foreach (Image image in m_playerParts)
                {
                    if (m_grid.Children.Contains(image))
                        m_grid.Children.Remove(image);
                }
            }
        }

        private void DrawCursor()
        {
            if (InTargettingMode)
            {
                // Make sure we're the "last" in the list so we draw first, even in front of player
                m_grid.Children.Remove(m_viewCursor);
                m_grid.Children.Add(m_viewCursor);
                Grid.SetColumn(m_viewCursor, CenterX);
                Grid.SetRow(m_viewCursor, CenterY);
            }
            else
            {
                if (m_grid.Children.Contains(m_viewCursor))
                    m_grid.Children.Remove(m_viewCursor);
            }
        }

        private double Lerp(double first, double second, double coef)
        {
            return first + (second - first) * coef;
        }

        private void DrawTargettingOverlay()
        {
            if (InTargettingMode)
            {
                ClearOverlayImages();
                foreach (EffectivePoint e in TargetablePoints)
                {
                    int x = e.Position.X - UpperLeftViewPoint.X;
                    int y = e.Position.Y - UpperLeftViewPoint.Y;
                    if (m_engine.Map.IsPointOnMap(x, y))
                    {
                        Image i = new Image();
                        i.Source = m_images["CursorGreen"].Source;
                        i.Opacity = Math.Max(0, Lerp(0, .5, e.EffectiveStrength * e.EffectiveStrength));
                        m_grid.Children.Add(i);
                        m_overlayImages.Add(i);
                        Grid.SetColumn(i, x);
                        Grid.SetRow(i, y);
                    }
                }
            }
            else
            {
                ClearOverlayImages();
            }
        }

        private void ClearOverlayImages()
        {
            foreach (Image i in m_overlayImages)
            {
                if (m_grid.Children.Contains(i))
                    m_grid.Children.Remove(i);
            }
            m_overlayImages.Clear();
        }

        private void AddObjectLayerImage(Image image, int x, int y)
        {
            m_objectList.Add(image);
            m_grid.Children.Add(image);
            Grid.SetColumn(image, x);
            Grid.SetRow(image, y);
        }

        private Image GetOutOfSightImage()
        {
            Image i = new Image();
            i.Source = m_images["OutOfSight"].Source;
            return i;
        }

        private Image GetImageForMapObject(IMapObject o, IGameEngine engine)
        {
            Image i = new Image();
            switch (o.Name)
            {
                case "Opened Door":
                    i.Source = m_images["OpenDoor"].Source;
                    break;
                case "Closed Door":
                    i.Source = m_images["ClosedDoor"].Source;
                    break;
                case "Stairs Up":
                {  
                    StairMovmentType type = engine.GameState.IsStairMovementSpecial(o.Position);
                    switch (type)
                    {
                        case StairMovmentType.QuitGame:
                            i.Source = m_images["LastStairsUp"].Source;
                            break;
                        case StairMovmentType.None:
                        default:
                            i.Source = m_images["StairsUp"].Source;
                            break;
                    }
                    break;
                }
                case "Stairs Down":
                {  
                    StairMovmentType type = engine.GameState.IsStairMovementSpecial(o.Position);
                    switch (type)
                    {
                        case StairMovmentType.WinGame:
                            i.Source = m_images["LastStairsDown"].Source;
                            break;
                        case StairMovmentType.None:
                        default:
                            i.Source = m_images["StairsDown"].Source;
                            break;
                    }
                    break;
                }
                case "Treasure Chest":
                    i.Source = m_images["Chest"].Source;
                    break;
                case "Fountain":
                    i.Source = m_images["Fountain"].Source;
                    break;
                default:
                    throw new InvalidOperationException("GetImageForMapObject - can't find image for: " + o.Name);
            }
            return i;
        }

        private Image GetImageForItem(IItem i)
        {
            Image image = new Image();
            switch (i.Type)
            {
                case "Potion":
                    image.Source = m_images["Potion"].Source;
                    break;
                case "Wand":
                    image.Source = m_images["Wand"].Source;
                    break;
                case "Scroll":
                    image.Source = m_images["Scroll"].Source;
                    break;
                case "ChestArmor":
                    switch (GetArmorWeight(i))
                    {
                        case ArmorWeight.Light:
                            image.Source = m_images["LightChestArmor"].Source;
                            break;
                        case ArmorWeight.Standard:
                            image.Source = m_images["MediumChestArmor"].Source;
                            break;
                        case ArmorWeight.Heavy:
                            image.Source = m_images["HeavyChestArmor"].Source;
                            break;
                    }
                    break;
                case "Helm":
                    switch (GetArmorWeight(i))
                    {
                        case ArmorWeight.Light:
                            image.Source = m_images["LightHelm"].Source;
                            break;
                        case ArmorWeight.Standard:
                            image.Source = m_images["MediumHelm"].Source;
                            break;
                        case ArmorWeight.Heavy:
                            image.Source = m_images["HeavyHelm"].Source;
                            break;
                    }
                    break;
                case "Gloves":
                    switch (GetArmorWeight(i))
                    {
                        case ArmorWeight.Light:
                            image.Source = m_images["LightGloves"].Source;
                            break;
                        case ArmorWeight.Standard:
                            image.Source = m_images["MediumGloves"].Source;
                            break;
                        case ArmorWeight.Heavy:
                            image.Source = m_images["HeavyGloves"].Source;
                            break;
                    }
                    break;
                case "Boots":
                    switch (GetArmorWeight(i))
                    {
                        case ArmorWeight.Light:
                            image.Source = m_images["LightBoots"].Source;
                            break;
                        case ArmorWeight.Standard:
                            image.Source = m_images["MediumBoots"].Source;
                            break;
                        case ArmorWeight.Heavy:
                            image.Source = m_images["HeavyBoots"].Source;
                            break;
                    }
                    break;
                case "Axe":
                    image.Source = m_images["Axe"].Source;
                    break;
                case "Bow":
                    image.Source = m_images["Bow"].Source;
                    break;
                case "Staff":
                    image.Source = m_images["Staff"].Source;
                    break;
                case "Dagger":
                    image.Source = m_images["Dagger"].Source;
                    break;
                case "Sling":
                    image.Source = m_images["Sling"].Source;
                    break;
                case "Spear":
                    image.Source = m_images["Spear"].Source;
                    break;
                case "Sword":
                    image.Source = m_images["Sword"].Source;
                    break;
                default:
                    throw new InvalidOperationException("GetImageForItem - can't find image for: " + i.Type);
            }
            return image;
        }

        private ArmorWeight GetArmorWeight(IItem i)
        {
            IArmor armor = (IArmor)i;
            return armor.Weight;
        }

        private Image GetImageForMonster(IMonster m)
        {
            Image i = new Image();
            switch (m.BaseType)
            {
                case "Wolf":
                    i.Source = m_images["Wolf"].Source;
                    break;
                case "Orc Barbarian":
                    i.Source = m_images["Orc"].Source;
                    break;
                case "Orc Warrior":
                    i.Source = m_images["Orc Knight"].Source;
                    break;
                case "Goblin Healer":
                    i.Source = m_images["Goblin"].Source;
                    break;
                case "Goblin Slinger":
                    i.Source = m_images["Hobgoblin"].Source;
                    break;
                case "Kobold":
                    i.Source = m_images["Kobold"].Source;
                    break;
                case "Giant Rat":
                    i.Source = m_images["Rat"].Source;
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

        public MageCrawlPoint UpperLeftViewPoint
        {
            get
            {
                if (InTargettingMode)
                    return new MageCrawlPoint(TargetPoint.X - CenterX, TargetPoint.Y - CenterY);
                else
                    return m_engine.Player.Position - new MageCrawlPoint(CenterX, CenterY);
            }
        }
    }
}
