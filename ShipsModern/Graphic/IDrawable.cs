using ShipsForm.Data;
using ShipsForm.Logic.TilesSystem;
using ShipsModern.SupportEntities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ShipsForm.Graphic
{
    //This interface will add on entities with draw logic, that allows them to be on the scene.
    internal interface IDrawable
    {
        public ImageSource DownloadImage(string name, int size)
        {
            var svgData = SVGData.Instance;
            if (svgData is null)
                throw new System.Exception();
            if (svgData.Converted.ContainsKey(name))
            {
                if (svgData.Converted[name].Width == size || svgData.Converted[name].Height == size)
                    return svgData.Converted[name];
            }

            ImageSource src = svgData.ImageSourceFromSvg(name, size);
            return src;
        }
        public ImageSource GetSkin(int size);
        public SupportEntities.Point? GetCurrentPoint();
        public double GetRotation();
        public virtual int GetSize() { return Data.Configuration.Instance.DefaultImageSize; }
    }

    public interface IPathDrawable
    {
        public bool IsPath();
        public Point[] GetPoints();
    }
}
