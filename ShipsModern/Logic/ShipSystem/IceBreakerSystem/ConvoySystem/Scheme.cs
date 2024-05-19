

using System;

namespace ShipsForm.Logic.ShipSystem.IceBreakerSystem.ConvoySystem
{
    class Scheme
    {
        private int i_shipCounter = 0;
        private float f_tileDist;
        private float f_distanceBetweenIS;
        private float f_distanceBetweenS;
        private float f_shipImgLength;
        private float f_ibImgLength;
        public float Interval { 
            get
            {                
                if (i_shipCounter > 0)
                    return f_distanceBetweenS + f_shipImgLength / 2;
                else
                    return f_distanceBetweenIS + f_ibImgLength / 2;
            } 
        }

        public int Current { get { return i_shipCounter; } set {i_shipCounter = value; } }
        


        public Scheme()
        {
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new Exception("Config doesn't exist in context.");
            f_tileDist = data.TileDistance;
            f_distanceBetweenIS = data.DistanceBetweenIcebreakerAndShips * f_tileDist;
            f_distanceBetweenS = data.DistanceBetweenShips * f_tileDist;
            f_shipImgLength = data.ShipImageSize * f_tileDist;
            f_ibImgLength = data.IcebreakerImageSize * f_tileDist;          
        }
    }
}
