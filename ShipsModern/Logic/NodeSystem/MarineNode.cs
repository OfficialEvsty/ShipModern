

using ShipsForm.Exceptions;
using ShipsForm.Graphic;
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
