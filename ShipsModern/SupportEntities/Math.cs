
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
            int degrees = 360;
            double rotationInRadians = DegreesToRadians(rotation);
            Point vectorFromRotation = GetVectorFromDeegrees(rotationInRadians);
            Point directingVector = GetDirectingVector(sourceP, endP);
            Point normalizedDirectingV = GetNormalizedVector(directingVector);
            double rotationBeetwenVectors = GetRotationByTwoPoint(vectorFromRotation, normalizedDirectingV);
            double newRotation = (rotation + rotationBeetwenVectors) % degrees;
            return newRotation;
        }

        public static Point GetRotatedVector(double rotation, Point dir)
        {

            float xi = (float)(dir.X * System.Math.Cos(rotation) - dir.Y * System.Math.Sin(rotation)) ;
            float yi = (float)(dir.X * System.Math.Sin(rotation) + dir.Y * System.Math.Cos(rotation));
            return new Point(xi, yi);
        }

        public static double GetRotationByTwoPoint(Point startP, Point endP)
        {
            double cornerBeetwenTwoPointsInRadians = (startP.X * endP.X + startP.Y * endP.Y);
            double rotationInDegrees = RadiansToDegrees(cornerBeetwenTwoPointsInRadians);         
            return rotationInDegrees;
        }

        public static Point GetDirectingVector(Point fst, Point snd)
        {
            return new Point(snd.X - fst.X, snd.Y - fst.Y);
        }

        public static Point GetNormalizedVector(Point vector)
        {
            float absVector = (float)System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new Point(vector.X / absVector, vector.Y / absVector);
        }

        public static Point GetVectorFromDeegrees(double radians)
        {
            float cos = (float)System.Math.Cos(radians);
            float sin = (float)System.Math.Sin(radians);
            return new Point(cos, sin);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        public static double RadiansToDegrees(double radians)
        {
            double degrees = (180 / System.Math.PI) * radians;
            return degrees;
        }
        
    }
}
