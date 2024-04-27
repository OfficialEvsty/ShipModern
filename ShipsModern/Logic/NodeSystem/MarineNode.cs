

using ShipsForm.Exceptions;
using ShipsForm.Graphic;
using ShipsForm.Logic.ShipSystem.Behaviour;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.SupportEntities.PatternObserver;
using ShipsModern.SupportEntities;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShipsForm.Logic.NodeSystem
{
    internal class MarineNode : GeneralNode
    {
        public MarineNode(SupportEntities.Point relatedPointToSetNode)
        {
            Id = Manager.GetGuiElementID();
            m_relatedPoint = relatedPointToSetNode;
            EventObservable.NotifyObservers((IDrawable)this);
        }

        public MarineNode(Tile tile)
        {
            Id = Manager.GetGuiElementID();
            m_relatedPoint = new SupportEntities.Point(tile.X / ((float)Field.Instance.MapWidth - 1), tile.Y / ((float)Field.Instance.MapLength - 1));
            EventObservable.NotifyObservers((IDrawable)this);
        }

        public void ShipLeaveMarineNode(CargoShip cs)
        {
            if (cs.Behavior.State is SearchProfitRouteState)
                cs.Behavior.GoNextState();
        }

        public override SupportEntities.Point? GetCurrentPoint()
        {
            var tile = TileCoords;
            return new SupportEntities.Point(tile.X, tile.Y);
        }

        public override int GetSize()
        {
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            return data.MarineNodeImageSize;
        }

        public override ImageSource GetSkin(int size)
        {
            var modelName = "MarineNode";
            return ((IDrawable)this).DownloadImage(modelName, size);
        }
    }
}
