
using ShipsForm.Exceptions;
using System;

namespace ShipsForm.Logic.TilesSystem
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int TileCost { get; init; }
        public int Cost { get; private set; }
        public float Distance { get; private set; }
        public float CostDistance { get { return Cost + Distance; } }
        public bool Passable { get; set; }
        public Tile? Parent { get; init; }


        public static Tile? GetTileCoords(SupportEntities.Point point)
        {
            var data = Data.Configuration.Instance;
            if(data is null) throw new ArgumentNullException("data");
            if (point.X > 0 && point.Y > 0)
                return new Tile() { X = (int)(point.X), Y = (int)(point.Y) };
            return null;
        }

        public void SetDistance(int targetX, int targetY)
        {
            var data = Data.Configuration.Instance;
            if (data is null) throw new ConfigFileDoesntExistError();

            Distance = /*MathF.Max(MathF.Abs(targetX - X), MathF.Abs(targetY - Y));*/MathF.Sqrt(MathF.Pow(targetX - X, 2) + MathF.Pow(targetY - Y, 2));
        }

        public void SetCost(int cost)
        {
            Cost = cost + TileCost;
        }
    }
}
