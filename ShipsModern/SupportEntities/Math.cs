
using System;

namespace ShipsForm.SupportEntities
{
/// <summary>
/// Dima pidoras predlogil cherez tangens.
/// </summary>
    static class Math
    {
        public static float GetDistance(Point p1, Point p2)
        {
            return MathF.Sqrt(MathF.Pow(p1.X-p2.X, 2) + MathF.Pow(p1.Y - p2.Y, 2));
        }
        public static double GetRotation(double rotation, Point sourceP, Point endP)
        {
            double primaryRot = 90;
            Point vector = endP - sourceP;
            double newRotation = System.Math.Atan2(vector.Y, vector.X) * 180 / System.Math.PI + primaryRot;
            return newRotation;
        }
    }
}
