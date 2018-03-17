using System;
using System.Windows;

namespace F1Dashboard
{
    class Util
    {
        public static Point Rotate(Point point, Point center, double degrees)
        {
            double angle = DegToRad(degrees);
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            double new_p1x = (cos * (point.X - center.X)) - (sin * (point.Y - center.Y)) + center.Y;
            double new_p1y = (sin * (point.X - center.X)) + (cos * (point.Y - center.Y)) + center.Y;

            return new Point(new_p1x, new_p1y);
        }

        public static double DegToRad(double angle)
        {
            return Math.PI / 180 * angle;
        }

        public static float GetPercent(float value, float total)
        {
            return value * 100 / total;
        }

        public static float CalcDegrees(float value, float range_offset, float range_end, float total)
        {
            return (value * range_end / total) - range_offset;
        }

        public static float MPHtoKMH(float mph)
        {
            return mph * 3.6f;
        }
    }
}
