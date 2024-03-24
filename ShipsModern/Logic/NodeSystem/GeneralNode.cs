using ShipsForm.Logic.TilesSystem;
using ShipsForm.Graphic;
using System.Windows.Media;
using System;

namespace ShipsForm.Logic.NodeSystem
{
    abstract class GeneralNode : IDrawable
    {
        /// <summary>
        /// Related coords in scope [0, 1].
        /// </summary>
        protected SupportEntities.Point? m_relatedPoint = null;
        protected SupportEntities.Point? m_point = null;
        public Tile TileCoords
        {
            get
            {
                if (Field.Instance is null)
                    throw new Exception("Field was not created.");
                var tile = Field.GetTile(Field.Instance.Map, (int)GetCoords.X, (int)GetCoords.Y);
                if (tile is null)
                    throw new Exception("This tile doesn't exist or something went wrong...");
                return tile;
            }
        }
        public SupportEntities.Point GetRelatedCoords { get { return m_relatedPoint; } }
        public SupportEntities.Point GetCoords { get { return new SupportEntities.Point((m_relatedPoint.X * (Field.Instance.MapWidth - 1)), (m_relatedPoint.Y * (Field.Instance.MapLength - 1))); } }
    
        public abstract SupportEntities.Point? GetCurrentPoint();
        public double GetRotation()
        {
            return 0.0f;
        }
        public abstract ImageSource GetSkin(int size);
        public abstract int GetSize();        
    }
}
