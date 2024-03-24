using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Svg;
using ShipsForm.Data;
using ShipsForm.Exceptions;
using System.IO;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

namespace ShipsModern.SupportEntities
{
    /// <summary>
    /// Support class manipulating with svg data files, occurs rendering, storing and converting to ImageSource.
    /// </summary>
    public class SVGData
    {
        public static SVGData? Instance;
        public Dictionary<string, SvgDocument> UploadedSvg = new Dictionary<string, SvgDocument>();
        public Dictionary<string, ImageSource> Converted = new Dictionary<string, ImageSource>();

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromSvg(string key, int heightSize)
        {
            UploadedSvg.TryGetValue(key, out var svgDoc);
            if (svgDoc is null)
                throw new Exception();
            Instance.ResizeSvg(svgDoc, heightSize);
            var bitmap = svgDoc.Draw();
            var handle = bitmap.GetHbitmap();
            try
            {
                var src = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                if (Converted.ContainsKey(key))
                    Converted[key] = src;
                else Converted.Add(key, src);
                return src;
            }
            finally { DeleteObject(handle); }

        }

        /// <summary>
        /// Uploads all svg files directed in config file.
        /// </summary>
        /// <exception cref="ConfigFileDoesntExistError">Throws when config file doesn't exist.</exception>
        public static void UploadAllData()
        {
            if (Instance == null)
            {
                Instance = new SVGData();
                var data = Configuration.Instance;
                if (data is null)
                    throw new ConfigFileDoesntExistError();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\Skins\\");
                foreach (var (key, val) in data.SVG_Uris)
                {
                    var opened = SvgDocument.Open(path + val);
                    if(Instance.UploadedSvg.ContainsKey(key))
                        Instance.UploadedSvg[key] = opened;
                    else Instance.UploadedSvg.Add(key, opened);
                }

                Console.WriteLine("SVGs files successfully uploaded.");
            }
            
        }

        /// <summary>
        /// Returns resized svg file.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private SvgDocument ResizeSvg(SvgDocument svg, int height)
        {
            var k = svg.Bounds.Width / svg.Bounds.Height;
            svg.Width = height * k;
            svg.Height = height;
            return svg;
        }
    }
}
