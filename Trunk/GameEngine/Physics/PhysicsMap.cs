namespace Magecrawl.GameEngine.Physics
{   
    internal class PhysicsMap
    {
        internal struct MapCell
        {
            public bool Transparent;
            public bool Walkable;
            public bool Visible;
        }

        internal int Width
        {
            get;
            private set;
        }

        internal int Height
        {
            get;
            private set;
        }

        internal MapCell[,] Cells;

        internal PhysicsMap(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new MapCell[width, height];
        }

        internal void ClearVisibility()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j].Visible = false;
                }
            }
        }

        internal void Clear()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j].Visible = false;
                    Cells[i, j].Transparent = false;
                    Cells[i, j].Walkable = false;                    
                }
            }
        }
    }

}
