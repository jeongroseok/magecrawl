namespace GameEngine
{
    public struct Point
    {
        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return (lhs.X == rhs.X) && (lhs.Y == rhs.Y);
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Point other = (Point)obj;

            return (other.X == this.X) && (other.Y == this.Y) && base.Equals(obj);
        }

        public bool Equals(Point other)
        {
            return (other.X == this.X) && (other.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ base.GetHashCode();
        }
    }
}
