
using ShipsForm.Exceptions;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.SupportEntities;
using System;

namespace ShipsForm.Logic.TilesSystem
{
    [Serializable]
    public class Tile
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int TileCost { get; init; }
        public int Cost { get; private set; }
        public float Distance { get; private set; }
        public float CostDistance { get { return Cost + Distance; } }
        public bool Passable { get; set; }
        public string? Category {  get; set; }
        public Tile? Parent { get; set; }


        public static Tile? GetTileCoords(SupportEntities.Point point)
        {
            var data = Data.Configuration.Instance;
            if(data is null) throw new ArgumentNullException("data");
            if (point.X >= 0 && point.Y >= 0)
                return new Tile() { X = (int)(point.X), Y = (int)(point.Y) };
            return null;
        }

        // Реализация метода Equals для сравнения плиток по координатам
        public override bool Equals(object obj)
        {
            if (obj is Tile other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public void Heuristic(int targetX, int targetY)
        {
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            
            //Heuristic weight parameter, necessary for decreasing count of excess tiles in openSet.
            var e = data.WeightParameterForAStar;
            float dx = MathF.Abs(targetX - X);
            float dy = MathF.Abs(targetY - Y);
            Distance = e * MathF.Min(dx, dy);
            /*MathF.Max(MathF.Abs(targetX - X), MathF.Abs(targetY - Y)) + MathF.Min(MathF.Abs(targetX - X), MathF.Abs(targetY - Y));*//*e*MathF.Sqrt(MathF.Pow(targetX - X, 2) + MathF.Pow(targetY - Y, 2));*/
        }

        public void SetCost(int cost)
        {
            Cost = cost + TileCost;
        }

        public void SetDefaultCost(int defaultCost)
        {
            Cost = defaultCost;
        }

        public void SetF(float f)
        {
            Distance = f;
        }

        public void SetParent(Tile tile)
        {
            Parent = tile;
        }

        public Tile GetSerializableTile()
        {
            return new Tile() { Category=this.Category, Cost=this.Cost, Distance=this.Distance, 
                Parent=null, Passable = this.Passable, TileCost=this.TileCost, X=this.X, Y=this.Y};
        }
    }
}
