using unvell.D2DLib;

namespace Flappy
{
    public static class Extentions
    {
        public static D2DPoint Add(this D2DPoint point, D2DPoint other)
        {
            return new D2DPoint(point.X + other.X, point.Y + other.Y);
        }

        public static D2DPoint Subtract(this D2DPoint point, D2DPoint other)
        {
            return new D2DPoint(point.X - other.X, point.Y - other.Y);
        }

        public static bool Contains(this D2DRect rect, D2DPoint pnt)
        {
            return rect.X <= pnt.X &&
           pnt.X < rect.X + rect.Width &&
           rect.Y <= pnt.Y &&
           pnt.Y < rect.Y + rect.Height;
        }
    }
}
